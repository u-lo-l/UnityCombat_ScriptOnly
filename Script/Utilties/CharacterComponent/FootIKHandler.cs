using System;
using Cinemachine.Utility;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class FootIKHandler : MonoBehaviour
{
	private enum StepType { Up, Normal, Mix }
	private Animator animator;
	private const float RayLength = 1.5f;
	private float hipOffsetMin;
	private float footHeight;
	private LayerMask groundLayerMask;
	[field : SerializeField] public bool ApplyHipAdjustment {private get ; set ;} = true;
	[field : SerializeField] public bool ApplyFootIKPosition { private get; set; } = true;
	[field : SerializeField] public bool ApplyFootIKRotation { private get; set; } = true;
	[field : SerializeField] public bool ApplyStairIK { private get; set; } = true;
	[field : Header("Leg Setting")]
	[field : SerializeField] private float legLength = 0.65f;
	[SerializeField] StepType stepType = StepType.Mix;
	[Header("Foot Setting")]
	[SerializeField] private float footFrontLength = 0.14f;
	[SerializeField] private float footRearLength = 0.06f;
	private float FootLength => footFrontLength + footRearLength;


	[SerializeField, Range(0.01f, 10f)] float hipLerp = 0.1f;
	[SerializeField, Range(10f, 1000f)] float rotationLerp = 200;
	[SerializeField, Range(10f, 1000f)] float positoinLerp = 30;
	private float prevHipDown = 0;
	private float targetHipDownDistance = 0;
	private readonly Vector3[] currFootPosition = new Vector3[2];
	private readonly Vector3[] targetFootPosition = new Vector3[2];
	private readonly Quaternion[] currFootRotation = new Quaternion[2];
	private readonly Quaternion[] targetFootRotation = new Quaternion[2];
	private readonly RaycastHit[] footRayHit = new RaycastHit[2];
	private float sqrSpeed;

#region MonoBehaviour
	private void Awake()
	{
		animator = GetComponent<Animator>();
		Debug.Assert(animator != null, "[IK Handler] : Animator Not Found");
		footHeight = (animator.leftFeetBottomHeight + animator.rightFeetBottomHeight) / 2;

		groundLayerMask = GetLayerMask.GetObstacleLayerMask;

		Debug.Assert(legLength > 0, "[IK Handler] : Leg Length should be positive value");

		hipOffsetMin = -legLength / 2;

		stepType = StepType.Mix;
	}

	private void Start()
	{
		currFootPosition[0] = currFootPosition[1] = targetFootPosition[0] = targetFootPosition[1] = transform.position;
		currFootRotation[0] = currFootRotation[1] = targetFootRotation[0] = targetFootRotation[1] = transform.rotation;
	}
	private void LateUpdate()
	{
		sqrSpeed = Mathf.Pow(animator.GetFloat("SpeedZ"),2) + Mathf.Pow(animator.GetFloat("SpeedX"),2);
		sqrSpeed = Mathf.Max(0.5f, sqrSpeed);

		currFootRotation[0] = Quaternion.Slerp(currFootRotation[0], targetFootRotation[0], Time.deltaTime * rotationLerp);
		currFootRotation[1] = Quaternion.Slerp(currFootRotation[1], targetFootRotation[1], Time.deltaTime * rotationLerp);

		currFootPosition[0] = Vector3.Lerp(currFootPosition[0], targetFootPosition[0], Time.deltaTime * positoinLerp * sqrSpeed);
		currFootPosition[1] = Vector3.Lerp(currFootPosition[1], targetFootPosition[1], Time.deltaTime * positoinLerp * sqrSpeed);

		float interpolateValue = 0.1f;
		if (Math.Abs(prevHipDown - targetHipDownDistance) > 0.1f)
		{
			interpolateValue = Time.deltaTime * hipLerp;
		}
		prevHipDown = Mathf.Lerp(prevHipDown, targetHipDownDistance, interpolateValue);
	}

	private void OnAnimatorIK(int animationLayerIndex)
	{
		if (animationLayerIndex != 0)
			return ;
		SetHipPosition();
		SetFootIK(AvatarIKGoal.LeftFoot);
		SetFootIK(AvatarIKGoal.RightFoot);
	}

#endregion

#region Inverse Kinematics
	private void SetHipPosition()
	{
		if (ApplyHipAdjustment == false)
		{
			return ;
		}

		float leftFootGap = GetFootDisplacement(AvatarIKGoal.LeftFoot);
		float rightFootGap = GetFootDisplacement(AvatarIKGoal.RightFoot);
		float minFootDisplacement = Mathf.Min(leftFootGap, rightFootGap);

		Vector3 bodyWorldPosition = animator.bodyPosition;
		float lowerBodyHeight = bodyWorldPosition.y - animator.avatarRoot.transform.position.y;
		float hipDownThreshold = (legLength - lowerBodyHeight) * 0.75f;

		targetHipDownDistance  = Mathf.Min(minFootDisplacement + hipDownThreshold, 0);

		if (prevHipDown < hipOffsetMin)
			prevHipDown = 0f;

		bodyWorldPosition.y += prevHipDown;
		animator.bodyPosition = bodyWorldPosition;
	}

	private void SetFootIK(AvatarIKGoal goal)
	{
		if (Physics.Raycast(CreateFootIKRay(goal), out footRayHit[(int)goal], RayLength, groundLayerMask) == false)
		{
			return ;
		}

		if (ApplyFootIKRotation == true)
		{
			targetFootRotation[(int)goal] = CalculateNewFootRotation(goal, footRayHit[(int)goal]);
			animator.SetIKRotationWeight(goal, 1);
			animator.SetIKRotation(goal, currFootRotation[(int)goal]);
		}

		if (ApplyFootIKPosition == true)
		{
			targetFootPosition[(int)goal] = CalculateNewFootPosition(goal, footRayHit[(int)goal]);
			animator.SetIKPositionWeight(goal, 1);
			animator.SetIKPosition(goal, currFootPosition[(int)goal]);
		}
	}

	private float GetFootDisplacement(AvatarIKGoal goal)
	{
		if (Physics.Raycast(CreateFootIKRay(goal), out RaycastHit temp, RayLength, groundLayerMask) == true)
		{
			return temp.point.y - animator.rootPosition.y;
		}
		return 0;
	}
	private Ray CreateFootIKRay(AvatarIKGoal goal)
	{
		Vector3 footPosition = animator.GetIKPosition(goal);
		Vector3 rayOrigin = new(footPosition.x, animator.bodyPosition.y, footPosition.z);
		return new(rayOrigin, Vector3.down);
	}

	private Vector3 CalculateNewFootPosition(AvatarIKGoal goal, RaycastHit hit)
	{
		Vector3 newFootPosition = hit.point;
		float originalFootLocalPositionY = animator.GetIKPosition(goal).y - animator.avatarRoot.position.y;
		switch (stepType)
		{
			case StepType.Mix :
				float smoothStepRatio = (originalFootLocalPositionY - footHeight) / (legLength / 2 - footHeight);
				Vector3 groundNormalFactor = Mathf.SmoothStep(1f, 0f, smoothStepRatio) * hit.normal;
				Vector3 worldUpFactor = Mathf.SmoothStep(0f, 1f, smoothStepRatio) * Vector3.up;
				newFootPosition += (groundNormalFactor + worldUpFactor).normalized * originalFootLocalPositionY;
				break;
			case StepType.Normal :
				newFootPosition += hit.normal * originalFootLocalPositionY;
				break;
			case StepType.Up:
				newFootPosition += Vector3.up * originalFootLocalPositionY;
				break;
		}

		if (ApplyStairIK == false)
		{
			return newFootPosition;
		}

		Vector3 footForward = animator.GetIKRotation(goal) * Vector3.forward;
		Ray frontRay = new(newFootPosition - footForward * footRearLength, footForward);
		if (Physics.Raycast(frontRay, out RaycastHit frontHit, FootLength, groundLayerMask) == true)
		{
			if (Vector3.Dot(frontHit.normal, Vector3.up) < Mathf.Acos(Mathf.Deg2Rad * 45))
			{
				Vector3 dir = (-footForward + frontHit.normal).ProjectOntoPlane(hit.normal).normalized;
				newFootPosition += dir * footFrontLength;
			}
		}

		return newFootPosition;
	}

	private Quaternion CalculateNewFootRotation(AvatarIKGoal goal, RaycastHit hit)
	{
		Quaternion originRotation = animator.GetIKRotation(goal);
		Quaternion localRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
		return localRotation * originRotation;
	}
#endregion
}
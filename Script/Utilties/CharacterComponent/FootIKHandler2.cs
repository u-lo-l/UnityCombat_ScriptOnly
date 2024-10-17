using System;
using Cinemachine.Utility;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class FootIKHandler2 : MonoBehaviour
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
	[field : SerializeField] public bool ApplyStairIKRotation { private get; set; } = true;
	[field : Header("Leg Setting")]
	[field : SerializeField] private float legLength = 0.65f;
	[SerializeField] StepType stepType = StepType.Mix;
	[Header("Foot Setting")]
	[SerializeField] private float footFrontLength = 0.14f;
	[SerializeField] private float footRearLength = 0.06f;
	private float FootLength => footFrontLength + footRearLength;

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

	private void OnAnimatorIK(int animationLayerIndex)
	{
		if (ApplyHipAdjustment == true)
		{
			SetHipPosition();
		}

		SetFootIK(AvatarIKGoal.LeftFoot);
		SetFootIK(AvatarIKGoal.RightFoot);

	}
#endregion

#region Inverse Kinematics

	private float prevHipDown = 0;
	[SerializeField] float temp = 0.1f;
	private void SetHipPosition()
	{
		float leftFootGap = GetFootDisplacement(AvatarIKGoal.LeftFoot);
		float rightFootGap = GetFootDisplacement(AvatarIKGoal.RightFoot);
		float minFootDisplacement = Mathf.Min(leftFootGap, rightFootGap);

		Vector3 bodyWorldPosition = animator.bodyPosition;
		float lowerBodyHeight = bodyWorldPosition.y - animator.avatarRoot.transform.position.y;
		float hipDownThreshold = (legLength - lowerBodyHeight) * 0.75f;

		float targetHipDownDistance  = Mathf.Min(minFootDisplacement + hipDownThreshold, 0);

		float interpolateValue = 0.1f;
		if (Math.Abs(prevHipDown - targetHipDownDistance) > 0.1f)
		{
			interpolateValue = Time.deltaTime * temp;
			Debug.Log("Step UP");
		}
		prevHipDown = Mathf.Lerp(prevHipDown, targetHipDownDistance, interpolateValue);

		if (prevHipDown < hipOffsetMin)
			prevHipDown = 0f;

		bodyWorldPosition.y += prevHipDown;
		animator.bodyPosition = bodyWorldPosition;
	}

	private void SetFootIK(AvatarIKGoal goal)
	{
		if (Physics.Raycast(CreateFootIKRay(goal), out RaycastHit hit, RayLength, groundLayerMask) == false)
		{
			return ;
		}

		Quaternion newRotation = CalculateNewFootRotation(goal, hit);
		if (ApplyFootIKRotation == true)
		{
			animator.SetIKRotationWeight(goal, 1);
			animator.SetIKRotation(goal, newRotation);
		}


		Vector3 newFootPosition = CalculateNewFootPotiion(goal, hit);
		if (ApplyFootIKPosition == true)
		{
			animator.SetIKPositionWeight(goal, 1);
			animator.SetIKPosition(goal, newFootPosition);
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

	private Vector3 CalculateNewFootPotiion(AvatarIKGoal goal, RaycastHit hit)
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

		if (ApplyStairIKRotation == false)
		{
			return newFootPosition;
		}
		Vector3 footForward = animator.GetIKRotation(goal) * Vector3.forward;

		Ray frontRay = new(newFootPosition - footForward * footRearLength, footForward);
		Ray rearRay = new(newFootPosition + footForward * footFrontLength, -footForward);

		if (Physics.Raycast(frontRay, out RaycastHit frontHit, FootLength, groundLayerMask) == true)
		{
			if (Vector3.Dot(frontHit.normal, Vector3.up) < Mathf.Acos(Mathf.Deg2Rad * 45))
			{
				// float dist = (frontHit.point - newFootPosition).magnitude - footRearLength;
				Vector3 dir = (-footForward + frontHit.normal).ProjectOntoPlane(hit.normal).normalized;
				newFootPosition += dir * footFrontLength;
			}
		}
		else if (Physics.Raycast(rearRay, out RaycastHit rearHit, FootLength, groundLayerMask) == true)
		{
			if (Vector3.Dot(frontHit.normal, Vector3.up) < Mathf.Acos(Mathf.Deg2Rad * 45))
			{
				// float dist = (rearHit.point - newFootPosition).magnitude - footFrontLength;
				Vector3 dir = (footForward + rearHit.normal).ProjectOntoPlane(hit.normal).normalized;
				newFootPosition += dir * footRearLength;
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
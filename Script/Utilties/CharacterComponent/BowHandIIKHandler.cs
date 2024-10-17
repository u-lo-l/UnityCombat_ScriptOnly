using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class BowHandIIKHandler : MonoBehaviour
{
	public Canvas AimCanvas;
	public Image AimImage;
	public bool IsTargetingMode {private get; set;} = false;
	private readonly int rightHandIKPositionWeight = Animator.StringToHash("RightHandIKPositionWeight");
	private readonly int SpinePitchRotation = Animator.StringToHash("SpinePitchRotation");
	private PlayerMovementInputHandler inputHandler;
	[field : SerializeField] public Transform BowIKTransform {private get; set;}
	[field : SerializeField] public Transform StringIKTransform {private get; set;}
	[field : SerializeField] public Transform TargetTransform {private get; set;}
	[SerializeField] private Vector3 ikOffset;
	private Animator animator;
	[SerializeField, Range(-90, 90)] private float mouseRoll = 0;
	[SerializeField, Range(-90, 90)] private float mousePitch = 0;
	public bool ApplyLookAt = true;
	private void Awake()
	{
		animator = GetComponent<Animator>();
		inputHandler = GetComponent<PlayerMovementInputHandler>();
		if (AimCanvas != null)
		{
			AimCanvas.enabled = false;
		}
	}
	private void Update()
	{
		mouseRoll = Mathf.Clamp(mouseRoll - inputHandler.MouseInputVector.y * 0.1f, -45, 45);
		mousePitch = Mathf.Clamp(mousePitch + inputHandler.MouseInputVector.x * 0.1f, -45, 45);
		if (IsTargetingMode == true)
		{
			Vector3 aimScreenPosition = AimImage.rectTransform.localPosition;
			aimScreenPosition.x = mousePitch *  AimImage.rectTransform.sizeDelta.x * 0.25f;
			aimScreenPosition.y = -mouseRoll *  AimImage.rectTransform.sizeDelta.y * 0.5f - 100;
			AimImage.rectTransform.localPosition = aimScreenPosition;
		}
	}
	private void OnEnable()
	{
		mouseRoll = 0;
		mousePitch = 0;
		// EnableAimCanvas();
	}
	private void OnDisable()
	{
		mouseRoll = 0;
		mousePitch = 0;
		DisableAimCanvas();
	}
	public float arrowRightOffset;
	public float arrowLength;
	public float bowPitchOffset = -10;
	public float bowRollOffset = -10;
	private void OnAnimatorIK(int layerIndex)
	{
		if (animator.isHuman == false || layerIndex != AnimatorHash.Player.UpperActionLayer)
		{
			return;
		}
		float handIKweight = animator.GetFloat(rightHandIKPositionWeight);
		float spineFKWeight = animator.GetFloat(SpinePitchRotation);

		float roll = mouseRoll;
		float pitch = 20 + mousePitch;

		pitch *= spineFKWeight;
		// Calculate UpperBody For FK
		Quaternion rotation1 = Quaternion.AngleAxis(roll / 3, transform.right) * Quaternion.AngleAxis(pitch / 3, Vector3.up);
		Quaternion rotation2 = Quaternion.AngleAxis(roll * 2 / 3, transform.right) * Quaternion.AngleAxis(pitch * 2 / 3 , Vector3.up);
		Quaternion rotation3 = Quaternion.AngleAxis(roll, transform.right) * Quaternion.AngleAxis(pitch, Vector3.up);
		Vector3 newSpinePosition = animator.GetBoneTransform(HumanBodyBones.Spine).position;
		Vector3 newChestPosition = animator.GetBoneTransform(HumanBodyBones.Chest).position;
		Vector3 newShoulderPosition = animator.GetBoneTransform(HumanBodyBones.LeftShoulder).position;
		Vector3 newBowHandPosition = animator.GetBoneTransform(HumanBodyBones.LeftHand).position;
		newChestPosition    = newSpinePosition    + rotation1 * (newChestPosition - newSpinePosition);
		newShoulderPosition = newChestPosition    + rotation2 * (newShoulderPosition - newChestPosition);
		newBowHandPosition  = newShoulderPosition + rotation3 * (newBowHandPosition - newShoulderPosition);

		// Calculate ArrowDirection for IK
		Vector3 arrowVector = Quaternion.Euler(bowRollOffset,bowPitchOffset,0) * (newBowHandPosition - newShoulderPosition);
		Debug.DrawLine(newSpinePosition, newChestPosition, Color.magenta);
		Debug.DrawLine(newChestPosition, newShoulderPosition, Color.blue);
		Debug.DrawLine(newShoulderPosition, newBowHandPosition, Color.green);

		Vector3 arrowHeadPosition = newBowHandPosition + transform.rotation * Quaternion.Euler(0,pitch,0) * (arrowRightOffset * Vector3.right);
		Debug.DrawLine(newBowHandPosition, arrowHeadPosition, Color.white);

		Vector3 arrowHandPosition = newBowHandPosition - arrowLength * arrowVector.normalized;
		Debug.DrawLine(arrowHandPosition, arrowHeadPosition, Color.red);

		// Apply UpperBody FK
		Vector3 rpy = new(roll / 3, pitch / 3, 0);
		animator.SetBoneLocalRotation(HumanBodyBones.Spine, GetLocalRotationUpperBodyByObjectOrientation(HumanBodyBones.Spine, rpy));
		animator.SetBoneLocalRotation(HumanBodyBones.Chest, GetLocalRotationUpperBodyByObjectOrientation(HumanBodyBones.Chest, rpy));
		animator.SetBoneLocalRotation(HumanBodyBones.LeftShoulder, GetLocalRotationUpperBodyByObjectOrientation(HumanBodyBones.LeftShoulder, rpy));
		// Apply Hand IK
		animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
		animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
		animator.SetIKRotationWeight(AvatarIKGoal.RightHand, handIKweight);
		animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handIKweight);
		animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(arrowVector));
		if (spineFKWeight < 1)
		{
			if (StringIKTransform != null)
				animator.SetIKPosition(AvatarIKGoal.RightHand, StringIKTransform.position);
		}
		else
		{
			animator.SetIKPosition(AvatarIKGoal.RightHand, arrowHandPosition);
		}

		LookAtIK(spineFKWeight);
	}
	private void LookAtIK(float weigth)
	{
		if (ApplyLookAt == false)
		{
			return ;
		}
		Vector3 lookAt = animator.GetBoneTransform(HumanBodyBones.Head).position;
		lookAt +=  transform.rotation * Quaternion.Euler(mouseRoll, mousePitch, 0) * new Vector3(0.1f,0,1).normalized;
		animator.SetLookAtWeight(weigth);
		animator.SetLookAtPosition(lookAt);
	}

	private Quaternion GetLocalRotationUpperBodyByObjectOrientation(HumanBodyBones bone, Vector3 eularAngle)
	{
		Transform boneTransform = animator.GetBoneTransform(bone);
		Matrix4x4 originToObject = this.transform.localToWorldMatrix;
		Matrix4x4 originToAvatar = boneTransform.worldToLocalMatrix * originToObject;
		Vector3 rpy =  originToAvatar * eularAngle;
		return boneTransform.localRotation * Quaternion.Euler(rpy.x , rpy.y, rpy.z);
	}

	public void EnableAimCanvas()
	{
		if (AimCanvas != null)
		{
			AimCanvas.enabled = true;
			IsTargetingMode = true;
		}
	}

	public void DisableAimCanvas()
	{
		if (AimCanvas != null)
		{
			AimCanvas.enabled = false;
			IsTargetingMode = false;
		}
	}
}
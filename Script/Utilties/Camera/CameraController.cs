using System;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public static CameraController Instance;
	private CameraInputHandler inputHandler;
	private CinemachineVirtualCamera virtualCamera;
#region Camera Movement
	[SerializeField] private float FOV = 50f;
	[SerializeField, Range(1f,10f)] private float follingCameraArmLength = 3.0f;
	[SerializeField, Range(1f,10f)] private float targetingCameraArmLength = 4.0f;
	[SerializeField, Range(1f,10f)] private float aimingCameraArmLength = 2.0f;
	[SerializeField] private float minVerticalAngle = -45.0f;
	[SerializeField] private float maxVerticalAngle = 45.0f;
	[SerializeField, Range(0.05f, 0.5f)] private float rotationSpeed = 0.1f;
	[SerializeField] private bool invertX;
	[SerializeField] private bool invertY;
	private Player player;
	private Vector3 lookAt;
	private bool isControlledByUser = true;
	private Transform followingTarget;
	[SerializeField, Range(0.05f, 200f)] private float cameraFollowingSpeed = 50f;
	[SerializeField, Range(0.05f, 200f)] private float cameraRotatingSpeed = 50f;
	[SerializeField, Range(0.05f, 200f)] private float cameraAimFollowingSpeed = 200f;
	[SerializeField, Range(0.05f, 200f)] private float cameraAimRotatingSpeed = 200f;
	[SerializeField] private float roll = -20;
	[SerializeField] private float pitch = 180;
	private float invertXVal;
	private float invertYVal;
	private Vector2 mouseInput;

	[SerializeField] Vector3 freeLookCameraOffset;
	[SerializeField] Vector3 tagetingCameraOffset;
	[SerializeField] Vector3 aimingCameraOffset;
	[SerializeField] Transform LookAt;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
		}
		else
		{
			Destroy(gameObject);
		}
		// DontDestroyOnLoad(gameObject);
	}
	private void Start()
	{
		virtualCamera.m_Lens.FieldOfView= FOV;
		virtualCamera.LookAt = LookAt;

		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		inputHandler = GetComponent<CameraInputHandler>();
		followingTarget = LookAt.transform;

		Quaternion targetOrientation = Quaternion.Euler(roll, pitch, 0);
		Vector3 targetPosition = lookAt - targetOrientation * new Vector3(0f, 0f, follingCameraArmLength) + freeLookCameraOffset;

		virtualCamera.transform.SetPositionAndRotation(targetPosition, targetOrientation);
	}

	private void Update()
	{
		if (player == null || isControlledByUser == false)
		{
			return ;
		}

		if (player.IsFreeLookMode())
		{
			FreeLookCamera();
		}
		else if (player.IsAimingMode())
		{
			TargetingCameraAimMode();
		}
		else
		{
			TargetingCamera();
		}
	}

	private void FreeLookCamera()
	{
		if (inputHandler.enabled == false)
			inputHandler.enabled = true;

		mouseInput = inputHandler.MouseInputVector;
		invertXVal = invertX == false ? -1 :  1;
		invertYVal = invertY == false ?  1 : -1;

		roll += mouseInput.y * rotationSpeed * invertXVal;
		roll = Mathf.Clamp(roll, minVerticalAngle, maxVerticalAngle);
		pitch += mouseInput.x * rotationSpeed * invertYVal;

		lookAt = followingTarget.position;

		Quaternion targetOrientation = Quaternion.Euler(roll, pitch, 0);
		Vector3 targetPosition = lookAt -
								 targetOrientation * new Vector3(0f, 0f, follingCameraArmLength) +
								 targetOrientation * freeLookCameraOffset;
		targetOrientation = Quaternion.LookRotation(lookAt - targetPosition);

		LocateCamera(targetPosition, targetOrientation, cameraRotatingSpeed / 4, cameraFollowingSpeed / 4);
	}
	private void TargetingCamera()
	{
		if (inputHandler.enabled == true)
			inputHandler.enabled = false;

		lookAt = followingTarget.position;

		Vector3 targetPosition = lookAt - player.transform.rotation * new Vector3(0f, 0f, targetingCameraArmLength);
		Quaternion targetOrientation = Quaternion.LookRotation(lookAt - targetPosition);
		targetPosition += targetOrientation * tagetingCameraOffset;
		LocateCamera(targetPosition, targetOrientation, cameraRotatingSpeed / 9, cameraFollowingSpeed / 9);

		roll = virtualCamera.transform.eulerAngles.x - 5;
		pitch = virtualCamera.transform.eulerAngles.y;
	}
	private void TargetingCameraAimMode()
	{
		if (inputHandler.enabled == false)
			inputHandler.enabled = true;

		lookAt = followingTarget.position + followingTarget.rotation * aimingCameraOffset;

		Vector3 targetPosition = lookAt - player.transform.rotation * new Vector3(0f, 0f, aimingCameraArmLength);
		Quaternion targetOrientation = Quaternion.LookRotation(lookAt - targetPosition);

		LocateCamera(targetPosition, targetOrientation, cameraAimRotatingSpeed, cameraAimFollowingSpeed);

		roll = virtualCamera.transform.eulerAngles.x - 5;
		pitch = virtualCamera.transform.eulerAngles.y;
	}
	private void LocateCamera(Vector3 position, Quaternion orientation, float rotateSpeed, float followSpeed)
	{
		float smoothRate = Time.unscaledDeltaTime * rotateSpeed;
		virtualCamera.transform.rotation = Quaternion.Slerp(virtualCamera.transform.rotation, orientation, smoothRate);

		smoothRate = Time.unscaledDeltaTime * followSpeed;
		float sqrDistance = (followingTarget.position - transform.position).sqrMagnitude;
		sqrDistance = Mathf.Max(sqrDistance, 0.025f);
		smoothRate = sqrDistance < 1 ? smoothRate / sqrDistance : smoothRate;
		virtualCamera.transform.position = Vector3.Lerp(virtualCamera.transform.position, position, smoothRate);
	}

#endregion
}

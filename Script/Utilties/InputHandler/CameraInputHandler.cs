using UnityEngine;
using UnityEngine.InputSystem;

public class CameraInputHandler : MonoBehaviour
{
	private PlayerInput playerInput;
	[SerializeField] private bool externalInputBlocked = false;
	public Vector2 MouseInputVector {get; private set;}
	[SerializeField, Range(0.1f, 10f)] private float verticalSensitivity = 1f;
	[SerializeField, Range(0.1f, 10f)] private float horizontalSensitivity = 2f;
	[SerializeField] private EscManuUI escManuUI;
	private InputActionMap mouseInputActionMap;
	private void Awake()
	{
		playerInput = GetComponent<PlayerInput>();

		mouseInputActionMap = playerInput.actions.FindActionMap("Camera");
	}

	private void Start()
	{
		if (SequenceManager.Instance != null)
		{
			SequenceManager.Instance.RegisterSequencePlayEvent(-1, ReleaseControl);
			SequenceManager.Instance.RegisterSequenceFinishEvent(-1, GainControl);
		}
		externalInputBlocked = false;
		if (escManuUI != null)
		{
			escManuUI.OnMenuActive += ReleaseControl;
			escManuUI.OnMenuDeactive += GainControl;
		}
	}

	private void OnEnable()
	{
		mouseInputActionMap.FindAction("LookAround", true).performed += OnLookAroundPerformed;
		mouseInputActionMap.FindAction("LookAround", true).canceled += OnLookAroundCanceled;
	}

	private void OnDisable()
	{
		mouseInputActionMap.FindAction("LookAround", true).performed -= OnLookAroundPerformed;
		mouseInputActionMap.FindAction("LookAround", true).canceled -= OnLookAroundCanceled;
	}
	public void ReleaseControl()
	{
		externalInputBlocked = true;
	}
	public void GainControl()
	{
		externalInputBlocked = false;
	}
	private void OnLookAroundPerformed(InputAction.CallbackContext context)
	{
		if (externalInputBlocked == true)
		{
			this.MouseInputVector = Vector2.zero;
			return ;
		}
		Vector2 temp = context.ReadValue<Vector2>();
		this.MouseInputVector = new Vector2(temp.x * horizontalSensitivity, temp.y * verticalSensitivity);
	}

	private void OnLookAroundCanceled(InputAction.CallbackContext context)
	{
		this.MouseInputVector = Vector2.zero;
	}
}

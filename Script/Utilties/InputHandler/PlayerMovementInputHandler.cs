using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMovementInputHandler : MonoBehaviour
{
	[SerializeField] private bool externalInputBlocked = false;
	private PlayerInput playerInput;
	private Vector2 keyboardInputVector = Vector2.zero;
	private Vector2 mouseInputVector = Vector2.zero;
	[HideInInspector] public ref Vector2 KeyboardInputVector => ref keyboardInputVector;
	[HideInInspector] public ref Vector2 MouseInputVector => ref mouseInputVector;
	[SerializeField] private EscManuUI escManuUI;
	public event Action OnJump;
	public event Action OnRunToggle;
	public event Action OnDodge;
	public event Action OnDoSprint;
	public event Action OnStopSprint;
	public event Action OnTarget;
	private void Awake()
	{
		playerInput = GetComponent<PlayerInput>();
		SetInputAction();
	}
	private void Start()
	{
		externalInputBlocked = false;
		keyboardInputVector = Vector2.zero;
		SequenceManager.Instance?.RegisterSequencePlayEvent(-1, ReleaseControl);
		SequenceManager.Instance?.RegisterSequenceFinishEvent(-1, GainControl);
		if (escManuUI != null)
		{
			escManuUI.OnMenuActive += ReleaseControl;
			escManuUI.OnMenuDeactive += GainControl;
		}
	}

#region Input Action
	public void ReleaseControl()
	{
		externalInputBlocked = true;
		keyboardInputVector = Vector2.zero;
	}
	public void GainControl()
	{
		externalInputBlocked = false;
	}
	private void SetInputAction()
	{
		InputActionMap actionMap = playerInput.actions.FindActionMap("Player");

		actionMap.FindAction("Movement", true).performed += OnMovementPerformed;
		actionMap.FindAction("Movement", true).canceled += OnMovementCanceled;

		actionMap.FindAction("RunToggle", true).started += OnRunToggleStarted;
		actionMap.FindAction("Jump", true).started += OnJumpStarted;
		actionMap.FindAction("Sprint", true).started += OnSprintStarted;
		actionMap.FindAction("Sprint", true).canceled += OnSprintCanceled;
		actionMap.FindAction("Dodge", true).started += OnDodgeStarted;

		actionMap.FindAction("TargetingToggle", true).started += OnTargetPerformed;
		actionMap.FindAction("Aim", true).performed += OnAimPerformed;
	}
	private void OnMovementPerformed(InputAction.CallbackContext context)
	{
		if (externalInputBlocked == true)
		{
			this.KeyboardInputVector = Vector2.zero;
			return ;
		}
		this.keyboardInputVector = context.ReadValue<Vector2>().normalized;
	}
	private void OnMovementCanceled(InputAction.CallbackContext context)
	{
		this.keyboardInputVector = Vector2.zero;
	}

	private void OnAimPerformed(InputAction.CallbackContext context)
	{
		if (externalInputBlocked == true)
		{
			this.mouseInputVector = Vector2.zero;
			return ;
		}
		this.mouseInputVector = context.ReadValue<Vector2>();
	}


	private void OnRunToggleStarted(InputAction.CallbackContext context)
	{
		if (externalInputBlocked == true)
			return ;
		OnRunToggle?.Invoke();
	}
	private void OnJumpStarted(InputAction.CallbackContext context)
	{
		if (externalInputBlocked == true)
			return ;
		OnJump?.Invoke();
	}
	private void OnDodgeStarted(InputAction.CallbackContext context)
	{
		if (externalInputBlocked == true)
			return ;
		OnDodge?.Invoke();
	}
	private void OnSprintStarted(InputAction.CallbackContext context)
	{
		if (externalInputBlocked == true)
			return ;
		OnDoSprint?.Invoke();
	}
	private void OnSprintCanceled(InputAction.CallbackContext context)
	{
		OnStopSprint?.Invoke();
	}

#endregion
#region Targetting System
	public void OnTargetPerformed(InputAction.CallbackContext context)
	{
		OnTarget?.Invoke();
	}
#endregion
	public string KeyboardInputDirection()
	{
		if (externalInputBlocked == true)
			return " ";
		if (KeyboardInputVector.x > 0 && KeyboardInputVector.y == 0) // d
			return  "→";
		else if (KeyboardInputVector.x > 0 && KeyboardInputVector.y > 0) // wd
			return  "↗";
		else if (KeyboardInputVector.x > 0 && KeyboardInputVector.y < 0) // sd
			return  "↘";
		else if (KeyboardInputVector.x < 0 && KeyboardInputVector.y == 0) // a
			return  "←";
		else if (KeyboardInputVector.x < 0 && KeyboardInputVector.y > 0) // wa
			return  "↖";
		else if (KeyboardInputVector.x < 0 && KeyboardInputVector.y < 0) // sa
			return  "↙";
		else if (KeyboardInputVector.x == 0 && KeyboardInputVector.y > 0) // w
			return  "↑";
		else if (KeyboardInputVector.x == 0 && KeyboardInputVector.y < 0) // s
			return  "↓";
		else
			return " ";
	}

}

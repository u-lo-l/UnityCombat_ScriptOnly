using UnityEngine;
using UnityEngine.Rendering;

public abstract class PlayerMovementState : IState
{
	private Camera freeLookCamera = null;
	public enum State {None, Idle, Walk, Run, Sprint, Jump, Air, Dodge, Land};
	public State CurrentState {get; protected set;}
	protected PlayerMovementStateMachine movementStateMachine;
	protected Animator animator;
	protected Vector2 movementInput;
	protected Quaternion currentRotation;
	protected Quaternion targetRotation;
	protected float rotationSpeed = 20f;

	#region IState Methods
	protected PlayerMovementState(PlayerMovementStateMachine stateMachine)
	{
		movementStateMachine = stateMachine;
		animator = stateMachine.player.Animator;
	}
	public virtual void Enter()
	{
		if (freeLookCamera == null)
			freeLookCamera = Camera.main;
	}
	public virtual void Tick()
	{
		GetInputVector();
	}
	public virtual void FixedTick()	{}
	public virtual void Exit()	{}
	#endregion

	#region Main Methods
	// protected virtual void Move()
	// {
	// }
	private void GetInputVector()
	{
		movementInput = movementStateMachine.MovementInputHandler.KeyboardInputVector;
	}
	/// <summary>
	/// FreeLookState에서 플레이어가 이동할 시 targetRotation을 카메라가 바라보는 방향으로 정하고 해당 뱡항으로 회전시킨다.
	/// </summary>
	protected void RotateByCamera()
	{
		Player player = movementStateMachine.player;

		if (movementInput == Vector2.zero)
			return ;
		Vector3 cameraForward = new Vector3(freeLookCamera.transform.forward.x, 0, freeLookCamera.transform.forward.z).normalized;
		Vector3 cameraRight = new Vector3(freeLookCamera.transform.right.x, 0, freeLookCamera.transform.right.z).normalized;
		Vector3 targetDirection = (cameraForward * movementInput.y + cameraRight * movementInput.x).normalized;
		targetDirection.y = 0;

		currentRotation = player.transform.rotation;
		targetRotation = Quaternion.LookRotation(targetDirection);

		float rotationLerpRate = Time.unscaledDeltaTime * rotationSpeed / (player.CharacterController.velocity.magnitude + 1);
		Quaternion newRoation = Quaternion.Slerp(currentRotation, targetRotation, rotationLerpRate);
		player.transform.rotation = newRoation;
	}

	protected void RotateByTarget()
	{
		Debug.Assert(movementStateMachine.player.TargetTransform != null, "Targeting mode Error : Target Not Found");

		Player player = movementStateMachine.player;
		Vector3 targetDirection = player.TargetTransform.position - player.transform.position;
		targetDirection.y = 0;

		currentRotation = movementStateMachine.player.transform.rotation;
		targetRotation = Quaternion.LookRotation(targetDirection);

		float rotationLerpRate = Time.unscaledDeltaTime * rotationSpeed / player.CharacterController.velocity.magnitude;
		Quaternion newRoation = Quaternion.Slerp(currentRotation, targetRotation, rotationLerpRate);
		movementStateMachine.player.transform.rotation = newRoation;
	}
	#endregion

	#region Switch State
	protected void SwitchToIdlingState()
	{
		movementStateMachine.ChangeState(movementStateMachine.IdlingState);
	}
	protected void SwitchToWalkingState()
	{
		movementStateMachine.ChangeState(movementStateMachine.WalkingState);
	}
	protected void SwitchToRunningState()
	{
		movementStateMachine.ChangeState(movementStateMachine.RunningState);
	}
	protected void SwitchToSprintingState()
	{
		movementStateMachine.ChangeState(movementStateMachine.SprintingState);
	}
	protected void SwitchToDodgingState()
	{
		movementStateMachine.ChangeState(movementStateMachine.DodgingState);
	}
	protected void SwitchToJumpingState()
	{
		movementStateMachine.ChangeState(movementStateMachine.JumpingState);
	}
	protected void SwitchToAirborneState()
	{
		movementStateMachine.ChangeState(movementStateMachine.AirborneState);
	}
	protected void SwitchToLandingState()
	{
		movementStateMachine.ChangeState(movementStateMachine.LandingState);
	}
	#endregion
}
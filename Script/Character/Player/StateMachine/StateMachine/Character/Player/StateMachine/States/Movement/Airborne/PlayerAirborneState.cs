using UnityEngine;

public class PlayerAirborneState : PlayerMovementState
{
	protected Vector3 momentum;
	protected FootIKHandler footIKHandler;
	public PlayerAirborneState(PlayerMovementStateMachine stateMachine)
	 : base(stateMachine)
	{
		footIKHandler = stateMachine.player.GetComponent<FootIKHandler>();
		CurrentState = State.Air;
	}

	public override void Enter()
	{
		base.Enter();
		if (footIKHandler != null)
		{
			footIKHandler.ApplyFootIKPosition = false;
			footIKHandler.ApplyFootIKRotation = false;
			footIKHandler.ApplyHipAdjustment = false;
			footIKHandler.ApplyStairIK = false;
		}

		momentum = movementStateMachine.player.CharacterController.velocity * Time.timeScale;;
		movementStateMachine.player.Animator.CrossFadeInFixedTime("Falling", 0.7f);
		movementStateMachine.player.Animator.SetFloat(AnimatorHash.Player.SpeedZ, 0f);
		movementStateMachine.player.Animator.SetFloat(AnimatorHash.Player.SpeedX, 0f);
	}
	public override void Tick()
	{
		if (movementStateMachine.player.EnvironmentChecker.IsGrounded == true)
		{
			SwitchToIdlingState();
		}
		if (movementStateMachine.player.IsFreeLookMode())
			RotateByCamera();
		else
			RotateByTarget();

		movementInput = movementStateMachine.MovementInputHandler.KeyboardInputVector;
		momentum += movementStateMachine.player.transform.forward * movementInput.magnitude * 4f * Time.unscaledDeltaTime;

		movementStateMachine.Move(momentum);
	}
	public override void Exit()
	{
		base.Exit();
	}
}
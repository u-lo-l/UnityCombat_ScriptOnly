using UnityEngine;

public class PlayerJumpingState : PlayerAirborneState
{
	float jumpForce = 4;
	public PlayerJumpingState(PlayerMovementStateMachine stateMachine)
	 : base(stateMachine)
	{
		CurrentState = State.Jump;
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
		movementStateMachine.player.Animator.CrossFadeInFixedTime("Jump", 0.1f);
		movementStateMachine.player.Animator.SetFloat(AnimatorHash.Player.SpeedZ, 0f);
		movementStateMachine.player.Animator.SetFloat(AnimatorHash.Player.SpeedX, 0f);
		movementStateMachine.player.ForceReceiver.Jump(jumpForce);
	}

	public override void Tick()
	{
		if (movementStateMachine.player.ForceReceiver.IsMovingDown == true)
		{
			SwitchToAirborneState();
		}
		if (movementStateMachine.player.IsFreeLookMode())
			RotateByCamera();
		else
			RotateByTarget();
		movementInput = movementStateMachine.MovementInputHandler.KeyboardInputVector;
		momentum += movementInput.magnitude * Time.unscaledDeltaTime * movementStateMachine.player.transform.forward;

		movementStateMachine.Move(momentum);
	}
	public override void Exit()
	{
	}
}
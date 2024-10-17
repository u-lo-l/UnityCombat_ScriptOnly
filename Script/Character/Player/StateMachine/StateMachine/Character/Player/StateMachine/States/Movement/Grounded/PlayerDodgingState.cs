using UnityEngine;

public class PlayerDodgingState : PlayerGroundedState
{
	private Vector3 momentum;
	public PlayerDodgingState(PlayerMovementStateMachine stateMachine) : base(stateMachine)
	{
		CurrentState = State.Dodge;
	}
	public override void Enter()
	{
		if (movementStateMachine.player.IsFreeLookMode())
			FreeLookDodge();
		else
			TargetingDodge();
	}
	public override void Tick()
	{
		movementStateMachine.Move(momentum);
		if (movementStateMachine.IsTargetingMode() == true)
			RotateByTarget();
	}
	public override void Exit()
	{
		targetSpeed = 0f;
		movementStateMachine.player.MeshTrail.IsActive = false;
		movementStateMachine.CurrentSpeed_ref = 0f;
		base.Exit();
	}
	protected override float SetMovementSpeed()
	{
		return 0;
	}

	private void FreeLookDodge()
	{
		movementStateMachine.player.Animator.SetFloat(AnimatorHash.Player.SpeedZ, 0);

		momentum = movementStateMachine.player.CharacterController.velocity.normalized * targetSpeed;

		Vector2 inputVector = movementStateMachine.MovementInputHandler.KeyboardInputVector;

		if (inputVector == Vector2.zero)
		{
			inputVector = movementStateMachine.player.transform.forward;
			momentum = inputVector * 1.5f;
		}
		movementStateMachine.player.Animator.SetTrigger(AnimatorHash.Player.DodgeTrigger);
	}
	private void TargetingDodge()
	{
		Transform playerTransform = movementStateMachine.player.transform;
		Vector2 inputVector = movementStateMachine.MovementInputHandler.KeyboardInputVector;
		Vector3 dodgeDirection;
		int dodgeHash;
		if (inputVector.x > 0) // Right
		{
			dodgeDirection = playerTransform.right;
			dodgeHash = Animator.StringToHash("DodgeTrigger_R");
		}
		else if (inputVector.x < 0) // Left
		{
			dodgeDirection = -playerTransform.right;
			dodgeHash = Animator.StringToHash("DodgeTrigger_L");
		}
		else if (inputVector.y > 0) // forward
		{
			dodgeDirection = playerTransform.forward;
			dodgeHash = Animator.StringToHash("DodgeTrigger_F");
		}
		else // back or none
		{
			dodgeDirection = -playerTransform.forward;
			dodgeHash = Animator.StringToHash("DodgeTrigger_B");
		}
		momentum = dodgeDirection * 1.5f;
		movementStateMachine.player.Animator.SetTrigger(dodgeHash);
	}
}
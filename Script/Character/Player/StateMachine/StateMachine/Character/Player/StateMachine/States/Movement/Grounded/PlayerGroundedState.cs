using UnityEngine;

public abstract class PlayerGroundedState : PlayerMovementState
{
	private readonly float acceleration = 8f;
	private FootIKHandler footIKHandler;
	protected float targetSpeed;
	protected PlayerGroundedState(PlayerMovementStateMachine stateMachine) : base(stateMachine)
	{
		footIKHandler = stateMachine.player.GetComponent<FootIKHandler>();
	}
	public override void Enter()
	{
		base.Enter();
		if (footIKHandler != null)
		{
			footIKHandler.ApplyFootIKPosition = true;
			footIKHandler.ApplyFootIKRotation = true;
			footIKHandler.ApplyHipAdjustment = true;
			footIKHandler.ApplyStairIK = true;
		}
	}

	public override void Tick()
	{
		targetSpeed = SetMovementSpeed();
		base.Tick();
		if (movementStateMachine.player.EnvironmentChecker.IsGrounded == false)
		{
			SwitchToAirborneState();
			return ;
		}
		if (movementStateMachine.player.IsFreeLookMode())
		{
			RotateByCamera();
			MoveForward();
		}
		else
		{
			RotateByTarget();
			MoveAroundTarget();
		}
	}

	public override void FixedTick()
	{
		base.FixedTick();
	}

	public override void Exit()
	{
		base.Exit();
	}

	protected void MoveForward()
	{
		ref float currentSpeed = ref movementStateMachine.CurrentSpeed_ref;
		if (Mathf.Abs(currentSpeed - targetSpeed) < 0.05f)
		{
			currentSpeed = targetSpeed;
		}
		else
		{
			currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.unscaledDeltaTime * acceleration);
		}
		Vector3 velocity = GetForwardDirection() * currentSpeed;

		movementStateMachine.Move(velocity);
		AnimateRealVelocity();
	}

	protected void MoveAroundTarget()
	{
		ref float currentSpeed = ref movementStateMachine.CurrentSpeed_ref;
		if (Mathf.Abs(currentSpeed - targetSpeed) < 0.05f)
		{
			currentSpeed = targetSpeed;
		}
		else
		{
			currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.unscaledDeltaTime * acceleration);
		}
		Vector3 velocity = GetTargetAroundDirection() * currentSpeed;
		float speedZ = Vector3.Dot(velocity, movementStateMachine.player.EnvironmentChecker.FixedForward);
		float speedX = Vector3.Dot(velocity, movementStateMachine.player.transform.right) * 0.8f;

		velocity = speedZ * movementStateMachine.player.EnvironmentChecker.FixedForward +
				   speedX * movementStateMachine.player.transform.right;

		movementStateMachine.Move(velocity);
		AnimateRealVelocity();
	}

	private void AnimateRealVelocity()
	{
		Player player = movementStateMachine.player;
		Vector3 realVelocity = player.CharacterController.velocity;
		float realSpeedZ = Vector3.Dot(realVelocity, player.EnvironmentChecker.FixedForward);
		float realSpeedX = Vector3.Dot(realVelocity, player.transform.right);
		realSpeedZ = Mathf.Abs(realSpeedZ) < 0.001 ? 0 : realSpeedZ * Time.timeScale;
		realSpeedX = Mathf.Abs(realSpeedX) < 0.001 ? 0 : realSpeedX * Time.timeScale;
		movementStateMachine.player.Animator.SetFloat(AnimatorHash.Player.SpeedZ, realSpeedZ);
		movementStateMachine.player.Animator.SetFloat(AnimatorHash.Player.SpeedX, realSpeedX);
	}

	private Vector3 GetForwardDirection()
	{
		return movementStateMachine.player.EnvironmentChecker.FixedForward;
	}
	private Vector3 GetTargetAroundDirection()
	{
		Vector3 forwardVector = movementInput.y * movementStateMachine.player.EnvironmentChecker.FixedForward;
		Vector3 sideVector = movementInput.x * movementStateMachine.player.transform.right;
		Vector3 result = (forwardVector + sideVector).normalized;
		return result;
	}
	protected abstract float SetMovementSpeed();
}
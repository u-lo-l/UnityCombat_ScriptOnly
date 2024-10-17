using UnityEngine;

public class PlayerIdlingState : PlayerGroundedState
{
	public PlayerIdlingState(PlayerMovementStateMachine stateMachine)
	 : base(stateMachine)
	{
	}
	public override void Enter()
	{
		movementStateMachine.player.MeshTrail.IsActive = false;
		movementStateMachine.player.PlayLocomotion();
		base.Enter();
		CurrentState = State.Idle;
	}
	public override void Tick()
	{
		if (movementStateMachine.CanMove == true)
		{
			base.Tick();
			if (movementInput != Vector2.zero)
			{
				if (movementStateMachine.ShouldRun == false)
				{
					SwitchToWalkingState();
					return ;
				}
				else
				{
					SwitchToRunningState();
					return ;
				}
			}
		}
		else
		{
			base.MoveForward(); //천천히 정지하기 위함.
		}
	}

	protected override float SetMovementSpeed()
	{
		return 0;
	}
}
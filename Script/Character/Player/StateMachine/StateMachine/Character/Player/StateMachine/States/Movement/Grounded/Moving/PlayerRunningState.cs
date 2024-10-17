using UnityEngine;

public class PlayerRunningState : PlayerGroundedState
{
	public PlayerRunningState(PlayerMovementStateMachine stateMachine)
	 : base(stateMachine)
	{
	}

	public override void Enter()
	{
		base.Enter();
		CurrentState = State.Run;
	}
	public override void Tick()
	{
		base.Tick();
		if (movementStateMachine.CanMove == false || movementInput == Vector2.zero)
		{
			SwitchToIdlingState();
			return ;
		}
		else if (movementStateMachine.ShouldRun == false)
		{
			SwitchToWalkingState();
			return ;
		}
		else if (movementStateMachine.ShouldSprint == true && movementStateMachine.player.IsFreeLookMode())
		{
			SwitchToSprintingState();
			return ;
		}
	}
	public override void Exit()
	{
		base.Exit();
	}
	protected override float SetMovementSpeed()
	{
		return movementStateMachine.player.CharacterStatus.RunSpeed;
	}
}
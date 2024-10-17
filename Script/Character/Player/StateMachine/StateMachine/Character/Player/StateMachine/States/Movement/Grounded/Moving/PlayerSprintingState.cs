using UnityEngine;

public class PlayerSprintingState : PlayerGroundedState
{
	public PlayerSprintingState(PlayerMovementStateMachine stateMachine)
	 : base(stateMachine)
	{
	}

	public override void Enter()
	{
		base.Enter();
		CurrentState = State.Sprint;
	}
	public override void Tick()
	{
		base.Tick();
		if (movementInput == Vector2.zero)
		{
			SwitchToIdlingState();
			return ;
		}
		else if (movementStateMachine.ShouldRun == false)
		{
			SwitchToWalkingState();
			return ;
		}
		else if (movementStateMachine.ShouldSprint == false || movementStateMachine.player.IsTargetingMode())
		{
			SwitchToRunningState();
			return ;
		}
	}
	public override void FixedTick()
	{
	}
	public override void Exit()
	{
		base.Exit();
	}
	protected override float SetMovementSpeed()
	{
		return movementStateMachine.player.CharacterStatus.SprintSpeed;
	}
}
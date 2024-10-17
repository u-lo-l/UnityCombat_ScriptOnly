using UnityEngine;

public class PlayerWalkingState : PlayerGroundedState
{
	public PlayerWalkingState(PlayerMovementStateMachine stateMachine)
	 : base(stateMachine)
	{
	}

	public override void Enter()
	{
		base.Enter();
		CurrentState = State.Walk;
	}
	public override void Tick()
	{
		base.Tick();
		if (movementStateMachine.CanMove == false || movementInput == Vector2.zero)
		{
			SwitchToIdlingState();
			return ;
		}
		else if (movementStateMachine.ShouldRun == true)
		{
			SwitchToRunningState();
			return ;
		}
	}

	public override void Exit()
	{
		base.Exit();
	}
	protected override float SetMovementSpeed()
	{
		return movementStateMachine.player.CharacterStatus.WalkSpeed;
	}
}
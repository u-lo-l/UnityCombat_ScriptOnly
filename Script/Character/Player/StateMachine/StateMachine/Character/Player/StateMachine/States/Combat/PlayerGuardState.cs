using UnityEngine;

public class PlayerGuardState : PlayerCombatState
{
	public PlayerGuardState(PlayerCombatStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		CurrentState = State.Guard;
		
	}
	public override void Tick()
	{
		
	}
	public override void FixedTick()
	{
		
	}
	public override void Exit()
	{
		
	}
}
using System;
using UnityEngine;

[Serializable]
public abstract class AICombatState : IState
{
	public enum State { Hold, Equip, Damage, Down, GetUp, Dead, Attack, Guard };
	public State CurrentState {get; protected set;}
	protected AICombatStateMachine combatStateMachine;
	protected Animator animator;
	protected AICombatState(AICombatStateMachine stateMachine)
	{
		combatStateMachine = stateMachine;
		animator = stateMachine.Enemy.Animator;
	}

	public virtual void Enter()
	{
		
	}
	public virtual void Tick()
	{
		
	}
	public virtual void FixedTick()
	{
		
	}
	public virtual void Exit()
	{
		
	}

	protected virtual void SwitchToHoldingState()
	{
		combatStateMachine.ChangeState(combatStateMachine.HoldingState);
	}
	protected virtual void SwitchToEquipingState(int weaponIndex)
	{
		combatStateMachine.ChangeState(new AIEquipState(combatStateMachine));
	}
	protected virtual void SwitchToAttackinState()
	{
		combatStateMachine.ChangeState(combatStateMachine.ActionState);
	}
	protected virtual void SwitchToSkillState()
	{
		combatStateMachine.ChangeState(combatStateMachine.SkillState);
	}
	protected virtual void SwitchToGuardingState()
	{
		combatStateMachine.ChangeState(combatStateMachine.GuardingState);
	}
	protected virtual void SwitchToDownState(float duration = 1f)
	{
		combatStateMachine.DownedState.Duration = duration;
		combatStateMachine.ChangeState(combatStateMachine.DownedState);
	}
	protected virtual void SwitchToAirborneState(float duration = 1f)
	{
		combatStateMachine.AirborneState.Duration = duration;
		combatStateMachine.ChangeState(combatStateMachine.DownedState);
	}
}
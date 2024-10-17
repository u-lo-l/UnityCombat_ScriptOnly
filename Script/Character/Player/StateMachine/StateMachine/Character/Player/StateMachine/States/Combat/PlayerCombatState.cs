using UnityEngine;

public abstract class PlayerCombatState : IState
{
	public enum State { Hold, Equip, Damage, FastAttack, StrongAttack, Action, Skill, Guard };
	public State CurrentState {get; protected set;}
	protected PlayerCombatStateMachine combatStateMachine;
	protected PlayerCombatState(PlayerCombatStateMachine stateMachine)
	{
		combatStateMachine = stateMachine;
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
	// protected virtual void SwitchToHoldingState()
	// {
	// 	combatStateMachine.ChangeState(combatStateMachine.HoldingState);
	// }
	// protected virtual void SwitchToEquipingState(int weaponIndex)
	// {
	// 	combatStateMachine.EquippingState.WeaponIndex = weaponIndex;
	// 	combatStateMachine.ChangeState(combatStateMachine.EquippingState);
	// }
	// protected virtual void SwitchToAttackinState()
	// {
	// 	combatStateMachine.ChangeState(combatStateMachine.FastActionState);
	// }
	// protected virtual void SwitchToGuardingState()
	// {
	// 	combatStateMachine.ChangeState(combatStateMachine.GuardingState);
	// }
}
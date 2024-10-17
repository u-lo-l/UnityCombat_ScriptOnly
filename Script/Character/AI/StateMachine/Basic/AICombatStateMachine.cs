using System;
using UnityEngine;
using static PlayerCombatInputHandler;

// Hold, Attack, Damage, Guard
[Serializable]
public class AICombatStateMachine : StateMachine
{
	public EnemyDynamic Enemy { get; protected set; }
	public WeaponHandler WeaponHandler {get; private set;}
	public AIHoldState HoldingState {get; protected set;}
	public AIEquipState EquippingState {get; protected set;}
	public AIActionState ActionState {get; protected set;}
	public AISkillState SkillState {get; protected set;}
	public AIGuardState GuardingState {get; protected set;}
	public AIDamageState DamagingState {get; protected set;}
	public DownState DownedState {get; protected set;}
	public AirborneState AirborneState {get; protected set;}
	public GetUpState GettingUpState {get; protected set;}

	public AIDeadState DeadState {get; protected set;}

	public AICombatStateMachine(EnemyDynamic enemy, WeaponHandler weaponHandler)
	{
		Enemy = enemy;
		WeaponHandler = weaponHandler;
		HoldingState = new AIHoldState(this);
		EquippingState = new AIEquipState(this);
		ActionState = new AIActionState(this);
		SkillState = new AISkillState(this);
		GuardingState = new AIGuardState(this);
		DamagingState = new AIDamageState(this);
		DeadState = new AIDeadState(this);
		DownedState = new DownState(this);
		AirborneState = new AirborneState(this);
		GettingUpState = new GetUpState(this);
		currentState = HoldingState;
	}
	public override void Tick()
	{
		base.Tick();
	}

	public virtual void ChangeState(AICombatState newState)
	{
		// Debug.Log($"[Combat] : Changing State from {currentState} to {newState}");
		Enemy.InvokeOnCombatStateChanged(newState.CurrentState.ToString());

		if (base.currentState == ActionState && newState == ActionState)
		{
			if (WeaponHandler.CanNextCombo == false)
			{
				Debug.Log($"[AICombatStateMachine] : CanNextCombo is false");
				return;
			}
			ActionState.Exit();
			ActionState.Enter();
			return ;
		}
		else if (base.currentState != ActionState && newState == ActionState)
		{
			if (WeaponHandler.CanAttack == false)
				return;
		}

		if (base.currentState == DownedState && newState == DownedState)
		{
			DownedState.Duration += 0.05f;
			Enemy.Animator.Play(AnimatorHash.Enemy.Down, AnimatorHash.Enemy.DownLayer, 0.28f);
			return ;
		}

		if(newState == HoldingState && currentState != HoldingState)
		{
			Enemy.PlayLocomotion();
		}
		else if (base.currentState == ActionState)
		{
			WeaponHandler.ResetAttackIndex();
		}
		base.ChangeState(newState);
	}
	public AICombatState.State GetCurrentState()
	{
		AICombatState combatState = currentState as AICombatState;
		return combatState == null ? AICombatState.State.Hold : combatState.CurrentState;
	}

	public bool TryEquip()
	{
		return false;
	}

	public virtual bool CanAttack(int attackIndex)
	{
		if (WeaponHandler.ArmedType == WeaponType.Unarmed)
			return false;
		if (GetCurrentState() == AICombatState.State.Hold)
		{
			return WeaponHandler.CanAttack;
		}
		if (GetCurrentState() == AICombatState.State.Attack && WeaponHandler.CanNextCombo == true)
		{
			bool condition1 = WeaponHandler.HasNextCombo(AttackType.FastAttack) == true;
			bool condition2 = attackIndex == -1 || WeaponHandler.ActionIndex + 1 == attackIndex;
			if (condition1 && condition2)
				return true;
		}
		return false;
	}
	public virtual bool TryAttack(int attackIndex)
	{
		if (Enemy.GetTargetDistance() > WeaponHandler.WeaponRange)
		{
			Debug.Log($"[AICombatStateMachine] : player out of attack range");
			return false;
		}
		if (CanAttack(attackIndex) == false)
		{
			// Debug.Log($"[AICombatStateMachine] : cannot attack");
			return false;
		}
		WeaponHandler.SetAttackIndex(attackIndex);
		ChangeState(ActionState);
		return true;
	}

	public bool OnDamage()
	{
		if (GetCurrentState() == AICombatState.State.Damage)
			return false;
		ChangeState(DamagingState);
		return true;
	}
	public bool ShouldStop()
	{
		return WeaponHandler.ShouldStopForAttack() || GetCurrentState() == AICombatState.State.Damage;
	}
}
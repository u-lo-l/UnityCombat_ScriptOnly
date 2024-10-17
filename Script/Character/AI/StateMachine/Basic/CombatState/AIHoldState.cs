using UnityEngine;

public class AIHoldState : AICombatState
{
	protected const float WaitTime = 0.1f;
	public float Duration;
	protected bool waitMode = false;
	protected bool IsArmed => combatStateMachine.WeaponHandler.ArmedType != WeaponType.Unarmed;
	protected SkillHandler skillHandler;
	public AIHoldState(AICombatStateMachine stateMachine) : base(stateMachine)
	{
		CurrentState = State.Hold;
		combatStateMachine = stateMachine;
	}
	public override void Enter()
	{
		combatStateMachine.Enemy.LayerFadeOut(animator, AnimatorHash.Enemy.ActionLayer, 0.5f);
		combatStateMachine.WeaponHandler.ResetAttackIndex();
		if (combatStateMachine.Enemy.CanMove == false)
		{
			Duration = WaitTime;
		}
		waitMode = Duration > 0;
	}
	public override void Tick()
	{
		WaitForDelay();

		
		Transform playerTransform = combatStateMachine.Enemy.GetTargetTransform();
		if (IsArmed == true)
		{
			if (playerTransform == null)
			{
				SwitchToEquipingState(weaponIndex : 2); // Unequip;
				return ;
			}
			float distance = (combatStateMachine.Enemy.transform.position - playerTransform.position).sqrMagnitude;
			if (distance < combatStateMachine.Enemy.AttackRange() * combatStateMachine.Enemy.AttackRange())
			{
				if (Random.Range(0,4) == 0 && combatStateMachine.WeaponHandler.CanSkill(0) == true)
					SwitchToSkillState();
				else
					combatStateMachine.TryAttack(0);
			}
		}
		else
		{
			if (playerTransform != null)
			{
				SwitchToEquipingState(weaponIndex : 2); // Equip
			}
		}
	}
	public override void FixedTick()
	{
		
	}
	public override void Exit()
	{
		
	}
	protected bool WaitForDelay()
	{
		if (waitMode == true)
		{
			if (Duration <= 0)
			{
				Duration = 0;
				waitMode = false;
				combatStateMachine.Enemy.CanMove = true;
				return false;
			}
			Duration -= Time.deltaTime;
			return true;
		}
		return false;
	}
}
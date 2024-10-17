using UnityEngine;

public class DefensiveHoldState : AIHoldState
{
	public DefensiveHoldState(AICombatStateMachine stateMachine) : base(stateMachine)
	{
		CurrentState = State.Hold;
		combatStateMachine = stateMachine;
	}
	public override void Enter()
	{
		Debug.Log("Enter Defensive Hold");
		base.Enter();
	}
	public override void Tick()
	{
		if (waitMode == true)
		{
			if (Duration <= 0)
			{
				waitMode = false;
				combatStateMachine.Enemy.CanMove = true;
			}
			Duration -= Time.deltaTime;
			return;
		}

		bool isArmed = combatStateMachine.WeaponHandler.ArmedType != WeaponType.Unarmed;
		Transform playerTransform = combatStateMachine.Enemy.GetTargetTransform();

		if (playerTransform == null)
		{
			if (isArmed == true)
			{
				combatStateMachine.ChangeState(combatStateMachine.EquippingState);
				return ;
			}
		}
		else // playerTransform exist
		{
			float distance = (playerTransform.position - combatStateMachine.Enemy.transform.position).magnitude;
			if (isArmed == false)
			{
				if (combatStateMachine.WeaponHandler.WeaponRange > distance)
				{
					// SwitchToRunAwayState();
				}
				else
				{
					combatStateMachine.ChangeState(combatStateMachine.EquippingState);
				}
			}
			else
			{
				if (combatStateMachine.WeaponHandler.WeaponRange > distance)
				{
					combatStateMachine.ChangeState(combatStateMachine.ActionState);
				}
				else
				{
					combatStateMachine.ChangeState(combatStateMachine.GuardingState);
				}
			}
		}
	}
	public override void FixedTick()
	{
		
	}
	public override void Exit()
	{
		
	}
}
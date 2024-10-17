using System;
using UnityEngine;
using static PlayerCombatInputHandler;

public class BossCombatStateMachine : AICombatStateMachine
{
	public BossCombatStateMachine(EnemyDynamic enemy, WeaponHandler weaponHandler)
	 : base(enemy, weaponHandler)
	{
		Enemy = enemy as EnemyBoss;
		HoldingState = new BossHoldState(this);
		EquippingState = null;
		ActionState = new BossActionState(this);
		GuardingState = new BossGuardState(this);
		DamagingState = new BossDamageState(this);
		DeadState = new BossDeadState(this);
		currentState = HoldingState;
	}
	public bool CanAction = true;
	public void ForceHold()
	{
		CanAction = false;
	}
	public void ReleaseHold()
	{
		CanAction = true;
	}
	public override void ChangeState(AICombatState newState)
	{
		Debug.Log($"[BOSS COMBAT Combat] : Changing State from {currentState} to {newState}");
		Enemy.InvokeOnCombatStateChanged(newState.CurrentState.ToString());

		if (base.currentState == ActionState && newState == ActionState)
		{
			if (WeaponHandler.CanNextCombo == false)
			{
				return;
			}
			ActionState.Exit();
			ActionState.Enter();
			return ;
		}
		// else if (base.currentState != ActionState && newState == ActionState)
		// {
		// 	if (CanAttack(WeaponHandler.ActionIndex) == false)
		// 		return;
		// }

		if (base.currentState == DownedState && newState == DownedState)
		{
			DownedState.Duration += 0.5f;
			Enemy.Animator.Play(AnimatorHash.Enemy.Down, AnimatorHash.Enemy.DamagedLayer, 0.28f);
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
		if (newState is AIDeadState)
			Debug.Log("IS Already Dead");
		if (currentState == newState)
			return ;
		currentState?.Exit();
		currentState = newState;
		currentState.Enter();
	}
	public override bool CanAttack(int attackIndex)
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
	public override bool TryAttack(int attackIndex = 0)
	{
		if (CanAttack(attackIndex) == false)
		{
			return false;
		}
		if ((Enemy as EnemyBoss).AngleBetweenTarget() != null)
		{
			float angle = (Enemy as EnemyBoss).AngleBetweenTarget().Value;
			Debug.Log("angle : " + angle);
			if (Mathf.Abs(angle) > (Enemy as EnemyBoss).MinRotationAngle)
			{
				if ((Enemy as EnemyBoss).IsRotating == false)
				{
					(Enemy as EnemyBoss).FaceToTarget();
				}
				return false;
			}
		}
		WeaponHandler.SetAttackIndex(attackIndex);
		ChangeState(ActionState);
		return true;
	}
}

public class BossHoldState : AIHoldState
{
	EnemyBoss boss;
	public BossHoldState(BossCombatStateMachine stateMachine)
	 : base(stateMachine)
	{
		boss = stateMachine.Enemy as EnemyBoss;
	}
	public override void Enter()
	{
		combatStateMachine.Enemy.LayerFadeOut(animator, AnimatorHash.Boss.ActionLayer, 0.5f);
		if (combatStateMachine.Enemy.CanMove == false)
		{
			waitMode = true;
			Duration = WaitTime;
		}
	}
	public override void Tick()
	{
		if (boss.IsRotating == true)
			return ;
		float meleeRange = boss.MeleeDistance;
		float rangeRange = boss.RangeDistance;
		WaitForDelay();
		Transform playerTransform = combatStateMachine.Enemy.GetTargetTransform();
		if (playerTransform != null && IsArmed == true)
		{
			float distance = (combatStateMachine.Enemy.transform.position - playerTransform.position).sqrMagnitude;
			if (distance < meleeRange * meleeRange)
			{
				Debug.Log("dist : " + distance);
				combatStateMachine.WeaponHandler.SetAttackIndex(UnityEngine.Random.Range(0,2));
				combatStateMachine.ChangeState(combatStateMachine.ActionState);
			}
			else if (distance < rangeRange * rangeRange)
			{
				combatStateMachine.TryAttack(UnityEngine.Random.Range(2,4));
			}
		}
	}
}
public class BossActionState : AIActionState
{
	public BossActionState(BossCombatStateMachine stateMachine)
	 : base(stateMachine)
	{
		CurrentState = State.Attack;
	}

	public override void Enter()
	{
		combatStateMachine.Enemy.ForceStop();
		animator.SetInteger(AnimatorHash.Boss.ActionIndex, combatStateMachine.WeaponHandler.ActionIndex);
		animator.SetTrigger(AnimatorHash.Boss.ActionTrigger);
		combatStateMachine.WeaponHandler.DoFastAttack();
		combatStateMachine.Enemy.LayerFadeIn(animator, AnimatorHash.Boss.ActionLayer, 0f);
	}
}

public class BossGuardState : AIGuardState
{
	public BossGuardState(BossCombatStateMachine stateMachine)
	 : base(stateMachine)
	{
	}
}

public class BossDamageState : AIDamageState
{
	public BossDamageState(BossCombatStateMachine stateMachine)
	 : base(stateMachine)
	{
		CurrentState = State.Damage;
		HitActionHashes = new int[1, 4]
		{
			{
				AnimatorHash.Damaged.Damage_Middle_Front,
				AnimatorHash.Damaged.Damage_Middle_Rear,
			  	AnimatorHash.Damaged.Damage_Middle_Left,
				AnimatorHash.Damaged.Damage_Middle_Right
			},
		};
	}

	public override void Enter()
	{
		Height = 0;
		Direction = (HitDirection)UnityEngine.Random.Range(0, 4);
		combatStateMachine.Enemy.PlayLocomotion();
		combatStateMachine.Enemy.LayerFadeIn(animator, AnimatorHash.Boss.DamagedLayer);
		animator.CrossFadeInFixedTime(HitActionHashes[height, direction], FixedDuration, animatorDamagedLayer);
	}

	public override void Exit()
	{
		combatStateMachine.Enemy.LayerFadeOut(animator, AnimatorHash.Boss.DamagedLayer, 0.5f);
	}
}

public class BossDeadState : AIDeadState
{
	public BossDeadState(BossCombatStateMachine stateMachine)
	 : base(stateMachine)
	{
		CurrentState = State.Dead;
	}

	public override void Enter()
	{
		animator.SetTrigger(AnimatorHash.Boss.DieTrigger);
		animator.SetFloat(AnimatorHash.Boss.Speed, 0f);
		combatStateMachine.Enemy.ForceStop();

		combatStateMachine.Enemy.LayerFadeIn(animator, AnimatorHash.Boss.DamagedLayer);
	}
}

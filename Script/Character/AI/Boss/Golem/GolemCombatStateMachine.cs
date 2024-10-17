using UnityEngine;
using static PlayerCombatInputHandler;
public class GolemCombatStateMachine : AICombatStateMachine
{
	public enum Pattern {Kick01, Kick02, Punch, Slash, Skill};
	public int ComboCount = 0;
	private BossGolem golem;
	private Animator animator;
	private WeaponHandler weaponHandler;

	public GolemFastAttackState GolemFastAttackState;
	public GolemStrongAttackState GolemStrongAttackState;
	public GolemKick01State GolemKick01State;
	public GolemKick02State GolemKick02State;
	public GolemKick03State GolemKick03State;
	public GolemKick04State GolemKick04State;
	public GolemPunchState GolemPunchState;
	public GolemSlashState GolemSlashState;
	public GolemSlash02State GolemSlash02State;
	public GolemRollingState GolemRollingState;
	public GolemFlyingKneeState GolemFlyingKneeState;
	public GolemCombatStateMachine(EnemyDynamic enemy, WeaponHandler weaponHandler)
	 : base(enemy, weaponHandler)
	{
		golem = enemy as BossGolem;
		animator = golem.Animator;
		this.weaponHandler = weaponHandler;

		HoldingState = new GolemHoldState(this);
		EquippingState = null;
		GolemFastAttackState = new GolemFastAttackState(this);
		GolemStrongAttackState = new GolemStrongAttackState(this);
		GolemKick01State = new GolemKick01State(this);
		GolemKick02State = new GolemKick02State(this);
		GolemKick03State = new GolemKick03State(this);
		GolemKick04State = new GolemKick04State(this);
		GolemPunchState = new GolemPunchState(this);
		GolemSlashState = new GolemSlashState(this);
		GolemSlash02State = new GolemSlash02State(this);
		GolemRollingState = new GolemRollingState(this);
		GolemFlyingKneeState = new GolemFlyingKneeState(this);
		GuardingState = null;
		DamagingState = new AIDamageState(this);
		DownedState = new DownState(this);
		AirborneState = null;
		DeadState = new AIDeadState(this);
		currentState = HoldingState;
	}
	public override void ChangeState(AICombatState newState)
	{
		// Debug.Log($"[Combat] : Changing State from {currentState} to {newState}");
		Enemy.InvokeOnCombatStateChanged(newState.CurrentState.ToString());

		if (base.currentState == ActionState && newState == ActionState)
		{
			return ;
		}
		if (base.currentState == ActionState && newState == DamagingState)
		{
			HoldingState.Duration = 0;
		}
		else if (base.currentState != ActionState && newState == ActionState)
		{
			if (WeaponHandler.CanAttack == false)
				return;
		}

		if (base.currentState == DownedState && newState == DownedState)
		{
			DownedState.Duration += 0.05f;
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
		base.ChangeState(newState);
	}

	public void DoFastAttackCombo1() => ChangeState(GolemFastAttackState);
	public void DoStrongAttackCombo1() => ChangeState(GolemStrongAttackState);
	public void DoKickCombo1() => ChangeState(GolemKick01State);
	public void DoKickCombo2() => ChangeState(GolemKick02State);
	public void DoKickCombo3() => ChangeState(GolemKick03State);
	public void DoKickCombo4() => ChangeState(GolemKick04State);
	public void DoPunchCombo() => ChangeState(GolemPunchState);
	public void DoSlashCombo() => ChangeState(GolemSlashState);
	public void DoSlashCombo2() => ChangeState(GolemSlash02State);
	public void DoRollingCombo() => ChangeState(GolemRollingState);
	public void DoFlyingKneeCombo() => ChangeState(GolemFlyingKneeState);
}

public class GolemHoldState : AIHoldState
{
	private BossGolem golem;
	private GolemCombatStateMachine stateMachine;
	public GolemHoldState(GolemCombatStateMachine stateMachine) : base(stateMachine)
	{
		golem = stateMachine.Enemy as BossGolem;
		combatStateMachine = stateMachine;
		this.stateMachine = combatStateMachine as GolemCombatStateMachine;
	}
	public override void Enter()
	{
		golem.LayerFadeOut(animator, AnimatorHash.BossGolem.ActionLayer, 0.5f);
		waitMode = Duration > 0;
	}
	public override void Tick()
	{
		if (WaitForDelay() == true)
		{
			return ;
		}
		float distance = golem.GetTargetDistance().GetValueOrDefault();
		if (distance < 1f)
		{
			if (Random.Range(0,3) == 0)
				stateMachine.DoStrongAttackCombo1();
			else
				stateMachine.DoFastAttackCombo1();
		}
		else if (distance < 1.75f)
		{
			stateMachine.ChangeState(GetNextState());
		}
		else if (distance < 2.5f)
		{
			stateMachine.DoKickCombo3();
		}
		else if (distance > 4.0f && distance < 6f)
		{
			if (IsPlayerInfrontOfGolem(angle : 240f) == true)
			{
				if (Random.Range(0,2) == 0)
					stateMachine.DoRollingCombo();
				else
					stateMachine.DoFlyingKneeCombo();
			}
		}
		else if (distance > 8.0f && distance < 10.0f)
		{
			if (IsPlayerInfrontOfGolem(angle : 120f) == true)
			{
				stateMachine.DoKickCombo4();
			}
		}

	}
	public override void Exit()
	{
	}
	private GolemActionState GetNextState()
	{
		GolemCombatStateMachine stateMachine = combatStateMachine as GolemCombatStateMachine;
		int weight = Random.Range(0,10);
		int handPatternWeight = golem.CharacterStatus.GetHpRate < 0.5f ? 2 : -2;
		int pivot = 5 + handPatternWeight;

		if (weight < pivot)
		{
			return (weight % 2) switch
			{
				0 => stateMachine.GolemKick01State,
				1 => stateMachine.GolemKick02State,
				_ => null,
			};
		}
		else
		{
			return (weight % 3) switch
			{
				0 => stateMachine.GolemPunchState,
				1 => stateMachine.GolemSlashState,
				2 => stateMachine.GolemSlash02State,
				_ => null,
			};
		}
	}
	private bool IsPlayerInfrontOfGolem(float angle = 10)
	{
		angle *= 0.5f;
		Vector3 direction = golem.GetTargetTransform().position - golem.transform.position;
		bool result = Vector3.Dot(direction.normalized, golem.transform.forward) > Mathf.Cos(angle * Mathf.Deg2Rad);
		return result;
	}
}

public abstract class GolemActionState : AIActionState
{
	protected BossGolem golem;
	protected GolemMovementStateMachine movementStateMachine;
	protected bool isTriggered = false;
	protected bool isRotated = false;
	protected int triggerHash;
	protected float waitingTime;
	public GolemActionState(AICombatStateMachine stateMachine) : base(stateMachine)
	{
		golem = stateMachine.Enemy as BossGolem;
		movementStateMachine = golem.movementStateMachine as GolemMovementStateMachine;
	}
	public override void Enter()
	{
		golem.ForceStop();
		golem.LayerFadeIn(animator, AnimatorHash.BossGolem.ActionLayer, 0.25f);
		SetActionIndex(0);
		animator.ResetTrigger("CancelTrigger");
		combatStateMachine.WeaponHandler.CurrentWeapon.AttackingType = AttackType.Else;
		waitingTime = 0.5f;
	}
	public override void Tick()
	{
		
	}
	public override void Exit()
	{
		combatStateMachine.Enemy.LayerFadeOut(animator, AnimatorHash.Boss.ActionLayer, 0.2f);
		animator.ResetTrigger(triggerHash);
		combatStateMachine.WeaponHandler.OnNextComboDisable();
		golem.CanMove = true;
		combatStateMachine.HoldingState.Duration = Random.Range(0.1f, waitingTime);
	}
	protected void SetActionIndex(int actionIndex)
	{
		combatStateMachine.WeaponHandler.CurrentWeapon.ActionIndex = actionIndex;
	}
}

public class GolemFastAttackState : GolemActionState
{
	public GolemFastAttackState(AICombatStateMachine stateMachine)
	 : base(stateMachine)
	{
		triggerHash = Animator.StringToHash("FastAttackTrigger");
	}

	public override void Enter()
	{
		base.Enter();
		isTriggered = false;
		isRotated = false;
		combatStateMachine.WeaponHandler.CurrentWeapon.AdditionalIndex = 0;
		combatStateMachine.WeaponHandler.CurrentWeapon.AttackingType = AttackType.FastAttack;
		combatStateMachine.WeaponHandler.CurrentWeapon.ActionIndex = 0;
	}

	public override void Tick()
	{
		if (isRotated == false)
		{
			movementStateMachine.RotateToTarget(0.1f);
			isRotated = true;
			return ;
		}
		if (movementStateMachine.IsRotating == true)
		{
			return ;
		}
		if (isTriggered == false)
		{
			animator.SetTrigger(triggerHash);
			isTriggered = true;
			return ;
		}
	}
	public override void Exit()
	{
		base.Exit();
		combatStateMachine.HoldingState.Duration = Random.Range(0f, 0.1f);
	}
}

public class GolemStrongAttackState : GolemActionState
{
	public GolemStrongAttackState(AICombatStateMachine stateMachine)
	 : base(stateMachine)
	{
		triggerHash = Animator.StringToHash("StrongAttackTrigger");
	}

	public override void Enter()
	{
		base.Enter();
		isTriggered = false;
		isRotated = false;
		combatStateMachine.WeaponHandler.CurrentWeapon.AdditionalIndex = 0;
		combatStateMachine.WeaponHandler.CurrentWeapon.AttackingType = AttackType.StrongAttack;
		combatStateMachine.WeaponHandler.CurrentWeapon.ActionIndex = 0;
	}

	public override void Tick()
	{
		if (isRotated == false)
		{
			movementStateMachine.RotateToTarget(0.1f);
			isRotated = true;
			return ;
		}
		if (movementStateMachine.IsRotating == true)
		{
			return ;
		}
		if (isTriggered == false)
		{
			animator.SetTrigger(triggerHash);
			isTriggered = true;
			return ;
		}
	}
	public override void Exit()
	{
		base.Exit();
		combatStateMachine.HoldingState.Duration = Random.Range(0f, 0.1f);
	}
}

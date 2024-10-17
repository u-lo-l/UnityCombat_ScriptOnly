using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static PlayerCombatInputHandler;
using static PlayerCombatState;
/// <summary>
/// State : Wait Equip Attack Guard Parry(Guard) Counter(Attack) Execution(Attack) Damaged Dead(Damaged)
/// 	- Hold
/// 		- Equip
/// 	- Attack
/// 		- Counter
/// 		- Execution
/// 	- Guard
/// 		- Parry
/// 	- Damaged
/// 		-Dead
/// </summary>
public class PlayerCombatStateMachine : StateMachine
{
	public Player Player {get; private set;}
	public WeaponHandler WeaponHandler {get; private set;}
	public PlayerHoldState HoldingState {get; private set;}
	public PlayerEquipState EquippingState {get; private set;}
	public PlayerFastActionState FastActionState {get; private set;}
	public PlayerStrongActionState StrongActionState {get; private set;}
	public PlayerFastSkillState FastSkillState {get; private set;}
	public PlayerStrongSkillState StrongSkillState {get; private set;}
	public PlayerCommonSkillState CommonSkillState {get; private set;}
	public PlayerGuardState GuardingState {get; private set;}
	public PlayerDamageState DamagingState {get; private set;}
	public PlayerCombatStateMachine(Player player, WeaponHandler weaponHandler, PlayerCombatInputHandler combatInputHandler)
	{
		Player = player;
		WeaponHandler = weaponHandler;
		HoldingState = new(this);
		EquippingState = new(this);
		FastActionState = new(this);
		StrongActionState = new(this);
		FastSkillState = new(this);
		StrongSkillState = new(this);
		CommonSkillState = new(this);
		GuardingState = new(this);
		DamagingState = new(this);
	}

	/// <summary>
	/// 1. Attack -> Attack
	/// 2. Attack -> Skill
	/// 3. Hold -> Attack
	/// 4. Hold -> Skill
	/// 5. Hold -> Equip
	/// 6. Any -> Hold
	/// </summary>
	/// <param name="newState"></param>
	public void ChangeState(PlayerCombatState newState)
	{
		PlayerCombatState nowState = base.currentState as PlayerCombatState;
		Debug.Log($"Change State from {nowState?.CurrentState} to {newState?.CurrentState}");
		// this case is No.1 : Attack -> Attack
		if (IsAttackState(nowState) && IsAttackState(newState))
		{
			bool hasNextCombo = false;
			if (newState == FastActionState)
				hasNextCombo = WeaponHandler.HasNextCombo(AttackType.FastAttack) == true;
			else if (newState == StrongActionState)
				hasNextCombo = WeaponHandler.HasNextCombo(AttackType.StrongAttack) == true;

			if (hasNextCombo== false || WeaponHandler.CanNextCombo == false)
				return;
			currentState.Exit();
			newState.Enter();
			return ;
		}

		// this case is No.2 : Attack->Skill : 그냥 홀드로 넘길까
		if ((IsSkillState(nowState) || IsAttackState(nowState)) && IsSkillState(newState))
		{
			// nowState.Exit();
			// newState = HoldingState;
			// newState.Enter();
			return ;
		}

		if (IsSkillState(nowState) && IsDamageState(newState))
		{
			return ;
		}

		// this case is No.6 : Any -> Hold
		if(newState == HoldingState && currentState != HoldingState)
		{
			Player.PlayLocomotion();
		}
		// this case is No.6 special case : Action -> Hold
		else if (base.currentState is PlayerActionState)
		{
			WeaponHandler.ResetAttackIndex();
		}
		// this case is No.3~5: Hold -> Any
		base.ChangeState(newState);
	}
	private bool IsAttackState(PlayerCombatState state)
		=> state is PlayerActionState && state is not PlayerSkillState;
	private bool IsSkillState(PlayerCombatState state)
		=> state is PlayerSkillState;
	private bool IsDamageState(PlayerCombatState state)
		=> state is PlayerDamageState;

	public State GetCurrentState()
	{
		PlayerCombatState combatState = currentState as PlayerCombatState;
		return combatState.CurrentState;
	}
	public void TryEquip(int weaponIndex)
	{
		if (GetCurrentState() != State.Hold)
		{
			return ;
		}
		if (Player.CanEquip() == false)
		{
			return;
		}
		if (WeaponHandler.CanEquip(weaponIndex) == false)
		{
			return;
		}
		EquippingState.WeaponIndex = weaponIndex;
		ChangeState(EquippingState);
	}
	public bool TryAction(AttackType type, int actionIndex = -1)
	{
		if (WeaponHandler.ArmedType == WeaponType.Unarmed)
		{
			return false;
		}
		if (WeaponHandler.CanAttack == false)
		{
			return false;
		}

		State currentState = GetCurrentState();
		switch (currentState)
		{
			case State.FastAttack : // 약공에서 약공으로의 콤포
				if (type == AttackType.StrongAttack || WeaponHandler.HasNextCombo(AttackType.FastAttack) == false)
					return false;
				return DoFastAttack();
			case State.StrongAttack : // 강공에서 강공으로의 콤보
				if (type == AttackType.FastAttack || WeaponHandler.HasNextCombo(AttackType.StrongAttack) == false)
					return false;
				return DoStrongAttack();
			case State.Hold : // 공격의 시작
				WeaponHandler.SetAttackIndex(Mathf.Max(actionIndex, 0));
				return type == AttackType.FastAttack ? DoFastAttack() : DoStrongAttack();
			default :
			return false;
		}
	}
	private bool DoFastAttack()
	{
		ChangeState(FastActionState);
		return true;
	}
	private bool DoStrongAttack()
	{
		ChangeState(StrongActionState);
		return true;
	}

	public void TrySkill(int index)
	{
		switch (index)
		{
			case 0 : DoFastSkill(); break;
			case 1 : DoStrongSkill(); break;
			case 2 : DoCommonSkill(); break;
		}
	}
	private void DoFastSkill()
	{
		if (WeaponHandler.CanSkill(0) == true)
		{
			WeaponHandler.CurrentWeapon.AttackingType = AttackType.FastSkill;
			ChangeState(FastSkillState);
		}
	}

	private void DoStrongSkill()
	{
		if (WeaponHandler.CanSkill(1) == true)
		{
			WeaponHandler.CurrentWeapon.AttackingType = AttackType.StrongSkill	;
			ChangeState(StrongSkillState);
		}
	}

	private void DoCommonSkill()
	{
		if (WeaponHandler.CanSkill(2) == true)
		{
			Debug.Log("CommonSkill");
			ChangeState(CommonSkillState);
		}
	}
	public void TryGuard()
	{

	}

	public bool OnDamage()
	{
		if (GetCurrentState() == State.Damage)
			return false;
		ChangeState(DamagingState);
		return true;
	}
	public bool ShouldStop()
	{
		return WeaponHandler.ShouldStop() || GetCurrentState() == State.Damage;
	}
}
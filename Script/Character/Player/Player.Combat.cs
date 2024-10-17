using UnityEngine;
using static PlayerCombatInputHandler;

public partial class Player
{
	#region CombatStateMachine
	public bool CanEquip()
	{
		PlayerMovementState.State movementState = movementStateMachine.GetCurrentState();
		if (movementState == PlayerMovementState.State.Idle)
			return true;
		if (movementState == PlayerMovementState.State.Walk)
			return true;
		if (movementState == PlayerMovementState.State.Run)
			return true;
		if (movementState == PlayerMovementState.State.Sprint)
			return true;
		return false;
	}
	public void ResetToHold()
	{
		if (CharacterStatus.IsDead == false)
		{
			combatStateMachine.ChangeState(combatStateMachine.HoldingState);
		}
	}
	private void OnDoEquip(int weaponIndex)
	{
		combatStateMachine.TryEquip(weaponIndex);
	}
	private void OnDoAttack(AttackType attackType)
	{
		if (movementStateMachine.GetCurrentState() == PlayerMovementState.State.Jump)
			return;
		if (movementStateMachine.GetCurrentState() == PlayerMovementState.State.Air)
			return;
		if (movementStateMachine.GetCurrentState() == PlayerMovementState.State.Sprint)
			return;
		if (combatStateMachine.TryAction(attackType) == false)
			return ;
		if (combatStateMachine.ShouldStop() == true)
			movementStateMachine.ChangeState(movementStateMachine.IdlingState);
	}
	private void OnCancelAttack(AttackType attackType)
	{
		if (combatStateMachine.GetCurrentState() == PlayerCombatState.State.FastAttack)
		{
			// Animator.SetTrigger("StrongActionCancelTrigger");
			return ;
		}
		else if (combatStateMachine.GetCurrentState() == PlayerCombatState.State.StrongAttack)
		{
			Animator.SetTrigger("StrongActionCancelTrigger");
		}
	}
	private void OnDoSpell(int spellIndex)
	{
		if (movementStateMachine.GetCurrentState() == PlayerMovementState.State.Jump)
			return;
		if (movementStateMachine.GetCurrentState() == PlayerMovementState.State.Air)
			return;
		if (movementStateMachine.GetCurrentState() == PlayerMovementState.State.Sprint)
			return;
		combatStateMachine.TrySkill(spellIndex);
		if (combatStateMachine.ShouldStop() == true)
			movementStateMachine.ChangeState(movementStateMachine.IdlingState);
	}
	private void OnWeaponChanged(WeaponType prevType, WeaponType newType)
	{
		if (prevType == newType)
			return;
		PlayLocomotion();
	}
#endregion
}
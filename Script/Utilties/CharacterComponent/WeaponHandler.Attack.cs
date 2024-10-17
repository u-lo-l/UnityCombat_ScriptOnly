using UnityEngine;
using static PlayerCombatInputHandler;

public partial class WeaponHandler
{
	public void SetAttackIndex(int index = 0)
	{
		if (index >= CurrentWeapon.Stat.FastActionCount)
		{
			Debug.Log($"{name} | [WeaponHandler] Attack Index out of range");
			index = 0;
		}
		ActionIndex = index;
		CurrentWeapon.ActionIndex = index;
	}
	public void IncreaseAttackIndex()
	{
		ActionIndex = CurrentWeapon == null ? -1 : CurrentWeapon.IncreaseAttackIndex();
	}
	public void ResetAttackIndex()
	{
		ActionIndex = 0;
	}
	public bool HasNextCombo(AttackType attackType)
	{
		if (CurrentWeapon == null)
			return false;
		if (CurrentWeapon.Stat == null)
			return false;
		if (attackType == AttackType.FastAttack)
		{
			return ActionIndex  < CurrentWeapon.Stat.FastActionCount;
		}
		else if (attackType == AttackType.StrongAttack)
		{
			return ActionIndex < CurrentWeapon.Stat.StrongActionCount;
		}
		return false;
	}
	public bool ShouldStopForAttack()
	{
		if (CurrentWeapon == null)
		{
			print("[WeaponHandler] : Can Move cause null weapon");
			return false;
		}
		if (CurrentWeapon.Stat == null)
		{
			print("[WeaponHandler] : Can Move cause null StatData");
			return false;
		}
		AttackType attackType = CurrentWeapon.AttackingType;
		if (attackType == AttackType.FastAttack && ActionIndex < CurrentWeapon.Stat.FastActionCount)
		{
			if (CurrentWeapon.Stat.FastActionData == null)
			{
				print("[WeaponHandler] : Can Move cause null FastActionData");
				return false;
			}
			return !CurrentWeapon.Stat.FastActionData[ActionIndex].CanMove;
		}
		else if (attackType == AttackType.StrongAttack && ActionIndex < CurrentWeapon.Stat.StrongActionCount)
		{
			if (CurrentWeapon.Stat.StrongActionData == null)
			{
				print("[WeaponHandler] : Can Move cause null StrongActionData");
				return false;
			}
			return !CurrentWeapon.Stat.StrongActionData[ActionIndex].CanMove;
		}
		else
		{
			print($"[WeaponHandler] : Can Move cause sth else | ActionIndex : {ActionIndex} | AttackType : {attackType}");
			return false;
		}
	}
	public void DoFastAttack()
	{
		this.DoAttack(AttackType.FastAttack);
	}
	public void DoStrongAttack()
	{
		this.DoAttack(AttackType.StrongAttack);
	}
	public void EndAttack()
	{
		if (CurrentWeapon is Melee)
		{
			(CurrentWeapon as Melee)?.DisableAllColliers();
		}
	}
	private void DoAttack(AttackType attackType)
	{
		CurrentWeapon.AttackingType = attackType;
		if (attackType == AttackType.FastAttack)
		{
			CurrentWeapon.FastAttack(ActionIndex);
		}
		else
		{
			CurrentWeapon.StrongAttack(ActionIndex);
		}
		if (AttackDelayCoroutine != null)
		{
			StopCoroutine(AttackDelayCoroutine);
		}
		AttackDelayCoroutine = StartCoroutine(WaitForAttackDelay(attackType));
	}
}
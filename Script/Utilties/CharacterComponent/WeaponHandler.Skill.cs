using System.Collections.Generic;
using UnityEngine;
using static PlayerCombatInputHandler;

public partial class WeaponHandler
{
	private void RegisterWeaponSkills()
	{
		if (TryGetComponent<SkillHandler>(out skillHandler) == false)
		{
			// Debug.Log("[WeaponHandler] : Skill handler not found");
			return ;
		}
		foreach(KeyValuePair<WeaponType, Weapon> element in weaponTable)
		{
			Weapon weapon = element.Value;
			if (weapon == null)
			{
				continue ;
			}

			WeaponType weaponType = element.Key;
			WeaponSkill[] weaponSkills = element.Value.Stat.WeaponSkillData;
			if (weaponSkills.Length == 2)
			{
				skillHandler.RegisterWeaponSkill(weaponType, weaponSkills[0], weaponSkills[1]);
			}
			else if (weaponSkills.Length == 1)
			{
				skillHandler.RegisterWeaponSkill(weaponType, weaponSkills[0], null);
			}
			else
			{
				skillHandler.RegisterWeaponSkill(weaponType, null, null);
			}
		}
	}
	public bool CanSkill(int index)
	{
		if (skillHandler == null)
			return false;
		WeaponType weaponType = CurrentWeapon == null ? WeaponType.Unarmed : CurrentWeapon.Type;
		switch(index)
		{
			case 0 :
				return skillHandler.CanFastSkill(weaponType);
			case 1 :
				return skillHandler.CanStrongSkill(weaponType);
			case 2 :
				return skillHandler.CanCommonSkill(weaponType);
		}
		return false;
	}

	public bool ShouldStopForSkill()
	{
		if (CurrentWeapon == null)
			return false;
		if (CurrentWeapon.Stat == null)
			return false;
		AttackType attackType = CurrentWeapon.AttackingType;
		if (attackType == AttackType.FastSkill)
		{
			if (CurrentWeapon.Stat.WeaponSkillData[0] == null)
				return false;
			return CurrentWeapon.Stat.WeaponSkillData[0].ShoulStop();
		}
		else if (attackType == AttackType.StrongSkill)
		{
			if (CurrentWeapon.Stat.WeaponSkillData[1] == null)
				return false;
			return CurrentWeapon.Stat.WeaponSkillData[1].ShoulStop();
		}
		return true;
	}

	private void UpdateFastAttackGauge(float amount)
	{
		if (skillHandler == null)
			return ;
		skillHandler.StackFastGauge(amount);
	}
	private void UpdateStrongAttackGauge (float amount)
	{
		if (skillHandler == null)
			return ;
		skillHandler.StackStrongGauge(amount);
	}
	public void FastSkillPerformed ()
	{
		if (skillHandler == null)
			return ;
		skillHandler.ResetFastSkill();
	}
	public void StrongSkillPerformed ()
	{
		if (skillHandler == null)
			return ;
		skillHandler.ResetStrongSkill();
	}
	public void CommonSkillFire()
	{
		StartCoroutine(skillHandler.CommonSkillCoolDown());
	}
}
using UnityEngine;
public partial class WeaponHandler
{
	public bool CanEquip(int weaponIndex)
	{
		int max = (int)WeaponType.Max;
		if (weaponIndex >= max)
			return false;
		return weaponTable[(WeaponType)weaponIndex] != null;
	}
	public void TryEquip(int weaponIndex)
	{
		print($"[WeaponHandler] [{gameObject.name} Trying to equip {weaponIndex}]");
		WeaponType targetType = WeaponType.Unarmed;
		if (weaponTable[(WeaponType)weaponIndex] != null)
		{
			targetType = (WeaponType)weaponIndex;
			print($"[WeaponHandler] [{gameObject.name} changes to {targetType}]");
		}
		SetMode(targetType);
		decalProjector.size = new Vector3(WeaponRange * 2, WeaponRange * 2, decalProjector.drawDistance);
	}
	public void ForceUnequip()
	{
		CurrentWeapon?.Unequip();
		decalProjector.size = new Vector3(0, 0, decalProjector.drawDistance);
		ChangeType(WeaponType.Unarmed);
	}
	private void SetMode(WeaponType newType)
	{
		CompletelyEquipped = false;
		if (ArmedType == newType)
		{
			CurrentWeapon?.Unequip();
			ChangeType(WeaponType.Unarmed);
			return ;
		}
		if (ArmedType != WeaponType.Unarmed)
		{
			CurrentWeapon?.Unequip();
		}
		ChangeType(newType);
		if (CurrentWeapon == null)
		{
			print("Current Weapon is NULL");
		}
		CurrentWeapon.Equip();
		if (CurrentWeapon.Stat.HasEquipAnimation == false)
		{
			CurrentWeapon.ActiveWeapon();
			print($"[WeaponHandler] {name} successfullly equipped");
			CompletelyEquipped = true;
		}
	}
	private void ChangeType(WeaponType newType)
	{
		if (newType == ArmedType)
			return ;
		WeaponType prevType = ArmedType;
		ArmedType = newType;
		OnWeaponChanged?.Invoke(prevType, newType);
		if (skillHandler != null)
		{
			skillHandler.ResetFastSkill();
			skillHandler.ResetStrongSkill();
		}
	}
}
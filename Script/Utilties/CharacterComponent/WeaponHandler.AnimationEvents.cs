using System.Collections;
using UnityEngine;
using static PlayerCombatInputHandler;

public partial class WeaponHandler
{
	public void OnActiveWeapon() // AnimEvent
	{
		print($"[WeaponHandler] : {name} OnActivateWeapon called : {ArmedType}");
		if (CurrentWeapon == null)
		{
			Debug.LogWarning($"[WeaponHandler] : weaponIndex  {ArmedType} is null");
			return ;
		}
		CurrentWeapon.ActiveWeapon();
		CompletelyEquipped = true;
	}
	public void OnActiveSpecificWeapon(int index)
	{
		if (CurrentWeapon == null)
		{
			Debug.LogWarning($"[WeaponHandler] : weaponIndex  {ArmedType} is null");
			return ;
		}
		CurrentWeapon.ActiveWeapon(index);
		CompletelyEquipped = true;
	}
	public void OnDeactiveSpecificWeapon(int index)
	{
		if (CurrentWeapon == null)
		{
			// Debug.LogWarning($"[WeaponHandler] : weaponIndex  {ArmedType} is null");
			return ;
		}
		CurrentWeapon.DeactiveWeapon(index);
		CompletelyEquipped = true;
	}
	private void OnColliderEnable(int index = 0) // AnimEvent
	{
		Melee meleeWeapon = weaponTable[ArmedType] as Melee;
		if (meleeWeapon == null)
			return ;
		meleeWeapon.EnableCollider(index);
	}
	public void OnColliderDisable(int index = 0) // AnimEvent
	{
		Melee meleeWeapon = weaponTable[ArmedType] as Melee;
		if (meleeWeapon == null)
			return ;
		meleeWeapon.DisableCollider(index);
	}
	public void OnNextComboEnable() // AnimEvent
	{
		CanNextCombo = true;
	}
	public void OnNextComboDisable() // AnimEvent
	{
		CanNextCombo = false;
	}
	private void OnSpellAction(int spellIndex) // AnimEvent
	{
		ActionIndex = spellIndex;
		CurrentWeapon.SpecialAction(spellIndex);
	}

	private void OnArrowReady()
	{
		Bow bow = CurrentWeapon as Bow;
		if (bow != null)
		{
			bow.IsArrowReady = true;
		}
	}
	private void OnWeaponCameraShake(CameraShakeImpulseData impulseData)
	{
		print("Make Manual CameraShake");
		impulseSource.Shake(impulseData);
	}

	private const float PitchMin = 0.8f;
	private const float PitchMax = 1.2f;
	private void OnPlayAttackSound()
	{
		Weapon weapon = CurrentWeapon;
		WeaponStatData Stat = weapon.Stat;
		int index = ActionIndex;
		AudioClip attackSound;
		float pitch = 1;
		switch(weapon.AttackingType)
		{
			case AttackType.FastAttack:
				attackSound = Stat.FastActionData[index].AttackSound;
				pitch = Stat.FastActionData[index].DefaultPitch;
			break;
			case AttackType.StrongAttack:
				attackSound = Stat.StrongActionData[index].AttackSound;
				pitch = Stat.StrongActionData[index].DefaultPitch;
			break;
			default:
				attackSound = null;
			break;
		}
		if (attackSound != null)
		{
			AudioSource.pitch = Random.Range(pitch * PitchMin, pitch * PitchMax);
			AudioSource.clip = attackSound;
			AudioSource.volume = AudioVolumeManager.EffectVolume;
			AudioSource.clip = attackSound;
			AudioSource.Play();
		}
	}

	public float RemainDelay;
	private IEnumerator WaitForAttackDelay(AttackType attackType)
	{
		CanAttack = false;
		WeaponStatData stat = CurrentWeapon.Stat;
		ActionData actionData;
		switch (attackType)
		{
			case AttackType.FastAttack:
				actionData = stat.FastActionData[ActionIndex];
				break;
			case AttackType.StrongAttack:
				actionData = stat.StrongActionData[ActionIndex];
				break;
			default :
				yield break;
		}

		float delayMin = CurrentWeapon == null ? 0 : actionData.Delay.x;
		float delayMax = CurrentWeapon == null ? 0 : actionData.Delay.y;
		float attackDelay = Random.Range(delayMin, delayMax);
		RemainDelay = 0;
		while (RemainDelay < attackDelay)
		{
			RemainDelay += Time.deltaTime;
			yield return null;
		}
		CanAttack = true;
		AttackDelayCoroutine = null;
	}
}
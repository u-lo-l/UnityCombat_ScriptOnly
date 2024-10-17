using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 주변의 적에게 번개를 내려치고, 번개를 맞은 적은 3초간 매초 지속피해를 입는다.
/// </summary>
[CreateAssetMenu(fileName = "Staff Strong Skill", menuName = "Spell/Player/Strong/Staff", order = 13)]
public class StrongStaffSkill : WeaponSkill_Throwing
{
	public override void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Staff] Strong Skill Executed by Player");

		player.StartCoroutine(ShootElectricity(player, weapon, GetLayerMask.GetEnemyLayerMask));
	}
	public override void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Staff] Strong Skill Executed by Enemy");
		throw new System.NotImplementedException();
	}
	public override void Finish(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Staff] Strong Skill Finishedr");
		throw new System.NotImplementedException();
	}

	private IEnumerator ShootElectricity(Character owner, Weapon weapon, LayerMask targetLayerMask)
	{
		SetMuzzle(owner, out Vector3 position, out Quaternion rotation);
		BouncingElectricity bouncingElectricity = CreateProjectile<BouncingElectricity>(position, rotation, owner, weapon, targetLayerMask);
		SetProjectildAudio(bouncingElectricity);
		yield return new WaitForSeconds(0.1f);
		bouncingElectricity.Shoot(true);
	}

	protected override void OnProjectileHit(Collider other, Weapon weapon, Vector3 hitPoint)
	{
		base.OnProjectileHit(other, weapon, hitPoint);
	}
	protected override void SetMuzzle(Character holder, out Vector3 position, out Quaternion rotation)
	{
		position = holder.transform.position + holder.Height * Vector3.up;
		rotation = holder.transform.rotation;
	}
}
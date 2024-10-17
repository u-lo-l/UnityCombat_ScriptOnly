using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 전방의 원뿔형으로 충격파 발사 (세트 W)
/// </summary>
[CreateAssetMenu(fileName = "Fist Fast Skill", menuName = "Spell/Player/Fast/Fist", order = 0)]
public class FastFistSkill : WeaponSkill
{
	[SerializeField] private GameObject energyExplosionPrefab;
	private const float ExplosionRadius = 3f;
	public override void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Fast] Fast Skill Executed by Player");

		Vector3 position = player.transform.position +
							Vector3.up * player.Height +
							player.EnvironmentChecker.FixedForward * ExplosionRadius;
		Quaternion rotation = player.transform.rotation;

		GameObject obj = Instantiate<GameObject>(energyExplosionPrefab, position, rotation, null);
		obj.SetActive(false);
		EnergyExplosion energyExplosion = obj.GetComponent<EnergyExplosion>();
		energyExplosion.Owner = player;
		energyExplosion.Weapon = weapon;
		energyExplosion.ExplosionRadius = ExplosionRadius;
		energyExplosion.TargetLayerMask = GetLayerMask.GetEnemyLayerMask;
		energyExplosion.AllyLayerMask = weapon.AllyLayerMask;
		energyExplosion.OnApplyExplosionDamage += OnApplyExplosionDamage;
		obj.SetActive(true);
	}
	public override void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Fast] Fast Skill Executed by Enemy");
		throw new System.NotImplementedException();
	}
	public override void Finish(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Fast] Fast Skill Finishedr");
		throw new System.NotImplementedException();
	}

	private void OnApplyExplosionDamage(Vector3 position, Weapon weapon, List<KeyValuePair<Character, IDamagable>> damagables)
	{
		foreach(var keyValue in damagables)
		{
			Vector3 forceDir = (position - keyValue.Key.transform.position).normalized;
			DamageProcessor.ApplyDamage(keyValue.Value, weapon, AttackData, keyValue.Key.transform.position, forceDir);
		}
	}
}
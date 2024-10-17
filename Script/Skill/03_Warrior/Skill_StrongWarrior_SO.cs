using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 무기를 위로 휘두르며 전방 땅에 일자로 충격파 발생 후 적들을 가운데로 모음
/// </summary>
[CreateAssetMenu(fileName = "Warrior Strong Skill", menuName = "Spell/Player/Strong/Warrior", order = 5)]
public class StrongWarriorSkill : WeaponSkill
{
	private const int BumpCount = 6;
	private const float BumpLength = 7;
	private float Step => BumpLength / BumpCount;
	[SerializeField] GameObject[] BumpPrefabs;
	public override void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Warrior] Strong Skill Executed by Player");
		PlayAttackSound(player);
		player.StartCoroutine(GroundBump(player, weapon));
	}
	public override void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Warrior] Strong Skill Executed by Enemy");
		throw new System.NotImplementedException();
	}
	public override void Finish(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Warrior] Strong Skill Finishedr");
		throw new System.NotImplementedException();
	}

	private IEnumerator GroundBump(Player player, Weapon weapon)
	{
		WaitForSeconds waitForNextBump = new(0.1f);
		LayerMask groundLayer = 1 << LayerMask.NameToLayer("Obstacle");
		for(int i = 0 ; i < BumpCount ; i++)
		{
			Vector3 origin = player.transform.position +
							 player.transform.up * player.Height +
							 player.transform.right * UnityEngine.Random.Range(-0.3f, 0.3f) +
							 (i + 1) * Step * player.transform.forward;

			if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 10f, groundLayer) == true)
			{
				int index = UnityEngine.Random.Range(0, BumpPrefabs.Length);
				GameObject obj = Instantiate<GameObject>(BumpPrefabs[index], hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), null);
				obj.SetActive(false);
				GroundBump groundBump = obj.GetComponent<GroundBump>();
				groundBump.Owner = player;
				groundBump.weapon = weapon;
				groundBump.OnRockHit += OnRockHit;
				groundBump.AllyLayerMask = weapon.AllyLayerMask;
				obj.SetActive(true);
			}
			yield return waitForNextBump;
		}
	}

	private void OnRockHit(Collider enemy, Weapon weapon, Vector3 HitPoint)
	{
		if (enemy.TryGetComponent<IDamagable>(out IDamagable damagable))
		{
			DamageProcessor.ApplyDamage(damagable, weapon, AttackData, HitPoint, Vector3.zero);
		}
	}

}
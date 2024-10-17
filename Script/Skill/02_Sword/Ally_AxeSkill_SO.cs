using UnityEngine;
using static GetLayerMask;

/// <summary>
/// 짧은 거리만큼 초승달 모양의 검기 발사
/// </summary>
[CreateAssetMenu(fileName = "Ally Fast Skill", menuName = "Spell/Ally/Fast/Axe")]
public class AllyAxeSkill : WeaponSkill
{
	[SerializeField] private GameObject explosionPrefab;
	public override void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		throw new System.NotImplementedException();
	}
	public override void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		MakeExplosion(enemy, weapon, explosionPrefab, 0.75f);
	}
	public override void Finish(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		throw new System.NotImplementedException();
	}

	private void MakeExplosion(Character player, Weapon weapon, GameObject prefab, float radius)
	{
		Vector3 center = player.transform.position + player.transform.forward + Vector3.up;
		if (Physics.Raycast(center, Vector3.down, out RaycastHit hit, 3f, GetObstacleLayerMask) == false)
			return ;

		GameObject explosionObj = Instantiate<GameObject>(explosionPrefab, weapon.WeaponObjTransform(0));
		explosionObj.SetActive(false);

		explosionObj.transform.localPosition = new Vector3(0,0.42f,0);
		Explosion explosion = explosionObj.GetComponent<Explosion>();
		explosion.SetExplosionRadius(radius);
		explosion.Weapon = weapon;
		explosion.TargetlayerMask = GetEnemyLayerMask;

		explosion.OnExplosion -= OnExplosion;
		explosion.OnExplosion += OnExplosion;
		explosion.transform.SetParent(null);

		explosionObj.SetActive(true);
	}
	private void OnExplosion(int hitCount, Weapon weapon, Collider[] colliders, Vector3 center)
	{
		Debug.Log($"[Spell_Explosion] | hit count : {hitCount}");
		for (int i = 0 ; i < hitCount ; i++)
		{
			if (colliders[i].gameObject.TryGetComponent<IDamagable>(out var damagable) == true)
			{
				DamageProcessor.ApplyDamage(damagable, weapon, AttackData, center, Vector3.zero);
			}
		}
	}

}


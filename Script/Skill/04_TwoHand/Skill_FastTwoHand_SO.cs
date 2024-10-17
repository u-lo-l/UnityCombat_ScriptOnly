using UnityEngine;
using static GetLayerMask;

/// <summary>
/// 마지막에 내려찍는 부분에서 땅에 강하게 충격파 발생. 적들을 튕겨냄
/// </summary>
[CreateAssetMenu(fileName = "TwoHand Fast Skill", menuName = "Spell/Player/Fast/TwoHand", order = 6)]
public class FastTwoHandSkill : WeaponSkill
{
	[SerializeField] private GameObject smallExplosionPrefab;
	[SerializeField] private GameObject largeExplosionPrefab;
	[SerializeField] private AudioClip flyingAudioClip;
	public override void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[TwoHand] Fast Skill Executed by Player");

		PlayAttackSound(player);
		MakeExplosion(player, weapon, smallExplosionPrefab, 1);
	}
	public override void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[TwoHand] Fast Skill Executed by Enemy");
		throw new System.NotImplementedException();
	}
	public override void Finish(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[TwoHand] Fast Skill Finishedr");
		PlayAttackSound(player);
		MakeExplosion(player, weapon, largeExplosionPrefab, 2);
	}

#region Explosion

	private void MakeExplosion(Player player, Weapon weapon, GameObject prefab, float radius)
	{
		Vector3 center = player.transform.position + player.transform.forward + Vector3.up;
		if (Physics.Raycast(center, Vector3.down, out RaycastHit hit, 3f, GetObstacleLayerMask) == false)
			return ;

		int enemyLayer = 1 << LayerMask.NameToLayer("Enemy");
		Vector3 point = hit.point + hit.normal * 0.05f;
		Quaternion localRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
		GameObject explosionobj = Instantiate<GameObject>(prefab, point, localRotation);
		explosionobj.SetActive(false);

		Explosion explosion = explosionobj.GetComponent<Explosion>();
		explosion.SetExplosionRadius(radius);
		explosion.Weapon = weapon;
		explosion.TargetlayerMask = GetEnemyLayerMask;
		explosion.OnExplosion -= OnExplosion;
		explosion.OnExplosion += OnExplosion;

		explosionobj.SetActive(true);
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
#endregion


}
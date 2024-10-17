using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static GetLayerMask;

/// <summary>
/// 주변의 적 만큼 작은 칼을 소환하여 수직으로 떨어뜨림
/// </summary>
[CreateAssetMenu(fileName = "GreatSword Fast Skill", menuName = "Spell/Player/Fast/GreatSword", order = 8)]
public class FastGreatSwordSkill : WeaponSkill_Throwing
{
	[SerializeField] private CameraShakeImpulseData impulseData;
	private const float SearchRadius = 5f;

#region WeaponSkill Part
	public override void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[GreatSword] Fast Skill Executed by Player");

		base.PlayAttackSound(player);

		Vector3 origin = player.transform.position + player.EnvironmentChecker.FixedForward * SearchRadius;
		Collider[] targetsBuffer = new Collider[10];
		VerticalFallingSword[] fallingSworBuffer = new VerticalFallingSword[10];

		projectileCount = Physics.OverlapSphereNonAlloc(origin, SearchRadius, targetsBuffer, GetEnemyLayerMask);
		Debug.Log($"targetCount : {projectileCount}");
		for(int i = 0 ; i < projectileCount ; i++)
		{
			Character enemy = targetsBuffer[i].GetComponent<Character>();
			float enemyHeight = enemy.Height;

			float spawnOffset = enemyHeight + 5f;
			Vector3 position = targetsBuffer[i].transform.position + Vector3.up * spawnOffset;

			fallingSworBuffer[i] = CreateProjectile<VerticalFallingSword>(position, Quaternion.identity, player, weapon, GetEnemyLayerMask);
			SetProjectildAudio(fallingSworBuffer[i]);
		}
	}
	public override void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[GreatSword] Fast Skill Executed by Enemy");
		throw new System.NotImplementedException();
	}
	public override void Finish(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[GreatSword] Fast Skill Finishedr");
		Debug.Assert(impulseData != null, "[GreatSword Fast Skill] : impulse Data Not Found");

		weapon.MakeCamereShake(impulseData);
	}
#endregion
#region Main Method
	protected override void SetMuzzle(Character holder, out Vector3 position, out Quaternion rotation)
	{
		position = Vector3.zero;
		rotation = Quaternion.identity;
	}
	#endregion
}
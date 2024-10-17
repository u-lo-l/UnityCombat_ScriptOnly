using UnityEngine;

/// <summary>
/// 주변 원형으로 충격파 생성 및 적 위로 띄우기
/// </summary>
[CreateAssetMenu(fileName = "Fist Strong Skill", menuName = "Spell/Player/Strong/Fist", order = 1)]
public class StrongFistSkill : WeaponSkill_Throwing
{
	[SerializeField] private GameObject fistPrefab;
	private const float ForwardOffset = 5f;
	private const float UpwardOffset = 10f;

	public override void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Fist] Strong Skill Executed by Player");
		PlayAttackSound(player);
	}
	public override void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Fist] Strong Skill Executed by Enemy");
		throw new System.NotImplementedException();
	}
	public override void Finish(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Fist] Strong Skill Finished");
		SetMuzzle(player, out Vector3 position, out Quaternion rotation);
		CreateProjectile<FistPunch>(position, rotation, player, weapon, GetLayerMask.GetEnemyLayerMask);
	}

	protected override void SetMuzzle(Character holder, out Vector3 position, out Quaternion rotation)
	{
		position = holder.transform.position +
				   holder.transform.forward * ForwardOffset +
				   Vector3.up * UpwardOffset;
		rotation = Quaternion.LookRotation(-holder.transform.forward);
		rotation *= Quaternion.Euler(90, 0, 0);
	}
}
using UnityEngine;
using static GetLayerMask;

/// <summary>
/// 주변의 적에게 번개를 내려치고, 번개를 맞은 적은 3초간 매초 지속피해를 입는다.
/// </summary>
[CreateAssetMenu(fileName = "Bow Strong Skill", menuName = "Spell/Player/Strong/Bow", order = 15)]
public class StrongBowSkill : WeaponSkill_Throwing
{
	public override void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Bow] Strong Skill Executed by Player");
		// player.SwitchToFreeLookMode();
		SetMuzzle(player, out Vector3 position, out Quaternion orientation);
		Projectile dragon = CreateProjectile<ShootDragon>(muzzlePosition : position,
														  muzzleRotation : orientation,
														  owner : player,
														  weapon : weapon,
														  targetLayerMask : GetEnemyLayerMask);
		SetProjectildAudio(dragon, 0.5f);
	}
	public override void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Bow] Strong Skill Executed by Enemy");
		throw new System.NotImplementedException();
	}
	public override void Finish(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Bow] Strong Skill Finishedr");
		throw new System.NotImplementedException();
	}
	protected override void SetMuzzle(Character holder, out Vector3 position, out Quaternion rotation)
	{
		Transform leftHand = holder.transform.FindByName("Socket:LeftHand");
		rotation = holder.transform.rotation;
		if (leftHand == null)
		{
			position = holder.transform.position + holder.transform.forward * 0.05f + holder.transform.right * 0.5f + holder.Height* 0.6f * Vector3.up;
		}
		else
		{
			position = leftHand.position - holder.transform.forward * 0.1f + holder.transform.right * 0.1f;
		}
	}
}
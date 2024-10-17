 using UnityEngine;

/// <summary>
/// 짧은 거리만큼 초승달 모양의 검기 발사
/// </summary>
[CreateAssetMenu(fileName = "Sword Fast Skill", menuName = "Spell/Player/Fast/Sword", order = 2)]
public class FastSwordSkill : WeaponSkill_Throwing
{
#region WeaponSkill Part
	public override void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Assert(projectilePrefab != null, "[Fast Sword Skill] : Projectile Prefab not found");

		SetMuzzle(player, out Vector3 position, out Quaternion orientation);
		if (aimPosition != null)
		{
			Vector3 dir = (aimPosition.Value - position).normalized;
			dir = dir == Vector3.zero ? player.transform.forward : dir;
			Quaternion.LookRotation(dir);
		}
		Projectile slashProjectile = CreateProjectile<SlashProjectile>(muzzlePosition : position,
																	   muzzleRotation : orientation,
																	   owner : player,
																	   weapon : weapon,
																	   targetLayerMask : GetLayerMask.GetEnemyLayerMask);
		SetProjectildAudio(slashProjectile);
	}
	public override void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		throw new System.NotImplementedException();
	}
	public override void Finish(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		throw new System.NotImplementedException();
	}
	#endregion

	protected override void SetMuzzle(Character holder, out Vector3 position, out Quaternion rotation)
	{
		position = holder.transform.position +
				   holder.EnvironmentChecker.FixedForward * 1.5f +
				   0.7f * holder.Height * Vector3.up;
		rotation = Quaternion.LookRotation(holder.EnvironmentChecker.FixedForward);
	}
}


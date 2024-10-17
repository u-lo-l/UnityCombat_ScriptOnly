
using UnityEngine;
using static GetLayerMask;

[CreateAssetMenu(fileName = "MagicBall", menuName = "Spell/MagicBall", order = 0)]
public class Spell_ThrowMagicBall : WeaponSkill_Throwing
{
	public override void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Assert(projectilePrefab != null, "[Staff Attack] : Projectile Prefab not found");
		if (attackData != null)
			this.AttackData = attackData;

		SetMuzzle(player, out Vector3 position, out Quaternion orientation);
		MagicBall magicBall = CreateProjectile<MagicBall>(	muzzlePosition : position,
															muzzleRotation : orientation,
															owner : player,
															weapon : weapon,
															targetLayerMask : GetEnemyLayerMask);
		SetProjectildAudio(magicBall);
	}

	public override void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Assert(projectilePrefab != null, "[Staff Attack] : Projectile Prefab not found");
		if (attackData != null)
			this.AttackData = attackData;

		SetMuzzle(enemy, out Vector3 position, out Quaternion orientation);
		if (enemy.GetTargetTransform() != null)
		{
			Vector3 direction = enemy.GetTargetTransform().position + Vector3.up * 0.5f - position;
			if (Vector3.Dot(direction, enemy.transform.forward) >= Mathf.Cos(Mathf.Deg2Rad * 30))
			{
				orientation = Quaternion.LookRotation(direction);
			}
		}
		MagicBall magicBall = CreateProjectile<MagicBall>(muzzlePosition : position,
														  muzzleRotation : orientation,
														  owner : enemy,
														  weapon : weapon,
														  targetLayerMask : GetPlayerLayerMask | GetAllyLayerMask);
		SetProjectildAudio(magicBall);
	}
	public override void Finish(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		throw new System.NotImplementedException();
	}

	protected override void SetMuzzle(Character holder, out Vector3 position, out Quaternion rotation)
	{
		Transform holderTransform = holder.transform;
		Vector3 offset = holderTransform.forward * 1f + holderTransform.up * holder.Height * 0.75f;
		position = holderTransform.position + offset;
		rotation = Quaternion.LookRotation(holder.EnvironmentChecker.FixedForward);
	}
	protected override void OnProjectileHit(Collider target, Weapon weapon, Vector3 hitPointOnWorld)
	{
		if (target.TryGetComponent<IDamagable>(out IDamagable damagable) == true)
		{
			DamageProcessor.ApplyDamage(damagable, weapon, AttackData, hitPointOnWorld, Vector3.zero);
			(weapon as Staff).OnAttackSuccess(AttackData.GuageAmount);
		}
	}
}

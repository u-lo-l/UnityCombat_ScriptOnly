
using Unity.VisualScripting;
using UnityEngine;

public abstract class WeaponSkill_Throwing : WeaponSkill
{
	[SerializeField] protected GameObject projectilePrefab;
	[SerializeField] protected AudioClip flyingAudioClip;
	[SerializeField] protected int projectileCount = 1;

	public override void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Assert(projectilePrefab != null, "[Fast Sword Skill] : Projectile Prefab not found");
		base.PlayAttackSound(player);


	}
	public override void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		throw new System.NotImplementedException();
	}
	public override void Finish(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		throw new System.NotImplementedException();
	}

	protected abstract void SetMuzzle(Character holder, out Vector3 position, out Quaternion rotation);
	protected virtual void OnProjectileHit(Collider target, Weapon weapon, Vector3 hitPointOnWorld)
	{
		if (target.TryGetComponent<IDamagable>(out IDamagable damagable) == true)
		{
			DamageProcessor.ApplyDamage(damagable, weapon, AttackData, hitPointOnWorld, Vector3.zero);
		}
	}

	protected void SetProjectildAudio(Projectile projectile, float volumeScaler = 1f)
	{
		AudioSource audioSource = projectile.AddComponent<AudioSource>();
		audioSource.playOnAwake = false;
		audioSource.pitch = Random.Range(1 - 0.5f, 1 + 0.5f);
		audioSource.minDistance = 1f;
		audioSource.maxDistance = 20f;
		audioSource.rolloffMode = AudioRolloffMode.Linear;
		audioSource.spatialBlend = 1f;
		audioSource.clip = flyingAudioClip;
		audioSource.volume = AudioVolumeManager.EffectVolume * volumeScaler;
		audioSource.Play();
	}

	protected TProjectile CreateProjectile<TProjectile>(Vector3 muzzlePosition,
														Quaternion muzzleRotation,
														Character owner,
														Weapon weapon,
														LayerMask targetLayerMask) where TProjectile : Projectile
	{
		GameObject projectileObject = Instantiate<GameObject>(projectilePrefab, muzzlePosition, muzzleRotation);
		projectileObject.SetActive(false);

		TProjectile projectile = projectileObject.GetComponent<TProjectile>();
		projectile.Owner = owner;
		projectile.Weapon = weapon;
		projectile.TargetLayerMask = targetLayerMask;
		projectile.OnProjectileHit -= OnProjectileHit;
		projectile.OnProjectileHit += OnProjectileHit;

		projectileObject.SetActive(true);
		return projectile;
	}
}

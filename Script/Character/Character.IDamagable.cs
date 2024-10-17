using UnityEngine;

public abstract partial class Character
{
	void IDamagable.ReduceHpOnDamage(float amount)
	{
		if (IsDead == false)
			this.CharacterStatus.OnDamage(amount);
	}
	void IDamagable.AddImpulseOnDamage(ActionData attackData, Vector3 force)
	{
		StartCoroutine(LaunchByAttack(attackData, force));
	}
	void IDamagable.PlayHitSound(AudioClip hitSound)
	{
		if (hitSound == null)
			return ;
		AudioSource.minDistance = 1f;
		AudioSource.maxDistance = 20f;
		AudioSource.rolloffMode = AudioRolloffMode.Linear;
		AudioSource.spatialBlend = 1f;
		float volume = AudioVolumeManager.EffectVolume;
		AudioSource.PlayOneShot(hitSound, volume);
	}
	void IDamagable.ApplyDamageColor()
	{
		StartCoroutine(DamageProcessor.ApplyDamageColor(skinMaterials, skinColors, 0.3f, 3));
	}
	void IDamagable.SwitchToDamageState(ActionData actionData)
	{
		this.OnDamage(actionData);
	}
	protected abstract void OnDamage(ActionData actionData);
	Transform IDamagable.GetTransform() => this.transform;
	bool IDamagable.IsDead() => IsDead;
	protected virtual void Die(int delay = 0)
	{
		if (ShouldSelfDestroy == true)
			Destroy(this.gameObject, delay);
	}
}
using UnityEngine;
public interface IDamagable
{
	public void ReduceHpOnDamage(float amount);
	public void AddImpulseOnDamage(ActionData attackData, Vector3 forceOnWorldCoordinate);
	public void PlayHitSound(AudioClip hitSoundClip);
	public void ApplyDamageColor();
	public void SwitchToDamageState(ActionData actionData);
	public Transform GetTransform();
	public bool IsDead();
}
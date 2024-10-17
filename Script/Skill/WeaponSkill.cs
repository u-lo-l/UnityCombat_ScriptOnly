
using System.Collections;
using Cinemachine;
using UnityEngine;

// [CreateAssetMenu(fileName = "Fist Fast Skill", menuName = "Spell/Player/FistFast")]
public abstract class WeaponSkill : ScriptableObject, ISpell
{
	[field : Header("Name")]
	[field : SerializeField] public string Name {get; protected set;}

	[field : Header("Data")]
	[field : SerializeField] public ActionData AttackData {get; protected set;}
	[SerializeField, Range(0.3f, 0.7f)] protected float pitchRandomRange = 0.5f;
	public abstract void Execute(Player player, Weapon weapon, ActionData attackData = null, Vector3? aimPosition = null);
	public abstract void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData = null, Vector3? aimPosition = null);
	public abstract void Finish(Player player, Weapon weapon, ActionData attackData = null, Vector3? aimPosition = null);
	public bool ShoulStop()
	{
		return !AttackData.CanMove;
	}
	protected void PlayClip(Character owner, AudioClip clip)
	{
		if (CheckAudioSourceExist(owner, out AudioSource audioSource) == false)
				return ;
		audioSource.PlayOneShot(clip);
	}
	protected void PlayAttackSound(Character owner)
	{
		if (CheckAudioSourceExist(owner, out AudioSource audioSource) == false)
			return ;
		if (this.AttackData.AttackSound != null)
		{
			audioSource.minDistance = 1f;
			audioSource.maxDistance = 20f;
			audioSource.rolloffMode = AudioRolloffMode.Linear;
			audioSource.spatialBlend = 1f;
			audioSource.pitch = Random.Range(1 - 0.5f, 1 + 0.5f);
			audioSource.clip = this.AttackData.AttackSound;
			audioSource.volume = AudioVolumeManager.EffectVolume;
			audioSource.Play();
		}
	}

	protected void PlayHitSound(Character owner)
	{
		if (CheckAudioSourceExist(owner, out AudioSource audioSource) == false)
			return ;
		if (AttackData.HitSound != null)
		{
			audioSource.minDistance = 1f;
			audioSource.maxDistance = 20f;
			audioSource.rolloffMode = AudioRolloffMode.Linear;
			audioSource.spatialBlend = 1f;
			audioSource.pitch = Random.Range(1 - pitchRandomRange, 1 + pitchRandomRange);
			audioSource.volume = AudioVolumeManager.EffectVolume;
			audioSource.clip = this.AttackData.HitSound;
			audioSource.Play();
		}
	}

	private bool CheckAudioSourceExist(Character owner, out AudioSource audioSource)
	{
		Transform armory = owner.transform.FindByName("Armory");
		if (armory == null)
		{
			if (owner.TryGetComponent<AudioSource>(out audioSource) == true)
				return true;
			else
				return false;
		}
		if(armory.TryGetComponent(out audioSource) == true)
			return true;
		else
			return false;
	}

	protected void PlayCameraShake(CinemachineImpulseSource impulseSource)
	{
		if (impulseSource!= null)
		{
			impulseSource.Shake(AttackData.ImpulseData);
		}
	}
}

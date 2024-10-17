using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MeleeAudioPlayer : MonoBehaviour
{
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private WeaponStatData weaponStat;
	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		Debug.Assert(weaponStat == null, "[MeleeAudioPlayer] : weaponStatData Not Found");
	}

	private void Play()
	{

	}
}

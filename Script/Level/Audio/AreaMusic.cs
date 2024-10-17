using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class AreaMusic : MonoBehaviour
{
	public LayerMask TriggingLayerMask;
	private BackGroundMusic bgmManager;
	private void OnEnable()
	{
		bgmManager = GetComponentInParent<BackGroundMusic>();
		Debug.Assert(bgmManager != null, $"[AreaMusic {name}] BackGroundMusic Not Found");
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((TriggingLayerMask & (1 << other.gameObject.layer)) == 0)
		{
			bgmManager.PushTrack(this.name);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((TriggingLayerMask & (1 << other.gameObject.layer)) == 0)
		{
			bgmManager.PopTrack();	
		}
	}
}

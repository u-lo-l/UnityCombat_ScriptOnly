using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SequenceTrigger : Sequence
{
	private new Collider collider;
	[SerializeField] private bool isTriggered = false;
	[SerializeField, Tooltip("Sequence를 발동시킬 GameObject이다. 일반적으로 Player가 입장해서 발동시키는 경우, Player로 지정한다.")]
	private LayerMask triggeringLayerMask;
	[SerializeField, Tooltip("Timeline")]

	private void Awake()
	{
		collider = GetComponent<Collider>();
		collider.isTrigger = true;
	}
	private void Start()
	{
		// triggeringLayer = GetLayerMask.GetPlayerLayer;
		// transform.SetParent(null);
	}
	private void OnTriggerEnter(Collider other)
	{
		print($"targetLayer : {(int)triggeringLayerMask} | currentLayer : {1 <<other.gameObject.layer}");
		if (isTriggered == true)
		{
			return ;
		}
		if ((1 << other.gameObject.layer & triggeringLayerMask.value) != 0)
		{
			isTriggered = true;
			PlayDirector();
			return ;
		}
	}
	public void ReStart()
	{
		isTriggered = false;
		isPlayed = false;
		collider.enabled = true;
	}
	public override void PlayDirector()
	{
		print("Try PlayDirector");
		if (repeatType == RepeatType.Once | isPlayed == false)
		{
			collider.enabled = false;
			Renderer renderer = GetComponent<Renderer>();
			if (renderer != null)
				renderer.enabled = false;
		}
		base.PlayDirector();
	}
}

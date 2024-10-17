using System.Collections;
using UnityEngine;

public class LifeTimeComponent : MonoBehaviour
{
	[field : SerializeField] public float LifeTime { get; set; } = 10f;
	[field : SerializeField] public GameObject Owner { private get; set; }
	private Coroutine lifeTimeCoroutine = null;
	private float elapsedTime = 0;
	public float RemainLifetimeRate => 1 - elapsedTime / LifeTime;
	private void OnEnable()
	{
		LifeTime *= Random.Range(0.8f, 1.2f);
		if (Owner == null || LifeTime <= 0)
			return ;
		lifeTimeCoroutine = StartCoroutine(CountLifeTime());
	}
	private void OnDisable()
	{
		if (lifeTimeCoroutine != null)
			StopCoroutine(lifeTimeCoroutine);
	}

	private IEnumerator CountLifeTime()
	{
		while (elapsedTime < LifeTime)
		{
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		lifeTimeCoroutine = null;
		if (AnimateDestroyClip() == false)
			Destroy(Owner);
		else
			Destroy(Owner, 0.6f);
	}

	private bool AnimateDestroyClip()
	{
		if (Owner.TryGetComponent<Animator>(out Animator animator) == false)
		{
			return false;
		}
		if (animator.HasState(6, Animator.StringToHash("Dissappear")) == false)
		{
			return false;
		}
		animator.SetTrigger("DisapearTrigger");
		return true;
	}
}

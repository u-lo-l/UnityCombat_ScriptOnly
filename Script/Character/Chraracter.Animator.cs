using System.Collections;
using UnityEngine;
public abstract partial class Character
{
private Coroutine FadingCoroutine;
	public void LayerFadeIn(Animator animator, int Layer, float fadeTime = 0)
	{
		if (FadingCoroutine != null)
		{
			StopCoroutine(FadingCoroutine);
			FadingCoroutine = null;
		}
		if (fadeTime == 0)
		{
			animator.SetLayerWeight(Layer, 1);
			return;
		}
		FadingCoroutine = StartCoroutine(LayerFadeInOut(animator, Layer, fadeTime));
	}

	public void LayerFadeOut(Animator animator, int Layer, float fadeTime = 0)
	{
		if (FadingCoroutine != null)
		{
			StopCoroutine(FadingCoroutine);
			FadingCoroutine = null;
		}
		if (fadeTime == 0)
		{
			animator.SetLayerWeight(Layer, 0);
			return;
		}
		FadingCoroutine = StartCoroutine(LayerFadeInOut(animator, Layer, -fadeTime));
	}

	private IEnumerator LayerFadeInOut(Animator animator, int Layer, float fadeTime)
	{
		float step = Time.deltaTime / fadeTime;

		float from = animator.GetLayerWeight(Layer);
		float to = 1;
		float elapsedTime = 0;

		if (fadeTime < 0)
		{
			to = 0f;
			fadeTime = -fadeTime;
		}

		if (Mathf.Approximately(to, from) == true)
		{
			animator.SetLayerWeight(Layer, to);
			yield break;
		}

		while (elapsedTime < fadeTime)
		{
			animator.SetLayerWeight(Layer, from);
			elapsedTime += Time.deltaTime;
			from += step;
			yield return null;
		}
		animator.SetLayerWeight(Layer, to);
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FrameStopManager : MonoBehaviour
{
	[SerializeField] private Volume globalVolume;
	private Vignette vignette = null;
	private const float InitialVignetteIntensity = 0.3f;
	private const float SlowedVignetteIntensity = 0.35f;
	private ChromaticAberration chromaticAberration = null;
	private const float InitialAberrationIntensity = 0f;
	private const float SlowedAberrationIntensity = 1f;
	public static FrameStopManager Instance;
	private List<IFrameStoppable> unslowables;
	private List<IFrameStoppable> stoppables;
	private readonly WaitForEndOfFrame waitForEndOfFrame = new();
	// [SerializeField] float slowMotionSpeed = 0.01f;
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			stoppables = new();
			unslowables = new();
			if (globalVolume != null)
			{
				globalVolume.profile.TryGet<Vignette>(out vignette);
				ResetVignetteEffect();
				globalVolume.profile.TryGet<ChromaticAberration>(out chromaticAberration);
				ResetChromaticAberrationEffect();
			}
		}
		else
		{
			Destroy(gameObject);
		}
		// DontDestroyOnLoad(gameObject);
	}

	public void RegisterStoppable(IFrameStoppable character)
	{
		if (!stoppables.Contains(character))
		{
			stoppables.Add(character);
		}
	}
	public void UnregisterStoppable(IFrameStoppable character)
	{
		if (stoppables.Contains(character))
		{
			stoppables.Remove(character);
		}
	}
	public void RegisterUnslowable(IFrameStoppable character)
	{
		if (!unslowables.Contains(character))
		{
			unslowables.Add(character);
		}
	}
	public void UnregisterUnslowable(IFrameStoppable character)
	{
		if (unslowables.Contains(character))
		{
			unslowables.Remove(character);
		}
	}
#region Hit Stop
	private Coroutine hitStopCoroutine;
	private float remainStopTime = 0;
	public void StopAll(int duration)
	{
		float time = ((float)duration) * Time.fixedDeltaTime;
		if (time <= remainStopTime)
			return ;
		remainStopTime = time;
		if (hitStopCoroutine != null)
		{
			StopCoroutine(hitStopCoroutine);
			remainSlowTime = 0;
			foreach (var character in stoppables)
			{
				character.Release();
			}
		}
		hitStopCoroutine = StartCoroutine(HitStopCoroutine());
	}
	private IEnumerator HitStopCoroutine(float speed = 0f)
	{
		foreach (var character in stoppables)
		{
			character.Freeze(speed);
		}

		while (remainStopTime > 0)
		{
			remainStopTime -= Time.deltaTime;
			yield return null;
		}
		remainSlowTime = 0;

		foreach (var character in stoppables)
		{
			character.Release();
		}
		hitStopCoroutine = null;
	}
#endregion

#region Evade Slow
	private Coroutine dodgeSlowCoroutine;
	private float remainSlowTime = 0;
	private readonly float slowFactor = 0.3f;
	public void SlowEnemies(int duration = 3)
	{
		Debug.Log("Evade!");
		float time = (float)duration;
		if (time <= remainSlowTime)
			return ;
		remainSlowTime = time;
		if (dodgeSlowCoroutine != null)
		{
			StopCoroutine(dodgeSlowCoroutine);
			foreach (var character in unslowables)
			{
				character.Release();
				character.RelaseFastMode();
			}
			Time.timeScale = 1;
			ResetVignetteEffect();
			ResetChromaticAberrationEffect();
		}
		dodgeSlowCoroutine = StartCoroutine(SlowCoroutine());
	}

	private IEnumerator SlowCoroutine()
	{
		Time.timeScale = slowFactor;
		ApplyVignetteEffect();
		ApplyChromaticAberrationEffect();
		foreach (var character in unslowables)
		{
			character.Freeze(1 / slowFactor);
			character.FastMode();
		}

		while (remainSlowTime > 0)
		{
			remainSlowTime -= Time.deltaTime / Time.timeScale;
			yield return null;
		}

		foreach (var character in unslowables)
		{
			character.Release();
			character.RelaseFastMode();
		}
		dodgeSlowCoroutine = null;
		Time.timeScale = 1;
		ResetVignetteEffect();
		ResetChromaticAberrationEffect();
	}

	private void ApplyVignetteEffect()
	{
		if (vignette != null)
			vignette.intensity.value = SlowedVignetteIntensity;
	}
	private void ResetVignetteEffect()
	{
		if (vignette != null)
			vignette.intensity.value = InitialVignetteIntensity;
	}

	private void ApplyChromaticAberrationEffect()
	{
		if (vignette != null)
			chromaticAberration.intensity.value = SlowedAberrationIntensity;
	}
	private void ResetChromaticAberrationEffect()
	{
		if (vignette != null)
			chromaticAberration.intensity.value = InitialAberrationIntensity;
	}
#endregion
}

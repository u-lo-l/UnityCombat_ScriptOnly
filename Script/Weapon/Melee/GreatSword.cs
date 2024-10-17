using System.Collections;
using UnityEngine;

public class GreatSword : Melee
{
	private Coroutine resizingCoroutine;
	private Transform weaponTransform;
	private Vector3 originalLocalPosition;

	protected override void Awake()
	{
		base.Awake();
		EquipAnimationHash = AnimatorHash.Player.EquipGreatSword;
	}
	protected override void Start()
	{
		base.Start();
		weaponTransform = weaponObjs[0].transform;
		originalLocalPosition = weaponTransform.localPosition;
	}

	public bool TryGetCapsulcollider(out CapsuleCollider capsuleCollider)
	{
		capsuleCollider = colliders[0] as CapsuleCollider;
		return capsuleCollider != null;
	}

	public void EnlargeGreatSword(float time, float scale = 5f ,AnimationCurve curve = null)
	{
		if (resizingCoroutine != null)
		{
			StopCoroutine(resizingCoroutine);
		}
		resizingCoroutine = StartCoroutine(WeaponEnlargement(time, scale, curve));
	}

	public void ResizeGreatSword(float time, AnimationCurve curve = null)
	{
		if (resizingCoroutine != null)
		{
			StopCoroutine(resizingCoroutine);
		}
		resizingCoroutine = StartCoroutine(WeaponRecoverSize(time, curve));
	}

	private IEnumerator WeaponEnlargement(float time, float scale = 5f ,AnimationCurve curve = null)
	{
		float elapsedTime = 0;
		Vector3 startScale = weaponTransform.localScale;
		Vector3 newPositionLocalOffset = new(-2f, 1f, 0);
		while (elapsedTime < time)
		{
			elapsedTime += Time.deltaTime;
			float lerpRate = elapsedTime / time;
			if (curve != null)
				lerpRate = curve.Evaluate(lerpRate);
			weaponTransform.localScale = Vector3.Lerp(startScale, Vector3.one * scale, lerpRate);
			weaponTransform.localPosition = Vector3.Lerp(originalLocalPosition, originalLocalPosition + newPositionLocalOffset, lerpRate);
			yield return null;
		}
		resizingCoroutine = null;
	}

	private IEnumerator WeaponRecoverSize(float time, AnimationCurve curve = null)
	{
		float elapsedTime = 0;
		Vector3 startScale = weaponTransform.localScale;
		Vector3 startLocalPosition = weaponTransform.localPosition;
		while (elapsedTime < time)
		{
			elapsedTime += Time.deltaTime;
			float lerpRate = elapsedTime / time;
			if (curve != null)
				lerpRate = curve.Evaluate(lerpRate);
			weaponTransform.localScale = Vector3.Lerp(startScale, Vector3.one, lerpRate);
			weaponTransform.localPosition = Vector3.Lerp(startLocalPosition, originalLocalPosition, lerpRate);
			yield return null;
		}
		resizingCoroutine = null;
	}
}

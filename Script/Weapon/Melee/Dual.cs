using System.Collections;
using UnityEngine;
public class Dual : Melee
{
	private Transform[] weaponTransform = new Transform[2];
	private Vector3[] originalLocalPosition = new Vector3[2];
#region Monobehaviour
	protected override void Awake()
	{
		base.Awake();
		EquipAnimationHash = AnimatorHash.Player.EquipDual;
	}
	protected override void Start()
	{
		base.Start();
		weaponTransform[0] = weaponObjs[0].transform;
		weaponTransform[1] = weaponObjs[1].transform;
	}
	#endregion
#region WeaponEnlargement
private Coroutine[] resizingCoroutine = new Coroutine[2];

	public void EnlargeBlade(float time, float scale = 5f ,AnimationCurve curve = null)
	{
		resizingCoroutine ??= new Coroutine[2];
		if (resizingCoroutine[0] != null)
		{
			StopCoroutine(resizingCoroutine[0]);
		}
		if (resizingCoroutine[1] != null)
		{
			StopCoroutine(resizingCoroutine[1]);
		}
		resizingCoroutine[0] = StartCoroutine(WeaponEnlargement(0, time, scale, new Vector3(0,0,0), curve));
		resizingCoroutine[1] = StartCoroutine(WeaponEnlargement(1, time, scale, new Vector3(0,0,0), curve));
	}

	public void ResizeBlade(float time, AnimationCurve curve = null)
	{
		resizingCoroutine ??= new Coroutine[2];
		if (resizingCoroutine[0] != null)
		{
			StopCoroutine(resizingCoroutine[0]);
		}
		if (resizingCoroutine[1] != null)
		{
			StopCoroutine(resizingCoroutine[1]);
		}
		resizingCoroutine[0] = StartCoroutine(WeaponRecoverSize(0, time, curve));
		resizingCoroutine[1] = StartCoroutine(WeaponRecoverSize(1, time, curve));
	}

	private IEnumerator WeaponEnlargement(int index, float time, float scale, Vector3 offset, AnimationCurve curve = null)
	{
		float elapsedTime = 0;
		Vector3 startScale = weaponTransform[index].localScale;
		while (elapsedTime < time)
		{
			elapsedTime += Time.deltaTime;
			float lerpRate = elapsedTime / time;
			if (curve != null)
				lerpRate = curve.Evaluate(lerpRate);
			weaponTransform[index].localScale = 
				Vector3.Lerp(startScale, Vector3.one * scale, lerpRate);
			weaponTransform[index].localPosition = 
				Vector3.Lerp(originalLocalPosition[index], originalLocalPosition[index] + offset, lerpRate);
			yield return null;
		}
		resizingCoroutine[index] = null;
	}

	private IEnumerator WeaponRecoverSize(int index, float time, AnimationCurve curve = null)
	{
		float elapsedTime = 0;
		Vector3 startScale = weaponTransform[index].localScale;
		Vector3 startLocalPosition = weaponTransform[index].localPosition;
		while (elapsedTime < time)
		{
			elapsedTime += Time.deltaTime;
			float lerpRate = elapsedTime / time;
			if (curve != null)
				lerpRate = curve.Evaluate(lerpRate);
			weaponTransform[index].localScale = Vector3.Lerp(startScale, Vector3.one, lerpRate);
			weaponTransform[index].localPosition = Vector3.Lerp(startLocalPosition, originalLocalPosition[index], lerpRate);
			yield return null;
		}
		resizingCoroutine[index] = null;
	}
#endregion
}

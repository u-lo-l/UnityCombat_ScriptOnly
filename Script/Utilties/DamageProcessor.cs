using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerMovementInputHandler;
public class DamageInfo
{
	public float DamageAmount;
	public float KnockbackForce;
	public int StopFrames;
	public Vector3 HitPosition;
	public GameObject HitParticle;
}
public static class DamageProcessor
{
	/// <summary>
	/// ApplyDamage가 해야 할 일
	/// 1. victim에 실제로 대미지를 주는 행위
	/// 2. HitPoin에 HitParticle 생성
	/// 	=> 자주 Construct / Destroy 되지만 실제 수는 많지 않으므로, Weapon에서 풀링하자.
	/// 3. victim에 HitSound 재생
	/// 4. victim에 ApplyImpact 호출
	/// 5. Character를 DamageState로 전환
	/// </summary>
	public static void ApplyDamage(IDamagable victim, Weapon weapon, ActionData attackData, Vector3 hitPointOnWorld, Vector3 outerForce)
	{
		if (weapon != null)
			weapon.ActivateHitParicle(victim, hitPointOnWorld, attackData);

		Vector3 towardVector = hitPointOnWorld - weapon.Owner.transform.position;
		towardVector.y = 0;
		towardVector.Normalize();
		if (outerForce == Vector3.zero)
		{
			outerForce = attackData.ThrustPower * attackData.ThrustDirection;
		}
		else
		{
			outerForce *= attackData.ThrustPower;
		}
		Quaternion localRotation = Quaternion.FromToRotation(Vector3.forward, towardVector);

		victim.ReduceHpOnDamage(attackData.Damage);
		victim.PlayHitSound(attackData.HitSound);

		victim.AddImpulseOnDamage(attackData, localRotation * outerForce);

		victim.ApplyDamageColor();
		if (attackData.CanMove == false)
			FrameStopManager.Instance.StopAll(attackData.StopFrame);
		victim.SwitchToDamageState(attackData);
	}

	public static IEnumerator ApplyDamageColor(List<Material> skinMaterials, List<Color> originColors, float time, int blinkCount = 1)
	{
		int interval = blinkCount * 2 - 1;
		int skinCount = skinMaterials.Count;
		Color damageColor = new(0.75f, 0.3f, 0.3f);

		List<Material> emissiveMaterails = new();
		for (int i = 0 ; i < skinCount ; i++)
		{
			if (skinMaterials[i].IsKeywordEnabled("_EMISSION") == true)
			{
				emissiveMaterails.Add(skinMaterials[i]);
				skinMaterials[i].DisableKeyword("_EMISSION");
			}
		}

		WaitForSeconds WaitForBlinkTime = new WaitForSeconds(time / interval);

		SetMaterialColor(skinMaterials, damageColor);
		for(int j = 1 ; j < blinkCount ; j++)
		{
			yield return WaitForBlinkTime;
			SetMaterialColor(skinMaterials, originColors);
			yield return WaitForBlinkTime;
			SetMaterialColor(skinMaterials, damageColor);
		}
		yield return WaitForBlinkTime;

		SetMaterialColor(skinMaterials, originColors);
		foreach(var mat in emissiveMaterails)
		{
			mat.EnableKeyword("_EMISSION");
		}
	}
	private static void SetMaterialColor(List<Material> materials, Color color)
	{
		foreach(Material mat in materials)
		{
			mat.color = color;
		}
	}
	private static void SetMaterialColor(List<Material> materials, List<Color> colors)
	{
		if (materials.Count != colors.Count)
		{
			Debug.Assert(false, "Num of Materials and Num of Colors doesn'y match");
			return;
		}
		int count = materials.Count;
		for(int i = 0 ; i < count ; i++)
		{
			materials[i].color = colors[i];
		}
	}
}



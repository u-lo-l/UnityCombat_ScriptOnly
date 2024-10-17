using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class WeaponStatData : ScriptableObject
{
	[Serializable] public class AdditionalData
	{
		[field : SerializeField ] public string Name {get; private set;}
		[field : SerializeField ] public ActionData[] ActionData {get; private set;}
	}
	[field : Header("Weapon Name")]
	[field : SerializeField] public string Name { get; protected set; }

	[field : Header("Weapon Type")]
	[field : SerializeField] public WeaponType Type { get; protected set; }


	[field : Header("Default Stat")]
	[field : SerializeField] public float Range { get; private set; } = 1f;

	[field : Header("Animation")]
	[field : SerializeField] public bool HasEquipAnimation { get; private set; } = true;


	[field : Header("Attacks")]
	[field : SerializeField] public GameObject FastHitParticle { get; private set; }
	[field : SerializeField] public ActionData[] FastActionData { get; private set; }
	[field : SerializeField] public GameObject StrongHitParticle { get; private set; }
	[field : SerializeField] public ActionData[] StrongActionData { get; private set; }
	[field : SerializeField] public List<AdditionalData> AdditionalActionData { get; private set; } = null;
	[field : SerializeField] public WeaponSkill[] WeaponSkillData { get; private set; }
	public int FastActionCount => FastActionData.Length;
	public int StrongActionCount => StrongActionData.Length;
	public int AdditionalActionSize => AdditionalActionData == null ? 0 : AdditionalActionData.Count;
	public int AdditionalActionCount(int i)
	{
		if (i < AdditionalActionSize)
			return AdditionalActionData[i].ActionData.Length;
		else
			return 0;
	}
	[field : SerializeField] public WeaponElementData[] WeaponElements { get; private set; } = new WeaponElementData[2];
	public int ElementCount => WeaponElements == null ? 0 : WeaponElements.Length;
}
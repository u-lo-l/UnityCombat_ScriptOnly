using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SwordStatData", menuName = "StatData/SwordStatData")]
public class SwordStatData : WeaponStatData
{
	public SwordStatData()
	{
		Type = WeaponType.Sword;
	}
}
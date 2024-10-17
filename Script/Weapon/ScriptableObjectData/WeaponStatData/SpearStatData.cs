using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SpearStatData", menuName = "StatData/SpearStatData")]
public class SpearStatData : WeaponStatData
{
	public SpearStatData()
	{
		Type = WeaponType.Spear;
	}
}
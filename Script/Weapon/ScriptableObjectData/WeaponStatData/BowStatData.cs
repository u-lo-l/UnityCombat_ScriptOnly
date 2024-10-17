using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "BowStatData", menuName = "StatData/BowStatData")]
public class BowStatData : WeaponStatData
{
	public BowStatData()
	{
		Type = WeaponType.Bow;
	}
}
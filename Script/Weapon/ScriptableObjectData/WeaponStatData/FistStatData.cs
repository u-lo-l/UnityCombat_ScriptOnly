using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "FistStatData", menuName = "StatData/FistStatData")]
public class FistStatData : WeaponStatData
{
	public FistStatData()
	{
		Type = WeaponType.Fist;
	}
}
using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "DualStatData", menuName = "StatData/DualStatData")]
public class DualStatData : WeaponStatData
{
	public DualStatData()
	{
		Type = WeaponType.Dual;
	}
}
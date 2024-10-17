using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "StaffStatData", menuName = "StatData/StaffStatData")]
public class StaffStatData : WeaponStatData
{
	public StaffStatData()
	{
		Type = WeaponType.Staff;
	}
}
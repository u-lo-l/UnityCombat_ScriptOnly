using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "WarriorStatData", menuName = "StatData/WarriorStatData")]
public class WarriorStatData : WeaponStatData
{
	public WarriorStatData()
	{
		Type = WeaponType.Warrior;
	}
}
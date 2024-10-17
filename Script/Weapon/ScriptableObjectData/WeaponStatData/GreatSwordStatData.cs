using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "GreatSwordStatData", menuName = "StatData/GreatSwordStatData")]
public class GreatSwordStatData : WeaponStatData
{
	public GreatSwordStatData()
	{
		Type = WeaponType.GreatSword;
	}
}
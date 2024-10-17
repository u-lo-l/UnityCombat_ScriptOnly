using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "TwohandStatData", menuName = "StatData/TwohandStatData")]
public class TwohandStatData : WeaponStatData
{
	public TwohandStatData()
	{
		Type = WeaponType.TwoHand;
	}
}
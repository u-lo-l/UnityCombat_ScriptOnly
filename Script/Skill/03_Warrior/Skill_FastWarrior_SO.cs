using System.Collections;
using UnityEngine;

/// <summary>
/// 진행방향의 적을 옆으로 밀치면서 마지막 공격은 적을 스턴
/// 이 떄 전방의 공격은 무시
/// </summary>
[CreateAssetMenu(fileName = "Warrior Fast Skill", menuName = "Spell/Player/Fast/Warrior", order = 4)]
public class FastWarriorSkill : WeaponSkill
{
	public override void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Warrior] Fast Skill Executed by Player");
		(weapon as Warrior).SetSheildMode(ShieldMode.Push);
	}
	public override void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Warrior] Fast Skill Executed by Enemy");
		throw new System.NotImplementedException();
	}
	public override void Finish(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Warrior] Fast Skill Finished");
		(weapon as Warrior).SetSheildMode(ShieldMode.Attack);
	}


}
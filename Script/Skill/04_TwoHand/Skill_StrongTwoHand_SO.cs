using System.Collections;
using UnityEngine;

/// <summary>
/// 가렌 E. 키보드로 이동 가능.
/// </summary>
[CreateAssetMenu(fileName = "WarrioTwoHandr Strong Skill", menuName = "Spell/Player/Strong/TwoHand", order = 7)]
public class StrongTwoHandSkill : WeaponSkill
{
	public override void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[TwoHand] Strong Skill Executed by Player");
		player.StartCoroutine(PlayFor3Seconds(player.Animator));
		base.PlayAttackSound(player);
	}
	public override void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[TwoHand] Strong Skill Executed by Enemy");
		throw new System.NotImplementedException();
	}
	public override void Finish(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[TwoHand] Strong Skill Finishedr");
	}

	private IEnumerator PlayFor3Seconds(Animator animator)
	{
		yield return new WaitForSeconds(3f);
		animator.SetTrigger("StrongSkillEndTrigger");
	}
}
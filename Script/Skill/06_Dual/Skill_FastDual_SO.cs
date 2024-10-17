using UnityEngine;

/// <summary>
/// 베는 Effect만 잘 구현하자. 아마 이 클래스가 필요없을 수도 있다.
/// </summary>
[CreateAssetMenu(fileName = "Dual Fast Skill", menuName = "Spell/Player/Fast/Dual", order = 10)]
public class FastDualSkill : WeaponSkill
{
	[field : Header("Additional Data")]
	[field : SerializeField] public ActionData[] AdditionalActionData  {get; protected set;}

	[field : Header("Size Curves")]
	[SerializeField, Range(1f, 10f)] private float enlargementAmount = 5f;
	[SerializeField, Range(0.01f, 2f)] private float enlargementTime = 1f;
	[SerializeField] private AnimationCurve enlargementCurve;
	[SerializeField, Range(0.01f, 2f)] private float recoverTime = 1f;
	[SerializeField] private AnimationCurve recoverCurve;
	public override void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Dual] Fast Skill Executed by Player");
		Dual dual = weapon as Dual;

		dual.EnlargeBlade(enlargementTime, enlargementAmount, enlargementCurve);

		base.PlayAttackSound(player);
	}
	public override void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Dual] Fast Skill Executed by Enemy");
		throw new System.NotImplementedException();
	}
	public override void Finish(Player player, Weapon weapon, ActionData attackData = null, Vector3? aimPosition = null)
	{
		Debug.Log("[Dual] Fast Skill Finishedr");
		Dual dual = weapon as Dual;
		dual.ResizeBlade(recoverTime, recoverCurve);
	}
}
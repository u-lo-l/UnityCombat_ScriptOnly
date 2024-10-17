using UnityEngine;

public class Staff : Weapon
{
	StaffStatData staffStat;
	protected override void Awake()
	{
		base.Awake();
		EquipAnimationHash = AnimatorHash.Player.EquipStaff;
		staffStat = Stat as StaffStatData;
	}
	protected override void Start()
	{
		base.Start();
	}

	public void OnAttackSuccess(float amount)
	{
		base.AttackSucceed(amount);
	}
}

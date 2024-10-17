public class Warrior : Melee
{
#region Monobehaviour
	protected override void Awake()
	{
		base.Awake();
		EquipAnimationHash = AnimatorHash.Player.EquipWarrior;
		WarriorStatData WarriorStat = Stat as WarriorStatData;
	}
	protected override void Start()
	{
		base.Start();
	}
	#endregion
	public void SetSheildMode(ShieldMode shieldMode)
	{
		if(weaponObjs[0].TryGetComponent<ShieldTrigger>(out ShieldTrigger shield) == false)
		{
			print("NotFound");
			return ;
		}
		print("Found");
		shield.Mode = shieldMode;
	}
}

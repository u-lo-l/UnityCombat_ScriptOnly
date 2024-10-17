using UnityEngine;
public class Fist : Melee
{
#region Monobehaviour
	protected override void Awake()
	{
		base.Awake();
		EquipAnimationHash = AnimatorHash.Player.EquipFist;
	}
	protected override void Start()
	{
		base.Start();
	}
	#endregion
}

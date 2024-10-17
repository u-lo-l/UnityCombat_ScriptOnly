using UnityEngine;

public class Twohand : Melee
{
	protected override void Awake()
	{
		base.Awake();
		EquipAnimationHash = AnimatorHash.Player.EquipTwohand;
	}
	protected override void Start()
	{

		base.Start();
	}
}

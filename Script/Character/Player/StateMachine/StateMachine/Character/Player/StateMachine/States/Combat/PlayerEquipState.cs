using UnityEngine;

public class PlayerEquipState : PlayerHoldState
{
	public int WeaponIndex {private get; set;}
	private readonly Animator animator;

	public PlayerEquipState(PlayerCombatStateMachine stateMachine) : base(stateMachine)
	{
		animator = stateMachine.Player.Animator;
		CurrentState = State.Equip;
		Debug.Assert(animator != null, "[Equipping State] Cannot Find Animator");
		animator.SetLayerWeight(AnimatorHash.Player.EquipLayer, 0.0f);
	}
	public override void Enter()
	{
		combatStateMachine.Player.LayerFadeIn(animator, AnimatorHash.Player.EquipLayer, 0f);
		combatStateMachine.WeaponHandler.TryEquip(WeaponIndex);
	}
	public override void Exit()
	{
		combatStateMachine.Player.LayerFadeOut(animator, AnimatorHash.Player.EquipLayer, 0.0f);
		if (combatStateMachine.WeaponHandler.CompletelyEquipped == false)
		{
			combatStateMachine.WeaponHandler.ForceUnequip();
		}
		else
		{
			animator.Play(AnimatorHash.Player.ArmedStateHashed[WeaponIndex], AnimatorHash.Player.ActionLayer);
			combatStateMachine.Player.bowHandIIKHandler.enabled = combatStateMachine.WeaponHandler.CurrentWeapon is Bow;
		}
	}

}
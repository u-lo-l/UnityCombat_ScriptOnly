using UnityEngine;

public class AIEquipState : AICombatState
{
	public AIEquipState(AICombatStateMachine stateMachine) : base(stateMachine)
	{
		combatStateMachine = stateMachine;

		// Debug.Assert(animator != null, "[Equipping State] Cannot Find Animator");
		CurrentState = State.Equip;
		combatStateMachine.Enemy.NavMeshAgent.speed = 0.5f;
		animator.SetLayerWeight(AnimatorHash.Enemy.EquipLayer, 0.0f);
	}
	public override void Enter()
	{
		// Debug.Log("Enter Equip");
		WeaponType weaponType = combatStateMachine.WeaponHandler.GetFirstWeaponType();
		combatStateMachine.Enemy.LayerFadeIn(animator, AnimatorHash.Enemy.EquipLayer, 0f);
		combatStateMachine.WeaponHandler.TryEquip((int)weaponType);
	}
	public override void Tick()
	{

	}
	public override void FixedTick()
	{

	}
	public override void Exit()
	{
		if (combatStateMachine.WeaponHandler.CompletelyEquipped == false)
		{
			Debug.Log("Not Completely Equipped");
			combatStateMachine.WeaponHandler.ForceUnequip();
		}
		else
		{
			Debug.Log("Completely Equipped");
		}
		combatStateMachine.Enemy.LayerFadeOut(animator, AnimatorHash.Enemy.EquipLayer, 0f);
	}
}
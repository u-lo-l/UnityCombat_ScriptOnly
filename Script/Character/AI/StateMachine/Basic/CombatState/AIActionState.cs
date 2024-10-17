using UnityEngine;

public class AIActionState : AICombatState
{
	protected WeaponHandler weaponHandler;
	private bool nextComboTried;
	public AIActionState(AICombatStateMachine stateMachine) : base(stateMachine)
	{
		CurrentState = State.Attack;
		combatStateMachine = stateMachine;
		weaponHandler = stateMachine.WeaponHandler;
	}
	public override void Enter()
	{
		combatStateMachine.WeaponHandler.CanNextCombo = false;
		nextComboTried = false;
		combatStateMachine.Enemy.ForceStop(0.75f);
		combatStateMachine.Enemy.FaceToTarget();
		combatStateMachine.Enemy.LayerFadeIn(animator, AnimatorHash.Enemy.ActionLayer, 0f);
		combatStateMachine.WeaponHandler.DoFastAttack();

		animator.SetTrigger(AnimatorHash.Enemy.ActionTrigger);
	}
	public override void Tick()
	{
		if (combatStateMachine.WeaponHandler.CanNextCombo == true && nextComboTried == false)
		{
			nextComboTried = true;
			if (Random.Range(0, 10) != 0)
			{
				// Debug.Log("Try Combo");
				combatStateMachine.TryAttack(-1);
			}
		}
	}
	public override void FixedTick()
	{
		
	}
	public override void Exit()
	{
		if (weaponHandler.CanNextCombo == true)
			weaponHandler.IncreaseAttackIndex();
		else
			weaponHandler.ResetAttackIndex();
		weaponHandler.EndAttack();
		// combatStateMachine.Enemy.LayerFadeOut(animator, AnimatorHash.Enemy.ActionLayer, 0f);
		animator.ResetTrigger(AnimatorHash.Enemy.ActionTrigger);
	}
}

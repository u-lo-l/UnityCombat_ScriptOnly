using UnityEngine;

public class PlayerStrongActionState : PlayerActionState
{
	public PlayerStrongActionState(PlayerCombatStateMachine stateMachine)
	 : base(stateMachine)
	{
		CurrentState = State.StrongAttack;
	}
	public override void Enter()
	{
		base.Enter();
		combatStateMachine.WeaponHandler.DoStrongAttack();
		bool canMove  = !weaponHandler.ShouldStopForAttack();
		Debug.Log($"canMove : {canMove}");
		combatStateMachine.Player.movementStateMachine.CanMove = canMove;
		if (canMove == true)
		{
			combatStateMachine.Player.LayerFadeOut(animator, AnimatorHash.Player.ActionLayer, 0f);
			combatStateMachine.Player.LayerFadeIn(animator, AnimatorHash.Player.UpperActionLayer, 0f);
			combatStateMachine.Player.Animator.SetTrigger(AnimatorHash.Player.StrongActionCanMoveTrigger);
		}
		else
		{
			combatStateMachine.Player.Animator.SetTrigger(AnimatorHash.Player.StrongActionTrigger);
		}
	}
	public override void Tick()
	{
		base.Tick();
	}
	public override void Exit()
	{
		base.Exit();
		combatStateMachine.Player.Animator.ResetTrigger(AnimatorHash.Player.StrongActionTrigger);
	}
}

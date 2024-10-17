public class PlayerFastActionState : PlayerActionState
{
	public PlayerFastActionState(PlayerCombatStateMachine stateMachine)
	 : base(stateMachine)
	{
		CurrentState = State.FastAttack;
	}
	public override void Enter()
	{
		base.Enter();
		combatStateMachine.WeaponHandler.DoFastAttack();
		bool canMove  = !weaponHandler.ShouldStopForAttack();
		combatStateMachine.Player.movementStateMachine.CanMove = canMove;
		if (canMove == true)
		{
			combatStateMachine.Player.LayerFadeOut(animator, AnimatorHash.Player.ActionLayer, 0f);
			combatStateMachine.Player.LayerFadeIn(animator, AnimatorHash.Player.UpperActionLayer, 0f);
		}
		combatStateMachine.Player.Animator.SetTrigger(AnimatorHash.Player.FastActionTrigger);
	}
	public override void Tick()
	{
		base.Tick();
	}
	public override void Exit()
	{
		base.Exit();
		combatStateMachine.Player.Animator.ResetTrigger(AnimatorHash.Player.FastActionTrigger);
	}
}

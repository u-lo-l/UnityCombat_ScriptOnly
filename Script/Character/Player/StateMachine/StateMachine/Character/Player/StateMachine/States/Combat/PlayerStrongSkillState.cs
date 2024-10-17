public class PlayerStrongSkillState : PlayerSkillState
{
	public PlayerStrongSkillState(PlayerCombatStateMachine stateMachine) : base(stateMachine)
	{
	}
	public override void Enter()
	{
		base.Enter();
		animator.SetTrigger("StrongSkillTrigger");
	}
	public override void Tick()
	{
		base.Tick();
	}

	public override void Exit()
	{
		base.Exit();
		animator.ResetTrigger("StrongSkillTrigger");
		weaponHandler.StrongSkillPerformed();
	}
}

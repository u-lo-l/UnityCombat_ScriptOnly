public class PlayerFastSkillState : PlayerSkillState
{
	public PlayerFastSkillState(PlayerCombatStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		base.Enter();
		animator.SetTrigger("FastSkillTrigger");
	}
	public override void Tick()
	{
		base.Tick();
	}

	public override void Exit()
	{
		base.Exit();
		animator.ResetTrigger("FastSkillTrigger");
		weaponHandler.FastSkillPerformed();
	}
}

public class PlayerCommonSkillState : PlayerSkillState
{
	public PlayerCommonSkillState(PlayerCombatStateMachine stateMachine) : base(stateMachine)
	{
	}
	public override void Enter()
	{
		base.Enter();
		animator.CrossFadeInFixedTime("CommonSkill", 0.1f, AnimatorHash.Player.ActionLayer);
		weaponHandler.CommonSkillFire();
	}
	public override void Tick()
	{

	}

	public override void Exit()
	{
		base.Exit();
	}
}
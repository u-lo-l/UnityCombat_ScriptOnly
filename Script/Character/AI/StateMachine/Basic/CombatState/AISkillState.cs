using UnityEngine;

public class AISkillState : AIActionState
{
	public AISkillState(AICombatStateMachine stateMachine) : base(stateMachine)
	{
	}
	public override void Enter()
	{
		Debug.Log("AI SKill State");
		combatStateMachine.Enemy.ForceStop(0.75f);
		combatStateMachine.Enemy.FaceToTarget();
		combatStateMachine.Enemy.LayerFadeIn(animator, AnimatorHash.Enemy.ActionLayer, 0f);
		animator.ResetTrigger("ActionTrigger");
		animator.SetTrigger("SkillTrigger");
	}
	public override void Tick()
	{

	}
	public override void FixedTick()
	{
		
	}
	public override void Exit()
	{
		animator.ResetTrigger("SkillTrigger");
		weaponHandler.FastSkillPerformed();
		base.Exit();
	}
}
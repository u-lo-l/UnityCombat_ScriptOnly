using UnityEngine;

public abstract class PlayerSkillState : PlayerActionState
{
	protected PlayerSkillState(PlayerCombatStateMachine stateMachine) : base(stateMachine)
	{
		CurrentState = State.Skill;
	}

	public override void Enter()
	{
		base.Enter();
		combatStateMachine.Player.movementStateMachine.CanMove = !weaponHandler.ShouldStopForSkill();
	}
	public override void Tick()
	{
		// Do Nothing;
	}
	public override void Exit()
	{
		weaponHandler.ResetAttackIndex();
		weaponHandler.EndAttack();
	}
};

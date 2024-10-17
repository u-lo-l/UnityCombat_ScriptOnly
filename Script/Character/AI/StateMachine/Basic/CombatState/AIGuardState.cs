using UnityEngine;

public class AIGuardState : AICombatState
{
	public AIGuardState(AICombatStateMachine stateMachine) : base(stateMachine)
	{
		combatStateMachine = stateMachine;
		CurrentState = State.Guard;
	}
	public override void Enter()
	{
		combatStateMachine.Enemy.LayerFadeIn(animator, AnimatorHash.Enemy.GuardingLayer, 0.5f);
		animator.SetTrigger("GuardTrigger");
	}
	public override void Tick()
	{
		Transform playerTransform = combatStateMachine.Enemy.GetTargetTransform();
		if (playerTransform == null)
		{
			combatStateMachine.ChangeState(combatStateMachine.HoldingState);
		}
		else
		{
			float distance = (playerTransform.position - combatStateMachine.Enemy.transform.position).magnitude;
			if (combatStateMachine.WeaponHandler.WeaponRange > distance)
			{
				combatStateMachine.ChangeState(combatStateMachine.ActionState);
			}
		}
	}
	public override void FixedTick()
	{
		
	}
	public override void Exit()
	{
		combatStateMachine.Enemy.LayerFadeOut(animator, AnimatorHash.Enemy.GuardingLayer, 0.5f);
		animator.SetTrigger("UnguardTrigger");
	}
}
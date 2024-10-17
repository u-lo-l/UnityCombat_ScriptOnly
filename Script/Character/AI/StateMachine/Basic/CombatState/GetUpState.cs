using UnityEngine;

public class GetUpState : AICombatState
{
	public const float MinDuration = 1.2f;
	public float Duration { private get ; set; }
	public GetUpState(AICombatStateMachine stateMachine) : base(stateMachine)
	{
		CurrentState = State.GetUp;
	}

	public override void Enter()
	{
		Duration = Mathf.Max(MinDuration, Duration);
		combatStateMachine.Enemy.ForceStop(Duration);
		combatStateMachine.Enemy.LayerFadeIn(combatStateMachine.Enemy.Animator, AnimatorHash.Enemy.DownLayer, 0f);
		animator.SetTrigger(AnimatorHash.Enemy.GetUpTrigger);
	}

	public override void Tick()
	{
		if (Duration > 0)
		{
			Duration -= Time.deltaTime;
			return ;
		}
		combatStateMachine.ChangeState(combatStateMachine.HoldingState);
	}

	public override void Exit()
	{
		combatStateMachine.Enemy.LayerFadeOut(combatStateMachine.Enemy.Animator, AnimatorHash.Enemy.DownLayer, 0f);
		combatStateMachine.Enemy.PlayLocomotion();
	}
}
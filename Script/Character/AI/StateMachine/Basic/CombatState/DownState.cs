using UnityEngine;

public class DownState : AICombatState
{
	public const float MinDuration = 1f;
	public float Duration { get ; set; }
	protected int animationHash;
	protected EnemyDynamic enemy;
	public DownState(AICombatStateMachine stateMachine) : base(stateMachine)
	{
		CurrentState = State.Down;
		enemy = stateMachine.Enemy;
		animationHash = AnimatorHash.Enemy.Down;
	}

	public override void Enter()
	{
		Duration = Mathf.Max(MinDuration, Duration);
		combatStateMachine.Enemy.ForceStop(Duration);
		combatStateMachine.Enemy.LayerFadeIn(combatStateMachine.Enemy.Animator, AnimatorHash.Enemy.DownLayer, 0f);
		animator.Play(animationHash, AnimatorHash.Enemy.DownLayer, 0f);
		combatStateMachine.Enemy.DisableFootIK();

		if (enemy.NavMeshAgent.enabled == true)
		{
			enemy.NavMeshAgent.velocity = Vector3.zero;
			enemy.NavMeshAgent.isStopped = true;
			enemy.NavMeshAgent.speed = 0;
			enemy.NavMeshAgent.updateRotation = false;
		}
	}

	public override void Tick()
	{
		if (Duration > 0)
		{
			Duration -= Time.deltaTime;
			return ;
		}
		Duration = 0;
		combatStateMachine.ChangeState(combatStateMachine.GettingUpState);
	}

	public override void Exit()
	{
		combatStateMachine.Enemy.PlayLocomotion();
		combatStateMachine.Enemy.LayerFadeOut(combatStateMachine.Enemy.Animator, AnimatorHash.Enemy.DownLayer, 0f);
		combatStateMachine.Enemy.EnableFootIK();
		enemy.NavMeshAgent.updateRotation = true;
	}
}

public class AirborneState : DownState
{
	public AirborneState(AICombatStateMachine stateMachine) : base(stateMachine)
	{
		animationHash = AnimatorHash.Enemy.Airborne;
	}
	public override void Enter()
	{
		base.Enter();
	}
	public override void Tick()
	{
		base.Tick();
	}
	public override void Exit()
	{
		base.Exit();
	}
}
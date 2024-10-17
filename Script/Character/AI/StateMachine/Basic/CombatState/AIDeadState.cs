using UnityEngine;

public class AIDeadState : AICombatState
{
	EnemyDynamic enemy;
	public AIDeadState(AICombatStateMachine stateMachine) : base(stateMachine)
	{
		CurrentState = State.Dead;
		enemy = stateMachine.Enemy;
	}
	public override void Enter()
	{
		Debug.Log("Enter Die STate");
		animator.SetTrigger(AnimatorHash.Enemy.DieTrigger);
		enemy.CanMove = false;
		
		if (enemy.NavMeshAgent.enabled == true)
		{
			enemy.NavMeshAgent.velocity = Vector3.zero;
			enemy.NavMeshAgent.isStopped = true;
			enemy.NavMeshAgent.speed = 0;
			enemy.NavMeshAgent.updateRotation = false;
		}

		enemy.DisableFootIK();

		animator.SetFloat(AnimatorHash.Enemy.SpeedX, 0);
		animator.SetFloat(AnimatorHash.Enemy.SpeedZ, 0);
		enemy.LayerFadeOut(animator, AnimatorHash.Enemy.ActionLayer, 0);
		enemy.LayerFadeOut(animator, AnimatorHash.Enemy.EquipLayer, 0);
		enemy.LayerFadeOut(animator, AnimatorHash.Enemy.DamagedLayer, 0);
		enemy.LayerFadeIn(animator, AnimatorHash.Enemy.DownLayer, 0);
	}
	public override void Tick()
	{

	}
}
using UnityEngine;
using UnityEngine.AI;

public class DefensiveEnemy : EnemyDynamic
{
	protected override void Start()
	{
		base.Start();
		movementStateMachine = new DefensiveAIMovementStateMachine(this, NavMeshAgent);
		combatStateMachine = new DefensiveAICombatStateMachine(this, weaponHandler);
		movementStateMachine.ChangeState(movementStateMachine.StoppingState);
		combatStateMachine.ChangeState(combatStateMachine.HoldingState);
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
	}
}

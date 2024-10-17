using UnityEngine;
using UnityEngine.AI;

public class DefensiveAIMovementStateMachine : AIMovementStateMachine
{
	public DefensiveAIMovementStateMachine(EnemyDynamic enemy, NavMeshAgent navMeshAgent)
	 : base(enemy, navMeshAgent)
	{
		StoppingState = new DefensiveStopState(this);
		FollowingState = new AIFollowState(this);
		WanderingState = new DefensiveWanderState(this);
		SteppingBackState = new AIStepBackState(this);
		currentState = StoppingState;
	}
}
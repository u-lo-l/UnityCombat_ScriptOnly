using System;
using UnityEngine;
using UnityEngine.AI;


// Wait, Wander, Patrol, Follow
[Serializable]
public class AggressiveAIMovementStateMachine : AIMovementStateMachine
{
	public AggressiveAIMovementStateMachine(EnemyDynamic enemy, NavMeshAgent navMeshAgent)
		: base(enemy, navMeshAgent)
	{
		StoppingState = new AIStopState(this);
		FollowingState = new AIFollowState(this);
		WanderingState = new AIWanderState(this);
		SteppingBackState = new AIStepBackState(this);
		StrafingState = new AIStrafeState(this);
		currentState = StoppingState;
	}
}
using System;
using UnityEngine;
using UnityEngine.AI;


// Wait, Wander, Patrol, Follow
[Serializable]
public class AllyMovementStateMachine : AIMovementStateMachine
{
	public AllyMovementStateMachine(EnemyDynamic enemy, NavMeshAgent navMeshAgent)
		: base(enemy, navMeshAgent)
	{
		StoppingState = new AIStopState(this);
		FollowingState = new AIFollowState(this);
		WanderingState = new AllyWanderState(this);
		SteppingBackState = new AIStepBackState(this);
		StrafingState = new AIStrafeState(this);
		currentState = StoppingState;
	}
}

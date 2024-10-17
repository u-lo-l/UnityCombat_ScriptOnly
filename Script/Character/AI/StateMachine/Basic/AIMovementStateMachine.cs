using System;
using UnityEngine;
using UnityEngine.AI;


// Wait, Wander, Patrol, Follow
[Serializable]
public class AIMovementStateMachine : StateMachine
{
	public EnemyDynamic Enemy { get; protected set; }
	public NavMeshAgent NavMeshAgent { get; protected set; }
	public AIStopState StoppingState { get; protected set; }
	public AIFollowState FollowingState { get; protected set; }
	public AIWanderState WanderingState { get; protected set; }
	public AIStepBackState SteppingBackState { get; protected set; }
	public AIStrafeState StrafingState { get; protected set; }
	public AIMovementStateMachine(EnemyDynamic enemy, NavMeshAgent navMeshAgent)
		: base()
	{
		this.Enemy = enemy;
		this.NavMeshAgent = navMeshAgent;
	}

	public virtual void ChangeState(AIMovementState newState)
	{
		if (newState == null)
		{
			Debug.LogWarning("[AIMovementState] : ChangeState : newState is null");
			return ;
		}
		Enemy.InvokeOnMovementStateChanged(newState.CurrentState.ToString());
		base.ChangeState(newState);
	} 
	public virtual void ForceStop()
	{
		NavMeshAgent.velocity = Vector3.zero;
		NavMeshAgent.isStopped = true;
		ChangeState(StoppingState);
	}
	public virtual AIMovementState.State GetCurrentState()
	{
		AIMovementState movementState = currentState as AIMovementState;
		return movementState.CurrentState;
	}
}
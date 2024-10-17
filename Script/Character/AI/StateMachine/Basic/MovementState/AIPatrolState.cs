using UnityEngine;
using UnityEngine.AI;

public class AIPatrolState : AIWanderState
{
	public AIPatrolState(AIMovementStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		controller.isStopped = true;
		controller.updateRotation = true;
		CurrentState = State.Patrol;
	}
	public override void FixedTick()
	{

	}
	public override void Tick()
	{
		base.Tick();
	}
	public override void Exit()
	{

	}
};
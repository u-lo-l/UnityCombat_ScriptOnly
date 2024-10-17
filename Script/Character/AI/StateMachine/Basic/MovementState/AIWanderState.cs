using UnityEngine;
using UnityEngine.AI;

public class AIWanderState : AIMovementState
{
	protected Vector3 initialPoint;
	protected Vector3 goalPosition;
	protected Vector3 prevGoalPosition;
	protected float wanderRadius;
	protected float PositionMinGap => wanderRadius / 2;

	protected float TargetFoundStopDelay;
	protected float PathFoundStopDelay;
	protected float ArrivalStopDelay;
	public AIWanderState(AIMovementStateMachine stateMachine)
	 : base(stateMachine)
	{
		CurrentState = State.Wander;
		initialPoint = stateMachine.Enemy.transform.position;
		wanderRadius = stateMachine.Enemy.WanderRadius;

		TargetFoundStopDelay = 0.5f;
		PathFoundStopDelay = 1f;
		ArrivalStopDelay = 2f;
	}
	public override void Enter()
	{
		base.Enter();
		if (controller != null)
		{
			controller.isStopped = false;
			controller.updateRotation = true;
			goalPosition = movementStateMachine.Enemy.transform.position;
			controller.stoppingDistance = controller.speed / 2;
		}

		if (CreateRandomPath() == false)
		{
			movementStateMachine.StoppingState.Duration = 1f;
			SwitchToWaitState();
			return;
		}
	}
	public override void FixedTick()
	{

	}
	public override void Tick()
	{
		if (movementStateMachine.Enemy.HasTargetPlayer() == true)
		{
			movementStateMachine.StoppingState.Duration = TargetFoundStopDelay;
			SwitchToWaitState();
		}

		if (controller.hasPath == false)
		{
			// Debug.LogWarning("Lost the Path");
			movementStateMachine.StoppingState.Duration = PathFoundStopDelay;
			movementStateMachine.ChangeState(movementStateMachine.StoppingState);
		}
		else
		{
			if (CheckArrival() == true)
			{
				// Debug.Log("Arrived");
				movementStateMachine.StoppingState.Duration = ArrivalStopDelay;
				movementStateMachine.ChangeState(movementStateMachine.StoppingState);
			}
			else
			{
				base.Tick();
			}
		}
	}
	public override void Exit()
	{
		if (controller.enabled == true)
		{
			controller.ResetPath();
		}
	}

	protected override float GetTaretSpeed()
	{
		movementStateMachine.Enemy.CharacterStatus.SpeedModifier = Random.Range(0.8f, 1.2f);
		return movementStateMachine.Enemy.CharacterStatus.WalkSpeed;
	}

	protected bool CheckArrival()
	{
		float remainDistance = (movementStateMachine.Enemy.transform.position - controller.pathEndPosition).sqrMagnitude;
		return remainDistance < controller.stoppingDistance * controller.stoppingDistance;
	}

	protected const int MaxTrial = 20;
	protected virtual bool CreateRandomPath()
	{
		// Debug.Log("Trying to make new path");
		prevGoalPosition = goalPosition;
		for (int i = 0 ; i < MaxTrial ; i++)
		{
			if (GetRandomPoint(initialPoint, out goalPosition) == true)
			{
				if (Vector3.Distance(goalPosition, prevGoalPosition) < PositionMinGap)
					continue;
				if (controller.CalculatePath(goalPosition, navMeshPath) == false)
					continue;
				if (navMeshPath.status != NavMeshPathStatus.PathComplete)
					continue;

				controller.SetPath(navMeshPath);
				return true;
			}
		}
		return false;
	}

	protected bool GetRandomPoint(Vector3 center, out Vector3 result)
	{
		Vector3 randomPoint = center + Random.insideUnitSphere * wanderRadius;
		if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas) == true)
		{
			result = hit.position;
			return true;
		}
		result = Vector3.zero;
		return false;
	}
};
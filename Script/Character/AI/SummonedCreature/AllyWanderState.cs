
using UnityEngine;
using UnityEngine.AI;

public class AllyWanderState : AIWanderState
{
	private Transform pivotTransform;
	public AllyWanderState(AIMovementStateMachine stateMachine)
	 : base(stateMachine)
	{
		pivotTransform = (stateMachine.Enemy as SummonedAlly).SummonerTransform;
		TargetFoundStopDelay = 0.5f;
		PathFoundStopDelay = 1f;
		ArrivalStopDelay = 0.5f;
	}
	public override void Enter()
	{
		base.Enter();
	}
	public override void Tick()
	{
		if ((pivotTransform.position - movementStateMachine.Enemy.transform.position).sqrMagnitude > 100)
		{
			if (CreateRandomPath() == false)
				SwitchToWaitState();
			else
				controller.speed = movementStateMachine.Enemy.CharacterStatus.RunSpeed * 1.5f;
		}
		else
			controller.speed = movementStateMachine.Enemy.CharacterStatus.WalkSpeed;
		base.Tick();
	}
	public override void Exit()
	{
		base.Exit();
	}

	protected override bool CreateRandomPath()
	{
		prevGoalPosition = goalPosition;

		for (int i = 0 ; i < MaxTrial ; i++)
		{
			if (GetRandomPoint(pivotTransform.position, out goalPosition) == true)
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
}
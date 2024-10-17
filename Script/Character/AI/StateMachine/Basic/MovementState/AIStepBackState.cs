using UnityEngine;
using UnityEngine.AI;

public class AIStepBackState : AIMovementState
{
	private float pivotSpeed;
	public AIStepBackState(AIMovementStateMachine stateMachine)
	: base(stateMachine)
	{
		CurrentState = State.StepBack;
	}
	public override void Enter()
	{
		// Debug.Log("Enter to StepBack State");

		base.Enter();
		controller.updateRotation = false;
		controller.speed = pivotSpeed = movementStateMachine.Enemy.CharacterStatus.WalkSpeed / 2;
		controller.isStopped = false;
		controller.stoppingDistance = controller.speed / 10;
		controller.ResetPath();
	}
	public override void Tick()
	{
		Transform targetTf = movementStateMachine.Enemy.GetTargetTransform();
		if (targetTf == null)
		{
			// Debug.Log("[StepBackState] : Player Not Found");
			movementStateMachine.ChangeState(movementStateMachine.StoppingState);
			return ;
		}

		Vector3 vectorToTarget = targetTf.position - movementStateMachine.Enemy.transform.position;
		if (Vector3.Dot(vectorToTarget, Vector3.up) < Mathf.Cos(Mathf.Deg2Rad * 15))
		{
			Vector3 lookDirection = new(vectorToTarget.x, 0, vectorToTarget.z);
			movementStateMachine.Enemy.transform.rotation = Quaternion.LookRotation(lookDirection.normalized);
		}
		
		if (controller.hasPath == false)
		{
			if (GetStepBackPath(-vectorToTarget) == true)
			{
				controller.SetPath(navMeshPath);
			}
			else
			{
				movementStateMachine.StoppingState.Duration = 0f;
				SwitchToWaitState();
			}
		}
		else
		{
			if (CheckArrival() == true)
			{
				SwitchToWaitState();
			}
			else
			{
				AdjustMovementSpeed(vectorToTarget);
				base.Tick();
			}
		}
	}
	public override void Exit()
	{
		if (controller.enabled == true)
		{
			controller.updateRotation = true;
			controller.ResetPath();
		}
	}
	protected override float GetTaretSpeed()
	{
		movementStateMachine.Enemy.CharacterStatus.SpeedModifier = Random.Range(0.8f, 1.2f);
		return movementStateMachine.Enemy.CharacterStatus.WalkSpeed;
	}

	private bool CheckArrival()
	{
		return controller.remainingDistance < controller.stoppingDistance;
	}

	private bool GetStepBackPath(Vector3 direction)
	{
		Detector detector = movementStateMachine.Enemy.Detector;
		float stepBackDistance = detector.detectingNearDistance;

		Vector3 initialPoint = movementStateMachine.Enemy.transform.position;
		Vector3 samplePoint = initialPoint + stepBackDistance * direction.normalized;

		float sampleRadius = detector.detectingNearDistance / 3	;

		return TryFindPath(samplePoint, out NavMeshHit hit, sampleRadius);
	}

	private bool TryFindPath(Vector3 center, out NavMeshHit hit, float radius)
	{
		if (NavMesh.SamplePosition(center, out hit, radius, NavMesh.AllAreas) == false)
		{
			return false;
		}
		if (controller.CalculatePath(hit.position, navMeshPath) == false)
		{
			return false;
		}
		return navMeshPath.status == NavMeshPathStatus.PathComplete;
	}

	private void AdjustMovementSpeed(Vector3 dir)
	{
		float nearRange = movementStateMachine.Enemy.Detector.detectingNearDistance;
		float displacement = dir.magnitude - nearRange;
		displacement = -Mathf.Min(displacement, 0);
		float speedModifier = Mathf.LerpUnclamped(1f, 1.2f, 1 - displacement / nearRange);

		controller.speed = pivotSpeed * speedModifier;
		controller.acceleration = controller.speed * 10;
		controller.stoppingDistance = controller.speed / 10;
	}
}
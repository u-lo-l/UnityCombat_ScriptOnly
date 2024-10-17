using System.Collections;
using AnimatorHash;
using UnityEngine;
using UnityEngine.AI;

public class GolemMovementStateMachine : AIMovementStateMachine
{
	private readonly BossGolem golem;
	private readonly GolemCombatStateMachine combatStateMachine;
	public bool IsMoving {get; private set;}
	public bool IsRotating {get; private set;}
	public GolemMovementStateMachine(EnemyDynamic enemy, NavMeshAgent navMeshAgent)
	 : base(enemy, navMeshAgent)
	{
		golem = enemy as BossGolem;
		combatStateMachine = golem.combatStateMachine as GolemCombatStateMachine;
		StoppingState = new GolemStopState(this);
		FollowingState = new GolemFollowState(this);
		WanderingState = new AIWanderState(this);
		SteppingBackState = new AIStepBackState(this);
		StrafingState = new AIStrafeState(this);
	}
	public override void Tick()
	{
		base.Tick();
	}
	public void RotateToTarget(float time = 0.5f)
	{
		Transform playerTransform = golem.GetTargetTransform();
		if(playerTransform == null)
			return ;
		golem.StartCoroutine(Rotate(playerTransform, time));
	}

	private IEnumerator Rotate(Transform targetTransform, float time)
	{
		golem.NavMeshAgent.enabled = false;
		Vector3 towards = (targetTransform.position - golem.transform.position);
		towards.y = 0;
		towards.Normalize();
		if (towards == Vector3.zero)
			yield break;
		Quaternion startRotation = golem.transform.rotation;
		Quaternion targetRotation = Quaternion.LookRotation(towards);
		float elapsedTime = 0;
		IsRotating = true;
		while (elapsedTime < time)
		{
			float lerpRate = elapsedTime / time;
			golem.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, lerpRate);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		golem.transform.rotation = targetRotation;
		IsRotating = false;
		golem.NavMeshAgent.enabled = true;
	}
}

public class GolemStopState : AIStopState
{
	private readonly BossGolem golem;
	public GolemStopState(AIMovementStateMachine stateMachine)
	 : base(stateMachine)
	{
		CurrentState = State.Stop;
		golem = stateMachine.Enemy as BossGolem;
	}
	public override void Enter()
	{
		Duration = Mathf.Max(0.25f, Duration);
		Debug.Log($"Golem stops for {Duration}s");

		base.Enter();
		animator.SetFloat(AnimatorHash.BossGolem.SpeedZ, 0);
		animator.SetFloat(AnimatorHash.BossGolem.SpeedX, 0);
		if (controller.enabled)
		{
			controller.speed = 0;
			controller.isStopped = true;
		}
	}
	public override void Tick()
	{
		if (Duration > 0)
		{
			Duration -= Time.deltaTime;
			return ;
		}
		if (golem.CanMove == false)
		{
			return ;
		}
		if (golem.GetTargetTransform() == true)
		{
			CasePlayerFound();
		}
		else
		{
			SwitchToWanderState();
		}
	}
	public override void Exit()
	{
		base.Exit();
	}
	protected override void CasePlayerFound()
	{
		if (golem.combatStateMachine.HoldingState.Duration <= 0)
		{
			float sqrDistance = movementStateMachine.Enemy.GetTargetSqrDistance();
			if (sqrDistance == float.PositiveInfinity)
			{
				return ;
			}

			float range = movementStateMachine.Enemy.AttackRange();
			if ( sqrDistance < range * range)
			{
				return ;
			}
			Debug.Log("To Follow State");
			SwitchToFollowState();
		}
		else
		{
			if (movementStateMachine.Enemy.GetTargetDistance() < movementStateMachine.Enemy.Detector.detectingNearDistance * 0.7f)
			{
			Debug.Log("To StepBack State");
				SwitchToStepBackState();
			}
			else
			{
			Debug.Log("To Starfe State");
				SwitchToStrafeState();
			}
		}
	}
}

public class GolemFollowState : AIFollowState
{
	private readonly BossGolem golem;
	public GolemFollowState(AIMovementStateMachine stateMachine)
	 : base(stateMachine)
	{
		CurrentState = State.Follow;
		golem = stateMachine.Enemy as BossGolem;
	}

	public override void Enter()
	{
		base.Enter();
		controller.speed = golem.CharacterStatus.RunSpeed;
	}
	public override void Tick()
	{
		base.Tick();
	}

	public override void Exit()
	{
		base.Exit();
	}
}

public class GolemWanderState : AIWanderState
{
	private readonly BossGolem golem;
	public GolemWanderState(AIMovementStateMachine stateMachine)
	 : base(stateMachine)
	{
		CurrentState = State.Follow;
		golem = stateMachine.Enemy as BossGolem;
	}
	public override void Enter()
	{
		Debug.Log("Golem enters to Wander state");
		controller.isStopped = false;
		controller.updateRotation = true;
		controller.speed = golem.CharacterStatus.WalkSpeed;
		goalPosition = movementStateMachine.Enemy.transform.position;

		if (CreateRandomPath() == false)
		{
			movementStateMachine.StoppingState.Duration = 1f;
			SwitchToWaitState();
		}
		else
		{
			// Debug.Log("Success to find Path");
		}
	}
	public override void Tick()
	{
		float speedZ = Vector3.Dot(golem.transform.forward, controller.velocity);
		float speedX = Vector3.Dot(golem.transform.right, controller.velocity);

		animator.SetFloat(AnimatorHash.BossGolem.SpeedZ, speedZ);
		animator.SetFloat(AnimatorHash.BossGolem.SpeedX, speedX);

		if (golem.GetTargetTransform() != null)
		{
			SwitchToFollowState();
		}
		if (controller.hasPath == false)
		{
			movementStateMachine.StoppingState.Duration = 1f;
			SwitchToWaitState();
		}
		else
		{
			if (CheckArrival() == true)
			{
				Debug.Log("Golem Arrived");
				movementStateMachine.StoppingState.Duration = 3f;
				SwitchToWaitState();
			}
			else
			{
				return ;
			}
		}
	}
	public override void Exit()
	{
	}
}

public class GolemStrafeState : AIStrafeState
{
	public GolemStrafeState(AIMovementStateMachine stateMachine)
	 : base(stateMachine)
	{
	}
}

public class GolemStepBackState : AIStepBackState
{
	public GolemStepBackState(AIMovementStateMachine stateMachine)
	 : base(stateMachine)
	{
	}
}
using UnityEngine;
using UnityEngine.AI;

public class BossMovementStateMachine : AIMovementStateMachine
{	
	public BossRotateState rotatingState;
	public BossMovementStateMachine(EnemyDynamic enemy, NavMeshAgent navMeshAgent, WeaponHandler weaponHandler)
	 : base(enemy, navMeshAgent)
	{
		StoppingState = new BossStopState(this, weaponHandler);
		FollowingState = new BossFollowState(this, weaponHandler);
		WanderingState = new BossWanderState(this, weaponHandler);
		rotatingState = new BossRotateState(this);
		currentState = StoppingState;
	}

	public void SwitchToRotateState(float angleInDegrees)
	{
		rotatingState.Angle = angleInDegrees;
		ChangeState(rotatingState);
	}
}

public class BossStopState : AIStopState
{
	WeaponHandler weaponHandler;
	public BossStopState(BossMovementStateMachine stateMachine, WeaponHandler weaponHandler)
	 : base(stateMachine)
	{
		CurrentState = State.Stop;
		this.weaponHandler = weaponHandler;
	}
	
	public override void Enter()
	{
		Duration = Mathf.Max(0.25f, Duration);
		Debug.Log("boss stop" + Duration);
		if (controller.enabled == false)
			return ;
		controller.speed = 0;
		controller.isStopped = true;
	}
	public override void Tick()
	{
		float meleeRange = (movementStateMachine.Enemy as EnemyBoss).MeleeDistance;
		animator.SetFloat(AnimatorHash.Boss.Speed, movementStateMachine.Enemy.NavMeshAgent.velocity.magnitude);
		if (Duration > 0)
		{
			Duration -= Time.deltaTime;
			return;
		}
		if (movementStateMachine.Enemy.CanMove == false)
		{
			return;
		}
		// if (weaponHandler.CanAttack == false)
		// {
		// 	SwitchToWanderState();
		// 	return ;
		// }
		if (movementStateMachine.Enemy.GetTargetDistance() < meleeRange)
		{
			return ;
		}
		if (movementStateMachine.Enemy.GetTargetTransform() != null)
		{
			SwitchToFollowState();
			return ;
		}
		SwitchToWanderState();
	}

	public override void Exit()
	{
		base.Exit();
	}
}
public class BossRotateState : AIMovementState
{
	private readonly EnemyBoss enemy;
	public float Angle { private get; set; }
	public BossRotateState(AIMovementStateMachine stateMachine) : base(stateMachine)
	{
		CurrentState = State.Stop;
		enemy = stateMachine.Enemy as EnemyBoss;
	}
	public override void Enter()
	{
		base.Enter();
		animator.SetFloat(AnimatorHash.Boss.Speed, 0);
		controller.updateRotation = false;
		controller.isStopped = true;
		
		enemy.IsRotating = true;
		animator.SetFloat(AnimatorHash.Boss.RotateAngle, Angle);
		animator.SetTrigger(AnimatorHash.Boss.RotateTrigger);
	}
	public override void Tick()
	{
		// Do Nothing;
	}
	public override void Exit()
	{
		if (controller.enabled == true)
		{
			controller.isStopped = false;
			controller.updateRotation = true;
		}
		base.Exit();
		enemy.IsRotating = false;
		animator.SetFloat(AnimatorHash.Boss.RotateAngle, 0);
		animator.ResetTrigger(AnimatorHash.Boss.RotateTrigger);
	}

	protected override float GetTaretSpeed()
	{
		return movementStateMachine.Enemy.CharacterStatus.WalkSpeed;
	}
}

public class BossFollowState : AIFollowState
{
	WeaponHandler weaponHandler;
	public BossFollowState(BossMovementStateMachine stateMachine, WeaponHandler weaponHandler)
	 : base(stateMachine)
	{
		CurrentState = State.Follow;
		this.weaponHandler = weaponHandler;
	}
	public override void Enter()
	{
		Debug.Log("boss follow");
		controller.isStopped = false;
		controller.speed = 4;
		CurrentState = State.Follow;
	}
	public override void Tick()
	{
		animator.SetFloat(AnimatorHash.Boss.Speed, movementStateMachine.Enemy.NavMeshAgent.velocity.magnitude);
		float meleeRange = (movementStateMachine.Enemy as EnemyBoss).MeleeDistance;
		if (movementStateMachine.Enemy.CanMove == false)
		{
			SwitchToWaitState();
			return;
		}
		if (movementStateMachine.Enemy.GetTargetTransform() != null)
		{
			// if (weaponHandler.CanAttack == false)
			// {
			// 	Debug.Log("[Boss] : Cant Attack. so wander");
			// 	SwitchToWanderState();
			// 	return ;
			// }
			if (movementStateMachine.Enemy.GetTargetDistance() < meleeRange)
			{
				SwitchToWaitState();
			}
			else
			{
				controller.SetDestination(movementStateMachine.Enemy.GetTargetTransform().position);
			}
		}
		else
		{
			SwitchToWanderState();
		}
	}
}

public class BossWanderState : AIWanderState
{
	WeaponHandler weaponHandler;
	public BossWanderState(BossMovementStateMachine stateMachine, WeaponHandler weaponHandler)
	 : base(stateMachine)
	{
		CurrentState = State.Wander;
		wanderRadius = 50f;
		this.weaponHandler = weaponHandler;
		initialPoint = stateMachine.Enemy.transform.position;
	}
	public override void Enter()
	{
		Debug.Log("Boss Wander");
		controller.isStopped = false;
		controller.speed = 1.5f;
		goalPosition = movementStateMachine.Enemy.transform.position;

		if (CreateRandomPath() == false)
		{
			// Debug.Log("Fail to find Path");
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
		animator.SetFloat(AnimatorHash.Boss.Speed, movementStateMachine.Enemy.NavMeshAgent.velocity.magnitude);
		if (weaponHandler.CanAttack == false)
		{
			return ;
		}

		if (movementStateMachine.Enemy.GetTargetTransform() != null)
		{
			SwitchToFollowState();
		}

		if (controller.hasPath == false)
		{
			// Debug.LogWarning("Lost the Path");
			movementStateMachine.StoppingState.Duration = 1f;
			SwitchToWaitState();
		}
		else
		{
			if (CheckArrival() == true)
			{
				// Debug.Log("Arrived");
				movementStateMachine.StoppingState.Duration = 3f;
				SwitchToWaitState();
			}
			else
			{
				return ;
			}
		}
	}
}
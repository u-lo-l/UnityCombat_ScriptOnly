using System;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public abstract class AIMovementState : IState
{
	public enum State { Stop, Wander, Patrol, Follow, Rush, StepBack, RunAway, Strafe }
	public State CurrentState {get; protected set;}
	protected AIMovementStateMachine movementStateMachine;
	protected Animator animator;
	protected NavMeshAgent controller;
	protected NavMeshPath navMeshPath;

	protected AIMovementState(AIMovementStateMachine stateMachine)
	{
		movementStateMachine = stateMachine;
		animator = stateMachine.Enemy.Animator;
		controller = stateMachine.Enemy.NavMeshAgent;
		navMeshPath = new();
	}

	public virtual void Enter()
	{
		controller.speed = GetTaretSpeed();
	}
	public virtual void Exit()	{ }
	public virtual void FixedTick()	{ }
	public virtual void Tick()
	{
		Vector3 velocity = Vector3.zero;
		if (controller.enabled == true)
			velocity += movementStateMachine.Enemy.NavMeshAgent.velocity;
		velocity += movementStateMachine.Enemy.rigidbody.velocity;
		float forwardSpeed = Vector3.Dot(velocity, movementStateMachine.Enemy.EnvironmentChecker.FixedForward);
		float sideSpeed = Vector3.Dot(velocity, movementStateMachine.Enemy.transform.right);

		animator.SetFloat(AnimatorHash.Enemy.SpeedZ, forwardSpeed);
		animator.SetFloat(AnimatorHash.Enemy.SpeedX, sideSpeed);
	}
	protected abstract float GetTaretSpeed();
	protected void SwitchToWaitState()
	{
		movementStateMachine.ChangeState(movementStateMachine.StoppingState);
	}

	protected void SwitchToFollowState()
	{
		movementStateMachine.ChangeState(movementStateMachine.FollowingState);
	}

	protected void SwitchToWanderState()
	{
		movementStateMachine.ChangeState(movementStateMachine.WanderingState);
	}
	protected void SwitchToStepBackState()
	{
		movementStateMachine.ChangeState(movementStateMachine.SteppingBackState);
	}

	protected void SwitchToStrafeState()
	{
		movementStateMachine.ChangeState(movementStateMachine.StrafingState);
	}
};
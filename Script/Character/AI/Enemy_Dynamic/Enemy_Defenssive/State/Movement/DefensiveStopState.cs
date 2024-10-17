using UnityEngine;
using UnityEngine.Experimental.AI;

public class DefensiveStopState : AIStopState
{
	public DefensiveStopState(AIMovementStateMachine stateMachine) : base(stateMachine)
	{
		this.movementStateMachine = stateMachine;
	}

	public override void Enter()
	{
		Duration = 0.25f;
		// Debug.Log($"new stop : {Duration}");
		controller.speed = 0f;
		CurrentState = State.Stop;
	}
	public override void FixedTick()
	{

	}
	public override void Tick()
	{
		animator.SetFloat(AnimatorHash.Enemy.SpeedZ, movementStateMachine.Enemy.NavMeshAgent.velocity.magnitude);
		if (Duration > 0)
		{
			Duration -= Time.deltaTime;
			return;
		}
		if (movementStateMachine.Enemy.CanMove == false)
		{
			return;
		}
		if (movementStateMachine.Enemy.GetTargetTransform() == null)
		{
			SwitchToWanderState();
			return;
		}
		if (CurrentState != State.StepBack)
		{
			movementStateMachine.ChangeState(new AIStepBackState(movementStateMachine));
		}
	}
	public override void Exit()
	{
		Duration = 0;
	}
};
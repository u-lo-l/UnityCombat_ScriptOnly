using UnityEngine;
using UnityEngine.AI;

public class DefensiveWanderState : AIWanderState
{
	public new DefensiveAIMovementStateMachine movementStateMachine;
	public DefensiveWanderState(AIMovementStateMachine stateMachine)
	 : base(stateMachine)
	{
		this.movementStateMachine = stateMachine as DefensiveAIMovementStateMachine;
	}

	public override void Enter()
	{
		base.Enter();
	}
	public override void FixedTick()
	{

	}
	public override void Tick()
	{
		if (movementStateMachine.Enemy.GetTargetTransform() != null)
		{
			movementStateMachine.ChangeState(movementStateMachine.SteppingBackState);
		}

		if (controller.hasPath == false)
		{
			// Debug.LogWarning("Lost the Path");
			movementStateMachine.StoppingState.Duration = 0f;
			movementStateMachine.ChangeState(movementStateMachine.StoppingState);
		}
		else
		{
			if (CheckArrival() == true)
			{
				// Debug.Log("Arrived");
				movementStateMachine.StoppingState.Duration = 3f;
				movementStateMachine.ChangeState(movementStateMachine.StoppingState);
			}
			else
			{
				// Debug.DrawLine(initialPoint + Vector3.up * 0.1f, goalPosition + Vector3.up * 0.1f, Color.green);
				// Debug.DrawLine(movementStateMachine.Enemy.transform.position + Vector3.up * 0.1f, goalPosition + Vector3.up * 0.1f, Color.red);
				animator.SetFloat(AnimatorHash.Enemy.SpeedZ, movementStateMachine.Enemy.NavMeshAgent.velocity.magnitude);
			}
		}
	}
	public override void Exit()
	{
		controller.ResetPath();
	}
};
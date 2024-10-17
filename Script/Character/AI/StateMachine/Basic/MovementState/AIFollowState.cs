using UnityEngine;
using UnityEngine.AI;

public class AIFollowState : AIMovementState
{
	public AIFollowState(AIMovementStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{	
		base.Enter();
		CurrentState = State.Follow;
		controller.speed = movementStateMachine.Enemy.CharacterStatus.RunSpeed;
		controller.isStopped = false;
		controller.updateRotation = true;
		controller.acceleration = controller.speed * 10;
		controller.stoppingDistance = movementStateMachine.Enemy.AttackRange() * 0.75f;
	}
	public override void FixedTick()
	{

	}
	public override void Tick()
	{
		base.Tick();
		if (movementStateMachine.Enemy.CanMove == false)
		{
			SwitchToWaitState();
			return ;
		}
		if (movementStateMachine.Enemy.GetTargetTransform() == null)
		{
			SwitchToWaitState();
			return ;
		}
		controller.SetDestination(movementStateMachine.Enemy.GetTargetTransform().position);
	}
	public override void Exit()
	{

	}

	protected override float GetTaretSpeed()
	{
		movementStateMachine.Enemy.CharacterStatus.SpeedModifier = Random.Range(0.8f, 1.2f);
		return movementStateMachine.Enemy.CharacterStatus.RunSpeed;
	}
};
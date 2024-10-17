

public class AIRushState : AIFollowState
{
	public AIRushState(AIMovementStateMachine stateMachine) : base(stateMachine)
	{
	}
	public override void Enter()
	{
		controller.isStopped = false;
		controller.speed = 12;
		CurrentState = State.Rush;
		controller.stoppingDistance = controller.speed / 2;
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
}
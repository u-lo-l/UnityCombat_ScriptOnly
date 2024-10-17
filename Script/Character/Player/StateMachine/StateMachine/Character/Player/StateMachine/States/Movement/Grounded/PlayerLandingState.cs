
public class PlayerLandingState : PlayerGroundedState
{
	public PlayerLandingState(PlayerMovementStateMachine stateMachine)
	 : base(stateMachine)
	{
	}
	public override void Enter()
	{
		movementStateMachine.player.MeshTrail.IsActive = false;
		base.Enter();
		CurrentState = State.Land;
		animator.SetTrigger("LandTrigger");
	}
	public override void Tick()
	{

	}
	public override void Exit()
	{
		base.Exit();
	}
	protected override float SetMovementSpeed()
	{
		return 0;
	}
}
public class AggressiveEnemy : EnemyDynamic
{
	protected override void Start()
	{
		base.Start();
		movementStateMachine = new AggressiveAIMovementStateMachine(this, NavMeshAgent);
		combatStateMachine = new AggressiveAICombatStateMachine(this, weaponHandler);
		movementStateMachine.ChangeState(movementStateMachine.StoppingState);
		combatStateMachine.ChangeState(combatStateMachine.HoldingState);
	}
}

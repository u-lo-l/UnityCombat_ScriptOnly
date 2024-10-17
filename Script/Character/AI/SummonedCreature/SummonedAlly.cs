using UnityEngine;

public class SummonedAlly : EnemyDynamic
{
	public Transform SummonerTransform {get; set;}

	protected override void Awake()
	{
		base.Awake();
	}
	protected override void Start()
	{
		base.Start();
		movementStateMachine = new AllyMovementStateMachine(this, NavMeshAgent);
		combatStateMachine = new AggressiveAICombatStateMachine(this, weaponHandler);
		movementStateMachine.StoppingState.Duration = 2f;
		movementStateMachine.ChangeState(movementStateMachine.StoppingState);
		combatStateMachine.ChangeState(combatStateMachine.HoldingState);
	}
}

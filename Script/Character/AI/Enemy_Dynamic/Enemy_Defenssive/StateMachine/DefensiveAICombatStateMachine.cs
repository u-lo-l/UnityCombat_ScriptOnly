using UnityEngine;

public class DefensiveAICombatStateMachine : AICombatStateMachine
{
	public DefensiveAICombatStateMachine(EnemyDynamic enemy, WeaponHandler weaponHandler)
	 : base(enemy, weaponHandler)
	{
		HoldingState = new DefensiveHoldState(this);
		currentState = HoldingState;
	}

	public override void Tick()
	{
		base.Tick();
	}
}
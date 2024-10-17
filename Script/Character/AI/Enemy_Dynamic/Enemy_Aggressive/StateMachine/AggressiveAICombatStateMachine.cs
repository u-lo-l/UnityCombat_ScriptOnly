using System;

// Hold, Attack, Damage, Guard
[Serializable]
public class AggressiveAICombatStateMachine : AICombatStateMachine
{
	public AggressiveAICombatStateMachine(EnemyDynamic enemy, WeaponHandler weaponHandler)
	 : base(enemy, weaponHandler)
	{
		currentState = HoldingState;
	}
	public override void Tick()
	{
		base.Tick();
	}
}
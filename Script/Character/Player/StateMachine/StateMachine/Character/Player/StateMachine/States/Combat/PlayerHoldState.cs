using UnityEngine;

public class PlayerHoldState : PlayerCombatState
{
	private const float WaitTime = 0.1f;
	private float delay;
	private bool waitMode = false;
	private Animator animator;
	public PlayerHoldState(PlayerCombatStateMachine stateMachine) : base(stateMachine)
	{
		animator = stateMachine.Player.Animator;
	}

	public override void Enter()
	{
		combatStateMachine.Player.LayerFadeOut(animator, AnimatorHash.Player.UpperActionLayer, 0f);
		combatStateMachine.Player.LayerFadeOut(animator, AnimatorHash.Player.ActionLayer, 0.5f);
		CurrentState = State.Hold;
		combatStateMachine.WeaponHandler.ResetAttackIndex();
		if (combatStateMachine.Player.movementStateMachine.CanMove == false)
		{
			waitMode = true;
			delay = WaitTime;
		}
		if (combatStateMachine.WeaponHandler.ArmedType != WeaponType.Unarmed)
		{
		}
		if (combatStateMachine.WeaponHandler.ArmedType == WeaponType.Staff)
		{

		}
	}
	public override void Tick()
	{
		if (waitMode == true)
		{
			if (delay <= 0)
			{
				waitMode = false;
				combatStateMachine.Player.movementStateMachine.CanMove = true;
			}
			delay -= Time.deltaTime;
		}
	}
	public override void FixedTick()
	{

	}
	public override void Exit()
	{

	}
}
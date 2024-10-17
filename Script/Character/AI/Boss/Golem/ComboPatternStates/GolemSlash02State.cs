using UnityEngine;
public class GolemSlash02State : GolemActionState
{
	private enum Combo {ZombiePunch, DrawAndAttack, LowSlash, TurnSlash, End};
	private Combo currentCombo;
	public GolemSlash02State(AICombatStateMachine stateMachine) : base(stateMachine)
	{
		triggerHash = Animator.StringToHash("SlashTrigger02");
	}
	public override void Enter()
	{
		base.Enter();
		Debug.Log("Enter TO Slash02");
		currentCombo = Combo.ZombiePunch;
		isTriggered = false;
		isRotated = false;
		combatStateMachine.WeaponHandler.CurrentWeapon.AdditionalIndex = 6;
		animator.SetTrigger(triggerHash);
	}

	public override void Tick()
	{
		switch(currentCombo)
		{
			case Combo.ZombiePunch :
				animator.SetBool("ShouldFollowPlayer", true);
				Slash02Pattern(nextCombo : Combo.DrawAndAttack); break ;
			case Combo.DrawAndAttack :
				Slash02Pattern(nextCombo : Combo.LowSlash); break ;
			case Combo.LowSlash :
				Slash02Pattern(nextCombo : Combo.TurnSlash); break ;
			case Combo.TurnSlash :
				Slash02Pattern(nextCombo : Combo.End); break ;
		}
	}
	public override void Exit()
	{
		base.Exit();
		animator.SetBool("ShouldFollowPlayer", false);
	}

	private void Slash02Pattern(Combo nextCombo)
	{
		SetActionIndex((int)currentCombo);
		if (isRotated == false)
		{
			movementStateMachine.RotateToTarget(0.1f);
			isRotated = true;
			return ;
		}
		if (movementStateMachine.IsRotating == true)
		{
			return ;
		}
		if (isTriggered == false)
		{
			animator.SetTrigger(triggerHash);
			isTriggered = true;
			return ;
		}
		if (combatStateMachine.WeaponHandler.CanNextCombo == false)
		{
			return;
		}
		combatStateMachine.WeaponHandler.OnNextComboDisable();
		currentCombo = nextCombo;
		isTriggered = false;
		isRotated = false;
		waitingTime += 1f;
	}
}
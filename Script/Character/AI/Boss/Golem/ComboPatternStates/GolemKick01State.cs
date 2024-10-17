using UnityEngine;
public class GolemKick01State : GolemActionState
{
	private enum Combo {FlipKick, DoubleKick, SlashKick, End};
	private Combo currentCombo;
	public GolemKick01State(AICombatStateMachine stateMachine) : base(stateMachine)
	{
		triggerHash = Animator.StringToHash("KickTrigger01");
	}
	public override void Enter()
	{
		base.Enter();
		Debug.Log("Enter TO Kick01");
		currentCombo = Combo.FlipKick;
		isTriggered = false;
		isRotated = false;
		combatStateMachine.WeaponHandler.CurrentWeapon.AdditionalIndex = 0;
	}

	public override void Tick()
	{
		switch(currentCombo)
		{
			case Combo.FlipKick :
				Kick01Pattern(nextCombo : Combo.DoubleKick); break;
			case Combo.DoubleKick :
				Kick01Pattern(nextCombo : Combo.SlashKick); break ;
			case Combo.SlashKick :
				Kick01Pattern(nextCombo : Combo.End); break ;
		}
	}
	public override void Exit()
	{
		base.Exit();
	}

	private void Kick01Pattern(Combo nextCombo)
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
using UnityEngine;
public class GolemSlashState : GolemActionState
{
	private enum Combo {FlashCut, DoubleStrike,  End};
	private Combo currentCombo;
	public GolemSlashState(AICombatStateMachine stateMachine) : base(stateMachine)
	{
		triggerHash = Animator.StringToHash("SlashTrigger");
	}
	public override void Enter()
	{
		base.Enter();
		Debug.Log("Enter TO Slash");
		currentCombo = Combo.FlashCut;
		isTriggered = false;
		isRotated = false;
		combatStateMachine.WeaponHandler.CurrentWeapon.AdditionalIndex = 5;
	}

	public override void Tick()
	{
		switch(currentCombo)
		{
			case Combo.FlashCut :
				Punch01Pattern(nextCombo : Combo.DoubleStrike); break ;
			case Combo.DoubleStrike :
				Punch01Pattern(nextCombo : Combo.End); break ;
		}
	}
	public override void Exit()
	{
		base.Exit();
	}

	private void Punch01Pattern(Combo nextCombo)
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
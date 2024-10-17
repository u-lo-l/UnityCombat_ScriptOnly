using UnityEngine;
public class GolemKick02State : GolemActionState
{
	private enum Combo {KneeKick, SideKick, SideBlast, BrushKick, End};
	private Combo currentCombo;
	public GolemKick02State(AICombatStateMachine stateMachine) : base(stateMachine)
	{
		triggerHash = Animator.StringToHash("KickTrigger02");
	}
	public override void Enter()
	{
		base.Enter();
		Debug.Log("Enter TO Kick2");
		currentCombo = Combo.KneeKick;
		isTriggered = false;
		isRotated = false;
		combatStateMachine.WeaponHandler.CurrentWeapon.AdditionalIndex = 1;
	}

	public override void Tick()
	{
		switch(currentCombo)
		{
			case Combo.KneeKick :
				Kick02Pattern(nextCombo : Combo.SideKick); break;
			case Combo.SideKick :
				Kick02Pattern(nextCombo : Combo.SideBlast); break ;
			case Combo.SideBlast :
				Kick02Pattern(nextCombo : Combo.BrushKick); break ;
			case Combo.BrushKick :
				Kick02Pattern(nextCombo : Combo.End); break ;
		}
	}
	public override void Exit()
	{
		base.Exit();
	}

	private void Kick02Pattern(Combo nextCombo)
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
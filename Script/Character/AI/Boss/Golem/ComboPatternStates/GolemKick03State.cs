using UnityEngine;
public class GolemKick03State : GolemActionState
{
	private enum Combo {Giratoria, MidKick, FrontKick, End};
	private Combo currentCombo;
	public GolemKick03State(AICombatStateMachine stateMachine) : base(stateMachine)
	{
		triggerHash = Animator.StringToHash("KickTrigger03");
	}
	public override void Enter()
	{
		base.Enter();
		Debug.Log("Enter TO Kick3");
		currentCombo = Combo.Giratoria;
		isTriggered = false;
		isRotated = false;
		combatStateMachine.WeaponHandler.CurrentWeapon.AdditionalIndex = 2;
	}

	public override void Tick()
	{
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
			animator.SetBool("ShouldFollowPlayer", true);
			isTriggered = true;
			return ;
		}
		switch(currentCombo)
		{
			case Combo.Giratoria :
				Kick03Pattern(nextCombo : Combo.MidKick); break;
			case Combo.MidKick :
				Kick03Pattern(nextCombo : Combo.FrontKick); break ;
			case Combo.FrontKick :
				Kick03Pattern(nextCombo : Combo.End); break ;
		}
	}
	public override void Exit()
	{
		base.Exit();
		animator.SetBool("ShouldFollowPlayer", false);
	}

	private void Kick03Pattern(Combo nextCombo)
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
using UnityEngine;
public class GolemKick04State : GolemActionState
{
	private enum Combo {SprintToSlide, SideKick, Pontera ,End};
	private Combo currentCombo;
	public GolemKick04State(AICombatStateMachine stateMachine) : base(stateMachine)
	{
		triggerHash = Animator.StringToHash("KickTrigger04");
	}
	public override void Enter()
	{
		base.Enter();
		Debug.Log("Enter TO Kick4");
		currentCombo = Combo.SprintToSlide;
		isTriggered = false;
		isRotated = false;
		combatStateMachine.WeaponHandler.CurrentWeapon.AdditionalIndex = 3;
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
			case Combo.SprintToSlide :
				Kick04Pattern(nextCombo : Combo.SideKick); break;
			case Combo.SideKick :
				Kick04Pattern(nextCombo : Combo.Pontera); break ;
			case Combo.Pontera :
				Kick04Pattern(nextCombo : Combo.End); break ;
		}
	}
	public override void Exit()
	{
		base.Exit();
		animator.SetBool("ShouldFollowPlayer", false);
	}

	private void Kick04Pattern(Combo nextCombo)
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
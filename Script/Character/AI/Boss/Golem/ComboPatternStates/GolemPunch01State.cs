using UnityEngine;
public class GolemPunchState : GolemActionState
{
	private enum Combo {BackPunch, Elbow, UpperCut, End};
	private Combo currentCombo;
	public GolemPunchState(AICombatStateMachine stateMachine) : base(stateMachine)
	{
		triggerHash = Animator.StringToHash("PunchTrigger");
	}
	public override void Enter()
	{
		base.Enter();
		Debug.Log("Enter TO Punch");
		currentCombo = Combo.BackPunch;
		isTriggered = false;
		isRotated = false;
		combatStateMachine.WeaponHandler.CurrentWeapon.AdditionalIndex = 4;
	}

	public override void Tick()
	{
		switch(currentCombo)
		{
			case Combo.BackPunch :
				Punch01Pattern(nextCombo : Combo.Elbow); break;
			case Combo.Elbow :
				Punch01Pattern(nextCombo : Combo.UpperCut); break ;
			case Combo.UpperCut :
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
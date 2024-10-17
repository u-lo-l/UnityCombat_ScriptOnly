using UnityEngine;

public partial class Player
{
	public Transform TargetTransform;
	private Transform GetTargetTransform()
	{
		return targeterComponent.GetTargetTransform(this.transform);
	}
	public bool IsFreeLookMode() => movementStateMachine.IsFreeLookMode();
	public bool IsTargetingMode() => movementStateMachine.IsTargetingMode();
	public bool IsAimingMode() => movementStateMachine.IsTargetingMode() && weaponHandler.CurrentWeapon is Bow;
	public void SwitchToFreeLookMode()
	{
		if (targeterComponent != null)
			targeterComponent.gameObject.SetActive(false);
		movementStateMachine.SwitchToFreeLookMode();
		if (combatStateMachine.WeaponHandler.CurrentWeapon is Bow && bowHandIIKHandler != null)
		{
			bowHandIIKHandler.DisableAimCanvas();
			bowHandIIKHandler.TargetTransform = null;
		}

	}
	public void SwitchToTargetingMode()
	{
		targeterComponent.gameObject.SetActive(true);
		TargetTransform = GetTargetTransform();
		if (TargetTransform != null)
		{
			movementStateMachine.SwitchToTargetingMode();
			Animator.SetBool(AnimatorHash.Player.IsFreeLookState, false);
			if (combatStateMachine.WeaponHandler.CurrentWeapon is Bow && bowHandIIKHandler != null)
			{
				bowHandIIKHandler.TargetTransform = TargetTransform;
				if (TargetTransform != null)
				{
					bowHandIIKHandler.EnableAimCanvas();
				}
			}
		}
		else
		{
			targeterComponent.gameObject.SetActive(false);
		}
	}
	private void SubscribeTargetEvent()
	{
		MovementInputHandler.OnTarget += ToggleTargetingState;
		targeterComponent.OnTargetRemoved += OnTargetRemoved;
	}
	private void ToggleTargetingState()
	{
		if (IsTargetingMode() == true)
			SwitchToFreeLookMode();
		else
			SwitchToTargetingMode();
	}
	private void OnTargetRemoved()
	{
		if (IsFreeLookMode() == true)
			return ;
		switch(targeterComponent.AutoChangeTarget)
		{
			case true :
				TargetTransform = GetTargetTransform();
				if (TargetTransform == null)
					SwitchToFreeLookMode();
			break;
			case false :
				SwitchToFreeLookMode();
			break;
		}
	}
}
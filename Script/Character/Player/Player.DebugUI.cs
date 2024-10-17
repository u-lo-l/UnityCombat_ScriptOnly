using UnityEngine;

public partial class Player
{
	public string str_MovementState => movementStateMachine.GetCurrentState().ToString();
	public string str_InputDirection => MovementInputHandler.KeyboardInputDirection();
	public string str_RunToggle => movementStateMachine.ShouldRun ? "ON" : "OFF";
	public string str_IsGrounded => EnvironmentChecker.IsGrounded.ToString();
	public string str_CanMove => movementStateMachine.CanMove.ToString();
	public string str_CanJump => movementStateMachine.CanJump().ToString();
	public string str_CanDodge => (movementStateMachine.CanDodge() && weaponHandler.ArmedType != WeaponType.Unarmed).ToString();
	public string str_DodgeCoolDown => movementStateMachine.RemainDodgeEnergy.ToString("F2");
	public string str_Velocity => CharacterController.velocity.ToString();
	public string str_CombatState => combatStateMachine.GetCurrentState().ToString();
	public string str_Weapon => weaponHandler.ArmedType.ToString();
	public string str_NextComboEnable => weaponHandler.CanNextCombo.ToString();
}
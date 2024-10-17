// using UnityEngine;

// public class PlayerSpellState : PlayerCombatState
// {
// 	private int spellIndex = -1;
// 	private const int MaxSpellIndex = 3;
// 	private WeaponHandler weaponHandler;

// 	public PlayerSpellState(PlayerCombatStateMachine stateMachine) : base(stateMachine)
// 	{
// 		weaponHandler = stateMachine.WeaponHandler;
// 	}
// 	public override void Enter()
// 	{
// 		CurrentState = State.Spell;
// 		combatStateMachine.Player.movementStateMachine.CanMove = false;
// 		if (spellIndex < 0 || spellIndex > MaxSpellIndex)
// 			combatStateMachine.ChangeState(combatStateMachine.HoldingState);
// 		weaponHandler.StartAttack(weaponHandler.AttackIndex);
// 		combatStateMachine.Player.Animator.SetInteger(AnimatorHash.Player.ActionIndex, spellIndex);
// 		combatStateMachine.Player.Animator.SetTrigger(AnimatorHash.Player.ActionTrigger);
// 	}
// 	public override void Exit()
// 	{
// 		spellIndex = -1;
// 		combatStateMachine.Player.Animator.ResetTrigger(AnimatorHash.Player.ActionTrigger);
// 		combatStateMachine.Player.Animator.SetInteger(AnimatorHash.Player.ActionIndex, -1);
// 		weaponHandler.EndAttack(weaponHandler.AttackIndex);
// 	}
// 	public void SetSpellIndex(int index)
// 	{
// 		if (index < 0 || index > MaxSpellIndex)
// 		{
// 			Debug.LogWarning("SpellIndex Out Of Range");
// 		}
// 		else
// 		{
// 			spellIndex = index;
// 		}
// 	}
// }
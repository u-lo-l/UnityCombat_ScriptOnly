using System;
using System.Collections;
using UnityEngine;

public class PlayerDamageState : PlayerCombatState
{
	private Animator animator;
	private int animatorDamagedLayer;
	private Action[ , ] HitActions;

	public PlayerDamageState(PlayerCombatStateMachine stateMachine) : base(stateMachine)
	{
		animator = stateMachine.Player.Animator;
		animatorDamagedLayer = AnimatorHash.Player.DamagedLayer;
		HitActions = new Action[3, 4]
		{
			{ HitUpperFront, HitUpperRear, HitUpperLeft, HitUpperRight },
			{ HitMiddleFront, HitMiddleRear, HitMiddleLeft, HitMiddleRight },
			{ HitLowerFront, HitLowerRear, HitLowerLeft, HitLowerRight }
		};
	}

	public override void Enter()
	{
		CurrentState = State.Damage;
		combatStateMachine.Player.movementStateMachine.CanMove = false;
		combatStateMachine.Player.PlayLocomotion();
		
		combatStateMachine.Player.LayerFadeIn(animator, AnimatorHash.Player.DamagedLayer, 0f);
		int height = UnityEngine.Random.Range(0, 3);
		int direction = UnityEngine.Random.Range(0, 4);
		HitActions[height, direction]();
	}
	public override void Tick()
	{

	}
	public override void FixedTick()
	{
		
	}
	public override void Exit()
	{
		combatStateMachine.Player.LayerFadeOut(animator, AnimatorHash.Player.DamagedLayer, 0f);
		combatStateMachine.Player.movementStateMachine.CanMove = true;
	}

	private void HitUpperFront()
	{
		animator.Play("Damage_High_Front", animatorDamagedLayer);
	}
	private void HitUpperRear()
	{
		animator.Play("Damage_High_Rear", animatorDamagedLayer);
	}
	private void HitUpperLeft() 
	{
		animator.Play("Damage_High_Left", animatorDamagedLayer);
	}
	private void HitUpperRight() 
	{
		animator.Play("Damage_High_Right", animatorDamagedLayer);
	}
	
	private void HitMiddleFront() 
	{
		animator.Play("Damage_Middle_Front", animatorDamagedLayer);
	}
	private void HitMiddleRear() 
	{
		animator.Play("Damage_Middle_Rear", animatorDamagedLayer);
	}
	private void HitMiddleLeft() 
	{
		animator.Play("Damage_Middle_Left", animatorDamagedLayer);
	}
	private void HitMiddleRight() 
	{
		animator.Play("Damage_Middle_Right", animatorDamagedLayer);
	}

	private void HitLowerFront() 
	{
		animator.Play("Damage_Low_Left", animatorDamagedLayer);
	}
	private void HitLowerRear() 
	{
		animator.Play("Damage_Low_Right", animatorDamagedLayer);
	}
	private void HitLowerLeft() 
	{
		animator.Play("Damage_Low_Left", animatorDamagedLayer);
	}
	private void HitLowerRight() 
	{
		animator.Play("Damage_Low_Right", animatorDamagedLayer);
	}
}
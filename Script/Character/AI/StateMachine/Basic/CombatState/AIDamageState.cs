using System;
using UnityEngine;
public class AIDamageState : AICombatState
{
	public enum HitDirection { Front, Rear, Left, Right };
	public enum HitHeight { Upper, Middle, Lower };
	public HitDirection Direction { private get; set; }
	protected int direction => (int)Direction;
	public HitHeight Height { private get; set; }
	protected int height => (int)Height;
	protected int[ , ] HitActionHashes;
	protected int animatorDamagedLayer;
	protected int animatorActionLayer;
	protected float FixedDuration => 0.02f * Time.timeScale;
	public AIDamageState(AICombatStateMachine stateMachine) : base(stateMachine)
	{
		CurrentState = State.Damage;
		animatorDamagedLayer = AnimatorHash.Enemy.DamagedLayer;
		animatorActionLayer = AnimatorHash.Enemy.ActionLayer;

		HitActionHashes = new int[3,4]
		{
			{
				AnimatorHash.Damaged.Damage_High_Front,
				AnimatorHash.Damaged.Damage_High_Rear,
			  	AnimatorHash.Damaged.Damage_High_Left,
				AnimatorHash.Damaged.Damage_High_Right
			},
			{
				AnimatorHash.Damaged.Damage_Middle_Front,
				AnimatorHash.Damaged.Damage_Middle_Rear,
			  	AnimatorHash.Damaged.Damage_Middle_Left,
				AnimatorHash.Damaged.Damage_Middle_Right
			},
			{
				AnimatorHash.Damaged.Damage_Low_Left,
				AnimatorHash.Damaged.Damage_Low_Right,
				AnimatorHash.Damaged.Damage_Low_Left,
				AnimatorHash.Damaged.Damage_Low_Right
			},
		};
	}
	public override void Enter()
	{
		Height = (HitHeight)UnityEngine.Random.Range(0, 3);
		Direction = (HitDirection)UnityEngine.Random.Range(0, 4);
		combatStateMachine.Enemy.PlayLocomotion();
		combatStateMachine.Enemy.LayerFadeIn(animator, animatorDamagedLayer, 0);
		combatStateMachine.Enemy.LayerFadeOut(animator, animatorActionLayer, 0);

		animator.CrossFadeInFixedTime(HitActionHashes[height, direction], FixedDuration, animatorDamagedLayer);
		combatStateMachine.Enemy.ForceStop(1f);
	}
	public override void Tick()
	{

	}
	public override void FixedTick()
	{

	}
	public override void Exit()
	{
		animator.SetLayerWeight(animatorDamagedLayer, 0);
	}
}
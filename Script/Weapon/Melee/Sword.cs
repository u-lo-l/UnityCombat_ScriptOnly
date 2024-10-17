using System;
using System.Collections;
using UnityEngine;
public class Sword : Melee
{
#region Monobehaviour
	protected override void Awake()
	{
		base.Awake();
		EquipAnimationHash = AnimatorHash.Player.EquipSword;
	}
	protected override void Start()
	{
		base.Start();
	}
#endregion
	private Vector3 slashStartPosition;
	private Coroutine slashCoroutine;
	public void SlashStarts(float time, float distance)
	{
		slashStartPosition = Owner.transform.position;
		slashCoroutine = StartCoroutine(LightiningSlash(Owner as Player, time, distance));
	}
	public event Action<RaycastHit[], Sword> OnSlashHits;
	public void SlashEnds()
	{
		Vector3 direction = Owner.transform.position - slashStartPosition;
		float radius = Owner.Height / 2;
		Vector3 origin = slashStartPosition + radius * Vector3.up;
		LayerMask targetLayerMask = GetLayerMask.GetEnemyLayerMask;
		RaycastHit[] slashtargets = Physics.SphereCastAll(origin, radius, direction, direction.magnitude, targetLayerMask);
		OnSlashHits?.Invoke(slashtargets, this);
		StopCoroutine(slashCoroutine);
		slashCoroutine = null;
	}

	private IEnumerator LightiningSlash(Player owner, float slashTime, float slashDistance)
	{
		float elapsedTime = 0;
		while (elapsedTime < slashTime)
		{
			Vector3 direction = owner.EnvironmentChecker.FixedForward;
			owner.movementStateMachine.DeltaMove(direction * slashDistance / slashTime * Time.deltaTime);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}
}

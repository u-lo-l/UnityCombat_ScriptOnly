using System.Collections;
using UnityEngine;

public abstract partial class Character
{
	protected IEnumerator LaunchByAttack(ActionData attackData, Vector3 force)
	{
		yield return WaitForStopFrame(attackData.StopFrame);
		yield return ApplyImpulse(force, attackData.ImpactingTime);
		yield return WaitDuringAirborne();
		if (IsDead == true)
		{
			print($"{name} is Dead");
			yield return null;
			if (collider != null)
				this.collider.isTrigger = true;
		}
	}
	protected IEnumerator WaitForStopFrame(int frame)
	{
		for(int i = 0 ; i < frame ; i++)
			yield return new WaitForEndOfFrame();
	}
	protected abstract IEnumerator ApplyImpulse(Vector3 force, float time);
	protected abstract IEnumerator WaitDuringAirborne();
	private Coroutine ImpulseCoroutine;
	public void AddImpluse(Vector3 force, float time)
	{
		if (ImpulseCoroutine != null)
			StopCoroutine(ImpulseCoroutine);
		ImpulseCoroutine = StartCoroutine(ApplyImpulse(force, time));
	}
}
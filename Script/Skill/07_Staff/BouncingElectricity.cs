using System.Collections;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class BouncingElectricity : Projectile
{
	[SerializeField] private ParticleSystem electricParticles;

	private const int MaxTargetCount = 6;
	private const float ElectricityRadius = 5f;
	private readonly Collider[] targets = new Collider[MaxTargetCount];
	protected override void Awake()
	{
		base.Awake();
	}
	private void OnEnable()
	{
		electricParticles.Stop();
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;
		rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		collider.isTrigger = true;
	}
	protected override void OnTriggerEnter(Collider other)
	{
		if (DefaultTrigginCheck(other) == true)
		{
			Debug.Log("Bounce");
			Shoot(false);
		}
		else
		{
			Debug.Log("Bounce Fail");
			Destroy(gameObject);
		}
	}
	public void Shoot(bool isFirstShoot = true)
	{
		Debug.Assert(Owner != null, "[BouncingElectricity] : Owner Not Found");
		Debug.Assert(TargetLayerMask >= 0, "[BouncingElectricity] : TargetLayer Not Found");

		Character nextTarget = FindNextCharacter(isFirstShoot);
		print("target : " + (nextTarget == null ? "null" : $"{nextTarget.gameObject.name}" ));
		if (nextTarget == null || hitList.Count > MaxTargetCount)
		{
			electricParticles.Stop();
			collider.enabled = false;
			Destroy(gameObject, 0.5f);
			return ;
		}

		if (electricParticles.isPlaying == false)
		{
			print("Play particle");
			electricParticles.Play();
		}

		Vector3 target = nextTarget.transform.position + Vector3.up * nextTarget.Height * 0.5f;

		if (movingCorountine != null)
		{
			StopCoroutine(movingCorountine);
		}
		movingCorountine = StartCoroutine(MoveTo(target));
	}

	private Character FindNextCharacter(bool isFirstShoot)
	{
		Debug.Log($"Try Find Next Chracter");
		Vector3 center = isFirstShoot ? transform.position + Owner.transform.forward * ElectricityRadius / 2 : transform.position;

		int targetFound = Physics.OverlapSphereNonAlloc(center, ElectricityRadius, targets, TargetLayerMask);
		Debug.Log($"TragetFound = {targetFound}");

		for(int i = 0 ; i < targetFound ; i++)
		{
			if (targets[i].TryGetDamagable(out IDamagable damagable, Owner, this.gameObject, Weapon.AllyLayerMask, hitList) == true)
			{
				if (hitList.Contains(damagable) == false)
					return targets[i].GetComponent<Character>();
			}
		}

		return null;
	}
	private Coroutine movingCorountine = null;
	private IEnumerator MoveTo(Vector3 position)
	{
		Vector3 start = transform.position;
		float time = (position - start).magnitude / launchPower;
		float elapsedTime = 0;
		while(elapsedTime < time)
		{
			elapsedTime += Time.deltaTime;
			transform.position = Vector3.Lerp(start, position, elapsedTime / time);
			yield return null;
		}
		movingCorountine = null;
	}
}
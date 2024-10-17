using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(AudioSource))]
public class MagicArrow : Projectile
{
	[SerializeField] private ParticleSystem arrowParticles;
	public float StartingSpeed {private get; set;} = 50f;
	private Coroutine lifeTimeChecker = null;
	protected override void Awake()
	{
		base.Awake();
		if (gameObject.TryGetComponent<ParticleSystem>(out arrowParticles) == false)
		{
			Debug.Assert(false, "[MagicArrow] : ParticleSystem Not Found");
		}
		if (gameObject.TryGetComponent<Rigidbody>(out rigidbody) == false)
		{
			Debug.Assert(false, "[MagicArrow] : Rigidbody Not Found");
		}
		if (gameObject.TryGetComponent<Collider>(out collider) == false)
		{
			Debug.Assert(false, "[MagicArrow] : CapsuleCollider Not Found");
		}
		if (gameObject.TryGetComponent<AudioSource>(out audioSource) == false)
		{
			Debug.Assert(false, "[MagicArrow] : AudioSource Not Found");
		}
	}
	private void OnEnable()
	{
		arrowParticles.Stop();
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;
		rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		collider.isTrigger = true;
	}
	protected override void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == Owner.gameObject.layer)
			return ;
		if (DefaultTrigginCheck(other) == true)
		{
			StopArrow(other);
		}
		else if (other.gameObject.layer == GetLayerMask.GetObstacleLayer)
		{
			StopArrow(other);
		}
	}
	public void Shoot()
	{
		Debug.Assert(Owner != null, "[MagicArrow] : Owner Not Found");
		Debug.Assert(TargetLayerMask >= 0, "[MagicArrow] : TargetLayerMask Not Found");
		Debug.Assert(StartingSpeed >= 0, "[MagicArrow] : StartingSpeed Not Found");

		this.transform.SetParent(null);
		arrowParticles.Play();
		rigidbody.isKinematic = false;
		rigidbody.AddForce(transform.forward * StartingSpeed, ForceMode.VelocityChange);
		lifeTimeChecker = StartCoroutine(CountMaxLifeTime(lifeTime));
	}
	private void StopArrow(Collider other)
	{
		rigidbody.isKinematic = true;
		collider.enabled = false;
		arrowParticles.Stop();
		this.transform.position = SetStopPosition(other);
		this.transform.SetParent(other.transform);
		if (lifeTimeChecker != null)
		{
			StopCoroutine(lifeTimeChecker);
		}
		Destroy(gameObject, 5);
	}

	private Vector3 SetStopPosition(Collider other)
	{
		float randomOffset =  UnityEngine.Random.Range(-0.2f, 0.2f);
		if (other.gameObject.GetComponent<TerrainCollider>() == true)
		{
			return this.transform.position + randomOffset * transform.forward;
		}
		else if (other.gameObject.GetComponent<MeshCollider>() == true)
		{
			return this.transform.position + randomOffset * transform.forward;
		}
		else
		{
			return other.ClosestPoint(Owner.transform.position) + randomOffset * transform.forward;
		}
	}
	private IEnumerator CountMaxLifeTime(float time)
	{
		yield return new WaitForSeconds(time);
		Destroy(this.gameObject);
	}

}

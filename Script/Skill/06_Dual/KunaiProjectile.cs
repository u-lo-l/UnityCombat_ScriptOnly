using System;
using UnityEngine;

public class KunaiProjectile : Projectile
{
	protected override void Awake()
	{
		base.Awake();
	}
	private void Start()
	{
		rigidbody.AddForce(transform.forward * launchPower, ForceMode.Force);
		Destroy(this.gameObject, lifeTime);
	}
}
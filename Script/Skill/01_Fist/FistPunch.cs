using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GetLayerMask;

public class FistPunch : Projectile
{
	[SerializeField] private GameObject DustExplosionPrefab;
	[SerializeField] private AudioClip explosionClip;

	private GameObject DustEffect;
	public float AftereffectsRadius {private get; set;}
	protected override void Awake()
	{
		base.Awake();
		DustEffect = Instantiate<GameObject>(DustExplosionPrefab, this.transform);
		DustEffect.SetActive(false);
	}
	private void Start()
	{
		rigidbody.AddForce(Vector3.down * 2000f);
	}

	protected override void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == GetObstacleLayer)
		{
			rigidbody.isKinematic = true;
			DustEffect.transform.localPosition = Vector3.forward * 0.5f;
			DustEffect.SetActive(true);
			audioSource.clip = explosionClip;
			audioSource.Play();
			Destroy(this.gameObject, 3f);
		}
		if (DefaultTrigginCheck(other) == false)
			return ;
	}
}


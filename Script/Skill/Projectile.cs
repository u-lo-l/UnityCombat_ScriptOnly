using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Projectile : MonoBehaviour
{
	public Character Owner {protected get; set;}
	public Weapon Weapon {protected get; set;}
	public LayerMask TargetLayerMask {protected get; set;}
	protected new Collider collider;
	protected new Rigidbody rigidbody;
	protected AudioSource audioSource;
	protected List<IDamagable> hitList = new();
	[SerializeField] protected float launchPower;
	[SerializeField] protected float lifeTime = 1f;

	public event Action<Collider,Weapon,Vector3> OnProjectileHit;

	protected virtual void Awake()
	{
		collider = GetComponent<Collider>();
		rigidbody = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
		collider.isTrigger = true;
		rigidbody.isKinematic = false;
		gameObject.layer = GetLayerMask.GetWeaponLayer;
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		if (DefaultTrigginCheck(other) == false)
			return ;
	}

	protected bool DefaultTrigginCheck(Collider other)
	{
		if (other.gameObject.layer == Owner.gameObject.layer)
			return false;
		if (((1 << other.gameObject.layer) & TargetLayerMask) != 0)
		{
			if (other.TryGetDamagable(out IDamagable damagable, Owner, gameObject, Weapon.AllyLayerMask, hitList) == false)
			{
				return false;
			}
			hitList.Add(damagable);
			OnProjectileHit?.Invoke(other, Weapon, transform.position);
			return true;
		}
		else
		{
			return false;
		}
	}
}
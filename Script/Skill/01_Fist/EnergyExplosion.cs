using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(SphereCollider))]
public class EnergyExplosion : MonoBehaviour
{
	private new SphereCollider collider;
	public Character Owner {private get; set;}
	public Weapon Weapon {private get; set;}
	public float ExplosionRadius {private get; set;}
	public LayerMask TargetLayerMask {private get; set;}
	public event Action<Vector3, Weapon, List<KeyValuePair<Character, IDamagable>> > OnApplyExplosionDamage;
	public LayerMask AllyLayerMask {private get; set;}
	private void Awake()
	{
		collider = GetComponent<SphereCollider>();
	}

	private IEnumerator Start()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius, TargetLayerMask);
		List<KeyValuePair<Character, IDamagable>> damagables = new();
		foreach(var target in colliders)
		{
			if (target.TryGetDamagable(out IDamagable damagable, Owner, gameObject, AllyLayerMask) == true)
			{
				Character victim = target.GetComponent<Character>();
				damagables.Add(new KeyValuePair<Character, IDamagable>(victim, damagable));
			}
		}
		yield return new WaitForSeconds(0.5f);
		OnApplyExplosionDamage?.Invoke(transform.position, Weapon, damagables);
	}	
}

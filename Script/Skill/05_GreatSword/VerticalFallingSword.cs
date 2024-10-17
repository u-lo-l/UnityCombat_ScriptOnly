using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class VerticalFallingSword : Projectile
{
	[SerializeField] private AnimationCurve fallingCurve;
	[field : SerializeField] public float ReadyTime { get; private set; }= 0.5f;
	protected override void Awake()
	{
		collider = GetComponent<CapsuleCollider>();
		rigidbody = GetComponent<Rigidbody>();
		collider.isTrigger = true;
		rigidbody.isKinematic = true;
		gameObject.layer = GetLayerMask.GetWeaponLayer;
	}
	private IEnumerator Start()
	{
		collider.enabled = false;
		float elapsedTime;
		Quaternion targetRotation;
		float readyTime = ReadyTime / 4;
		for (int i = 0; i < 2; i++)
		{
			elapsedTime = 0;
			targetRotation = Quaternion.LookRotation(Vector3.up);
			float lerpRate;
			while (elapsedTime < readyTime)
			{
				lerpRate = elapsedTime / readyTime;
				transform.rotation = Quaternion.SlerpUnclamped(this.transform.rotation, targetRotation, lerpRate);
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			elapsedTime = 0;
			targetRotation = Quaternion.LookRotation(Vector3.down);
			while (elapsedTime < readyTime)
			{
				lerpRate = elapsedTime / readyTime;
				transform.rotation = Quaternion.SlerpUnclamped(this.transform.rotation, targetRotation, lerpRate);
				elapsedTime += Time.deltaTime;
				yield return null;
			}
		}
		collider.enabled = true;

		while (true)
		{
			transform.Translate(Vector3.down * Time.deltaTime * 50f, Space.World);
			yield return null;
		}
	}

}

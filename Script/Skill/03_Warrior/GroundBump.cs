using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class GroundBump : MonoBehaviour
{
	private new CapsuleCollider collider;
	[SerializeField] public ActionData attackData {private get; set;}
	[SerializeField] private float heightBase;
	[SerializeField] private float bumpTime = 0.5f;
	public Character Owner;
	public Weapon weapon;
	private List<IDamagable> hitList;
	public LayerMask AllyLayerMask {private get; set;}
	public event Action<Collider, Weapon, Vector3> OnRockHit;
	private void Awake()
	{
		collider = GetComponent<CapsuleCollider>();
		collider.isTrigger = true;
		hitList = new();
		gameObject.layer = LayerMask.NameToLayer("Weapon");
	}

	private IEnumerator Start()
	{
		float height = heightBase + UnityEngine.Random.Range(0.5f, 2f);;
		transform.localScale *= height * 0.5f;
		transform.position -= height * transform.up;
		Vector3 originPosition = transform.position;
		Vector3 targetPosition = originPosition + 0.9f * height * transform.up;
		float elapsedTime = 0;
		while(elapsedTime < bumpTime)
		{
			transform.position = Vector3.Lerp(originPosition, targetPosition, elapsedTime / bumpTime);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		transform.position = targetPosition;

		yield return new WaitForSeconds(bumpTime);

		elapsedTime = 0;
		while(elapsedTime < bumpTime)
		{
			transform.position = Vector3.Lerp(targetPosition, originPosition, elapsedTime / bumpTime);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		transform.position = targetPosition;
		Destroy(this.gameObject);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == Owner.gameObject.layer)
			return ;
		if (other.TryGetDamagable(out IDamagable damagable, Owner, gameObject, AllyLayerMask, hitList) == false)
		{
			return ;
		}
		hitList.Add(damagable);
		OnRockHit?.Invoke(other, weapon ,transform.position);
	}

}

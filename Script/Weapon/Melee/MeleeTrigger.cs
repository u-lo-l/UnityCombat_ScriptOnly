using System;
using UnityEngine;

public class MeleeTrigger : MonoBehaviour
{
	protected Character owner;
	public event Action<Collider> OnTriggerIn;
	public event Action<Collider> OnTriggerOut;

	protected virtual void Awake()
	{
		GetComponent<Collider>().isTrigger = true;
	}
	protected virtual void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == owner.gameObject)
			return ;
		OnTriggerIn?.Invoke(other);
	}
	protected virtual void OnTriggerExit(Collider other)
	{
		if (other.gameObject == owner.gameObject)
			return ;
		OnTriggerOut?.Invoke(other);
	}
	public void SetOwner(Character owner)
	{
		this.owner = owner;
	}
}

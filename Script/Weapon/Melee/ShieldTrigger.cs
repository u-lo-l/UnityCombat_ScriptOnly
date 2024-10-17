using System;
using UnityEngine;

public enum ShieldMode { Attack, Guard, Push };
public class ShieldTrigger : MeleeTrigger
{
	[field : SerializeField] public ShieldMode Mode { get; set; } = ShieldMode.Attack;
	protected override void Awake()
	{
		base.Awake();
	}

	protected override void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == owner.gameObject)
			return ;
		switch(Mode)
		{
			case ShieldMode.Attack:
				print("Sheild Normal Attack");
				base.OnTriggerEnter(other);
			break;
			case ShieldMode.Guard:
			break;
			case ShieldMode.Push :
				bool isLeft = Vector3.Dot(transform.right, other.transform.position - transform.position) < 0;
				Vector3 forceDirection = isLeft ? -transform.right : transform.right;
				Debug.Log($"{other.gameObject.name} on the " + (isLeft ? "left" : "right"));

				if(other.TryGetComponent(out Character character) == true)
				{
					Debug.Log($"Pushing Character : {character.gameObject.name}");
					character.AddImpluse(forceDirection * 200, 50);
				}
				else if (other.TryGetComponent(out Rigidbody body) == true)
				{
					Debug.Log($"Pushing Object : {body.gameObject.name}");
					body.AddForce(forceDirection * 500);
				}
				base.OnTriggerEnter(other);
			break;
		}
	}
	protected override void OnTriggerExit(Collider other)
	{
		base.OnTriggerExit(other);
	}
}

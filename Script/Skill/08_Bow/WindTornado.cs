
using UnityEngine;
public class WindTornado : Projectile
{
	protected override void Awake()
	{
		base.Awake();
	}

	private void OnEnable()
	{
		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;
		rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		collider.isTrigger = true;

		rigidbody.AddForce(this.transform.forward * launchPower);
		Destroy(this.gameObject, lifeTime);
	}

	protected override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
	}
}

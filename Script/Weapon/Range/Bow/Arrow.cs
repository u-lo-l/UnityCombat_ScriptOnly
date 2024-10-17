using Tiny;
using UnityEngine;

public class Arrow : Projectile
{
	private Trail trail;

	private bool isShot = false;
	protected override void Awake()
	{
		base.Awake();
		trail = GetComponent<Trail>();
		rigidbody.useGravity = false;
		collider.enabled = false;
		trail.enabled = false;
	}

	Vector3 prevPosition;
	private void Update()
	{
		if (isShot == true)
		{
			Vector3 dir = (transform.position - prevPosition).normalized;
			if (dir != Vector3.zero)
			{
				transform.rotation = Quaternion.LookRotation(dir);
			}
			prevPosition = transform.position;
		}
	}
	public void Shoot()
	{
		transform.SetParent(null);
		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;
		rigidbody.AddForce(transform.forward * launchPower);
		collider.enabled = true;
		isShot = true;
		trail.enabled = true;
		Destroy(gameObject, lifeTime);
	}


	protected override void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == GetLayerMask.GetObstacleLayer)
		{
			print("hit Obstacle");
			rigidbody.isKinematic = true;
		}
		if (DefaultTrigginCheck(other) == false)
			return ;
		Destroy(gameObject);
	}
}

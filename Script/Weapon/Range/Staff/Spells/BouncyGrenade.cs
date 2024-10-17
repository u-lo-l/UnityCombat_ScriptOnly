using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class BouncyGrenade : MonoBehaviour
{
	private static readonly float MaxScaleFactor = 3f;
	private float scaleFactor = 1f;
	public GameObject Owner {private get; set;}
	private new Rigidbody rigidbody;
	private float explosionTime;
	private float explosionRange = 2f;
	public event Action<Vector3, float> OnExplosion;
	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
		rigidbody.useGravity = true;
		gameObject.layer = LayerMask.NameToLayer("Weapon");
	}
	private void Start()
	{
		explosionTime = 1.5f + UnityEngine.Random.Range(0, 1f);
	}
	Vector3 prevVelocity;
	private void FixedUpdate()
	{
		prevVelocity = rigidbody.velocity;
		if (explosionTime > 0)
		{
			explosionTime -= Time.fixedDeltaTime;
			return ;
		}
		else if (explosionTime < 0)
		{
			explosionTime = 0;
			Explode();
		}
	}
	private void OnCollisionEnter(Collision other)
	{
		if (Owner == other.gameObject)
		{
			return ;
		}
		if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
		{
			prevVelocity.x *= 2;
			prevVelocity.y *= -0.6f;
			prevVelocity.z *= 2;
			rigidbody.velocity = prevVelocity;

			explosionRange = Mathf.Min(explosionRange * 1.25f, MaxScaleFactor);
			scaleFactor = Mathf.Min(scaleFactor * 1.25f , MaxScaleFactor);
			transform.localScale = scaleFactor * Vector3.one;
		}
		if (other.gameObject.GetComponent<Character>() != null)
		{
			explosionTime = -1;
		}
	}
	private void Explode()
	{
		OnExplosion?.Invoke(transform.position, explosionRange);
		Destroy(this.gameObject);
	}

	public void SetForwardSpeed(float speed)
	{
		this.rigidbody.velocity = speed * transform.forward;
	}
}
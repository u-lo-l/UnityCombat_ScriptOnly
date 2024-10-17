using System;
using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
	[SerializeField] float explosionDelay = 1f;
	[SerializeField] private float radius = 2;
	private Collider[] colliderBuffer;
	public Weapon Weapon {get; set;}
	public LayerMask TargetlayerMask {get; set;}
	public event Action<int, Weapon, Collider[], Vector3> OnExplosion;

	private void Awake()
	{
		colliderBuffer = new Collider[5];
	}
	private IEnumerator Start()
	{
		yield return new WaitForSeconds(explosionDelay);
		int hitCount = Physics.OverlapSphereNonAlloc(transform.position, radius, colliderBuffer, TargetlayerMask);
		if (hitCount > 0)
			OnExplosion?.Invoke(hitCount, Weapon, colliderBuffer, transform.position);

		Destroy(this.gameObject, 3.0f);
	}

	public void SetExplosionRadius(float radius)
	{
		this.radius = Mathf.Max(1f, radius);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color (0.5f, 0, 0, 0.5f);
		Gizmos.DrawSphere(transform.position, radius);
	}
}

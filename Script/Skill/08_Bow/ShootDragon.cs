using System.Collections;
using Tiny;
using UnityEngine;

public class ShootDragon : Projectile
{
	private Coroutine shootCoroutine;
	[SerializeField] Trail trail;
	protected override void Awake()
	{
		base.Awake();
		rigidbody = GetComponent<Rigidbody>();
		trail.enabled = false;
	}

	private void OnEnable()
	{
		shootCoroutine = StartCoroutine(ShootDragonCoroutine());

	}
	private void OnDisable()
	{
		if (shootCoroutine != null)
			StopCoroutine(shootCoroutine);
	}

	protected override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
	}

	private Vector3 initialScale = new(0.01f,0.01f, 0.5f);
	private Vector3 finalScale = new(1, 1, 1);
	private IEnumerator ShootDragonCoroutine()
	{
		transform.localScale = initialScale;
		yield return new WaitForSeconds(0.2f);
		Destroy(this.gameObject, lifeTime);
		rigidbody.AddForce(2 * launchPower * transform.forward);
		yield return new WaitForSeconds(0.05f);
		float elapsedTime = 0;
		trail.enabled = true;
		rigidbody.AddForce(-transform.forward * launchPower);
		while (elapsedTime < 0.1f)
		{
			elapsedTime += Time.deltaTime;
			transform.localScale = Vector3.Lerp(initialScale, finalScale, elapsedTime * 10);
			yield return null;
		}
	}
}

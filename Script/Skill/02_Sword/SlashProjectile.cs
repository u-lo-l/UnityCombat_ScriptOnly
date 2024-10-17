using UnityEngine;
using UnityEngine.VFX;

public class SlashProjectile : Projectile
{
	[SerializeField] private VisualEffect[] slashVFX;
	protected override void Awake()
	{
		base.Awake();
	}
	private void OnEnable()
	{
		Destroy(this.gameObject, lifeTime);
		rigidbody.AddForce(transform.forward * launchPower);
		slashVFX[0].Play();
		slashVFX[1].Play();
	}

	private void OnDisable()
	{
		slashVFX[0].Stop();
		slashVFX[1].Stop();
	}

	protected override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
	}
}
using UnityEngine;

public class MagicBall : Projectile
{
	[SerializeField] private GameObject particle;
	protected override void Awake()
	{
		base.Awake();
	}
	private void Start()
	{
		Destroy(this.gameObject, lifeTime);
		rigidbody.AddForce(this.transform.forward * launchPower);
	}
	protected override void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == this.gameObject.layer)
			return ;
		if (DefaultTrigginCheck(other) == false)
			return ;
		rigidbody.isKinematic = true;
		Destroy(particle);
		Destroy(gameObject, 1);
	}
}

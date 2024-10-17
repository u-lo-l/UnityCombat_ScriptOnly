using UnityEngine;

[CreateAssetMenu(fileName = "ThrowGrenade", menuName = "Spell/Grenade")]
public class Spell_ThrowGrenade : ScriptableObject, ISpell
{
	[SerializeField] private GameObject grenadePrefab;
	private ActionData attackData;
	private GameObject owner;
	public void Execute(Player player, Weapon weapon, ActionData attackData = null, Vector3? aimPosition = null)
	{
		throw new System.NotImplementedException();
	}

	public void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition)
	{
		Debug.Log("Throw Grenade by Boss");
		if (grenadePrefab == null || aimPosition == null)
			return ;
		int grenadeCount = 5 + UnityEngine.Random.Range(-1,2) * 2;
		float angle = Random.Range(10, 30);
		this.owner = enemy.gameObject;
		this.attackData = attackData;
		Transform holderTransform = enemy.transform;

		Vector3 toward = holderTransform.position - aimPosition.Value;

		float heightDiff = Mathf.Abs(toward.y) + 5f;
		toward.y = 0;
		float distanceDiff = Mathf.Max(4f, toward.magnitude - 4.5f);

		Vector3[] muzzlePositions = new Vector3[grenadeCount];
		Quaternion[] muzzleRotations = new Quaternion[grenadeCount];

		for(int i = 0 ; i < grenadeCount ; i++)
		{
			float ang = (i - grenadeCount / 2) * angle;
			muzzleRotations[i] = holderTransform.rotation * Quaternion.Euler(0f, ang, 0f);
		}

		GameObject[] grenadeObjs = new GameObject[grenadeCount];
		BouncyGrenade[] grenades = new BouncyGrenade[grenadeCount];
		for (int i = 0 ; i < grenadeCount ; i++)
		{
			Vector3 offset = muzzleRotations[i] * Vector3.forward *  4f + holderTransform.up * 5f;
			muzzlePositions[i] = holderTransform.position + offset;

			grenadeObjs[i] = Instantiate<GameObject>(grenadePrefab, muzzlePositions[i], muzzleRotations[i], null);
			grenades[i] = grenadeObjs[i].GetComponent<BouncyGrenade>();
			grenades[i].SetForwardSpeed(CalculateInitialVelocity(distanceDiff, heightDiff));
			grenades[i].Owner = enemy.gameObject;
			grenades[i].OnExplosion += OnExplosion;
		}
	}
	private float CalculateInitialVelocity(float distance, float height)
	{
		if (height < 0)
			return 600f;
		return Mathf.Sqrt(Mathf.Abs(Physics.gravity.y) * distance * distance / (2 * height));
	}
	private void OnExplosion(Vector3 position, float radius)
	{
		// if (attackData.hitParticle == null)
		// {
		// 	Debug.Assert(false, "hit particle not found");
		// 	return ;
		// }
		// Instantiate<GameObject>(attackData.hitParticle, position, Quaternion.identity);
		// Collider[] colliders = Physics.OverlapSphere(position, radius);
		// foreach(Collider c in colliders)
		// {
		// 	if (c.gameObject == owner)
		// 	{
		// 		continue;
		// 	}
		// 	if (true == c.TryGetComponent<IDamagable>(out IDamagable damagable))
		// 	{
		// 		DamageProcessor.ApplyDamage(damagable, null, attackData, position, Vector3.zero);
		// 	}
		// }
	}
}
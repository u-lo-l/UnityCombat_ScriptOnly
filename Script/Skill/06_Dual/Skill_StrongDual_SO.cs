using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Dual Strong Skill", menuName = "Spell/Player/Strong/Dual", order = 11)]
public class StrongDualSkill : WeaponSkill_Throwing
{

	[field : Header("Additional Data")]
	public ActionData[] AditionalActionData  {get; protected set;}
	private	const float MaxSkillTime = 3f;
#region ISkill
	public override void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Dual] Strong Skill Executed by Player");

		rotatingKunai = Instantiate<GameObject>(kunaiEffectPrefab, player.transform);
		rotatingKunai.transform.localPosition = player.Height * 0.1f * Vector3.up;
		kunaiRotatingCoroutune = player.StartCoroutine(KunaiRotationEffect(player));

		throwingKunaiCoroutine = player.StartCoroutine(ThrowingKunai(player, weapon, GetLayerMask.GetEnemyLayerMask));
	}
	public override void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Dual] Strong Skill Executed by Enemy");
		throw new System.NotImplementedException();
	}
	public override void Finish(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Dual] Strong Skill Finishedr");
		Destroy(rotatingKunai);

		if (kunaiRotatingCoroutune != null)
		{
			player.StopCoroutine(kunaiRotatingCoroutune);
			kunaiRotatingCoroutune = null;
		}
		if (throwingKunaiCoroutine != null)
		{
			player.StopCoroutine(throwingKunaiCoroutine);
			throwingKunaiCoroutine = null;
		}
	}
#endregion
#region Rotating Kunai Effect
	[Header("Rotating Kunai Effect")]
	[SerializeField] private GameObject kunaiEffectPrefab;
	private GameObject rotatingKunai;
	public Coroutine kunaiRotatingCoroutune;
	private IEnumerator KunaiRotationEffect(Player player)
	{
		float elapsedTime = 0;
		Animator animator =	player.Animator;
		while(MaxSkillTime > elapsedTime)
		{
			Vector3 leftArmPos = animator.GetBoneTransform(HumanBodyBones.LeftShoulder).position
								  - animator.GetBoneTransform(HumanBodyBones.Hips).position;
			Vector3 rightArmPos = animator.GetBoneTransform(HumanBodyBones.RightShoulder).position
								  - animator.GetBoneTransform(HumanBodyBones.Hips).position;

			Vector3 kunaiUp = Vector3.Cross(player.transform.forward, leftArmPos - rightArmPos);
			kunaiUp = Vector3.Lerp(kunaiUp, Vector3.up, 0.5f);
			Quaternion localRotation = Quaternion.FromToRotation(Vector3.up, kunaiUp);
			rotatingKunai.transform.localRotation = localRotation;

			elapsedTime += Time.deltaTime;
			yield return null;
		}
		kunaiRotatingCoroutune = null;
	}
#endregion

#region  Throwing Kunai
	[Header("Throwing Kunai")]
	[SerializeField] private float throwingInterval = 0.4f;
	private Coroutine throwingKunaiCoroutine;
	private IEnumerator ThrowingKunai(Player player, Weapon weapon, LayerMask targetLayerMask)
	{
		float elapsedTime = 0;
		float throwingTime = throwingInterval;
		while (elapsedTime < MaxSkillTime)
		{
			elapsedTime += Time.deltaTime;
			throwingTime -= Time.deltaTime;
			if (throwingTime < 0)
			{
				throwingTime = throwingInterval;
				float startingOrientation = UnityEngine.Random.Range(0f, 45f);
				for(int i = 0 ; i < projectileCount ; i++)
				{
					base.PlayAttackSound(player);
					Vector3 position = 0.75f * player.Height * Vector3.up;
					float pitch = startingOrientation + 360 / projectileCount * i + UnityEngine.Random.Range(-5f, 5f);
					float roll = UnityEngine.Random.Range(-3f, 3f);
					Quaternion orientation = Quaternion.Euler(roll, pitch, 0);
					KunaiProjectile kunai = CreateProjectile<KunaiProjectile>(position, orientation, player, weapon, targetLayerMask);
					SetProjectildAudio(kunai);
				}
			}
			yield return null;
		}
		throwingKunaiCoroutine = null;
	}

	protected override void SetMuzzle(Character holder, out Vector3 position, out Quaternion rotation)
	{
		throw new NotImplementedException();
	}
	#endregion
}
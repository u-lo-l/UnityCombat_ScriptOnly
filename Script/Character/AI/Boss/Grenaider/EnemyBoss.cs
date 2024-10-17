using System;
using System.Collections;
using Cinemachine;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class EnemyBoss : EnemyDynamic
{
	private GameObject hpBar;
	private CinemachineImpulseSource impulseSource;
	[field : SerializeField] public bool IsSleeping { private get; set; } = true;
	[SerializeField] private AudioClip wakeUpSFX;
	[SerializeField] private AudioClip deadSFX;
	private AudioSource audioSource;
	public float MeleeDistance = 2f;
	public float RangeDistance = 15f;
	public bool IsRotating = false;
	public float MinRotationAngle = 30f;
	[SerializeField] private Sequence bossDieSequence;

	protected override void Awake()
	{
		base.Awake();
		collider.enabled = false;
		hpBar = transform.FindByName("HpBar")?.gameObject;
		hpBar?.SetActive(false);
		audioSource = GetComponent<AudioSource>();
		impulseSource = GetComponent<CinemachineImpulseSource>();

		this.NavMeshAgent.enabled = false;
		this.Detector.enabled = false;
		this.EnvironmentChecker.enabled = false;
		this.CharacterStatus.enabled = false;
		this.audioSource.enabled = false;
	}
	protected override void Start()
	{
		base.Start();
		destroyDelay = 10;
		movementStateMachine = new BossMovementStateMachine(this, NavMeshAgent, weaponHandler);
		combatStateMachine = new BossCombatStateMachine(this, weaponHandler);

		SequenceManager.Instance.RegisterSequencePlayEvent(0, WakeUp);
		SequenceManager.Instance.RegisterSequencePlayEvent(1, DeadStartSequence);
		SequenceManager.Instance.RegisterSequenceFinishEvent(1, DeadFinishSequence);
	}
	protected override void LateUpdate()
	{
		if (IsSleeping == false)
		{
			base.LateUpdate();
		}
	}

	protected override void OnAnimatorMove()
	{
		this.transform.rotation *= Animator.deltaRotation;
	}

	public override void PlayLocomotion()
	{
		if (IsRotating == true)
			return ;
		int LocomotionHash = Animator.StringToHash("Move");

		Animator.CrossFadeInFixedTime(LocomotionHash, 0.1f);
	}


	public void ResetToStop(float Delay = 0)
	{
		movementStateMachine.StoppingState.Duration = Delay;
		movementStateMachine?.ChangeState(movementStateMachine.StoppingState);
	}

	public override void FaceToTarget()
	{
		if (IsRotating == true)
			return ;
		float angleDifference = AngleBetweenTarget().GetValueOrDefault();
		if (Mathf.Abs(angleDifference) <= MinRotationAngle)
		{
		}
			// this.transform.rotation *= Quaternion.Euler(0, -angleDifference, 0);
		// else
		// {
		// 	(movementStateMachine as BossMovementStateMachine).SwitchToRotateState(angleDifference);
		// }
	}

	// public override void OnDamage(ActionData attackData, Vector3 dir)
	// {
	// 	if (combatStateMachine.GetCurrentState() == AICombatState.State.Damage)
	// 		return;
	// 	colorChangingCoroutine = StartCoroutine(DamageProcessor.ApplyDamageColor(skinMaterials, skinColors, 0.3f, 3));

	// 	float damage = attackData.Damage;
	// 	int stopFrame = attackData.StopFrame;
	// 	if (combatStateMachine.GetCurrentState() == AICombatState.State.Guard)
	// 	{
	// 		damage /= 2;
	// 		stopFrame /= 2;
	// 	}

	// 	CharacterStatus.OnDamage(damage);
	// 	StartCoroutine(LaunchByAttack(attackData, dir));
	// 	if (CharacterStatus.IsDead == true)
	// 	{
	// 		OnDead();
	// 	}
	// 	else if (combatStateMachine.GetCurrentState() == AICombatState.State.Guard)
	// 	{
	// 		movementStateMachine.ChangeState(movementStateMachine.StoppingState);
	// 	}
	// 	else if (UnityEngine.Random.Range(0, 4) == 1)
	// 	{
	// 		combatStateMachine.ChangeState(combatStateMachine.DamagingState);
	// 	}
	// }
	protected override void OnDead()
	{
		base.OnDead();
		audioSource.clip = deadSFX;
		audioSource.Play();
		bossDieSequence.PlayDirector();
	}
	private void DeadStartSequence()
	{
		Time.timeScale = 0.1f;
	}
	private void DeadFinishSequence()
	{
		Time.timeScale = 1f;
	}

#region Boss WakeUp
	public event Action OnBossActivate;
	public void  WakeUp()
	{
		if (IsSleeping == false)
		{
			return ;
		}
		this.NavMeshAgent.enabled = true;
		this.Detector.enabled = true;
		this.EnvironmentChecker.enabled = true;
		this.CharacterStatus.enabled = true;
		this.audioSource.enabled = true;
		Animator.SetTrigger(AnimatorHash.Boss.WakeUpTrigger);
		audioSource.clip = wakeUpSFX;
		audioSource.PlayDelayed(1f);
	}

	public void ActivateBoss()
	{
		OnBossActivate?.Invoke();
		collider.enabled = true;
		hpBar?.SetActive(true);
		movementStateMachine.ChangeState(movementStateMachine.StoppingState);
		combatStateMachine.ChangeState(combatStateMachine.HoldingState);
		weaponHandler.TryEquip((int)WeaponType.Fist);

		MeleeDistance = weaponHandler.WeaponRange;

		IsSleeping = false;
	}
	public float? AngleBetweenTarget()
	{
		Transform tf = GetTargetTransform();
		if (tf == null)
			return null;

		Vector3 towardVector = tf.position - this.transform.position;

		float u = Vector3.Dot(towardVector, transform.right);
		float v = Vector3.Dot(towardVector, transform.forward);

		float angleDifference = Mathf.Atan2(u, v) * Mathf.Rad2Deg;
		return angleDifference;
	}
#endregion

#region Animation Event
	private void OnStepCameraShake(CameraShakeImpulseData impulseData)
	{
		if (Detector.Target == null || impulseData == null)
			return;
		impulseSource.Shake(impulseData);
	}
#endregion
#region Custom Physics
	protected override IEnumerator ApplyImpulse(Vector3 force, float time)
	{
		force *= 0.1f;
		rigidbody.isKinematic = false;
		Animator.applyRootMotion = false;
		rigidbody.AddForce(force);
		yield return new WaitForSeconds(time);
		rigidbody.isKinematic = true;
		Animator.applyRootMotion = true;
	}
#endregion

	private void OnDrawGizmos()
	{
		if(Application.isPlaying == false)
			return ;

		Gizmos.color = Color.gray;
		Gizmos.DrawWireSphere(transform.position, MeleeDistance);
		Gizmos.DrawWireSphere(transform.position, RangeDistance);
	}
}

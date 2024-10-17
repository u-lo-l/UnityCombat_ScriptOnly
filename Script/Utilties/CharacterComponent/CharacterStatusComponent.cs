using System;
using UnityEngine;

[DisallowMultipleComponent]
public class CharacterStatusComponent : MonoBehaviour
{
	[field : Header("Health Point")]
	[field : SerializeField] public float MaxHealthPoint { get ; private set; } = 100f;
	[field : SerializeField] public float CurrentHealthPoint { get; private set; }
	public bool IsDead => CurrentHealthPoint == 0;
	public float GetHpRate => CurrentHealthPoint / MaxHealthPoint;

	[field : Header("Speed")]
	[field : Tooltip("걷기나 달리기 속도")]
	[SerializeField] private EnvironmentChecker environmentChecker;
	[SerializeField, Range(0f, 4f)] private float walkSpeed = 1f;
	public float WalkSpeed => walkSpeed * SpeedModifier;
	[SerializeField, Range(0f, 8f)] private float runSpeed = 2f;
	public float RunSpeed => runSpeed * SpeedModifier;
	[SerializeField, Range(0f, 12f)] private float sprintSpeed = 4f;
	public float SprintSpeed => sprintSpeed * SpeedModifier;
	[field : SerializeField] public float SpeedModifier {private get; set;} = 1f;

	[field : Header("Stamina")]
	[field : Tooltip("Life Essence : 스킬 등을 사용하는 데 필요한 수치")]
	[field : SerializeField, Range(0f, 100f)] public float Stamina { get ; private set; }

	[field : Header("Poise")]
	[field : Tooltip("가드나 회피를 사용하는 데 필요한 수치")]
	[field : SerializeField, Range(0f, 1f)] public float Poise { get ; private set; }

	[field : Header("Fear")]
	[field : Tooltip("Fear가 일정 수치를 넘어서면 공격력이 낮아진다. Enemy의 경우 도망가거나 공격하는 확률에 영향을 준다.")]
	[field : SerializeField, Range(0f, 1f)] public float Fear { get ; private set; }

	public event Action OnHealthPointChanged;
	public event Action OnDead;

	private void Awake()
	{
		environmentChecker = GetComponent<EnvironmentChecker>();
	}
	private void Start()
	{
		CurrentHealthPoint = MaxHealthPoint;
	}
	private void Update()
	{
		Vector3 fixedForward = environmentChecker.FixedForward;
		SpeedModifier = Vector3.Dot(Vector3.down, fixedForward);
		SpeedModifier = Mathf.Clamp(1 + SpeedModifier, 0.75f, 1.25f);
	}
	public void OnDamage(float amount)
	{
		CurrentHealthPoint = Mathf.Max(CurrentHealthPoint - amount, 0);
		OnHealthPointChanged?.Invoke();
		if (IsDead == true)
		{
			OnDead?.Invoke();
		}
	}
	public void ResetCharacterStatus()
	{
		CurrentHealthPoint = MaxHealthPoint;
		Stamina = 0;
		Poise = 0;
		Fear = 0;
		OnHealthPointChanged?.Invoke();
	}
	public void RecoverHp(float amount)
	{
		CurrentHealthPoint = Mathf.Min(CurrentHealthPoint + amount, MaxHealthPoint);
		OnHealthPointChanged?.Invoke();
	}
	public void RecoverByLostHp(float ratio)
	{
		float lostHp = MaxHealthPoint - CurrentHealthPoint;
		CurrentHealthPoint = Mathf.Min(CurrentHealthPoint + lostHp * ratio, MaxHealthPoint);
		OnHealthPointChanged?.Invoke();
	}
}

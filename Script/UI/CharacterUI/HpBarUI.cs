using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HpBarUI : MonoBehaviour
{
	private CharacterStatusComponent characterStatus;
	[SerializeField] Character character;
	[SerializeField] Image middleground;
	[SerializeField] Image foreground;
	private float fillAmount;
	private Coroutine HpBarDecreasingCoroutine = null;
	private void Awake()
	{
		Debug.Assert(character != null, $"[HpBar] {transform.root.gameObject.name} character not found");
		Debug.Assert(middleground != null || foreground != null, "[HpBar] Image not found");
		characterStatus = character.GetComponent<CharacterStatusComponent>();
		Debug.Assert(characterStatus != null, "[HpBar] CharacterStatusComponent not found");
	}

	private void OnEnable()
	{
		characterStatus.OnHealthPointChanged += HealthPointDecrease;
		characterStatus.OnDead += Dead;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void OnDisable()
	{
		characterStatus.OnHealthPointChanged -= HealthPointDecrease;
		characterStatus.OnDead -= Dead;
	}

	private void Start()
	{
		fillAmount = 1;
		foreground.fillAmount = fillAmount;
		middleground.fillAmount = fillAmount;
	}

	private void HealthPointDecrease()
	{
		if (HpBarDecreasingCoroutine != null)
		{
			StopCoroutine(HpBarDecreasingCoroutine);
		}
		HpBarDecreasingCoroutine = StartCoroutine(DecreaseHealthPointBar());
	}
	private void ResetHealthPoint()
	{
		foreground.fillAmount = middleground.fillAmount = 1;
	}

	private void Dead()
	{
		// Destroy(gameObject, 3);

	}

	private IEnumerator DecreaseHealthPointBar()
	{
		int waitFrame = 5;
		float SmoothRatio = 10 * Time.fixedDeltaTime;
		float targetFillAmount = characterStatus.CurrentHealthPoint / characterStatus.MaxHealthPoint;
		foreground.fillAmount = targetFillAmount;
		if (middleground.fillAmount > targetFillAmount)
		{
			while (fillAmount > targetFillAmount)
			{
				for (int i = 0 ; i < waitFrame ; i++)
				{
					middleground.fillAmount = Mathf.SmoothStep(middleground.fillAmount, targetFillAmount, SmoothRatio);
					yield return null;
				}
			}
		}
		else
		{
			while (fillAmount < targetFillAmount)
			{
				for (int i = 0 ; i < waitFrame ; i++)
				{
					middleground.fillAmount = Mathf.SmoothStep(middleground.fillAmount, targetFillAmount, SmoothRatio);
					yield return null;
				}
			}
		}
		yield break;
	}
}

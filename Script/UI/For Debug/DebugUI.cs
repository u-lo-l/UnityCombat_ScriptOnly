using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour
{
	[SerializeField] private GameObject timeScaler;
	private float timeScale = 1.0f;
	private Slider slider;
	private TMP_Text sliderText;
	[SerializeField] private GameObject characterInfo;
	[SerializeField] private GameObject inputDevice;
	[SerializeField] private Player player;
	[SerializeField] private TMP_Text infoText;

	private bool isActive = false;

	private void Awake()
	{
		Reset();
	}
	private void Reset()
	{
		timeScaler = transform.Find("TimeScaler").gameObject;
		characterInfo = transform.Find("CharacterInfo").gameObject;
		inputDevice = transform.Find("InputDevice").gameObject;

		slider = timeScaler?.GetComponentInChildren<Slider>();
		sliderText = timeScaler?.GetComponentInChildren<TMP_Text>();

		if (infoText == null)
			infoText = characterInfo?.GetComponentInChildren<TMP_Text>();

		timeScaler.SetActive(false);
		characterInfo.SetActive(false);
		inputDevice.SetActive(false);
	}
	private void Start()
	{
		timeScale = Time.timeScale;
		slider.maxValue = 2f;
		slider.minValue = 0.1f;
		slider.value = timeScale;
		sliderText.text = $"time scale : {timeScale}";
	
		infoText.text = "";

		timeScaler.SetActive(isActive);
		characterInfo.SetActive(isActive);
		inputDevice.SetActive(isActive);
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			isActive = !isActive;
			timeScaler.SetActive(isActive);
			characterInfo.SetActive(isActive);
			inputDevice.SetActive(isActive);
		}
		if (player.isActiveAndEnabled == false)
			return ;
		if (isActive == false)
			return;
		infoText.text = 
		$"<b>Move : </b><i>{player.str_MovementState}</i><b> {player.str_InputDirection}</b>\n" +
		$"<b>ShoulRun : </b><i>{player.str_RunToggle}</i>\n" +
		$"<b>IsGrounded : </b><i>{player.str_IsGrounded}</i>\n" +
		$"<b>CanMove : </b><i>{player.str_CanMove}</i>\n" +
		$"<b>CanJump : </b><i>{player.str_CanJump}</i>\n" +
		$"<b>CanDodge : </b><i>{player.str_CanDodge} ({player.str_DodgeCoolDown}s)</i>\n" +
		$"<b>Velocity : </b><i>{player.str_Velocity}</i>\n" +
		$"\n" +
		$"<b>Combat : </b><i>{player.str_CombatState}</i>\n" +
		$"<b>Weapon : </b><i>{player.str_Weapon}</i>\n" +
		$"<b>NextCombo : </b><i>{player.str_NextComboEnable}</i>";
	}
	public void OnTimeScaleChanged()
	{
		timeScale = slider.value;
		sliderText.text = $"time scale : {timeScale:F2}";
		Time.timeScale = timeScale;
	}
}

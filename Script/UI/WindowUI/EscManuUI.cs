using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EscManuUI : MonoBehaviour
{
	private PlayerInput playerInput;
	private InputActionMap uiInputActionMap;
	[SerializeField] GameObject menuCanvas;
	[SerializeField] Slider bgmSlider;
	[SerializeField] Slider footstepSlider;
	[SerializeField] Slider effectSlider;
	private bool isMenuActive = false;
	public event Action OnMenuActive;
	public event Action OnMenuDeactive;
	private void Awake()
	{
		playerInput = GetComponent<PlayerInput>();
		uiInputActionMap = playerInput.actions.FindActionMap("UI");
	}
	private void Start()
	{
		menuCanvas.SetActive(isMenuActive);
	}
	private void OnEnable()
	{
		DeactivateCanvas();
		uiInputActionMap.FindAction("ToggleMenu", true).canceled -= OnToggleMenu;
		uiInputActionMap.FindAction("ToggleMenu", true).canceled += OnToggleMenu;

	}
	private void OnDisable()
	{
		DeactivateCanvas();
		uiInputActionMap.FindAction("ToggleMenu", true).canceled -= OnToggleMenu;
	}
	private void OnToggleMenu(InputAction.CallbackContext c)
	{
		if (isMenuActive == true)
			DeactivateCanvas();
		else
			ActivateCanvas();
	}
	private void ActivateCanvas()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		Time.timeScale = 0f;
		isMenuActive = true;
		bgmSlider.value = AudioVolumeManager.BackGroundMusicVolume;
		footstepSlider.value = AudioVolumeManager.FootStepVolume;
		effectSlider.value = AudioVolumeManager.EffectVolume;
		menuCanvas.SetActive(true);
		OnMenuActive?.Invoke();
	}
	private void DeactivateCanvas()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Time.timeScale = 1f;
		isMenuActive = false;
		menuCanvas.SetActive(false);
		OnMenuDeactive?.Invoke();
	}
#region Buttons
	public void OnBackToGameClicked()
	{
		DeactivateCanvas();
	}
	public void OnBackToTitleClicked()
	{
		DeactivateCanvas();
		SceneLoader.Instance.BackToTitleScene();
	}
	public void OnQuitClicked()
	{
		DeactivateCanvas();
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
#endregion

#region Sliders
	public void OnBGMVolumeChanged(float unsused)
	{
		AudioVolumeManager.BackGroundMusicVolume = bgmSlider.value;
	}

	public void OnFootstepVolumeChanged(float unsused)
	{
		AudioVolumeManager.FootStepVolume = footstepSlider.value;
	}

	public void OnEffectVolumeChanged(float unsused)
	{
		AudioVolumeManager.EffectVolume = effectSlider.value;
	}
#endregion
}

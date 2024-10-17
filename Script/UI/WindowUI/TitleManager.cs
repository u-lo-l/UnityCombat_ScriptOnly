using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
	[SerializeField] private GameObject panel;
	[SerializeField] private GameObject startButton;
	[SerializeField] private GameObject quitButton;
	private SceneLoader sceneLoader;
	private void Awake()
	{
		sceneLoader = SceneLoader.Instance;

		Debug.Assert(panel != null, "[TitleManager] : panel Not Found");
		
		Debug.Assert(startButton != null, "[TitleManager] : startButton Not Found");
		Debug.Assert(quitButton != null, "[TitleManager] : quitButton Not Found");
	}
	private void Start()
	{
		startButton.SetActive(true);
		quitButton.SetActive(true);
	}
	public void OnEnable()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
	public void OnStartClicked()
	{
		print("Should Load Next Scene");
		startButton.SetActive(false);
		quitButton.SetActive(false);
		sceneLoader.Load(SceneLoader.Scene.MapDesert);
	}

	public void OnQuitClicked()
	{
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}


}

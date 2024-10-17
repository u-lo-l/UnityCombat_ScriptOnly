using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
	[SerializeField] private GameObject panel;
	[SerializeField] private GameObject progressBar;
	[SerializeField] private GameObject progressImage;
	private Image progressBarImage;
	private void Awake()
	{
		Debug.Assert(panel != null, "[ProgressBarUI] : panel Not Found");

		Debug.Assert(progressBar != null, "[ProgressBarUI] : progressBar Not Found");

		Debug.Assert(progressImage != null, "[ProgressBarUI] : progresprogressImagesBar Not Found");
		progressBarImage = progressImage.GetComponent<Image>();
		Debug.Assert(progressBarImage != null, "[ProgressBarUI] : progressImage Not Found");
	}
	private void OnEnable()
	{
		progressBarImage.fillAmount = 0;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		print("Entered To Loading Scene");
		StartCoroutine(WaitForLoading());
	}
	private IEnumerator WaitForLoading()
	{
		float lastProgress = 0;
		float progress;
		while(true)
		{
			progress = SceneLoader.Instance.GetLoadingProgress();
			if (progress < 1f)
			{
				lastProgress= Mathf.Lerp(lastProgress, progress, Time.deltaTime * 40);
				progressBarImage.fillAmount = lastProgress;
				print($"[Loadinge] : {lastProgress}%");
				yield return null;
			}
			else
			{
				print($"[Loading] : ALL loaded");
				progressBarImage.fillAmount = 1f;
				yield break;
			}
		}
	}
}

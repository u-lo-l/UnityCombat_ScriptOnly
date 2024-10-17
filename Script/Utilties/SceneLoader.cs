using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// LoadingScene이 따로 없는 Scene Transition Manager이다.<br/>
/// LoadingScene이 생기면 구조가 달라져야한다.<br/>
/// 그 땐 Coding Monkey형님 영상 보고 오자.<br/>
/// </summary>
public class SceneLoader : MonoBehaviour
{
	public enum Scene{ TitleScene, LoadingScene, MapDesert }
	public static SceneLoader Instance {get ; private set;}

	private Action onLoaderCallback;
	AsyncOperation asyncLoadingOperation = null;
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}
	public void BackToTitleScene() => this.Load(Scene.TitleScene);
	public void Load (Scene scene)
	{
		onLoaderCallback = () => {
			StartCoroutine(LoadScene(scene));
		};
		StartCoroutine(LoadSceneDirectly(Scene.LoadingScene));
	}

	float progress = 0;
	public float GetLoadingProgress()
	{
		if (asyncLoadingOperation == null || asyncLoadingOperation.isDone)
			return 1f;
		else
			return progress;
	}

	public void LoadCallback()
	{
		if (onLoaderCallback != null)
		{
			onLoaderCallback?.Invoke();
			onLoaderCallback = null;
		}
	}

	private readonly WaitForSeconds waitForDelay = new(0.25f);

	private IEnumerator LoadSceneDirectly(Scene scene)
	{
		yield return null;
		asyncLoadingOperation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Single);
		while (asyncLoadingOperation.isDone == false)
		{
			progress = asyncLoadingOperation.progress;
			yield return null;
		}
		asyncLoadingOperation = null;
	}
	private IEnumerator LoadScene(Scene scene)
	{
		print($"Start To Load Scene : {scene.ToString()}");
		yield return null;
		progress = 0;
		asyncLoadingOperation = SceneManager.LoadSceneAsync(scene.ToString());
		asyncLoadingOperation.allowSceneActivation = false;
		while (asyncLoadingOperation.isDone == false)
		{
			progress = 0.5f;
			yield return waitForDelay;

			if (asyncLoadingOperation.progress >= 0.5f)
			{
				progress = 0.7f;
				yield return waitForDelay;
			}

			if (asyncLoadingOperation.progress >= 0.7f)
			{
				progress = 0.9f;
				yield return waitForDelay;
			}

			if (asyncLoadingOperation.progress >= 0.9f)
			{
				progress = 1.0f;
				yield return waitForDelay;
			}

			asyncLoadingOperation.allowSceneActivation = true;
			progress = asyncLoadingOperation.progress;
			yield return null;
		}
		asyncLoadingOperation = null;
	}
}


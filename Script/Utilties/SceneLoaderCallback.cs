using UnityEngine;

public class SceneLoaderCallback : MonoBehaviour
{
	private bool isFirstUpdate = true;

	private void LateUpdate()
	{
		if (isFirstUpdate == true)
		{
			isFirstUpdate = false;
			SceneLoader.Instance.LoadCallback();
		}
	}
	private void OnDisable()
	{
		isFirstUpdate = true;
	}
}
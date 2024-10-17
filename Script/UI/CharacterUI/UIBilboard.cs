using UnityEngine;

public class UIBilboard : MonoBehaviour
{
	private Camera mainCamera;
	private void Awake()
	{
		mainCamera = Camera.main;
	}

	private void Update()
	{
		this.transform.rotation = mainCamera.transform.rotation;
	}
}

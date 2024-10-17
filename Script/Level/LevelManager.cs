using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	[SerializeField] private string levelName;
	[SerializeField] private Transform respawnPoint;
	[SerializeField] private GameObject PlayerObject;
	[SerializeField] private bool shouldLockCursor = true;
	private Player player;
	[Header("Normal Mobs")]
	[SerializeField] GameObject[] mobPrefabs;
	[SerializeField] Transform[] mobTransforms;
	[SerializeField] int[] mobPrefabIndex;

	[Header("Boss")]
	[SerializeField] GameObject bossPrefab;
	[SerializeField] Transform bossTransform;

	private void Awake()
	{
		Debug.Assert(respawnPoint != null, "[Level Manager] : Respawn Point Not Found");
		Debug.Assert(PlayerObject != null, "[Level Manager] : Player Point Not Found");
		player = PlayerObject.GetComponent<Player>();
		// Debug.Assert(mobPrefabs != null, "[Level Manager] : mobPrefabs Not Found");
		// Debug.Assert(mobTransforms != null, "[Level Manager] : mobTransforms Not Found");
		// Debug.Assert(mobPrefabIndex != null && mobPrefabIndex.Length != 0, "[Level Manager] : mobPrefabIndex Not Found");
		// Debug.Assert(mobTransforms.Length == mobPrefabIndex.Length, "[Level Manager] : Transform and Index Does Not Match");

		// Debug.Assert(bossPrefab != null, "[Level Manager] : bossPrefab Not Found");
		// Debug.Assert(bossTransform != null, "[Level Manager] : bossTransform Not Found");
	}

	private void Start()
	{
		player.OnPlayerDeath += ResetLevel;
		Cursor.visible = !shouldLockCursor;
		Cursor.lockState = shouldLockCursor ? CursorLockMode.Locked : CursorLockMode.None;
	}

	private void ResetLevel()
	{
		StartCoroutine(RespawnPlayer());
	}

	private IEnumerator RespawnPlayer()
	{
		yield return new WaitForSeconds(6);
		SceneLoader.Instance.BackToTitleScene();
	}
}

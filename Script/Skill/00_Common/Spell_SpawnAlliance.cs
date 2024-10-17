using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "SpawnAlliance", menuName = "Spell/SpawnAlliance", order = 4)]
public class Spell_SpawnAlliance : ScriptableObject, ISpell
{
	[SerializeField] private int AllyCount = 2;
	[SerializeField] private float AllyLifetime = 60;
	[SerializeField] private GameObject AllyPrefab;
	public void Execute(Player player, Weapon weapon, ActionData attackData = null, Vector3? aimPosition = null)
	{
		GameObject[] alliancePool = new GameObject[AllyCount];
		Vector3 playerPosition = player.transform.position + player.transform.forward * 0.5f;

		Vector3 allyLookAt = playerPosition + player.transform.forward * 4f;
		float angle = 90;
		float step = 0;
		if (AllyCount < 2)
			angle = 0;
		else
			step = 180 / (AllyCount - 1);
		for(int i = 0 ; i < AllyCount ; i++)
		{
			alliancePool[i] = Instantiate<GameObject>(AllyPrefab);
			Vector3 temp = Quaternion.Euler(0, angle - i * step , 0) * player.transform.forward * 0.8f;
			alliancePool[i].transform.position = playerPosition + temp;
			alliancePool[i].transform.rotation = Quaternion.LookRotation(allyLookAt - alliancePool[i].transform.position);
			alliancePool[i].transform.FindByName("LifeTimeComponent").GetComponent<LifeTimeComponent>().LifeTime = AllyLifetime;
			alliancePool[i].GetComponent<SummonedAlly>().SummonerTransform = player.transform;
			if (alliancePool[i].TryGetComponent<NavMeshAgent>(out var navMeshAgent) == true)
			{
				navMeshAgent.enabled = false;
				Debug.DrawRay(playerPosition + temp + Vector3.up, Vector3.down, Color.red, 3f);
				if (Physics.Raycast(playerPosition + temp + Vector3.up, Vector3.down, out RaycastHit rayhit))
				{
					// navMeshAgent.nextPosition = rayhit.point;
					alliancePool[i].transform.position = rayhit.point;
				}
				if (navMeshAgent.isOnNavMesh == false)
				{
					Debug.Log($"{alliancePool[i].gameObject.name} is not on navmesh");
				}
				navMeshAgent.enabled = true;
			}
		}
	}

	public void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData = null, Vector3? aimPosition = null)
	{
	}
}
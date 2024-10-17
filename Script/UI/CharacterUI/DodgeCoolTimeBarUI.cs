using UnityEngine;
using UnityEngine.UI;

public class DodgeCoolTimeBarUI : MonoBehaviour
{
	[SerializeField] Player player;
	[SerializeField] Image foreground;
	private void Awake()
	{
		Debug.Assert(foreground != null, "[DodgeCoolTimeBarUI] Image not found");
		Debug.Assert(player != null, "[DodgeCoolTimeBarUI] player not found");
	}

	private void Update()
	{
		foreground.fillAmount = player.movementStateMachine.RemainDodgeEnergy / 100f;
		if (foreground.fillAmount < 0.5f)
		{
			foreground.color = Color.gray;
		}
		else
		{
			foreground.color = Color.white;
		}
	}
}



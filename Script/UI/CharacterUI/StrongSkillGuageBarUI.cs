using UnityEngine;
using UnityEngine.UI;
public class StrongSkillGuageBarUI : MonoBehaviour
{
	private SkillHandler skillHandler;
	[SerializeField] Image foreground;
	[SerializeField] Player player;
	private void Awake()
	{
		Debug.Assert(player != null, "[StrongSkillGuageBarUI] player not found");
		Debug.Assert(foreground != null, "[StrongSkillGuageBarUI] Image not found");
		skillHandler = player.GetComponent<SkillHandler>();
		Debug.Assert(skillHandler != null, "[StrongSkillGuageBarUI] skillHandler not found");
	}

	private void Start()
	{
		foreground.fillAmount = 0;
		skillHandler.OnStrongGuageChanged += UpdateStrongAttackGuage;
	}

	private void UpdateStrongAttackGuage()
	{
		foreground.fillAmount = skillHandler.StrongSkillGauge;
	}
}

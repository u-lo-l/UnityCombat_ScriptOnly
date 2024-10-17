using UnityEngine;
using UnityEngine.UI;

public class FastSkillGuageBarUI : MonoBehaviour
{
	private SkillHandler skillHandler;
	[SerializeField] Image foreground;
	[SerializeField] Character player;
	private void Awake()
	{
		Debug.Assert(player != null, "[FastSkillGuageBarUI] player not found");
		skillHandler = player.GetComponent<SkillHandler>();
		Debug.Assert(skillHandler != null, "[FastSkillGuageBarUI] skillHandler not found");
	}

	private void Start()
	{
		foreground.fillAmount = 0;
		skillHandler.OnFastGuageChanged += UpdateFastAttackGuage;
	}

	private void UpdateFastAttackGuage()
	{
		foreground.fillAmount = skillHandler.FastSkillGauge;
	}
}

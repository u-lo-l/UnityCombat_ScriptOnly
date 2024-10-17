using UnityEngine;
using UnityEngine.UI;

public class CreatureLifetimeBarUI : MonoBehaviour
{
	[SerializeField] LifeTimeComponent lifeTimeComponent;
	[SerializeField] Image foreground;
	private void Update()
	{
		foreground.fillAmount = lifeTimeComponent.RemainLifetimeRate;
	}
}

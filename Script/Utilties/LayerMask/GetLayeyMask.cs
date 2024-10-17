using UnityEngine;

public static class GetLayerMask
{
	public static LayerMask GetPlayerLayer => LayerMask.NameToLayer("Player");
	public static LayerMask GetAllyLayer => LayerMask.NameToLayer("Ally");
	public static LayerMask GetObstacleLayer => LayerMask.NameToLayer("Obstacle");
	public static LayerMask GetEnemyLayer => LayerMask.NameToLayer("Enemy");
	public static LayerMask GetGhoastLayer => LayerMask.NameToLayer("Ghoast");
	public static LayerMask GetWeaponLayer => LayerMask.NameToLayer("Weapon");

	public static LayerMask GetPlayerLayerMask => 1 << LayerMask.NameToLayer("Player");
	public static LayerMask GetAllyLayerMask => 1 << LayerMask.NameToLayer("Ally");
	public static LayerMask GetObstacleLayerMask => 1 << LayerMask.NameToLayer("Obstacle");
	public static LayerMask GetEnemyLayerMask => 1 << LayerMask.NameToLayer("Enemy");
	public static LayerMask GetGhoastLayerMask => 1 << LayerMask.NameToLayer("Ghoast");
	public static LayerMask GetWeaponLayerMask => 1 << LayerMask.NameToLayer("Weapon");
}
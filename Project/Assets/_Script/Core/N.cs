using UnityEngine;
using System.Collections;

public class N : MonoBehaviour
{
	#region Gears notifications
	public const string OnGameInput___				= "on.game.input";
	#endregion

	#region Obstacle notifications
	public const string ObstacleInvisible			= "obstacle.invisible";
	#endregion

	#region Game notifications
	public const string OnStart						= "on.start";
	public const string GamePlay					= "game.play";
	public const string GamePause					= "game.pause";
	public const string GameOver					= "game.over";

	/*
	public const string GamePlayerGetScoreItem		= "game.player.get.score_item";
	public const string GamePlayerPlacedOnRoad		= "game.player.placed_on_road";

	public const string GameAddScore				= "game.add.score";
	public const string GameRoadChangeStart__		= "game.road.change.start";
	public const string GameRoadChangeEnd 			= "game.road.change.end";
	public const string GameRoadsPlaced				= "game.roads.placed"*/
	#endregion

	#region Destructible notifications
	public const string DestructibleBreakEntity___ 	= "destructible.break.entity";
	#endregion

	#region ResourcesController notifications
	public const string RCStartLoad					= "rc.start.load";
	#endregion
}

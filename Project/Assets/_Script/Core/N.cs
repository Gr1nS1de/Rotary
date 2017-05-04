using UnityEngine;
using System.Collections;

public class N : MonoBehaviour
{
	#region Gears notifications
	public const string OnInputGear___				= "on.input.gear";
	public const string UpdateGearsChain			= "update.gears_chain";
	public const string OnGearsChainStuck_			= "on.gears_chain.stuck";
	public const string OnGearSelected_				= "on.gear.selected";
	public const string OnGearDeselected_			= "on.gear.deselected";
	public const string OnCurrentGearError_			= "on.current_gear.error";
	public const string OnGearsConnected__			= "on.gears.connected";
	//public const string OnConnectGears__			= "on.connect.gears";
	//public const string OnDisconnectGears__		= "on.disconnect.gears";

	#endregion

	#region Obstacle notifications
	public const string ObstacleInvisible			= "obstacle.invisible";
	#endregion

	#region Game notifications
	public const string GameOnStart					= "game.on.start";
	public const string GamePlayLevel_				= "game.play.level";
	public const string GamePause					= "game.pause";
	public const string GameOver					= "game.over";

	public const string GearsColliderTriggered_____	= "gears_collider.triggered";
	public const string StartGenerateLevel			= "start.generate.level";
	/*
	public const string GamePlayerGetScoreItem		= "game.player.get.score_item";
	public const string GamePlayerPlacedOnRoad		= "game.player.placed_on_road";

	public const string GameAddScore				= "game.add.score";
	public const string GameRoadChangeStart__		= "game.road.change.start";
	public const string GameRoadChangeEnd 			= "game.road.change.end";
	public const string GameRoadsPlaced				= "game.roads.placed"*/
	#endregion

	#region UI notifications
	public const string UIClickedStart 				= "ui.clicked.start";
	#endregion

	#region Destructible notifications
	public const string DestructibleBreakEntity___ 	= "destructible.break.entity";
	#endregion

	#region ResourcesController notifications
	public const string RCStartLoad					= "rc.start.load";
	public const string RCResetRoadModelTemplate	= "rc.reset.road.model.template";
	#endregion
}

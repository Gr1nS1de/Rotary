using UnityEngine;
using System.Collections;

public class N : MonoBehaviour
{
	#region Gears notifications
	public const string OnGameInput___				= "on.game.input";
	#endregion

	#region Game notifications
	public const string OnStart						= "on.start";
	public const string GamePlay					= "game.play";
	public const string GamePause					= "game.pause";
	public const string GameOver					= "game.over";
	public const string GameAddScore				= "game.add.score";

	public const string OnPlatformInvisible_		= "on.platform.invisible";
	public const string OnItemInvisible_			= "on.item.invisible";
	public const string OnPlayerInvisible			= "on.player.invisible";
	#endregion

	#region player notifications
	//public const string DestructibleBreakEntity___ 	= "destructible.break.entity";
	public const string PlayerImpactItem__ 			= "player.impact.item";
	public const string PlayerLeftPlatform_			= "player.left.platform";
	#endregion

	#region ResourcesController notifications
	public const string RCAwakeLoad					= "rc.awake.load";
	public const string RCLoadGameTheme_			= "rc.load.game_theme";
	#endregion
}

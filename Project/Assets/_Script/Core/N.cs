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

	public const string AddObjectToPool__			= "add.to.object.pool";
	public const string PoolObject____ 				= "pool.object";

	public const string OnPlatformInvisible_		= "on.platform.invisible";

	#endregion

	#region Destructible notifications
	public const string DestructibleBreakEntity___ 	= "destructible.break.entity";
	#endregion

	#region ResourcesController notifications
	public const string RCAwakeLoad					= "rc.awake.load";
	public const string RCLoadGameTheme_			= "rc.load.game_theme";
	#endregion
}

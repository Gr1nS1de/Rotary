using UnityEngine;
using System.Collections;

public class N : MonoBehaviour
{
	#region Input notifications
	public const string OnPlatformInput___			= "on.platform.input";
	public const string OnDoubleTapInput			= "on.double.tap.input";
	#endregion

	#region Purchase notifications
	public const string PurchaseDoubleCoin			= "purchase.double.coin";
	#endregion

	#region Game notifications
	public const string OnStart						= "on.start";
	public const string GameStart					= "game.start";
	public const string GamePause					= "game.pause";
	public const string GameContinue				= "game.continue";
	public const string GameOver_					= "game.over";
	public const string GameAddScore				= "game.add.score";

	public const string OnPlatformInvisible_		= "on.platform.invisible";
	public const string OnItemInvisible_			= "on.item.invisible";
	public const string OnPlayerInvisible			= "on.player.invisible";

	public const string OnPlayerNewRecord_			= "on.player.new.record";

	public const string GameThemeChanged_			= "game.theme.changed";
	#endregion

	#region Menu notifications
	public const string UIThemeChanged_				= "ui.theme.changed";
	public const string UIStateChanged_				= "ui.state.changed";

	public const string CenterButtonPressed_ 		= "center.button.pressed";
	public const string RightButtonPressed_ 		= "right.button.pressed";
	#endregion

	#region player notifications
	//public const string DestructibleBreakEntity___ 	= "destructible.break.entity";
	public const string PlayerImpactItem__ 			= "player.impact.item";
	public const string PlayerLeftPlatform_			= "player.left.platform";
	#endregion

	#region Resources notifications
	public const string RCAwakeLoad					= "rc.awake.load";
	public const string RCLoadGameTheme_			= "rc.load.game_theme";
	#endregion
}

using UnityEngine;
using System.Collections.Generic;
//using Destructible2D;

public class ResourcesController : Controller
{
	private RCModel 	_RCModel				{ get { return game.model.RCModel; } }

	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.RCAwakeLoad:
				{

					break;
				}

			case N.RCLoadGameTheme_:
				{
					GameThemeType gameThemeType = (GameThemeType)data [0];

					LoadGameTheme (gameThemeType);
					break;
				}
		}
	}

	private void LoadGameTheme(GameThemeType gameThemeType)
	{
		GameTheme gameTheme = new GameTheme 
		{
			GameThemeType = gameThemeType
			//BackgroundView = 

		};
		GM.Instance.SetGameTheme (gameTheme);
	}

}

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
					LoadItems ();
					break;
				}

			case N.RCLoadGameTheme_:
				{
					GameThemeType gameThemeType = (GameThemeType)data [0];

					LoadGameTheme (gameThemeType);
					LoadMenuTheme (gameThemeType);
					break;
				}
		}
	}

	private void LoadItems()
	{
		game.model.itemsFactoryModel.itemsPrefabsList.AddRange( Resources.LoadAll<ItemView> ("Items") );
	}

	private void LoadGameTheme(GameThemeType gameThemeType)
	{
		BackgroundView backgroundView = Resources.LoadAll<BackgroundView> (string.Format ("Theme/({0:00)_{1}", (int)gameThemeType, gameThemeType))[0];
		List<PlatformView> platformsViewList = new List<PlatformView>( Resources.LoadAll<PlatformView> (string.Format ("Theme/({0:00)_{1}", (int)gameThemeType, gameThemeType)));

		//Debug.LogErrorFormat("backgroundView = {0}. platformView = {1}. load resources path = {2}",backgroundView != null, platformsViewList != null, string.Format ("Theme/_{0}", gameThemeType) );
		GameTheme gameTheme = new GameTheme 
		{
			GameThemeType = gameThemeType,
			BackgroundView = backgroundView,
			PlatformsViewList = platformsViewList,
		};

		game.controller.SetGameTheme (gameTheme);

		backgroundView = null;
		platformsViewList = null;
	}

	private void LoadMenuTheme(GameThemeType gameThemeType)
	{
		BackgroundView backgroundView = Resources.LoadAll<BackgroundView> (string.Format ("Theme/({0:00)_{1}", (int)gameThemeType, gameThemeType))[0];
		List<PlatformView> platformsViewList = new List<PlatformView>( Resources.LoadAll<PlatformView> (string.Format ("Theme/({0:00)_{1}", (int)gameThemeType, gameThemeType)));

		//Debug.LogErrorFormat("backgroundView = {0}. platformView = {1}. load resources path = {2}",backgroundView != null, platformsViewList != null, string.Format ("Theme/_{0}", gameThemeType) );
		GameTheme gameTheme = new GameTheme 
		{
			GameThemeType = gameThemeType,
			BackgroundView = backgroundView,
			PlatformsViewList = platformsViewList,
		};

		game.controller.SetGameTheme (gameTheme);

	}
}

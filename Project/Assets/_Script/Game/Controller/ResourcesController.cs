﻿using UnityEngine;
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
		Resources.UnloadUnusedAssets ();

		BackgroundView backgroundView = Resources.LoadAll<BackgroundView> (string.Format ("Theme/_{0}", gameThemeType))[0];
		PlatformView platformView = Resources.LoadAll<PlatformView> (string.Format ("Theme/_{0}", gameThemeType))[0];
		Debug.LogErrorFormat("backgroundView = {0}. platformView = {1}. load resources path = {2}",backgroundView != null, platformView != null, string.Format ("Theme/_{0}", gameThemeType) );
		GameTheme gameTheme = new GameTheme 
		{
			GameThemeType = gameThemeType,
			BackgroundView = backgroundView,
			PlatformView = platformView
		};

		GM.Instance.SetGameTheme (gameTheme);
	}

}
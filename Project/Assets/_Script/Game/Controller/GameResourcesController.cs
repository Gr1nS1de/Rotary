﻿using UnityEngine;
using System.Collections.Generic;
//using Destructible2D;

public class GameResourcesController : Controller
{

	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.RCAwakeLoad:
				{
					LoadRockets ();
					break;
				}

			case N.RCLoadGameTheme_:
				{
					GameThemeType gameThemeType = (GameThemeType)data [0];

					LoadGameTheme (gameThemeType);
					LoadMenuTheme (gameThemeType);

					Resources.UnloadUnusedAssets ();
					break;
				}
		}
	}
		
	private void LoadRockets()
	{
		game.model.rocketsFactoryModel.rocketsPrefabsList.AddRange( Resources.LoadAll<RocketView> ("Rockets") );

	}

	private void LoadGameTheme(GameThemeType gameThemeType)
	{
		BackgroundView backgroundView = Resources.LoadAll<BackgroundView> (string.Format ("Theme/{0:00}_{1}", (int)gameThemeType, gameThemeType))[0];
		List<PlatformView> platformsViewList = new List<PlatformView>( Resources.LoadAll<PlatformView> (string.Format ("Theme/{0:00}_{1}", (int)gameThemeType, gameThemeType)));


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
		//Debug.LogErrorFormat ("Loaded path: {0}. sprites: {1}",string.Format ("Theme/{0:00}_{1}/RightButtons", (int)gameThemeType, gameThemeType) , Resources.LoadAll<Sprite> (string.Format ("Theme/{0:00)_{1}/RightButtons", (int)gameThemeType, gameThemeType)).Length);

		Sprite rightButtonSprite = Resources.LoadAll<Sprite> (string.Format ("Theme/{0:00}_{1}/RightButtons", (int)gameThemeType, gameThemeType))[0];
		List<Sprite> centerButtonsSprites = new List<Sprite>( Resources.LoadAll<Sprite> (string.Format ("Theme/{0:00}_{1}/CenterButtons", (int)gameThemeType, gameThemeType)));
		UIThemeModel themeModel = Resources.LoadAll<UIThemeModel> (string.Format ("Theme/{0:00}_{1}", (int)gameThemeType, gameThemeType))[0];

		UITheme menuTheme = new UITheme
		{
			GameThemeType = gameThemeType,
		};

		menuTheme.RightButtonsBGSPrite = rightButtonSprite;

		foreach (Sprite centerButton in centerButtonsSprites)
		{
			if (centerButton.name.Contains ("Play"))
				menuTheme.PlayButtonBGSprite = centerButton;
			else 
				if (centerButton.name.Contains ("CenterButton"))
					menuTheme.CenterButtonsBGSprite = centerButton;
				else if(centerButton.name.Contains("CenterButtonHalf"))
					menuTheme.CenterButtonHalfBGSPrite = centerButton;
		}

		menuTheme.IconsColor = themeModel.IconsColor;
		menuTheme.BackgroundColor = themeModel.BackgroundColor;

		ui.controller.SetMenuTheme (menuTheme);

	}
}

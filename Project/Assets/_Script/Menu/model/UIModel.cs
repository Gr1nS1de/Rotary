using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum UIState
{
	MAIN_MENU,
	PLAYING,
	PAUSE,
	GAME_OVER,
	SHOP
}

[System.Serializable]
public struct MenuTheme
{
	public GameThemeType GameThemeType;
	public Color IconsColor;
}

public class UIModel : Model
{

	#region UI Model
	public UIState					uiState				{ get { return _uiState; } 		set { _uiState 	= value; } }
	public MenuTheme 				menuTheme			{ get { return _menuTheme; } 	set { _menuTheme = value;} }

	[SerializeField]
	private UIState					_uiState 			= UIState.MAIN_MENU;
	[SerializeField]
	private MenuTheme				_menuTheme;
	#endregion

	#region Declare models reference
	public UIGameModel			UIGameModel				{ get { return _UIGameModel 		= SearchLocal<UIGameModel>(			_UIGameModel,		typeof(UIGameModel).Name);	} }
	public MainMenuModel		UIMainMenuModel			{ get { return _UIMainMenuModel		= SearchLocal<MainMenuModel>(		_UIMainMenuModel,	typeof(MainMenuModel).Name);	} }

	private UIGameModel			_UIGameModel;
	private MainMenuModel		_UIMainMenuModel;
	#endregion
}


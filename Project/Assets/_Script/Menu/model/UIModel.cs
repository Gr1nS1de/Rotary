using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum UIState
{
	MainMenu,
	InGame,
	Pause,
	GameOver,
	Store,
	Settings,
	PlayerSkin,
	Like,
	DailyGift,
	Achievements
}

[System.Serializable]
public struct UITheme
{
	public GameThemeType GameThemeType;
	public Color IconsColor;
	public Sprite PlayButtonBGSprite;
	public Sprite CenterButtonsBGSprite;
	public Sprite RightButtonsBGSPrite;
}

public class UIModel : Model
{

	#region UI Model
	public UIState				uiState				{ get { return _uiState; } 		set { _uiState 	= value; } }
	public UITheme 				menuTheme			{ get { return _menuTheme; } 	set { _menuTheme = value;} }
	public CanvasGroup 			mainMenuWindow		{ get { return _mainMenuWindow; } }
	public CanvasGroup 			storeWindow			{ get { return _storeWindow; } }
	public CanvasGroup 			settingsWindow		{ get { return _settingsWindow; } }
	public CanvasGroup 			likeWindow			{ get { return _likeWindow; } }
	public CanvasGroup 			dailyGiftWindow		{ get { return _dailyGiftWindow; } }
	public CanvasGroup 			achievementsWindow	{ get { return _achievementsWindow; } }
	public CanvasGroup 			playerSkinWindow	{ get { return _playerSkinWindow; } }
	public CanvasGroup 			inGameWindow		{ get { return _inGameWindow; } }
	public CanvasGroup 			pauseWindow			{ get { return _pauseWindow; } }
	public CanvasGroup 			gameOverWindow		{ get { return _gameOverWindow; } }

	[SerializeField]
	private CanvasGroup			_gameOverWindow;
	[SerializeField]
	private CanvasGroup			_pauseWindow;
	[SerializeField]
	private CanvasGroup			_inGameWindow;
	[SerializeField]
	private CanvasGroup			_playerSkinWindow;
	[SerializeField]
	private CanvasGroup			_achievementsWindow;
	[SerializeField]
	private CanvasGroup			_dailyGiftWindow;
	[SerializeField]
	private CanvasGroup			_likeWindow;
	[SerializeField]
	private CanvasGroup			_settingsWindow;
	[SerializeField]
	private CanvasGroup			_storeWindow;
	[SerializeField]
	private CanvasGroup			_mainMenuWindow;
	[SerializeField]
	private UIState				_uiState 			= UIState.MainMenu;
	[SerializeField]
	private UITheme				_menuTheme;
	#endregion

	#region Declare models reference
	public UIGameModel			UIGameModel				{ get { return _UIGameModel 		= SearchLocal<UIGameModel>(			_UIGameModel,		typeof(UIGameModel).Name);	} }
	public MainMenuModel		UIMainMenuModel			{ get { return _UIMainMenuModel		= SearchLocal<MainMenuModel>(		_UIMainMenuModel,	typeof(MainMenuModel).Name);	} }

	private UIGameModel			_UIGameModel;
	private MainMenuModel		_UIMainMenuModel;
	#endregion
}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum UIPanelState
{
	MainMenu,
	InGame
}

public enum UIWindowState
{
	MainMenu,
	Pause,
	GameOver,
	Store,
	Settings,
	PlayerSkin,
	Like,
	DailyGift,
	Achievements,
	GS_Achievements,
	GS_Leaderboard
}

[System.Serializable]
public struct UITheme
{
	public GameThemeType GameThemeType;
	public Color IconsColor;
	public Sprite PlayButtonBGSprite;
	public Sprite CenterButtonsBGSprite;
	public Sprite RightButtonsBGSPrite;
	public Sprite CenterButtonHalfBGSPrite;
}

public class UIModel : Model
{

	#region UI Model
	public UIPanelState			uiMainState				{ get { return _uiPanelState; } 	set { _uiPanelState 	= value;} }
	public UITheme 				menuTheme				{ get { return _menuTheme; } 		set { _menuTheme 		= value;} }
	public UIWindowState		uiWindowState			{ get { return _uiWindowState; } 	set { _uiWindowState 	= value; } }
	public CanvasGroup 			mainMenuWindow			{ get { return _mainMenuWindow; } }
	public CanvasGroup 			storeWindow				{ get { return _storeWindow; } }
	public CanvasGroup 			settingsWindow			{ get { return _settingsWindow; } }
	public CanvasGroup 			likeWindow				{ get { return _likeWindow; } }
	public CanvasGroup 			dailyGiftWindow			{ get { return _dailyGiftWindow; } }
	public CanvasGroup 			achievementsWindow		{ get { return _achievementsWindow; } }
	public CanvasGroup 			playerSkinWindow		{ get { return _playerSkinWindow; } }
	public CanvasGroup 			pauseWindow				{ get { return _pauseWindow; } }
	public CanvasGroup 			gameOverWindow			{ get { return _gameOverWindow; } }
	public CanvasGroup 			inGamePanel				{ get { return _inGamePanel; } }
	public CanvasGroup 			mainMenuPanel			{ get { return _mainMenuPanel; } }
	public CanvasGroup 			rightElementsPanel		{ get { return _rightElementsPanel; } }

	[SerializeField]
	private CanvasGroup			_rightElementsPanel;
	[SerializeField]
	private CanvasGroup			_mainMenuPanel;
	[SerializeField]
	private CanvasGroup			_inGamePanel;
	[SerializeField]
	private CanvasGroup			_gameOverWindow;
	[SerializeField]
	private CanvasGroup			_pauseWindow;
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
	private UIWindowState		_uiWindowState 			= UIWindowState.MainMenu;
	[SerializeField]
	private UITheme				_menuTheme;
	[SerializeField]
	private UIPanelState		_uiPanelState			= UIPanelState.MainMenu;
	#endregion

	#region Declare models reference
	public InGamePanelModel			inGamePanelModel			{ get { return _inGamePanelModel 		= SearchLocal<InGamePanelModel>(		_inGamePanelModel,		typeof(InGamePanelModel).Name);	} }
	public MainMenuPanelModel		mainMenuPanelModel			{ get { return _mainMenuPanelModel		= SearchLocal<MainMenuPanelModel>(		_mainMenuPanelModel,	typeof(MainMenuPanelModel).Name);	} }

	private MainMenuPanelModel		_mainMenuPanelModel;
	private InGamePanelModel		_inGamePanelModel;
	#endregion
}


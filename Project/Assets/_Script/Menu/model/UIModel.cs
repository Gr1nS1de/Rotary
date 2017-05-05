using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum UIState
{
	MAIN_MENU,
	PLAYING,
	PAUSE,
	GAME_OVER,
	SHOP
}

public class UIModel : Model
{

	#region UI Model
	public UIState					uiState				{ get { return _uiState; } 		set { _uiState 	= value; } }

	[SerializeField]
	private UIState					_uiState 			= UIState.MAIN_MENU;
	#endregion

	#region Declare models reference
	public UIGameModel			UIGameModel				{ get { return _UIGameModel 		= SearchLocal<UIGameModel>(			_UIGameModel,		typeof(UIGameModel).Name);	} }
	public UIMainMenuModel		UIMainMenuModel			{ get { return _UIMainMenuModel		= SearchLocal<UIMainMenuModel>(		_UIMainMenuModel,	typeof(UIMainMenuModel).Name);	} }

	private UIGameModel			_UIGameModel;
	private UIMainMenuModel		_UIMainMenuModel;
	#endregion
}


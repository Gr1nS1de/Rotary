using UnityEngine;
using System.Collections;

public class RightButtonsController : Controller
{
	public System.Action<UITheme> ActionUIThemeChanged = delegate{};

	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();
					break;
				}

			case N.RightButtonPressed_:
				{
					RightElementId rightElementId = (RightElementId)data [0];

					OnChangeWindowState (rightElementId);
					break;
				}

			case N.UIThemeChanged_:
				{
					UITheme uiTheme = (UITheme)data [0];

					ActionUIThemeChanged (uiTheme);
					break;
				}

			case N.UIStateChanged_:
				{
					UIState uiState = (UIState)data [0];


					break;
				}
		}
	}

	private void OnStart()
	{

	}

	private void OnChangeWindowState(RightElementId rightElementId)
	{
		UIState uiState = UIState.MainMenu;

		switch (rightElementId)
		{
			case RightElementId.ButtonStore:
				{
					uiState = UIState.Store;
					break;
				}

			case RightElementId.ButtonSettings:
				{
					uiState = UIState.Settings;
					break;
				}

			case RightElementId.ButtonPlayerSkin:
				{
					uiState = UIState.PlayerSkin;
					break;
				}

			case RightElementId.ButtonLike:
				{
					uiState = UIState.Like;
					break;
				}

			case RightElementId.ButtonLeaderboard:
				{
					
					break;
				}

			case RightElementId.ButtonBack:
				{
					
					break;
				}

			case RightElementId.ButtonAchievements:
				{
					uiState = UIState.Achievements;
					break;
				}
		}

		ui.controller.GoToState (uiState);
	}
}


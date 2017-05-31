using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RightButtonsController : Controller
{
	public System.Action<UITheme> ActionUIThemeChanged = delegate{};

	private Stack<UIWindowState> _backButtonStack = new Stack<UIWindowState> ();

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

					OnButtonPressed (rightElementId);
					break;
				}

			case N.UIThemeChanged_:
				{
					UITheme uiTheme = (UITheme)data [0];

					//Send theme changed event to all RightElenentView
					ActionUIThemeChanged (uiTheme);
					break;
				}

			case N.UIStateChanged_:
				{
					UIWindowState uiState = (UIWindowState)data [0];

					UpdateBackStack (uiState);
					UpdateRightButtons (uiState);
					break;
				}
		}
	}

	private void OnStart()
	{
		UpdateRightButtons (ui.model.uiWindowState);
	}

	private void OnButtonPressed(RightElementId rightElementId)
	{
		UIWindowState uiState = UIWindowState.MainMenu;

		switch (rightElementId)
		{
			case RightElementId.ButtonStore:
				{
					uiState = UIWindowState.Store;
					break;
				}

			case RightElementId.ButtonSettings:
				{
					uiState = UIWindowState.Settings;
					break;
				}

			case RightElementId.ButtonPlayerSkin:
				{
					uiState = UIWindowState.PlayerSkin;
					break;
				}

			case RightElementId.ButtonLike:
				{
					uiState = UIWindowState.Like;
					break;
				}

		#region Google Play Services
			case RightElementId.ButtonGSAchievements:
				{
					uiState = UIWindowState.GS_Achievements;
					break;
				}

			case RightElementId.ButtonLeaderboard:
				{
					uiState = UIWindowState.GS_Leaderboard;
					break;
				}
		#endregion

			case RightElementId.ButtonBack:
				{
					OnBack ();

					if(_backButtonStack.Count > 0)
						uiState = _backButtonStack.Peek ();
					break;
				}
		}

		ui.controller.GoToWindowState (uiState);
	}

	private void UpdateRightButtons(UIWindowState uiState)
	{
		switch (uiState)
		{
			case UIWindowState.MainMenu:
				{
					Enable4Buttons (RightElementId.ButtonSettings, RightElementId.ButtonPlayerSkin, RightElementId.ButtonLike, RightElementId.ButtonGameServices);
					break;
				}

			case UIWindowState.Settings:
				{
					Enable4Buttons (RightElementId.ButtonBack, RightElementId.ButtonPlayerSkin, RightElementId.ButtonLike, RightElementId.ButtonStore);
					break;
				}

			case UIWindowState.PlayerSkin:
				{
					Enable4Buttons (RightElementId.ButtonBack, RightElementId.ButtonSettings, RightElementId.ButtonLike, RightElementId.ButtonStore);
					break;
				}

			case UIWindowState.Like:
				{
					Enable4Buttons (RightElementId.ButtonBack, RightElementId.ButtonPlayerSkin, RightElementId.ButtonStore, RightElementId.ButtonSettings);
					break;
				}

			case UIWindowState.Store:
				{
					Enable4Buttons (RightElementId.ButtonBack, RightElementId.ButtonPlayerSkin, RightElementId.ButtonLike, RightElementId.ButtonSettings);
					break;
				}
		}
	}

	private void Enable4Buttons(RightElementId button1, RightElementId button2, RightElementId button3, RightElementId button4)
	{
		foreach (RightElementView rightElementView in ui.view.rightElementsArray)
		{
			if (rightElementView.ElementId == button1 || rightElementView.ElementId == button2 || rightElementView.ElementId == button3 || rightElementView.ElementId == button4)
				SetButtonActive (rightElementView, true);
			else
				SetButtonActive (rightElementView, false);
		}
	}

	private void SetButtonActive(RightElementView rightElementView, bool isActive)
	{
		if (isActive)
		{
			if (!rightElementView.gameObject.activeInHierarchy)
				rightElementView.gameObject.SetActive (isActive);
		}
		else
		{
			if (rightElementView.gameObject.activeInHierarchy )
				rightElementView.gameObject.SetActive (isActive);
		}
	}

	private void OnBack()
	{
		if (_backButtonStack.Count <= 1)
		{
			ClearBackStack ();
			ui.controller.GoToWindowState (UIWindowState.MainMenu);
		}
		else if (_backButtonStack.Count > 1)
		{
			_backButtonStack.Pop ();
		}

	}

	private void UpdateBackStack(UIWindowState uiState)
	{

		switch (uiState)
		{
			case UIWindowState.Store:
			case UIWindowState.Settings:
			case UIWindowState.PlayerSkin:
			case UIWindowState.Like:
				{
					if (_backButtonStack.Count == 0)
					{
						PushToBackStack (uiState);
					}
					else if (_backButtonStack.Count == 1)
					{
						PushToBackStack (uiState, true);
					}
					else
					{
						ClearBackStack ();
						PushToBackStack (uiState);
					}
					break;
				}

			case UIWindowState.Achievements:
				{
					PushToBackStack (uiState);
					break;
				}

			case UIWindowState.DailyGift:
				{
					PushToBackStack (uiState);
					break;
				}

			default:
				{
					break;
				}
		}
	}

	private void PushToBackStack (UIWindowState uiState, bool isPopLast = false)
	{
		if (isPopLast)
		{
			_backButtonStack.Pop();
		}

		_backButtonStack.Push (uiState);
	}

	private void ClearBackStack()
	{
		_backButtonStack.Clear ();
	}
}


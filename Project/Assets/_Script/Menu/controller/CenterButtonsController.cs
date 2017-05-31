using UnityEngine;
using System.Collections;

public class CenterButtonsController : Controller
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

			case N.CenterButtonPressed_:
				{
					CenterElementId centerElementId = (CenterElementId)data [0];

					OnButtonPressed (centerElementId);
					break;
				}

			case N.UIThemeChanged_:
				{
					UITheme uiTheme = (UITheme)data [0];

					ActionUIThemeChanged (uiTheme);
					break;
				}
		}
	}

	private void OnStart()
	{

	}

	private void OnButtonPressed(CenterElementId centerElementId)
	{
		switch (centerElementId)
		{
			case CenterElementId.Main_ButtonPlay:
				{
					Notify (N.GameStart);

					break;
				}

			case CenterElementId.Settings_ButtonCoinStore:
				{
					ui.controller.GoToWindowState (UIWindowState.Store);
					break;
				}

			case CenterElementId.Like_ButtonDailyGift:
				{
					ui.controller.GoToWindowState (UIWindowState.DailyGift);
					break;
				}

			case CenterElementId.Like_ButtonRate:
				{
					
					break;
				}

			case CenterElementId.Settings_ButtonLanguage:
				{
					break;
				}

			case CenterElementId.Settings_ButtonMusic:
				{
					
					break;
				}

			case CenterElementId.Settings_ButtonSFX:
				{
					break;
				}

			case CenterElementId.Store_ButtonCoinsPack_00:
				{
					break;
				}

			case CenterElementId.Store_ButtonCoinsPack_01:
				{
					break;
				}

			case CenterElementId.Store_ButtonDoubleCoin:
				{
					break;
				}
		}
	}
}


using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuPanelController : Controller
{
	private MainMenuPanelModel 		_mainMenuPanelModel			{ get { return ui.model.MainMenuPanelModel; } }

	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();

					break;
				}

			case N.GameAddScore:
				{
					break;
				}

			case N.GameOver_:
				{
					//GameOverData gameOverData = (GameOverData)data[0];

					OnGameOver ();
					break;
				}
		}
	}

	private void OnStart()
	{
		_mainMenuPanelModel.textCoinsCount.text = string.Format("{0}", Prefs.PlayerData.GetCoinsCount ());
		_mainMenuPanelModel.textCrystalsCount.text = string.Format("{0}", Prefs.PlayerData.GetCrystalsCount ());
	}

	private void OnGameOver()
	{
		
	}
}

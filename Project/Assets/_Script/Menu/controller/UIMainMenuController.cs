using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIMainMenuController : Controller
{
	private MainMenuModel 		_uiMainMenuModel			{ get { return ui.model.UIMainMenuModel; } }

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
					//_uiMainMenuModel.bestScoreText.text = game.model.currentScore.ToString();
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

	}

	private void OnGameOver()
	{
		SetActivePlayButton (true);
	}

	private void SetActivePlayButton(bool isActive)
	{

	}
}

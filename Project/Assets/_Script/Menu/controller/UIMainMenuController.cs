using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIMainMenuController : Controller
{
	private UIMainMenuModel 		_uiMainMenuModel			{ get { return ui.model.UIMainMenuModel; } }

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
					_uiMainMenuModel.bestScoreText.text = game.model.currentScore.ToString();
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

	#region Public methods
	public void ButtonGamePlay()
	{
		Notify (N.GameStartPlay);

		SetActivePlayButton (false);
	}
	#endregion

	private void OnGameOver()
	{
		SetActivePlayButton (true);
	}

	private void SetActivePlayButton(bool isActive)
	{
		Button playButton = ui.model.UIMainMenuModel.buttonPlay;

		playButton.gameObject.SetActive (isActive);
	}
}

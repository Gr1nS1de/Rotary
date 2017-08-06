using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GameSpeedController : Controller
{
	private GameSpeedState _currentGameSpeedState = GameSpeedState.NotDefined;
	private Tween _speedIncreasinTween = null;

	public override void OnNotification( string alias, Object target, params object[] data )
	{
		switch ( alias )
		{
			case N.OnStart:
				{
					OnStart();
					break;
				}

			case N.GamePlay:
				{
					SetGameSpeed (GameSpeedState.Speed_1);
					break;
				}

			case N.GameAddScore:
				{
					CheckGameSpeedState ();
					break;
				}
		}
	}

	private void OnStart()
	{
		SetGameSpeed( game.model.gameSpeedState);
	}

	private void CheckGameSpeedState()
	{
		int currentScore = core.playerDataModel.currentScore;
		GameSpeedState correctGameSpeedState = GameSpeedState.Speed_1;

		if (currentScore < 5)
		{
			//
		}
		else if (currentScore >= 5 && currentScore < 10)
		{
			correctGameSpeedState = GameSpeedState.Speed_2;
		}
		else if (currentScore >= 10 && currentScore < 20)
		{
			correctGameSpeedState = GameSpeedState.Speed_3;
		}
		else if (currentScore >= 20 && currentScore < 30)
		{
			correctGameSpeedState = GameSpeedState.Speed_4;
		}
		else if (currentScore >= 30 && currentScore < 40)
		{
			correctGameSpeedState = GameSpeedState.Speed_5;
		}
		else if (currentScore >= 40 && currentScore < 50)
		{
			correctGameSpeedState = GameSpeedState.Speed_6;
		}
		else if (currentScore >= 50)
		{
			correctGameSpeedState = GameSpeedState.Speed_7;
		}

		if (game.model.gameSpeedState != correctGameSpeedState)
		{
			SetGameSpeed (correctGameSpeedState);
		}
	}

	//https://ussrgames.atlassian.net/wiki/pages/viewpage.action?pageId=40027034
	public void SetGameSpeed(GameSpeedState speedState)
	{
		switch (speedState)
		{
			case GameSpeedState.Speed_1:
				{
					game.model.gameSpeed = 4.5f;
					game.model.platformsFactoryModel.verticalPlatformsGap = Random.Range(1.3f, 1.5f);

					DestroyIncreasingSpeedTween ();
					break;
				}

			case GameSpeedState.Speed_2:
				{
					_speedIncreasinTween = DOVirtual.Float (game.model.gameSpeed, 5f, 3f, (speed ) =>
					{
						game.model.gameSpeed = speed;
					});

					game.model.platformsFactoryModel.verticalPlatformsGap = Random.Range(1.1f, 1.3f);
					break;
				}

			case GameSpeedState.Speed_3:
				{
					_speedIncreasinTween = DOVirtual.Float (game.model.gameSpeed, 6f, 2f, (speed ) =>
					{
						game.model.gameSpeed = speed;
					});

					game.model.platformsFactoryModel.verticalPlatformsGap = Random.Range(0.8f, 1.1f);
					break;
				}

			case GameSpeedState.Speed_4:
				{
					_speedIncreasinTween = DOVirtual.Float (game.model.gameSpeed, 7f, 1f, (speed ) =>
					{
						game.model.gameSpeed = speed;
					});

					game.model.platformsFactoryModel.verticalPlatformsGap = Random.Range(0.6f, 0.8f);
					break;
				}

			case GameSpeedState.Speed_5:
				{
					_speedIncreasinTween = DOVirtual.Float (game.model.gameSpeed, 8f, 1f, (speed ) =>
					{
						game.model.gameSpeed = speed;
					});

					game.model.platformsFactoryModel.verticalPlatformsGap = Random.Range(0.4f, 0.6f);
					break;
				}
			case GameSpeedState.Speed_6:
				{	
					_speedIncreasinTween = DOVirtual.Float (game.model.gameSpeed, 9f, 1f, (speed ) =>
					{
						game.model.gameSpeed = speed;
					});

					game.model.platformsFactoryModel.verticalPlatformsGap = Random.Range(0.20f, 0.4f);
					break;
				}

			case GameSpeedState.Speed_7:
				{	
					_speedIncreasinTween = DOVirtual.Float (game.model.gameSpeed, 10f, 1f, (speed ) =>
					{
						game.model.gameSpeed = speed;
					});

					game.model.platformsFactoryModel.verticalPlatformsGap = Random.Range(0.10f, 0.2f);
					break;
				}
		}

		game.model.gameSpeedState = speedState;

		Notify (N.GameSpeedChanged_, NotifyType.GAME, speedState);
	}

	private void DestroyIncreasingSpeedTween()
	{
		if (_speedIncreasinTween != null)
		{
			if (_speedIncreasinTween.IsActive ())
				_speedIncreasinTween.Kill ();

			_speedIncreasinTween = null;
		}
	}
}


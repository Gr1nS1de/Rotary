﻿using UnityEngine;
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
		int currentScore = game.model.currentScore;
		GameSpeedState correctGameSpeedState = GameSpeedState.Speed_1;

		if (currentScore < 5)
		{
			//
		}
		else if (currentScore >= 5 && currentScore < 30)
		{
			correctGameSpeedState = GameSpeedState.Speed_2;
		}
		else if (currentScore >= 30 && currentScore < 50)
		{
			correctGameSpeedState = GameSpeedState.Speed_3;
		}
		else if (currentScore >= 50 && currentScore < 70)
		{
			correctGameSpeedState = GameSpeedState.Speed_4;
		}
		else
		{
			correctGameSpeedState = GameSpeedState.Speed_5;
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
					game.model.gameSpeed = 4f;
					game.model.platformsFactoryModel.verticalPlatformsGap = Random.Range(0.45f, 0.5f);

					DestroyIncreasingSpeedTween ();
					break;
				}

			case GameSpeedState.Speed_2:
				{
					_speedIncreasinTween = DOVirtual.Float (game.model.gameSpeed, 5f, 3f, (speed ) =>
					{
						game.model.gameSpeed = speed;
					});

					game.model.platformsFactoryModel.verticalPlatformsGap = Random.Range(0.4f, 0.45f);
					break;
				}

			case GameSpeedState.Speed_3:
				{
					_speedIncreasinTween = DOVirtual.Float (game.model.gameSpeed, 6f, 2f, (speed ) =>
					{
						game.model.gameSpeed = speed;
					});

					game.model.platformsFactoryModel.verticalPlatformsGap = Random.Range(0.35f, 0.4f);
					break;
				}

			case GameSpeedState.Speed_4:
				{
					_speedIncreasinTween = DOVirtual.Float (game.model.gameSpeed, 7f, 1f, (speed ) =>
					{
						game.model.gameSpeed = speed;
					});

					game.model.platformsFactoryModel.verticalPlatformsGap = Random.Range(0.30f, 0.35f);
					break;
				}

			case GameSpeedState.Speed_5:
				{
					_speedIncreasinTween = DOVirtual.Float (game.model.gameSpeed, 8f, 1f, (speed ) =>
					{
						game.model.gameSpeed = speed;
					});

					game.model.platformsFactoryModel.verticalPlatformsGap = Random.Range(0.25f, 0.3f);
					break;
				}
			case GameSpeedState.Speed_6:
				{	
					_speedIncreasinTween = DOVirtual.Float (game.model.gameSpeed, 9f, 1f, (speed ) =>
					{
						game.model.gameSpeed = speed;
					});

					game.model.platformsFactoryModel.verticalPlatformsGap = Random.Range(0.20f, 0.25f);
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

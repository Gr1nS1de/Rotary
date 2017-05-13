using UnityEngine;
using System.Collections;

public class GameSpeedController : Controller
{
	private GameSpeedState _currentGameSpeedState;

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
		GameSpeedState correctGameSpeedState = GameSpeedState.SPEED_1;

		if (currentScore < 5)
		{
			//
		}
		else if (currentScore >= 5 && currentScore < 30)
		{
			correctGameSpeedState = GameSpeedState.SPEED_2;
		}
		else if (currentScore >= 30 && currentScore < 50)
		{
			correctGameSpeedState = GameSpeedState.SPEED_3;
		}
		else if (currentScore >= 50 && currentScore < 70)
		{
			correctGameSpeedState = GameSpeedState.SPEED_4;
		}
		else
		{
			correctGameSpeedState = GameSpeedState.SPEED_5;
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
			case GameSpeedState.SPEED_1:
				{
					game.model.gameSpeed = 3f;
					break;
				}

			case GameSpeedState.SPEED_2:
				{
					game.model.gameSpeed = 4f;
					break;
				}

			case GameSpeedState.SPEED_3:
				{
					game.model.gameSpeed = 6f;
					break;
				}

			case GameSpeedState.SPEED_4:
				{
					game.model.gameSpeed = 7f;
					break;
				}

			case GameSpeedState.SPEED_5:
				{
					game.model.gameSpeed = 8f;
					break;
				}
			case GameSpeedState.SPEED_6:
				{	
					game.model.gameSpeed = 9f;
					break;
				}

			case GameSpeedState.SPEED_7:
				{	
					game.model.gameSpeed = 10f;
					break;
				}
		}

		_currentGameSpeedState = speedState;
		game.model.gameSpeedState = _currentGameSpeedState;
	}
}


using UnityEngine;
using System.Collections;

public class GameServicesController : Controller
{
	private const string LEADERBOARD_ID = "CgkI-OTq2vYEEAIQAA";

	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();
					break;
				}

			case N.OnPlayerNewRecord_:
				{
					int score = (int)data [0];

					UpdatePlayerHighscore (score);
					break;
				}

			case N.OnRightButtonPressed_:
				{
					RightElementId rightElementId = (RightElementId)data [0];

					if(rightElementId == RightElementId.ButtonLeaderboard)
						GooglePlayManager.Instance.ShowLeaderBoardById (LEADERBOARD_ID);
					break;
				}
		}

	}

	private void OnStart()
	{
		ConnectGameServices ();
	}

	private void ConnectGameServices()
	{
		GooglePlayConnection.ActionConnectionResultReceived += ActionConnectionResultReceived;

		if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
		{
			OnGameServicesConnected (true);
		}
	}

	private void ActionConnectionResultReceived(GooglePlayConnectionResult result) 
	{
		if(result.IsSuccess) 
		{
			Debug.Log("GS Connected!");
		} else {
			Debug.Log("GS Cnnection failed with code: " + result.code.ToString());
		}

		OnGameServicesConnected (result.IsSuccess);
	}

	private void OnGameServicesConnected(bool isSuccess)
	{
		if(isSuccess)
			ConnectLeaderboards ();

		Notify (N.OnGameServicesConnected_, NotifyType.ALL, isSuccess);
	}

	private void ConnectLeaderboards()
	{
		GooglePlayManager.ActionLeaderboardsLoaded += OnLeaderBoardsLoaded;
		GooglePlayManager.Instance.LoadLeaderBoards ();
	}

	private void OnLeaderBoardsLoaded(GooglePlayResult result) 
	{
		GooglePlayManager.ActionLeaderboardsLoaded -= OnLeaderBoardsLoaded;

		if(result.IsSucceeded) 
		{
			if( GooglePlayManager.Instance.GetLeaderBoard(LEADERBOARD_ID) == null)
			{
				Debug.Log("Leader boards loaded"+ LEADERBOARD_ID + " not found in leader boards list");
				return;
			}

			GPLeaderBoard leaderboard = GooglePlayManager.Instance.GetLeaderBoard(LEADERBOARD_ID);
			long score = leaderboard.GetCurrentPlayerScore(GPBoardTimeSpan.ALL_TIME, GPCollectionType.GLOBAL).LongScore;

			Debug.Log("  score"+  score.ToString());
		} else {
			AndroidMessage.Create("Leader-Boards Loaded error: ", result.Message);
		}
	}

	private void UpdatePlayerHighscore(int score)
	{
		if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
			GooglePlayManager.Instance.SubmitScoreById(LEADERBOARD_ID, score);
	}
}


using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;

public class AnalyticsController : Controller
{
	private int _lastScore = 0;

	public override void OnNotification( string alias, Object target, params object[] data )
	{
		switch ( alias )
		{
			case N.OnStart:
				{
					OnStart ();

					break;
				}

			case N.GameStartPlay:
				{
					CustomEventDelegate.OnEvent (new CEAnalytics
					{
						EventName = AnalyticsEventName.GameStates_v1,
						IsHasEventValue = false,
						Parameters = new Dictionary<string, object>() { { string.Format("01_{0}GameStart", game.model.gameType.ToString()), null } }
					});

					if(game.model.playedGamesCount >= 0 && game.model.playedGamesCount <= 100)
					{
						CustomEventDelegate.OnEvent (new CEAnalytics
						{
							EventName = AnalyticsEventName.First100Games_v1,
							IsHasEventValue = false,
							Parameters = new Dictionary<string, object>() { { string.Format("{0:00}_{1}GameStart", game.model.playedGamesCount, game.model.gameType.ToString()), null } }
						});
					}

					/*
					CustomEventDelegate.OnEvent(new CEAnalyticsProgression()
					{
						progressionStatus = GameAnalyticsSDK.GAProgressionStatus.Start,
						progression01 = string.Format("Classic{0}",PlayerPrefs.GetInt(Prefs.PlayerData.GamesPlayedCount)),
						progression02 = null,
						progression03 = null,
						value = _lastScore
					});*/

					break;
				}

			case N.GamePause:
				{
					CustomEventDelegate.OnEvent (new CEAnalytics
					{
						EventName = AnalyticsEventName.GameStates_v1,
						IsHasEventValue = false,
						Parameters = new Dictionary<string, object>() { { string.Format("02_{0}GamePause", game.model.gameType.ToString()), null } }
					});
					break;
				}

			case N.GameContinue:
				{
					CustomEventDelegate.OnEvent (new CEAnalytics
					{
						EventName = AnalyticsEventName.GameStates_v1,
						IsHasEventValue = false,
						Parameters = new Dictionary<string, object>() { { string.Format("03_{0}GameContinue", game.model.gameType.ToString()), null } }
					});
					break;
				}
					
			case N.GameOver_:
				{
					GameOverData gameOverData = (GameOverData)data[0];

					SendGameResultResourceEvent (gameOverData);

					CustomEventDelegate.OnEvent (new CEAnalytics
					{
						EventName = AnalyticsEventName.GameStates_v1,
						IsHasEventValue = false,
						Parameters = new Dictionary<string, object>() { { string.Format("04_{0}GameOver", gameOverData.GameType.ToString()), null } }
					});

					if(game.model.playedGamesCount >= 0 && game.model.playedGamesCount <= 101)
					{
						CustomEventDelegate.OnEvent (new CEAnalytics
						{
							EventName = AnalyticsEventName.First100Games_v1,
							IsHasEventValue = false,
							Parameters = new Dictionary<string, object>() { { string.Format("{0:00}_{1}GameOver", game.model.playedGamesCount, gameOverData.GameType.ToString()), null } }
						});
					}

					CustomEventDelegate.OnEvent(new CEAnalyticsProgression()
					{
						progressionStatus = GameAnalyticsSDK.GAProgressionStatus.Complete,
						progression01 = string.Format("{0}Game", gameOverData.GameType.ToString()),
						progression02 = string.Format("{0:00}_GameCompleted",game.model.playedGamesCount),
						progression03 = null,
						value = game.model.currentScore
					});

					break;
				}

		}
	}

	private void OnStart()
	{
	}

	private void SendGameResultResourceEvent(GameOverData gameOverData)
	{
		if(gameOverData.CoinsCount > 0)
		{
			CustomEventDelegate.OnEvent(new CEAnalyticsResources()
			{
				flowType = GAResourceFlowType.Source,
				amount = gameOverData.CoinsCount,
				resourceCurrency = AnalyticsResoucesCurrency.Coin,
				itemId = string.Format("{0}Game", gameOverData.GameType.ToString()),
				itemType = AnalyticsItemType.GameResult
			});
		}

		if (gameOverData.CrystalsCount > 0)
		{
			CustomEventDelegate.OnEvent (new CEAnalyticsResources () 
			{
				flowType = GAResourceFlowType.Source,
				amount = gameOverData.CrystalsCount,
				resourceCurrency = AnalyticsResoucesCurrency.Crystal,
				itemId = string.Format("{0}Game", gameOverData.GameType.ToString()),
				itemType = AnalyticsItemType.GameResult
			});
		}

		if(gameOverData.MagnetsCount > 0)
		{
			CustomEventDelegate.OnEvent(new CEAnalyticsResources()
			{
				flowType = GAResourceFlowType.Source,
				amount = gameOverData.MagnetsCount,
				resourceCurrency = AnalyticsResoucesCurrency.Magnet,
				itemId = string.Format("{0}Game", gameOverData.GameType.ToString()),
				itemType = AnalyticsItemType.GameResult
			});
		}
	}

}


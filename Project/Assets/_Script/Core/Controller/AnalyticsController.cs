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
						Parameters = new Dictionary<string, object>() { { "01_GameStartPlay", null } }
					});

					if(game.model.playedGamesCount >= 0 && game.model.playedGamesCount <= 10)
					{
						CustomEventDelegate.OnEvent (new CEAnalytics
						{
							EventName = AnalyticsEventName.FirstBattles_v1,
							IsHasEventValue = false,
							Parameters = new Dictionary<string, object>() { { string.Format("{0}GameStartPlay", game.model.playedGamesCount), null } }
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
						Parameters = new Dictionary<string, object>() { { "02_GamePause", null } }
					});
					break;
				}

			case N.GameContinue:
				{
					CustomEventDelegate.OnEvent (new CEAnalytics
					{
						EventName = AnalyticsEventName.GameStates_v1,
						IsHasEventValue = false,
						Parameters = new Dictionary<string, object>() { { "03_GameContinue", null } }
					});
					break;
				}
					
			case N.GameOver_:
				{
					SendGameResultResourceEvent ();

					CustomEventDelegate.OnEvent (new CEAnalytics
					{
						EventName = AnalyticsEventName.GameStates_v1,
						IsHasEventValue = false,
						Parameters = new Dictionary<string, object>() { { "04_GameOver", null } }
					});

					if(game.model.playedGamesCount >= 0 && game.model.playedGamesCount <= 11)
					{
						CustomEventDelegate.OnEvent (new CEAnalytics
						{
							EventName = AnalyticsEventName.FirstBattles_v1,
							IsHasEventValue = false,
							Parameters = new Dictionary<string, object>() { { string.Format("{0}GameOver", game.model.playedGamesCount), null } }
						});
					}

					CustomEventDelegate.OnEvent(new CEAnalyticsProgression()
					{
						progressionStatus = GameAnalyticsSDK.GAProgressionStatus.Complete,
						progression01 = string.Format("Classic{0}",PlayerPrefs.GetInt(Prefs.PlayerData.GamesPlayedCount)),
						progression02 = null,
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

	private void SendGameResultResourceEvent()
	{
		ItemTypes itemType = ItemTypes.Coin; 
		int itemCount = 1;

		switch (itemType)
		{
			case ItemTypes.Coin:
				{
					CustomEventDelegate.OnEvent(new CEAnalyticsResources()
					{
						flowType = GAResourceFlowType.Source,
						amount = itemCount,
						resourceCurrency = AnalyticsResoucesCurrency.Coin,
						itemId = string.Format("{0}Game", game.model.playedGamesCount),
						itemType = AnalyticsItemType.GameResult
					});
					break;
				}

			case ItemTypes.Crystal:
				{
					break;
				}

			case ItemTypes.Magnet:
				{
					break;
				}
		}
	}

}


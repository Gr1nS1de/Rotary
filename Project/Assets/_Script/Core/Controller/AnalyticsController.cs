using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

					CustomEventDelegate.OnEvent(new CEAnalyticsProgression()
					{
						progressionStatus = GameAnalyticsSDK.GAProgressionStatus.Start,
						progression01 = string.Format("Classic{0}",PlayerPrefs.GetString(Prefs.PlayerData.GamesPlayedCount)),
						progression02 = null,
						progression03 = null,
						value = _lastScore
					});

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

			case N.GameOver:
				{
					CustomEventDelegate.OnEvent (new CEAnalytics
					{
						EventName = AnalyticsEventName.GameStates_v1,
						IsHasEventValue = false,
						Parameters = new Dictionary<string, object>() { { "04_GameOver", null } }
					});
					break;

					CustomEventDelegate.OnEvent(new CEAnalyticsProgression()
					{
						progressionStatus = GameAnalyticsSDK.GAProgressionStatus.Complete,
						progression01 = string.Format("Classic{0}",PlayerPrefs.GetString(Prefs.PlayerData.GamesPlayedCount)),
						progression02 = null,
						progression03 = null,
						value = game.model.currentScore
					});
				}

		}
	}

	private void OnStart()
	{
	}

}


﻿using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using DG.Tweening;

public class RewardVideoController : Controller
{	
	public const string 	REWRD_VIDEO_ZONE_ID 			= "rewardedVideo";
	public const string 	REWARD_VIDEO_ID 				= "ca-app-pub-1531752806194430/5226856705";
	private const int 		REWARD_VIDEO_MAX_SHOW_COUNT 	= 5;

	private System.DateTime _rewardAdVideoTimestamp 		= new System.DateTime(0);
	private bool 			_isRewardedVideoShown 			= false;
	private bool			_isStartedShowVideo				= false;
	private int 			_rewardVideoShowCount			= 0;
	private CanvasGroup		_rewardVideoCanvasGroup 		= null;
	private Sequence 		_rewardVideoButtonIdleSequence 	= null;
	private int				_gamesPlayedThisSession			= 0;

	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();
					break;
				}

			case N.GameOver_:
				{
					//GameOverData gameOverData = (GameOverData)data[0];
					int playedGames = core.playerDataModel.playedGamesCount;

					if (playedGames == 5)
					{
						ShowRewardVideo ();
					}

					if(_rewardVideoShowCount <= REWARD_VIDEO_MAX_SHOW_COUNT && _gamesPlayedThisSession >= 2)
						CheckRewardVideoShowTime ();

					_gamesPlayedThisSession++;
					break;
				}
			
		}

	}

	void OnStart() 
	{
		Debug.Log("Unity Ads initialized: " + Advertisement.isInitialized);
		Debug.Log("Unity Ads is supported: " + Advertisement.isSupported);
		Debug.Log("Unity Ads test mode enabled: " + Advertisement.testMode);


		_rewardVideoCanvasGroup = ui.model.mainMenuPanelModel.panelRewardVideo;

		if (Prefs.PlayerTimers.IsRewardVideoTimerInited ())
		{
			_rewardAdVideoTimestamp = Prefs.PlayerTimers.GetRewardAdVideoTimestamp ();

			CheckRewardVideoShowTime ();
		}
	}

	#region Public methods
	public void ShowRewardedAd()
	{
		if (_isStartedShowVideo)
			return;
		
		var options = new ShowOptions { resultCallback = HandleShowResult };
		Advertisement.Show (REWRD_VIDEO_ZONE_ID, options);
		Notify (N.OnStartShowAdVideo);
		_isStartedShowVideo = true;
	}
	#endregion

	private bool IsCanShowRewardVideo()
	{
		return (Advertisement.IsReady (REWRD_VIDEO_ZONE_ID) && !_isRewardedVideoShown && UnbiasedTime.Instance.Now ().Ticks > Prefs.PlayerTimers.GetRewardAdVideoTimestamp ().Ticks);
	}

	private void ShowRewardVideo()
	{
		_rewardVideoShowCount++;
		SetActiveRewardVideoButton (true);
		Notify (N.UIShowRewardVideoAd);
	}

	private void SetActiveRewardVideoButton(bool isActive)
	{
		if (isActive)
		{
			_rewardVideoCanvasGroup.alpha = 0f;

			if (!_rewardVideoCanvasGroup.gameObject.activeInHierarchy)
				_rewardVideoCanvasGroup.gameObject.SetActive (true);

			if (_rewardVideoButtonIdleSequence == null)
			{
				_rewardVideoButtonIdleSequence = DOTween.Sequence ();

				_rewardVideoButtonIdleSequence.SetAutoKill (false).SetRecyclable (true);
				_rewardVideoButtonIdleSequence
					.Append (_rewardVideoCanvasGroup.GetComponent<RectTransform> ().DOPunchScale (new Vector3 (0.05f, 0.05f, 0f), 1f, 1))
					.SetLoops (-1);
			}

			_rewardVideoButtonIdleSequence.Play ();

			_isRewardedVideoShown = true;
		}
		else
		{
			_isRewardedVideoShown = false;
		}

		_rewardVideoCanvasGroup.DOFade (isActive ? 1f : 0f, 0.3f)
			.OnComplete (() =>
			{
				if (!isActive)
				{
					if (_rewardVideoCanvasGroup.gameObject.activeInHierarchy)
						_rewardVideoCanvasGroup.gameObject.SetActive (false);
				}
			});
	}

	private void CheckRewardVideoShowTime()
	{
		if (Prefs.PlayerTimers.IsRewardVideoTimerInited())
		{
			if (IsCanShowRewardVideo())
			{
				ShowRewardVideo ();
			}
		}
	}

	private void HandleShowResult(ShowResult result)
	{
		bool isSuccess = false;

		switch (result)
		{
			case ShowResult.Finished:
				Debug.Log ("The ad was successfully shown.");
				isSuccess = true;
				break;
			case ShowResult.Skipped:
				Debug.Log("The ad was skipped before reaching the end.");
				break;
			case ShowResult.Failed:
				Debug.LogError("The ad failed to be shown.");
				break;
		}

		if (isSuccess)
		{
			_rewardAdVideoTimestamp = UnbiasedTime.Instance.Now ().AddHours (1);
		}
		else
		{
			_rewardAdVideoTimestamp =  UnbiasedTime.Instance.Now ().AddMinutes (10);
		}

		Prefs.PlayerTimers.SetRewardAdVideoTimestamp (_rewardAdVideoTimestamp);

		Notify (N.OnEndShowAdVideo_, NotifyType.ALL, isSuccess);
		SetActiveRewardVideoButton (false);
		_isStartedShowVideo = false;
	}

	void OnApplicationPause (bool paused)
	{
		if (paused) 
		{
			if(_rewardAdVideoTimestamp.Ticks != 0)
				Prefs.PlayerTimers.SetRewardAdVideoTimestamp(_rewardAdVideoTimestamp);
		}
		else 
		{
			if(_rewardAdVideoTimestamp.Ticks != 0)
				_rewardAdVideoTimestamp = Prefs.PlayerTimers.GetRewardAdVideoTimestamp();
		}
	}

	void OnApplicationQuit () 
	{
		if(_rewardAdVideoTimestamp.Ticks != 0)
			Prefs.PlayerTimers.SetRewardAdVideoTimestamp(_rewardAdVideoTimestamp);
	}

}


using UnityEngine;
using System.Collections;
using DG.Tweening;

public enum DailyGiftElementId
{
	GiftHour_00,
	GiftDay_01,
	GiftDay_02,
	GiftDay_03,
	GiftDay_04,
	GiftDay_05,
	GiftDay_06,
	GiftDay_07,
	GiftDay_08,
	GiftDay_09,
	GiftDay_10,
	GiftNextDay_11
}

public class DailyGiftController : Controller
{
	private MainMenuPanelModel 		_mainMenuPanelModel			{ get { return ui.model.mainMenuPanelModel; } }

	private System.DateTime 	_hourGiftTimestamp;
	private System.DateTime 	_dayGiftTimestamp;
	private int 				_daysReturn;
	private int[] 				_giftsArray;

	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();
					break;
				}

			case N.OnPlayerGetGift_:
				{
					DailyGiftElementId dailyGiftElementId = (DailyGiftElementId)data [0];

					OnClickDailyGiftElement(dailyGiftElementId);
					break;
				}
		}
	}

	private void OnStart()
	{
		if (Prefs.PlayerTimers.GetGiftsArray ().Length == 0)
		{
			InitGiftsArray ();
		}

		SetDefaults ();
		CheckCurrentGifts ();

		InvokeRepeating("TickOneSecond",0f, 1f);
	}

	private void SetDefaults()
	{
		_hourGiftTimestamp = Prefs.PlayerTimers.GetHourGiftTimestamp ();
		_dayGiftTimestamp = Prefs.PlayerTimers.GetDayGiftTimestamp ();
		_daysReturn = Prefs.PlayerTimers.GetDaysReturn ();
		_giftsArray = Prefs.PlayerTimers.GetGiftsArray ();
	}

	private void InitGiftsArray()
	{
		int[] giftsArray = new int[24];
		int giftIndex = 0;

		for(int i = 0; i< giftsArray.Length; i+=2)
		{
			giftsArray [i] = giftIndex;
			giftsArray [i + 1] = 0;

			giftIndex++;
		}

		_giftsArray = giftsArray;
		Prefs.PlayerTimers.InitGiftsArray (giftsArray);
	}

	private void CheckCurrentGifts()
	{
		//Day gifts
		if (_dayGiftTimestamp.AddDays (1) < UnbiasedTime.Instance.Now ())
		{
			ClearDayGifts ();
		}

		if (IsHourGiftActive ())
			SetActiveGift (DailyGiftElementId.GiftHour_00, true);

		for (int i = 1; i < System.Enum.GetNames (typeof(DailyGiftElementId)).Length; i++)
		{
			DailyGiftElementId giftId = (DailyGiftElementId)System.Enum.ToObject (typeof(DailyGiftElementId), i);

			if (i <= _daysReturn && Prefs.PlayerTimers.IsGiftActive (giftId))
			{
				SetActiveGift (giftId, true);
			}
		}
	}

	private void OnClickDailyGiftElement(DailyGiftElementId elementId)
	{
		switch (elementId)
		{
			case DailyGiftElementId.GiftHour_00:
				{
					SetupHourGiftTime ();
					break;
				}

			default:
				{
					break;
				}

		}

		SetActiveGift (elementId, false);
	}

	private void SetActiveGift(DailyGiftElementId giftId, bool isActive)
	{
		Prefs.PlayerTimers.SetGiftActive (giftId, isActive);
		SetDefaults ();

		ui.view.GetDailyGiftElement (giftId).SetActive (isActive);
	}

	private void ClearDayGifts()
	{
		Prefs.PlayerTimers.ClearDaysReturn ();

		SetDefaults ();
	}

	private bool IsHourGiftActive()
	{
		return _giftsArray [1] == 1;
	}

	private void TickOneSecond()
	{
		System.DateTime unbiasedTime = UnbiasedTime.Instance.Now ();
		System.TimeSpan? giftHourTimer = null;

		if (unbiasedTime.Ticks >= _hourGiftTimestamp.Ticks)
		{
			if (!IsHourGiftActive ())
			{
				OnNewGiftTime (true);	//Hour gift
			}
		}
		else
		{
			if (!IsHourGiftActive ())
			{
				giftHourTimer = _hourGiftTimestamp.Subtract(unbiasedTime);

			}
		}

		if (unbiasedTime.Ticks >= _dayGiftTimestamp.Ticks)
		{
			OnNewGiftTime (false);	//Day gift
		}

		CheckHourGiftView (giftHourTimer);
	}

	private void OnNewGiftTime(bool isHour)
	{
		DailyGiftElementId giftId;

		if (isHour)
		{
			SetupHourGiftTime ();
			giftId = DailyGiftElementId.GiftHour_00;
		}
		else
		{
			_daysReturn++;
			Prefs.PlayerTimers.IncreaseDaysReturn ();

			int returnDaysId = Mathf.Clamp (_daysReturn, 0, System.Enum.GetNames (typeof(DailyGiftElementId)).Length);

			SetupDayGiftTime ();
			giftId = (DailyGiftElementId)System.Enum.Parse(typeof(DailyGiftElementId), System.Enum.GetNames(typeof(DailyGiftElementId))[returnDaysId]);
		}

		SetActiveGift (giftId, true);
	}

	private void CheckHourGiftView(System.TimeSpan? giftHourTimer)
	{
		if (!IsHourGiftActive())
		{
			if (_mainMenuPanelModel.panelHourGiftTitle.alpha > 0f)
			{
				_mainMenuPanelModel.panelHourGiftTitle.DOFade (0f, 0.5f);
			} 

			if (_mainMenuPanelModel.textHourGiftTimer.color.a < 1f)
			{
				_mainMenuPanelModel.textHourGiftTimer.DOFade (1f, 0.5f);
			}

			if(giftHourTimer !=  null)
				_mainMenuPanelModel.textHourGiftTimer.text = string.Format ("{0:00}:{1:00}", giftHourTimer.GetValueOrDefault().Minutes, giftHourTimer.GetValueOrDefault().Seconds);
		}
		else
		{
			if (_mainMenuPanelModel.textHourGiftTimer.color.a > 0f)
			{
				_mainMenuPanelModel.textHourGiftTimer.DOFade (0f, 0.5f);
			}  

			if (_mainMenuPanelModel.panelHourGiftTitle.alpha < 1f)
			{
				_mainMenuPanelModel.panelHourGiftTitle.DOFade (1f, 0.5f);
			}
		}
	}

	private void SetupHourGiftTime()
	{
		_hourGiftTimestamp = UnbiasedTime.Instance.Now ().AddHours (1);

		Prefs.PlayerTimers.SetHourGiftTimestamp(_hourGiftTimestamp);
	}

	private void SetupDayGiftTime()
	{
		_dayGiftTimestamp = _dayGiftTimestamp.Ticks > 0 ? _dayGiftTimestamp.AddDays (1) : UnbiasedTime.Instance.Now ().AddDays(1);
		_daysReturn++;

		Prefs.PlayerTimers.SetDayGiftTimestamp(_dayGiftTimestamp);
		Prefs.PlayerTimers.IncreaseDaysReturn ();
	}

	void OnApplicationPause (bool paused)
	{
		if (paused) 
		{
			Prefs.PlayerTimers.SetDayGiftTimestamp(_dayGiftTimestamp);
			Prefs.PlayerTimers.SetHourGiftTimestamp(_hourGiftTimestamp);
		}
		else 
		{
			_dayGiftTimestamp = Prefs.PlayerTimers.GetDayGiftTimestamp();
			_hourGiftTimestamp = Prefs.PlayerTimers.GetHourGiftTimestamp ();
			_daysReturn = Prefs.PlayerTimers.GetDaysReturn();
		}
	}

	void OnApplicationQuit () 
	{
		Prefs.PlayerTimers.SetDayGiftTimestamp(_dayGiftTimestamp);
		Prefs.PlayerTimers.SetHourGiftTimestamp(_hourGiftTimestamp);
	}
}


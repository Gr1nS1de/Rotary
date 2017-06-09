using UnityEngine;
using System.Collections;

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
	private System.DateTime 	_hourGiftTimestamp;
	private System.DateTime 	_dayGiftTimestamp;
	private int 				_daysReturn;

	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();
					break;
				}

			case N.OnClickDailtGiftElement_:
				{
					DailyGiftElementId dailyGiftElementId = (DailyGiftElementId)data [0];

					OnClickDailyGiftElement(dailyGiftElementId);
					break;
				}
					
		}
	}

	private void OnStart()
	{
		InvokeRepeating("TickOneSecond",0f, 1f);

		_hourGiftTimestamp = Prefs.PlayerTimers.GetHourGiftTimestamp ();
		_dayGiftTimestamp = Prefs.PlayerTimers.GetDayGiftTimestamp ();
		_daysReturn = Prefs.PlayerTimers.GetDaysReturn ();

		if (Prefs.PlayerTimers.GetGiftsArray ().Length == 0)
			InitGiftsArray ();
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
	}

	private void SetActiveGift(DailyGiftElementId elementId)
	{
		switch(elementId)
		{
			case DailyGiftElementId.GiftHour_00:
				{
					if (ui.view.GetDailyGiftElement (DailyGiftElementId.GiftHour_00).IsActive)
					{

					}
					break;
				}
		}
	}

	private void OnClickDailyGiftElement(DailyGiftElementId elementId)
	{
		switch (elementId)
		{
			case DailyGiftElementId.GiftHour_00:
				{
					GetHourGift ();
					break;
				}

			default:
				{
					GetDayGift (elementId);
					break;
				}
					
		}
	}

	private void TickOneSecond()
	{
		System.DateTime unbiasedTime = UnbiasedTime.Instance.Now ();

		if (unbiasedTime.Ticks >= _hourGiftTimestamp.Ticks)
		{
			GetHourGift ();
		}

		if (unbiasedTime.Ticks >= _dayGiftTimestamp.Ticks)
		{
			//GetDayGift ();
		}
	}

	private void GetHourGift()
	{
		_hourGiftTimestamp = UnbiasedTime.Instance.Now ().AddHours (1);

		Prefs.PlayerTimers.SetHourGiftTimestamp(_hourGiftTimestamp);
	}

	private void GetDayGift(DailyGiftElementId elementId)
	{
		_dayGiftTimestamp = UnbiasedTime.Instance.Now ().AddDays (1);
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


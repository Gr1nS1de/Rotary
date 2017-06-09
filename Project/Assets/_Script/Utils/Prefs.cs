using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Prefs
{
	public static class PlayerTimers
	{
		public const string RewardAdVideoTimestamp 	= "reward.ad.video.timestamp";
		public const string HourGiftTimestamp 		= "hour.gift.timestamp";
		public const string DayGiftTimestamp		= "day.gift.timestamp";
		public const string DaysReturn				= "days.return";
		public const string GiftsArray				= "gifts.array";

		public static bool IsRewardVideoTimerInited()
		{

			return PlayerPrefs.HasKey (RewardAdVideoTimestamp);
		}

		public static void InitGiftsArray(int[] giftsArray)
		{
			PlayerPrefsX.SetIntArray (GiftsArray, giftsArray);
		}

		public static bool IsGiftActive(DailyGiftElementId elementId)
		{
			int[] activatedGifts = GetGiftsArray ();

			return activatedGifts [(int)elementId * 2 + 1] == 1;
		}

		public static void SetGiftActivated(DailyGiftElementId giftId, bool isActivated)
		{
			int[] giftsArray = GetGiftsArray ();

			giftsArray [(int)giftId * 2 + 1] = isActivated ? 1 : 0;

			PlayerPrefsX.SetIntArray (GiftsArray, giftsArray);
		}

		public static int[] GetGiftsArray()
		{
			return PlayerPrefsX.GetIntArray (GiftsArray);
		}

		public static void IncreaseDaysReturn()
		{
			PlayerPrefs.SetInt (DaysReturn, GetDaysReturn() + 1);
		}

		public static int GetDaysReturn()
		{
			return PlayerPrefs.GetInt (DaysReturn, 0);
		}

		public static void ClearDaysReturn()
		{
			PlayerPrefs.SetInt (DaysReturn, 0);
			SetDayGiftTimestamp (new DateTime (0));
			int[] giftsArray = GetGiftsArray ();

			for (int i = 2; i < giftsArray.Length; i+=2)
			{
				giftsArray [i] = 0;
			}

			PlayerPrefsX.SetIntArray (GiftsArray, giftsArray);
		}

		public static DateTime GetRewardAdVideoTimestamp () 
		{
			long tmp = Convert.ToInt64(PlayerPrefs.GetString(RewardAdVideoTimestamp));

			return DateTime.FromBinary(tmp);
		}

		public static DateTime GetHourGiftTimestamp () 
		{
			long tmp = Convert.ToInt64(PlayerPrefs.GetString(HourGiftTimestamp, "0"));

			if (tmp == 0)
			{
				return new DateTime (0);
			}

			return DateTime.FromBinary(tmp);
		}

		public static DateTime GetDayGiftTimestamp () 
		{
			long tmp = Convert.ToInt64(PlayerPrefs.GetString(DayGiftTimestamp, "0"));

			if (tmp == 0)
			{
				return new DateTime (0);
			}

			return DateTime.FromBinary(tmp);
		}

		public static void SetRewardAdVideoTimestamp (DateTime time) 
		{
			PlayerPrefs.SetString(RewardAdVideoTimestamp, time.ToBinary().ToString());
		}

		public static void SetHourGiftTimestamp(DateTime time)
		{
			PlayerPrefs.SetString(HourGiftTimestamp, time.ToBinary().ToString());
		}

		public static void SetDayGiftTimestamp(DateTime time)
		{
			PlayerPrefs.SetString(DayGiftTimestamp, time.ToBinary().ToString());
		}
	}

	public static class PlayerData
	{
		public const string GamesPlayedCount 	= "games.played.count";
		public const string CurrentLanguage 	= "current.language";
		public const string CoinsCount			= "coins.count";
		public const string CrystalsCount		= "crystals.count";
		public const string IsDoubleCoin		= "is.double.coin";
		public const string Record 				= "record";
	
		public static void IncreasePlayedGamesCount()
		{
			PlayerPrefs.SetInt (GamesPlayedCount, GetPlayedGamesCount () + 1);
		}

		public static void CreditCoins(int count = 1)
		{
			PlayerPrefs.SetInt(CoinsCount, GetCoinsCount() + count);
		}

		public static void CreditCrystals(int count = 1)
		{
			PlayerPrefs.SetInt(CrystalsCount, GetCrystalsCount() + count);
		}

		public static void DebitCoins(int count = 1)
		{
			PlayerPrefs.SetInt(CoinsCount, (int)Mathf.Clamp(GetCoinsCount() - count, 0, Mathf.Infinity));
		}

		public static void DebitCrystals(int count = 1)
		{
			PlayerPrefs.SetInt(CrystalsCount, (int)Mathf.Clamp(GetCrystalsCount() - count, 0, Mathf.Infinity));
		}

		public static bool IsEnoughCoins(int price)
		{
			return PlayerPrefs.GetInt (CoinsCount) >= price;
		}

		public static bool IsEnoughCrystals(int price)
		{
			return PlayerPrefs.GetInt(CrystalsCount) >= price;
		}

		public static int GetCoinsCount()
		{
			return PlayerPrefs.GetInt (CoinsCount);
		}

		public static int GetCrystalsCount()
		{
			return PlayerPrefs.GetInt(CrystalsCount);
		}

		public static int GetPlayedGamesCount()
		{
			return PlayerPrefs.GetInt (GamesPlayedCount);
		}

		public static void SetDoubleCoin()
		{
			PlayerPrefs.SetInt (IsDoubleCoin, 1);
		}

		public static int GetDoubleCoin()
		{
			return PlayerPrefs.GetInt (IsDoubleCoin);
		}

		public static void SetRecord(int record)
		{
			PlayerPrefs.SetInt (Record, record);
		}

		public static int GetRecord()
		{
			return PlayerPrefs.GetInt (Record);
		}
	}
		
}

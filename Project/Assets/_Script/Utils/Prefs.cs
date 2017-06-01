﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Prefs
{
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

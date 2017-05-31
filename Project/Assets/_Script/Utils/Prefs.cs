using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Prefs
{
	public static class PlayerData
	{
		public const string GamesPlayedCount = "games.played.count";
		public const string CurrentLanguage = "current.language";
		public const string CoinsCount		= "coins.count";
		public const string CrystalsCount	= "crystals.count";
	
		public static void IncreasePlayedGamesCount()
		{
			PlayerPrefs.SetInt (GamesPlayedCount, GetPlayedGamesCount () + 1);
		}

		public static void IncreaseCoinsCount(int count = 1)
		{
			PlayerPrefs.SetString(CoinsCount, string.Format("{0}",GetCoinsCount() + count));
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
	}
		
}

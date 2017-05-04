using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Utility class.
/// </summary>
public static class Utils
{
	private const string LastScoreKey = "LAST_SCORE";
	private const string BestScoreKey = "BEST_SCORE";

	public static string GetTextFirstUpper(string text)
	{
		return string.Format("{0}{1}", char.ToUpper(text [0]), text.Substring(1));
	}
}

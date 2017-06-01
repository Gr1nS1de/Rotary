using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Globalization;

/// <summary>
/// Utility class.
/// </summary>
public static class Utils
{
	public static string GetTextFirstUpper(string text)
	{
		return string.Format("{0}{1}", char.ToUpper(text [0]), text.Substring(1));
	}

	public static string SweetMoney(int number) 
	{
		return number.ToString("N0", new NumberFormatInfo { NumberGroupSeparator = " " });
	}

	public static void RebuildLayoutGroups(RectTransform element)
	{
		if (element == null)
		{
			Debug.LogError ("Trying to rebuild layout for null element!");
			return;
		}

		List<LayoutGroup> layoutsList = new List<LayoutGroup>(element.GetComponentsInChildren<LayoutGroup> ());

		if (layoutsList.Count > 0)
		{
			layoutsList.ForEach ((layout) =>
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate (layout.GetComponent<RectTransform> ());
			});
		}
	}
}

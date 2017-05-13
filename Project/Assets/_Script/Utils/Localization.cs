using UnityEngine;
using System.Collections;
using SmartLocalization;

public static class Localization
{
	private const string 	PREFS_LANGUAGE_CODE 	= "LanguageCode";

	#region public methods
	//Default value - for init language on all scenes in Awake().
	//Any value - for changing language with check for dowload needed.  
	public static void InitLanguage(string langCode = "")
	{
		LanguageManager langManager = LanguageManager.Instance;

		//Define current language
		if (string.IsNullOrEmpty (langCode))
		{
			SmartCultureInfo deviceCulture = langManager.GetDeviceCultureIfSupported();

			if (!string.IsNullOrEmpty( GetCurrentLanguageCode ()))
			{
				langCode = GetCurrentLanguageCode ();
			} 
			else if (deviceCulture != null)
			{
				langCode = deviceCulture.languageCode;
			} else
				langCode = "en";
		}

		StoreCurrentLanguageCode (langCode);

		langManager.ChangeLanguage( langCode);
	}

	public static string GetLocalizationVersion(string langCode)
	{
		return PlayerPrefs.GetString (string.Format("{0}_version",langCode), "0");
	}
		
	//строка, возвращающая имя по ключу https://ussrgames.atlassian.net/wiki/pages/viewpage.action?pageId=14942307#id-Локализация-Соглашениеобименованииключейлокализации
	public static string CheckKey(string text)
	{
		if (string.IsNullOrEmpty (text))
		{
			Debug.LogError ("Text for localization is empty or null!");
			return "####";
		}

		if (IsKeyExist (text))
		{
			text = LanguageManager.Instance.GetTextValue(text);

			text = text.Replace (';', ',');
			text = text.Replace (">>", "\n");
		} else
		{
			text = string.Format ("##{0}##", text);
		}

		return text;
	}

	public static bool IsKeyExist(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			throw new System.Exception("Key is null or empty");
		}

		return LanguageManager.Instance.HasKey(key) ? true : false;
	}

	public static void StoreCurrentLanguageCode(string code)
	{
		PlayerPrefs.SetString (PREFS_LANGUAGE_CODE, code);
	}

	public static string GetCurrentLanguageCode()
	{
		return PlayerPrefs.GetString (PREFS_LANGUAGE_CODE);
	}
	#endregion

	#region private methods

	#endregion

}


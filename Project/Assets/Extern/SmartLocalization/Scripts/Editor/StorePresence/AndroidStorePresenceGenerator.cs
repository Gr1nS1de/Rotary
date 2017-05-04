// AndroidStorePresenceGenerator.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//
namespace SmartLocalization.Editor
{
using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Callbacks;

/// <summary>
/// Class that handles the generation of files so the list of localized languages will be shown in the Google Play store.
/// </summary>
public static class AndroidStorePresenceGenerator
{
	static readonly string PluginFolderName = "Plugins";
	static readonly string AndroidFolderName = "Android";
	static readonly string ResourceFolderName = "res";
	static readonly string ValuesFolderName	= "values";
	static readonly string StringsFileName	= "strings.xml";
	static readonly string SmartLocalizationDeclaration = "smart_localization_presence";
	static readonly string GenerateStorePresenceSaveKey = "smartlocalization_android_store_presence_gen";


	/// <summary>
	/// Gets or sets if the files should be generated on post-build to Android
	/// </summary>
	public static bool GenerateStorePresence
	{
		get
		{
			if(EditorPrefs.HasKey(GenerateStorePresenceSaveKey))
			{
				return EditorPrefs.GetBool(GenerateStorePresenceSaveKey);
			}
			
			return false;
		}
		set
		{
			EditorPrefs.SetBool(GenerateStorePresenceSaveKey, value);
		}
	}


	/// <summary>
	/// Returns if the project has a generated store presence for android
	/// </summary>
	public static bool HasStorePresence
	{
		get
		{
			return CheckStorePresence();
		}
	}

	/// <summary>
	/// Generates all the files for store presence
	/// </summary>
	/// <returns>If the operation was successful</returns>
	public static bool GeneratePresence()
	{
		if(HasStorePresence)
		{
			return true;
		}

		string currentDirectory = Application.dataPath + "/" + PluginFolderName;
		if(!DirectoryUtility.CheckAndCreate(currentDirectory))
		{
			return false;
		}

		currentDirectory += "/" + AndroidFolderName;
		if(!DirectoryUtility.CheckAndCreate(currentDirectory))
		{
			return false;
		}

		currentDirectory += "/" + ResourceFolderName;
		if(!DirectoryUtility.CheckAndCreate(currentDirectory))
		{
			return false;
		}

		//Create the default directory
		if(!DirectoryUtility.CheckAndCreate(currentDirectory + "/" + ValuesFolderName))
		{
			return false;
		}

		if(!AppendOrCreateStringsFile(currentDirectory + "/" + ValuesFolderName + "/" + StringsFileName))
		{
			return false;
		}
		
		var availableCultures = SmartCultureInfoEx.Deserialize(LocalizationWorkspace.AvailableCulturesFilePath());

		foreach(SmartCultureInfo cultureInfo in availableCultures.cultureInfos)
		{
			if(!IsLanguageSupported(cultureInfo.languageCode))
			{
				continue;
			}

			string currentLanguageFolderPath = GetValueFolderPath(currentDirectory, cultureInfo);

			if(!DirectoryUtility.CheckAndCreate(currentLanguageFolderPath))
			{
				return false;
			}

			if(!AppendOrCreateStringsFile(currentLanguageFolderPath + "/" + StringsFileName))
			{
				return false;
			}
		}

		AssetDatabase.Refresh();

		return true;
	}

	static string GetValueFolderPath(string currentDirectory, SmartCultureInfo cultureInfo)
	{
		if(cultureInfo.languageCode.Contains("-"))
		{
			var words = cultureInfo.languageCode.Split('-');
			string path = currentDirectory + "/" + ValuesFolderName + "-";
			if(words.Length > 0)
			{
				path += words[0];
			}
			if(words.Length > 1)
			{
				path += "-r" + words[1];
			}

			return path;
		}

		return currentDirectory + "/" + ValuesFolderName + "-" + cultureInfo.languageCode;
	}

   	static bool AppendOrCreateStringsFile(string fullPath)
   	{
		if(!FileUtility.Exists(fullPath))
		{
			return FileUtility.WriteToFile(fullPath, GetBasicStringXMLData());
		}

		if(DoesFileHavePresence(fullPath))
		{
			return true;
		}

		XDocument doc = XDocument.Load(fullPath);
		if(doc == null)
		{
			return false;
		}

		try
		{
			XElement resourceElement = doc.Root;
			XElement presence = new XElement("string");
			presence.Value = SmartLocalizationDeclaration;
			XAttribute nameAttrib = new XAttribute("name", SmartLocalizationDeclaration);
			presence.Add(nameAttrib);
			resourceElement.Add(presence);
			doc.Save(fullPath);
			return true;
		}
		catch(System.Exception ex)
		{
			Debug.LogError(ex.Message);
			return false;
		}
	}

	static bool DoesFileHavePresence(string filePath)
	{
		string fileData = string.Empty;
		FileUtility.ReadFromFile(filePath, out fileData);

		if(fileData.Contains(SmartLocalizationDeclaration))
		{
			return true;
		}

		return false;
	}

	static bool CheckStorePresence()
	{
		string pluginDirectory = "/" + PluginFolderName;
		string androidDirectory = pluginDirectory + "/" + AndroidFolderName;
		string resourceDirectory = androidDirectory + "/" + ResourceFolderName;

		if(DirectoryUtility.ExistsRelative(pluginDirectory) && 
		   DirectoryUtility.ExistsRelative(androidDirectory) && 
		   DirectoryUtility.ExistsRelative(resourceDirectory) && 
		   DirectoryUtility.ExistsRelative(resourceDirectory + "/" + ValuesFolderName) && 
		   FileUtility.ExistsRelative(resourceDirectory + "/" + ValuesFolderName + "/" + StringsFileName))
		{
			if(!DoesFileHavePresence(Application.dataPath + "/" + resourceDirectory + "/" + ValuesFolderName + "/" + StringsFileName))
			{
				return false;
			}

			var availableCultures = SmartCultureInfoEx.Deserialize(LocalizationWorkspace.AvailableCulturesFilePath());

			foreach(SmartCultureInfo cultureInfo in availableCultures.cultureInfos)
			{
				if(!IsLanguageSupported(cultureInfo.languageCode))
				{
					continue;
				}

				string currentLanguageFolderPath = GetValueFolderPath(resourceDirectory, cultureInfo);

				if(!DirectoryUtility.ExistsRelative(currentLanguageFolderPath))
				{
					return false;
				}
				if(!FileUtility.ExistsRelative(currentLanguageFolderPath + "/" + StringsFileName))
				{
					return false;
				}
				else if(!DoesFileHavePresence(Application.dataPath + "/" + currentLanguageFolderPath + "/" + StringsFileName))
				{
					return false;
				}
			}

			return true;
		}

		return false;
	}

	static string GetBasicStringXMLData()
	{
		return "<?xml version=\"1.0\" encoding=\"utf-8\"?> \n" + 
			"<resources> \n" +
				"<string name=\"smart_localization_presence\">smart_localization_presence</string> \n" +
			"</resources>";
	}

	/// <summary>
	/// Returns whether or not the language/locale is supported by Google Play
	/// Source: https://support.google.com/googleplay/android-developer/table/4419860?hl=en
	/// </summary>
	/// <returns>If the Language(Locale) is supported</returns>
	static bool IsLanguageSupported(string languageCode)
	{
		switch(languageCode)
		{
			case "af":
			case "am":
			case "bg":
			case "ca":
			case "zh-HK":
			case "zh-CN":
			case "zh-TW":
			case "hr":
			case "cs":
			case "da":
			case "nl":
			case "en-GB":
			case "en":
			case "en-US":
			case "et":
			case "fil":
			case "fi":
			case "fr-CA":
			case "fr-FR":
			case "fr":
			case "de":
			case "el":
			case "hi":
			case "hu":
			case "id":
			case "in":
			case "it":
			case "ja":
			case "ko":
			case "lv":
			case "lt":
			case "ms":
			case "no":
			case "pl":
			case "pt-BR":
			case "pt-PT":
			case "pt":
			case "ro":
			case "ru":
			case "sr":
			case "sk":
			case "sl":
			case "es-419":
			case "es-ES":
			case "es":
			case "sw":
			case "sv":
			case "th":
			case "tr":
			case "uk":
			case "zu":
				return true;
			default:
				return false;
		}
	}



	[PostProcessScene]
	static void OnPostProcessScene()
	{
		if(LocalizationWorkspace.Exists() && 
		   GenerateStorePresence && 
		   !HasStorePresence)
		{
			GeneratePresence();
		}
	}
}
}

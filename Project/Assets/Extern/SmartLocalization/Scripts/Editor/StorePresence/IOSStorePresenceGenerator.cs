// IOSStorePresenceGenerator.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//
using System.Xml.Linq;
using UnityEditor;
using System.IO;
using System.Xml;
using UnityEditor.Callbacks;

namespace SmartLocalization.Editor
{
using UnityEngine;
using System.Collections;

/// <summary>
/// Class that handles the generation of files so the list of localized languages will be shown in the App Store.
/// </summary>
public static class IOSStorePresenceGenerator
{
	static string GenerateStorePresenceSaveKey = "smartlocalization_ios_store_presence_gen";

	/// <summary>
	/// Gets or sets if the files should be generated on post-building to iOS
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


	private static XmlNode FindPlistDictNode(XmlDocument doc)
	{
		var currentChild = doc.FirstChild;
		while(currentChild != null)
		{
			if(currentChild.Name.Equals("plist") && currentChild.ChildNodes.Count == 1)
			{
				var dict = currentChild.FirstChild;
				if(dict.Name.Equals("dict"))
				{
					return dict;
				}
			}
			currentChild = currentChild.NextSibling;
		}
		return null;
	}

	private static XmlElement AddChildElement(XmlDocument doc, XmlNode parent, string elementName, string innerText=null)
	{
		var newElement = doc.CreateElement(elementName);
		if(!string.IsNullOrEmpty(innerText))
		{
			newElement.InnerText = innerText;
		}
		
		parent.AppendChild(newElement);
		return newElement;
	}

	private static XmlElement FindOrCreateElement(XmlDocument doc, XmlNode parent, string elementName, string innerText = null)
	{
		if (parent[elementName] != null)
		{
			return parent[elementName];
		}
		return AddChildElement(doc, parent, elementName, innerText);
	}
	
	private static bool HasKey(XmlNode dict, string keyName)
	{
		var currentChild = dict.FirstChild;
		while(currentChild != null)
		{
			if(currentChild.Name.Equals("key") && currentChild.InnerText.Equals(keyName))
			{
				return true;
			}
			currentChild = currentChild.NextSibling;
		}
		return false;
	}

	/// <summary>
	/// Post Process build method that handles the generation of store files.
	/// </summary>
	/// <param name="target">The build target</param>
	/// <param name="path">The path to the build</param>
	[PostProcessBuild(100)]
	public static void OnPostProcessBuild(BuildTarget target, string path)
	{
#if UNITY_4_6 || UNITY_4_7
		if(target != BuildTarget.iPhone)
#else
		if(target != BuildTarget.iOS)
#endif
		{
			return;
		}

		if(!GenerateStorePresence)
		{
			return;
		}

		const string fileName = "Info.plist";
		string fullPath = Path.Combine(path, fileName);
		
		var doc = new XmlDocument();
		doc.Load(fullPath);
		
		var dict = FindPlistDictNode(doc);
		if(dict == null)
		{
			Debug.LogError("[Smartlocalization] Error parsing " + fullPath);
			return;
		}

		AddChildElement(doc, dict, "key", "CFBundleLocalizations");
		var arrayKey = AddChildElement(doc, dict, "array");
		
		var availableCultures = SmartCultureInfoEx.Deserialize(LocalizationWorkspace.AvailableCulturesFilePath());
		
		foreach (var cultureInfo in availableCultures.cultureInfos)
		{
			AddChildElement(doc, arrayKey, "string", cultureInfo.languageCode);
		}

		doc.Save(fullPath);
		
		//the xml writer barfs writing out part of the plist header.
		//so we replace the part that it wrote incorrectly here
		string textPlist = null;
		using(var reader = new StreamReader(fullPath))
		{
			textPlist = reader.ReadToEnd();
		}
		
		int fixupStart = textPlist.IndexOf("<!DOCTYPE plist PUBLIC", System.StringComparison.Ordinal);
		if(fixupStart <= 0)
		{
			return;
		}
		int fixupEnd = textPlist.IndexOf('>', fixupStart);
		if(fixupEnd <= 0)
		{
			return;
		}
		
		string fixedPlist = textPlist.Substring(0, fixupStart);
		fixedPlist += "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">";
		fixedPlist += textPlist.Substring(fixupEnd+1);
		
		using(var writer = new StreamWriter(fullPath, false))
		{
			writer.Write(fixedPlist);
		}
	}
}
}
	
	
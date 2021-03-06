﻿namespace SmartLocalization.Editor
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;

	[RequireComponent (typeof (Text))]
	public class LocalizedText : MonoBehaviour 
	{
		public string localizedKey 	= "INSERT_KEY_HERE";
		public string localizedKeyRand = "";
		Text textObject;
		[Tooltip("Show empty string if key was not found.")]
		public bool IsShowEmpty		= false;			//Show empty string if key was not found.
		public LocalizationTextState textState;

		public string AddStringToEnd = "";

		void Awake()
		{
			if(LanguageManager.HasInstance)
				Localization.InitLanguage ();
		}
		
		void Start () 
		{
			textObject = this.GetComponent<Text>();
		
			//Subscribe to the change language event
			LanguageManager languageManager = LanguageManager.Instance;
			languageManager.OnChangeLanguage += OnChangeLanguage;
			
			//Run the method one first time
			OnChangeLanguage(languageManager);
		}
		
		void OnDestroy()
		{
			if(LanguageManager.HasInstance)
			{
				LanguageManager.Instance.OnChangeLanguage -= OnChangeLanguage;
			}
		}
		
		void OnChangeLanguage(LanguageManager languageManager)
		{
			if (IsShowEmpty)
			{
				if (!Localization.IsKeyExist (localizedKey))
				{
					textObject.text = "";
					return;
				}
			}

			bool isRandomText = !string.IsNullOrEmpty (localizedKeyRand);

			textObject.text = isRandomText ? Random.Range(0, 2 ) == 0 ? Localization.CheckKey (localizedKeyRand) : Localization.CheckKey (localizedKey) : Localization.CheckKey (localizedKey);

			switch (textState)
			{
				case LocalizationTextState.TO_LOWER:
					{
						textObject.text = textObject.text.ToLower ();
						break;
					}

				case LocalizationTextState.TO_UPPER:
					{
						textObject.text = textObject.text.ToUpper ();

						break;
					}

				case LocalizationTextState.TO_UPPER_FIRST:
					{
						textObject.text = Utils.GetTextFirstUpper (textObject.text);
						break;
					}

				default:
					break;
			}

			if (!string.IsNullOrEmpty (AddStringToEnd))
			{
				textObject.text += AddStringToEnd;
			}

		}
	}
}
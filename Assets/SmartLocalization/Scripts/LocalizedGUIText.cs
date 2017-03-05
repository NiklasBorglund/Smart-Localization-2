//
// LocalizedGUIText.cs
//
//
// Written by Niklas Borglund and Jakob Hillerström
//

namespace SmartLocalization
{
using UnityEngine;
using System.Collections;

public class LocalizedGUIText : MonoBehaviour 
{
	public string localizedKey = "INSERT_KEY_HERE";
	
	void Start () 
	{
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
		//Initialize all your language specific variables here
		GetComponent<GUIText>().text = LanguageManager.Instance.GetTextValue(localizedKey);
	}
}
}//namespace SmartLocalization

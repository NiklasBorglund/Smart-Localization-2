using UnityEngine;
using System.Collections;

//ApplicationExtensions.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//


namespace SmartLocalization
{
internal static class ApplicationExtensions 
{
	//using ToString() on an enum is not accepted when building the project
	// as a dll for web player projects. Using the solution below instead.
	internal static string GetSystemLanguage(){
		return GetStringValueOfSystemLanguage(Application.systemLanguage);
	}

	internal static string GetStringValueOfSystemLanguage(SystemLanguage systemLanguage)
	{
		switch(systemLanguage)
		{
			case SystemLanguage.Afrikaans:
				return "Afrikaans";
			case SystemLanguage.Arabic:
				return "Arabic";
			case SystemLanguage.Basque:
				return "Basque";
			case SystemLanguage.Belarusian:
				return "Belarusian";
			case SystemLanguage.Bulgarian:
				return "Bulgarian";
			case SystemLanguage.Catalan:
				return "Catalan";
			case SystemLanguage.Chinese:
				return "Chinese";
			case SystemLanguage.Czech:
				return "Czech";
			case SystemLanguage.Danish:
				return "Danish";
			case SystemLanguage.Dutch:
				return "Dutch";
			case SystemLanguage.English:
				return "English";
			case SystemLanguage.Estonian:
				return "Estonian";
			case SystemLanguage.Faroese:
				return "Faroese";
			case SystemLanguage.Finnish:
				return "Finnish";
			case SystemLanguage.French:
				return "French";
			case SystemLanguage.German:
				return "German";
			case SystemLanguage.Greek:
				return "Greek";
			case SystemLanguage.Hebrew:
				return "Hebrew";
			case SystemLanguage.Hungarian:
				return "Hungarian";
			case SystemLanguage.Icelandic:
				return "Icelandic";
			case SystemLanguage.Indonesian:
				return "Indonesian";
			case SystemLanguage.Italian:
				return "Italian";
			case SystemLanguage.Japanese:
				return "Japanese";
			case SystemLanguage.Korean:
				return "Korean";
			case SystemLanguage.Latvian:
				return "Latvian";
			case SystemLanguage.Lithuanian:
				return "Lithuanian";
			case SystemLanguage.Norwegian:
				return "Norwegian";
			case SystemLanguage.Polish:
				return "Polish";
			case SystemLanguage.Portuguese:
				return "Portuguese";
			case SystemLanguage.Romanian:
				return "Romanian";
			case SystemLanguage.Russian:
				return "Russian";
			case SystemLanguage.SerboCroatian:
				return "SerboCroatian";
			case SystemLanguage.Slovak:
				return "Slovak";
			case SystemLanguage.Slovenian:
				return "Slovenian";
			case SystemLanguage.Spanish:
				return "Spanish";
			case SystemLanguage.Swedish:
				return "Swedish";
			case SystemLanguage.Thai:
				return "Thai";
			case SystemLanguage.Turkish:
				return "Turkish";
			case SystemLanguage.Ukrainian:
				return "Ukrainian";
			case SystemLanguage.Unknown:
				return "Unknown";
			case SystemLanguage.Vietnamese:
				return "Vietnamese";
#if UNITY_5
			case SystemLanguage.ChineseSimplified:
				return "ChineseSimplified";
			case SystemLanguage.ChineseTraditional:
				return "ChineseTraditional";
#endif
			default:
				return "Unknown";
		}
	}
}
}

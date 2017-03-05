
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SmartLocalization
{
internal class PluralForms 
{
	/// <summary>
	/// Gets the correct plural key according to the number sent in as count.
	/// </summary>
	/// <returns>The plural key. Will return the passed in key if the language isn't supported</returns>
	/// <param name="languageCode">The ISO-639 language code that's currently loaded</param>
	/// <param name="baseKey">The original key without any plural suffixes</param>
	/// <param name="count">The number of occurrences of the value of the key. e.g. 1 apple, 2 apples</param>
	public static string GetPluralKey(string languageCode, string baseKey, int count)
	{
		if(Languages.ContainsKey(languageCode))
		{
			return baseKey + "_" + Languages[languageCode](count).ToString();
		}
		
		return baseKey;
	}
	
	/// <summary>
	/// Gets the plural key with a custom defined plural form method.
	/// </summary>
	/// <returns>The plural key</returns>
	/// <param name="baseKey">The original key without any plural suffixes</param>
	/// <param name="count">The number of occurrences of the value of the key. e.g. 1 apple, 2 apples</param>
	/// <param name="pluralForm">The plural form method to decide what key to use</param>
	public static string GetPluralKey(string baseKey, int count, Func<int, int> pluralForm)
	{
		return baseKey + "_" + pluralForm(count).ToString();
	}
	
	public static Dictionary<string, Func<int, int>> Languages = new Dictionary<string, Func<int, int>>
	{
		{ "ar", (n) => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 ? 4 : 5 }, //Arabic
		{ "bg", (n) => n != 1 ? 1 : 0 }, //Bulgarian
		{ "ca", (n) => n != 1 ? 1 : 0 }, //Catalan
		{ "zh-CHS", (n) => 0 }, //Chinese
		{ "cs", (n) => (n == 1) ? 0 : (n >= 2 && n <= 4) ? 1 : 2 }, //Czech
		{ "da", (n) => n != 1 ? 1 : 0 }, //Danish
		{ "de", (n) => n != 1 ? 1 : 0 }, //German
		{ "el", (n) => n != 1 ? 1 : 0 }, //Greek
		{ "en", (n) => n != 1 ? 1 : 0 }, //English
		{ "es", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "fi", (n) => n != 1 ? 1 : 0 }, //Finnish
		{ "fr", (n) => n > 1 ? 1 : 0 }, //French
		{ "he", (n) => n != 1 ? 1 : 0 }, //Hebrew
		{ "hu", (n) => n != 1 ? 1 : 0 }, //Hungarian
		{ "is", (n) => (n % 10 != 1 || n % 100 == 11) ? 1 : 0 }, //Icelandic
		{ "it", (n) => n != 1 ? 1 : 0 }, //Italian
		{ "ja", (n) => 0 }, //Japanese
		{ "ko", (n) => 0 }, //Korean
		{ "nl", (n) => n != 1 ? 1 : 0 }, //Dutch
		{ "no", (n) => n != 1 ? 1 : 0 }, //Norwegian
		{ "pl", (n) => n == 1 ? 0 : n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2 }, //Polish
		{ "pt", (n) => n != 1 ? 1 : 0 }, //Portuguese
		{ "ro", (n) => n == 1 ? 0 : (n == 0 || (n % 100 > 0 && n % 100 < 20)) ? 1 : 2 }, //Romanian
		{ "ru", (n) => n % 10 == 1 && n % 100 != 11 ? 0 : n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2 }, //Russian
		{ "hr", (n) => n % 10 == 1 && n % 100 != 11 ? 0 : n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2 }, //Croatian
		{ "sk", (n) => (n == 1) ? 0 : (n >= 2 && n <= 4) ? 1 : 2 }, //Slovak
		{ "sq", (n) => n != 1 ? 1 : 0 }, //Albanian
		{ "sv", (n) => n != 1 ? 1 : 0 }, //Swedish
		{ "th", (n) => 0 }, //Thai
		{ "tr", (n) => n > 1 ? 1 : 0 }, //Turkish
		{ "id", (n) => 0 }, //Indonesian
		{ "uk", (n) => n % 10 == 1 && n % 100 != 11 ? 0 : n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2 }, //Ukrainian
		{ "be", (n) => n % 10 == 1 && n % 100 != 11 ? 0 : n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2 }, //Belarusian
		{ "sl", (n) => n % 100 == 1 ? 1 : n % 100 == 2 ? 2 : n % 100 == 3 || n % 100 == 4 ? 3 : 0 }, //Slovenian
		{ "et", (n) => n != 1 ? 1 : 0 }, //Estonian
		{ "lv", (n) => n % 10 == 1 && n % 100 != 11 ? 0 : n != 0 ? 1 : 2 }, //Latvian
		{ "lt", (n) => n % 10 == 1 && n % 100 != 11 ? 0 : n % 10 >= 2 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2 }, //Lithuanian
		{ "fa", (n) => 0 }, //Persian
		{ "vi", (n) => 0 }, //Vietnamese
		{ "hy", (n) => n != 1 ? 1 : 0 }, //Armenian
		{ "eu", (n) => n != 1 ? 1 : 0 }, //Basque
		{ "mk", (n) => (n == 1 || n % 10 == 1) ? 1 : (n == 2 || n % 10 == 2) ? 2 : 0 }, //Macedonian
		{ "af", (n) => n != 1 ? 1 : 0 }, //Afrikaans
		{ "ka", (n) => 0 }, //Georgian
		{ "fo", (n) => n != 1 ? 1 : 0 }, //Faroese
		{ "hi", (n) => n != 1 ? 1 : 0 }, //Hindi
		{ "sw", (n) => n != 1 ? 1 : 0 }, //Swahili
		{ "gu", (n) => n != 1 ? 1 : 0 }, //Gujarati
		{ "ta", (n) => n != 1 ? 1 : 0 }, //Tamil
		{ "te", (n) => n != 1 ? 1 : 0 }, //Telugu
		{ "kn", (n) => n != 1 ? 1 : 0 }, //Kannada
		{ "mr", (n) => n != 1 ? 1 : 0 }, //Marathi
		{ "gl", (n) => n != 1 ? 1 : 0 }, //Gallegan -------------------------------
		{ "kok", (n) => n != 1 ? 1 : 0 }, //Konkani -------------------------------
		{ "ar-SA", (n) => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 ? 4 : 5 }, //Arabic
		{ "bg-BG", (n) => n != 1 ? 1 : 0 }, //Bulgarian
		{ "ca-ES", (n) => n != 1 ? 1 : 0 }, //Catalan
		{ "zh-TW", (n) => 0 }, //Chinese
		{ "cs-CZ", (n) => (n == 1) ? 0 : (n >= 2 && n <= 4) ? 1 : 2 }, //Czech
		{ "da-DK", (n) => n != 1 ? 1 : 0 }, //Danish
		{ "de-DE", (n) => n != 1 ? 1 : 0 }, //German
		{ "el-GR", (n) => n != 1 ? 1 : 0 }, //Greek
		{ "en-US", (n) => n != 1 ? 1 : 0 }, //English
		{ "fi-FI", (n) => n != 1 ? 1 : 0 }, //Finnish
		{ "fr-FR", (n) => n > 1 ? 1 : 0 }, //French
		{ "he-IL", (n) => n != 1 ? 1 : 0 }, //Hebrew
		{ "hu-HU", (n) => n != 1 ? 1 : 0 }, //Hungarian
		{ "is-IS", (n) => (n % 10 != 1 || n % 100 == 11) ? 1 : 0 }, //Icelandic
		{ "it-IT", (n) => n != 1 ? 1 : 0 }, //Italian
		{ "ja-JP", (n) => 0 }, //Japanese
		{ "ko-KR", (n) => 0 }, //Korean
		{ "nl-NL", (n) => n != 1 ? 1 : 0 }, //Dutch
		{ "nb-NO", (n) => n != 1 ? 1 : 0 }, //Norwegian
		{ "pl-PL", (n) => n == 1 ? 0 : n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2 }, //Polish
		{ "pt-BR", (n) => n > 1 ? 1 : 0 }, //Portuguese
		{ "ro-RO", (n) => n == 1 ? 0 : (n == 0 || (n % 100 > 0 && n % 100 < 20)) ? 1 : 2 }, //Romanian
		{ "ru-RU", (n) => n % 10 == 1 && n % 100 != 11 ? 0 : n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2 }, //Russian
		{ "hr-HR", (n) => n % 10 == 1 && n % 100 != 11 ? 0 : n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2 }, //Croatian
		{ "sk-SK", (n) => (n == 1) ? 0 : (n >= 2 && n <= 4) ? 1 : 2 }, //Slovak
		{ "sq-AL", (n) => n != 1 ? 1 : 0 }, //Albanian
		{ "sv-SE", (n) => n != 1 ? 1 : 0 }, //Swedish
		{ "th-TH", (n) => 0 }, //Thai
		{ "tr-TR", (n) => n > 1 ? 1 : 0 }, //Turkish
		{ "id-ID", (n) => 0 }, //Indonesian
		{ "uk-UA", (n) => n % 10 == 1 && n % 100 != 11 ? 0 : n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2 }, //Ukrainian
		{ "be-BY", (n) => n % 10 == 1 && n % 100 != 11 ? 0 : n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2 }, //Belarusian
		{ "sl-SI", (n) => n % 100 == 1 ? 1 : n % 100 == 2 ? 2 : n % 100 == 3 || n % 100 == 4 ? 3 : 0 }, //Slovenian
		{ "et-EE", (n) => n != 1 ? 1 : 0 }, //Estonian
		{ "lv-LV", (n) => n % 10 == 1 && n % 100 != 11 ? 0 : n != 0 ? 1 : 2 }, //Latvian
		{ "lt-LT", (n) => n % 10 == 1 && n % 100 != 11 ? 0 : n % 10 >= 2 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2 }, //Lithuanian
		{ "fa-IR", (n) => 0 }, //Persian
		{ "vi-VN", (n) => 0 }, //Vietnamese
		{ "hy-AM", (n) => n != 1 ? 1 : 0 }, //Armenian
		{ "eu-ES", (n) => n != 1 ? 1 : 0 }, //Basque
		{ "mk-MK", (n) => (n == 1 || n % 10 == 1) ? 1 : (n == 2 || n % 10 == 2) ? 2 : 0}, //Macedonian
		{ "af-ZA", (n) => n != 1 ? 1 : 0 }, //Afrikaans
		{ "ka-GE", (n) => 0 }, //Georgian
		{ "fo-FO", (n) => n != 1 ? 1 : 0 }, //Faroese
		{ "hi-IN", (n) => n != 1 ? 1 : 0 }, //Hindi
		{ "sw-KE", (n) => n != 1 ? 1 : 0 }, //Swahili
		{ "gu-IN", (n) => n != 1 ? 1 : 0 }, //Gujarati
		{ "ta-IN", (n) => n != 1 ? 1 : 0 }, //Tamil
		{ "te-IN", (n) => n != 1 ? 1 : 0 }, //Telugu
		{ "kn-IN", (n) => n != 1 ? 1 : 0 }, //Kannada
		{ "mr-IN", (n) => n != 1 ? 1 : 0 }, //Marathi
		{ "gl-ES", (n) => n != 1 ? 1 : 0 }, //Gallegan -------------------------------
		{ "kok-IN", (n) => n != 1 ? 1 : 0 }, //Konkani -------------------------------
		{ "ar-IQ", (n) => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 ? 4 : 5 }, //Arabic
		{ "zh-CN", (n) => 0 }, //Chinese
		{ "de-CH", (n) => n != 1 ? 1 : 0 }, //German
		{ "en-GB", (n) => n != 1 ? 1 : 0 }, //English
		{ "es-MX", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "fr-BE", (n) => n > 1 ? 1 : 0 }, //French
		{ "it-CH", (n) => n != 1 ? 1 : 0 }, //Italian
		{ "nl-BE", (n) => n != 1 ? 1 : 0 }, //Dutch
		{ "nn-NO", (n) => n != 1 ? 1 : 0 }, //Norwegian
		{ "pt-PT", (n) => n != 1 ? 1 : 0 }, //Portuguese
		{ "sv-FI", (n) => n != 1 ? 1 : 0 }, //Swedish
		{ "ar-EG", (n) => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 ? 4 : 5 }, //Arabic
		{ "zh-HK", (n) => 0 }, //Chinese
		{ "de-AT", (n) => n != 1 ? 1 : 0 }, //German
		{ "en-AU", (n) => n != 1 ? 1 : 0 }, //English
		{ "es-ES", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "fr-CA", (n) => n > 1 ? 1 : 0 }, //French
		{ "ar-LY", (n) => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 ? 4 : 5 }, //Arabic
		{ "zh-SG", (n) => 0 }, //Chinese
		{ "de-LU", (n) => n != 1 ? 1 : 0 }, //German
		{ "en-CA", (n) => n != 1 ? 1 : 0 }, //English
		{ "es-GT", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "fr-CH", (n) => n > 1 ? 1 : 0 }, //French
		{ "ar-DZ", (n) => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 ? 4 : 5 }, //Arabic
		{ "zh-MO", (n) => 0 }, //Chinese
		{ "en-NZ", (n) => n != 1 ? 1 : 0 }, //English
		{ "es-CR", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "fr-LU", (n) => n > 1 ? 1 : 0 }, //French
		{ "ar-MA", (n) => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 ? 4 : 5 }, //Arabic
		{ "en-IE", (n) => n != 1 ? 1 : 0 }, //English
		{ "es-PA", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "ar-TN", (n) => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 ? 4 : 5 }, //Arabic
		{ "en-ZA", (n) => n != 1 ? 1 : 0 }, //English
		{ "es-DO", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "ar-OM", (n) => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 ? 4 : 5 }, //Arabic
		{ "es-VE", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "ar-YE", (n) => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 ? 4 : 5 }, //Arabic
		{ "es-CO", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "ar-SY", (n) => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 ? 4 : 5 }, //Arabic
		{ "es-PE", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "ar-JO", (n) => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 ? 4 : 5 }, //Arabic
		{ "en-TT", (n) => n != 1 ? 1 : 0 }, //English
		{ "es-AR", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "ar-LB", (n) => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 ? 4 : 5 }, //Arabic
		{ "en-ZW", (n) => n != 1 ? 1 : 0 }, //English
		{ "es-EC", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "ar-KW", (n) => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 ? 4 : 5 }, //Arabic
		{ "en-PH", (n) => n != 1 ? 1 : 0 }, //English
		{ "es-CL", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "ar-AE", (n) => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 ? 4 : 5 }, //Arabic
		{ "es-UY", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "ar-BH", (n) => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 ? 4 : 5 }, //Arabic
		{ "es-PY", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "ar-QA", (n) => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 ? 4 : 5 }, //Arabic
		{ "es-BO", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "es-SV", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "es-HN", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "es-NI", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "es-PR", (n) => n != 1 ? 1 : 0 }, //Spanish
		{ "zh-CHT", (n) => 0 }, //Chinese
		{ "ms", (n) => 0 } //Malaysia
	};
}
}//namespace SmartLocalization
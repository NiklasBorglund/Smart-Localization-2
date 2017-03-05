// IAutomaticTranslator.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//

namespace SmartLocalization.Editor
{
using System.Collections.Generic;

/// <summary>
/// Event handler for initializing an automatic translator
/// </summary>
/// <param name="success">If the translator was initialized</param>
public delegate void InitializeTranslatorEventHandler(bool success);
/// <summary>
/// Event handler for getting the available translation languages for an Automatic Translator
/// </summary>
/// <param name="success">If the fetching operation was a success</param>
/// <param name="availableLanguages">A list of all the available languages</param>
public delegate void GetAvailableLanguagesEventHandler(bool success, List<string> availableLanguages);
/// <summary>
/// Event handler for translating a single string with an Automatic Translator
/// </summary>
/// <param name="success">If the translation was a successful operation</param>
/// <param name="key">The key of the translated text. Used for identification</param>
/// <param name="translatedText">The translated text</param>
public delegate void TranslateTextEventHandler(bool success, string key, string translatedText);
/// <summary>
/// Event handler for translating an array of strings with an Automatic Translator
/// </summary>
/// <param name="success">If the translation was a successful operation</param>
/// <param name="keys">A list of the translated keys. The indices in this list correspond with the list of translated values.</param>
/// <param name="translatedValues">A list of the translated values. The indices in this list correspond with the list of keys.</param>
public delegate void TranslateTextArrayEventHandler(bool success, List<string> keys, List<string> translatedValues);

/// <summary>
/// Generic Interface for Automatic Translators
/// </summary>
public interface IAutomaticTranslator 
{
	/// <summary>
	/// Returns if the Automatic Translator is currently in the process of initializing
	/// </summary>
	bool IsInitializing {get;}
	/// <summary>
	/// Returns if the Automatic Translator is initialized
	/// </summary>
	bool IsInitialized {get;}
	/// <summary>
	/// Returns if the Translator is not Initialized anymore due to an expiration of an access token
	/// </summary>
	bool InitializationDidExpire{get;}
	/// <summary>
	/// Initializes the Automatic Translator
	/// </summary>
	/// <param name="eventHandler">The Event handler returning success/failure of the operation</param>
	/// <param name="cliendID">The client API ID</param>
	/// <param name="clientSecret">The client API Secret</param>
	void Initialize(InitializeTranslatorEventHandler eventHandler, string cliendID, string clientSecret);
	/// <summary>
	/// Gets all the available languages that can be translated.
	/// </summary>
	/// <param name="eventHandler">Event handler returning the success/failure along with a list of available languages</param>
	void GetAvailableTranslationLanguages(GetAvailableLanguagesEventHandler eventHandler);
	/// <summary>
	/// Translate a single string
	/// </summary>
	/// <param name="eventHandler">The event handler returning the translated value</param>
	/// <param name="key">The key used for identification</param>
	/// <param name="textToTranslate">The text to translate</param>
	/// <param name="languageFrom">The language code to translate from</param>
	/// <param name="languageTo">The language code to translate to</param>
	void TranslateText(TranslateTextEventHandler eventHandler, string key, string textToTranslate, string languageFrom, string languageTo);
	
	/// <summary>
	///  Translate an array of texts to a specific language. Note: the event handler can be called multiple times if splitting of the array is necessary 
	/// </summary>
	/// <param name="eventHandler">The event handler returning the translated values. Can be called multiple times.</param>
	/// <param name="keys">A list of keys used for identification. Make sure the indices correspond with the list of translated values.</param>
	/// <param name="textsToTranslate">A list of strings to translate. Make sure the indices correspond with the list of keys.</param>
	/// <param name="languageFrom">The language code to translate from</param>
	/// <param name="languageTo">The language code to translate to</param>
	void TranslateTextArray(TranslateTextArrayEventHandler eventHandler, List<string> keys, List<string> textsToTranslate, string languageFrom, string languageTo);

}
} //namespace SmartLocalization.Editor
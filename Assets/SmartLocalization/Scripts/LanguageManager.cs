//
//  LanguageManager.cs
//
//
// Written by Niklas Borglund and Jakob Hillerström
//

namespace SmartLocalization
{
using UnityEngine;
using System.Collections.Generic;
using System;

	/// <summary>
/// Change language event handler.
/// </summary>
public delegate void ChangeLanguageEventHandler(LanguageManager thisLanguageManager);

/// <summary>
/// The language worker class for runtime language handling
/// </summary>
public class LanguageManager : MonoBehaviour, ISerializationCallbackReceiver
{
    private static LanguageManager instance = null;

    /// <summary>
    /// Returns an instance of the language manager. Creates a new one if no one exist. 
    /// If you don't want to create a new one, check the LanguageManager.HasInstance variable first.
    /// NOTE: This is not set to DontDestroyOnLoad. You will have to do that yourself.
    /// Call LanguageManager.SetDontDestroyOnLoad
    /// </summary>
    public static LanguageManager Instance
    {
        get
        {
			if((Application.platform == RuntimePlatform.OSXEditor ||
			   Application.platform == RuntimePlatform.WindowsEditor) && instance == null){
					instance = FindObjectOfType<LanguageManager>();	
			}
            if (instance == null)
            {
				if(!IsQuitting)
				{
					GameObject go = new GameObject();
					instance = go.AddComponent<LanguageManager>();
					go.name = "LanguageManager";

					if(!DidSetDontDestroyOnLoad && DontDestroyOnLoadToggle)
					{
						GameObject.DontDestroyOnLoad(instance);
						DidSetDontDestroyOnLoad = true;
					}
				}
            }

            return instance;
        }
    }

	/// <summary>
	/// Sets DontDestroyOnLoad flag on the LanguageManager object. 
	/// </summary>
	public static void SetDontDestroyOnLoad()
	{
		DontDestroyOnLoadToggle = true;

		if(instance != null && !DidSetDontDestroyOnLoad)
		{
			GameObject.DontDestroyOnLoad(instance);
			DidSetDontDestroyOnLoad = true;
		}
	}

	/// <summary>
	/// Returns if there is an active instance of the language manager in the game.
	/// </summary>
	public static bool HasInstance
	{
		get
		{
			return (instance != null);
		}
	}

	static bool IsQuitting = false;
	static bool DontDestroyOnLoadToggle = false;
	static bool DidSetDontDestroyOnLoad = false;


	[SerializeField, HideInInspector]List<string> serializedKeys = null;
	[SerializeField, HideInInspector]List<LocalizedObject> serializedValues = null;
	[SerializeField, HideInInspector]SmartCultureInfo serializedCulture = null;

	public void OnAfterDeserialize()
	{
		if(serializedKeys == null)
		{
			return;
		}

		languageDataHandler.LoadedValuesDictionary = new SortedDictionary<string,LocalizedObject>();

		for(int i = 0; i < serializedKeys.Count; ++i)
		{
			languageDataHandler.LoadedValuesDictionary.Add(serializedKeys[i], serializedValues[i]);
		}
		
		languageDataHandler.LoadedCulture = serializedCulture;

		serializedKeys.Clear();
		serializedValues.Clear();
		serializedCulture = null;
	}

	public void OnBeforeSerialize()
	{
		if(serializedKeys == null)
		{
			serializedKeys = new List<string>();
		}
		if(serializedValues == null)
		{
			serializedValues = new List<LocalizedObject>();
		}

		serializedKeys.Clear();
		serializedValues.Clear();

		if(languageDataHandler.LoadedValuesDictionary == null)
		{
			return;
		}

		foreach(var pair in languageDataHandler.LoadedValuesDictionary)
		{
			serializedKeys.Add(pair.Key);
			serializedValues.Add(pair.Value);
		}
		serializedCulture = CurrentlyLoadedCulture;
	}


	/// <summary>
	/// Occurs when a new language is loaded and initialized
	/// create a delegate method(ChangeLanguage(LanguageManager languageManager)) and subscribe
	/// </summary>
	public ChangeLanguageEventHandler OnChangeLanguage;
		
	public string defaultLanguage = "en";
	
	[SerializeField] SmartCultureInfoCollection	availableLanguages = null;	
	LanguageDataHandler languageDataHandler = new LanguageDataHandler();

	[Obsolete("This will be removed in the future. Use LanguageManager.GetTextValue and LanguageManager.GetAllKeys")]
	public SortedDictionary<string, LocalizedObject> LanguageDatabase
	{
		get
		{
			return languageDataHandler.LoadedValuesDictionary;
		}
	}
	
	/// <summary> Gets all the language values in raw text. NOTE: Potentially performance expensive. Use with caution. </summary>
	[Obsolete("This will be removed in the future. Use LanguageManager.GetTextValue and LanguageManager.GetAllKeys")]
	public Dictionary<string, string> RawTextDatabase
	{
		get
		{
			if(languageDataHandler.LoadedValuesDictionary == null)
			{
				return null;
			}

			var rawTextDictionary = new Dictionary<string, string>();
			foreach(var pair in languageDataHandler.LoadedValuesDictionary)
			{
				rawTextDictionary.Add(pair.Key, pair.Value.TextValue);
			}
			return rawTextDictionary;
		}
	}

	/// <summary>
	/// The number of supported languages in this project
	/// </summary>
	public int NumberOfSupportedLanguages
	{
		get
		{
			if(availableLanguages == null)
			{
				return 0;
			}

			return availableLanguages.cultureInfos.Count;
		}
	}

	[Obsolete("Use CurrentlyLoadedCulture")]
	public string LoadedLanguage
	{
		get
		{
			return CurrentlyLoadedCulture.languageCode;
		}
	}
	
	/// <summary>
	/// Gets the currently loaded SmartCultureInfo.
	/// </summary>
	/// <value>The currently loaded culture.</value>
	public SmartCultureInfo CurrentlyLoadedCulture
	{
		get
		{
			return languageDataHandler.LoadedCulture;
		}
	}

	/// <summary>
	/// Set this to true if you want extensive error logging
	/// </summary>
	public bool VerboseLogging
	{
		get
		{
			return languageDataHandler.VerboseLogging;
		}
		set
		{
			languageDataHandler.VerboseLogging = value;
		}
	}

	void Awake ()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if(instance != this)
		{
			if(VerboseLogging)
			{
				Debug.LogError("Found duplicate LanguageManagers! Removing one of them");
			}

			Destroy(this);
			return;
		}
		
		if(LoadAvailableCultures())
		{
			if(VerboseLogging)
			{
				Debug.Log ("LanguageManager.cs: Waking up");
			}	

			if(availableLanguages.cultureInfos.Count > 0)
			{
				SmartCultureInfo defaultLanguageCulture = availableLanguages.cultureInfos.Find(info => info.languageCode == defaultLanguage);
				
				if(defaultLanguageCulture != null)
				{
					ChangeLanguage(defaultLanguageCulture);
				}
				else
				{		
					//otherwise - load the first language in the list
					ChangeLanguage(availableLanguages.cultureInfos[0]);
					defaultLanguage = availableLanguages.cultureInfos[0].languageCode;
				}
			}
			else
			{
				Debug.LogError("LanguageManager.cs: No language is available! Use Window->Smart Localization tool to create a language");	
			}
		}
		else
		{
			Debug.LogError("LanguageManager.cs: No localization workspace is created! Use Window->Smart Localization tool to create one");
		}
	}

	void OnDestroy()
	{
		//Clear the event handler
		OnChangeLanguage = null;
	}

	void OnApplicationQuit()
	{
		IsQuitting = true;
	}

	bool LoadAvailableCultures()
	{
		TextAsset availableCulturesXML = Resources.Load(LanguageRuntimeData.AvailableCulturesFilePath()) as TextAsset;

		if(availableCulturesXML == null)
		{
			Debug.LogError("Could not load available languages! No such file!");
			return false;
		}

		availableLanguages = SmartCultureInfoCollection.Deserialize(availableCulturesXML);

		return true;
	}
	
	/// <summary>
	/// Creates and returns a new List<string> containing all language keys currently loaded into the system
	/// </summary>m>
	public List<string> GetAllKeys()
	{
		return languageDataHandler.GetAllKeys();
	}
		
	/// <summary>
	/// Change the language into a specified culture
	/// </summary>
	/// <param name="cultureInfo">The culture to change to</param>
	public void ChangeLanguage(SmartCultureInfo cultureInfo)
	{
		ChangeLanguage(cultureInfo.languageCode);
	}
	
	/// <summary>
	/// Change the language into a specified culture
	/// </summary>
	/// <param name="languageCode">The culture to change to</param>
	public void ChangeLanguage(string languageCode)
	{
		TextAsset languageData = Resources.Load(LanguageRuntimeData.LanguageFilePath(languageCode)) as TextAsset;
		
		if(languageData == null)
		{
			Debug.LogError("Failed to load language: " + languageCode);
			return;
		}
		
		LoadLanguage(languageData.text, languageCode);
		
		if(OnChangeLanguage != null)
		{
			OnChangeLanguage(this);	
		}
	}
		
	/// <summary>
	/// Tries to load a language with the specified data. Loading data from outside the project currently only supports text unless
	/// the asset is apparent(and correctly named in the right folder) in the workspace.
	/// </summary>
	/// <param name="languageDataInResX">The language data to load in the .resx format</param>
	/// <param name="languageCode">The ISO-639 code of the language to load</param>
	public void ChangeLanguageWithData(string languageDataInResX, string languageCode)
	{
		if(LoadLanguage(languageDataInResX, languageCode))
		{
			if(OnChangeLanguage != null){
				OnChangeLanguage(this);	
			}
		}
	}

	/// <summary>
	/// Appends the current language with text data. The method does not support appending localized assets.
	/// If duplicates are found, the old value <b>will be overwritten</b> with the new one.
	/// </summary>
	/// <param name="languageDataInResX">The language data to append in the .resx format</param>
	/// <returns>Returns if the operation was a success</returns>
	public bool AppendLanguageWithTextData(string languageDataInResX)
	{
		return languageDataHandler.Append(languageDataInResX);
	}

	bool LoadLanguage(string languageData, string languageCode)
	{	
		if(string.IsNullOrEmpty(languageData))
		{
			Debug.LogError("Failed to load language with ISO-639 code. Data was null or empty");
			return false;
		}
		var cultureToLoad = GetCultureInfo(languageCode);
		if(cultureToLoad == null)
		{
			Debug.LogError("Failed to load language with ISO-639 code: " + languageCode + ". Unable to find a corresponding SmartCultureInfo");
			return false;
		}
		
		if(languageDataHandler.Load(languageData))
		{
			languageDataHandler.LoadedCulture = cultureToLoad;
			return true;
		}
		return false;
	}
		
	
	[Obsolete("Use IsCultureSupported")]
	public bool IsLanguageSupported(string languageCode)
	{
		return IsCultureSupported(languageCode);
	}
	
	[Obsolete("Use IsCultureSupported")]
	public bool IsLanguageSupported(SmartCultureInfo cultureInfo)
	{
		return IsLanguageSupported(cultureInfo.languageCode);
	}
	
	[Obsolete("Use GetDeviceCultureIfSupported")]
	public SmartCultureInfo GetSupportedSystemLanguage()
	{
		return GetDeviceCultureIfSupported();
	}
	
	[Obsolete]
	public string GetSupportedSystemLanguageCode()
	{
		var deviceLanguage = GetDeviceCultureIfSupported();
		return deviceLanguage != null ? deviceLanguage.languageCode : string.Empty;
	}
	
	/// <summary>
	/// Gets the SmartCultureInfo of the current device language IF it's supported by the application. Otherwise it will return null
	/// </summary>
	public SmartCultureInfo GetDeviceCultureIfSupported()
	{
		if(availableLanguages == null)
		{
			return null;
		}
		
		string englishName = GetSystemLanguageEnglishName();
		
		return availableLanguages.cultureInfos.Find(info => info.englishName.ToLower() == englishName.ToLower());
	}
	
	/// <summary>
	/// Checks if the culture is supported by this application using the ISO-639 code
	/// </summary>
	public bool IsCultureSupported(string languageCode)
	{
		if(availableLanguages == null)
		{
			Debug.LogError("LanguageManager is not initialized properly!");
			return false;	
		}
		
		return availableLanguages.cultureInfos.Find(info => info.languageCode == languageCode) != null;
	}
	
	/// <summary>
	/// Checks if a culture is supported by this application
	/// </summary>
	/// <param name="cultureInfo">The culture info to check</param>
	/// <returns>If the culture is supported</returns>
	public bool IsCultureSupported(SmartCultureInfo cultureInfo)
	{
		return IsCultureSupported(cultureInfo.languageCode);
	}

	/// <summary>
	/// Checks if the language is supported with the English name. i.e. "English" "French" etc.
	/// </summary>
	/// <param name="englishName">The English name of the language</param>
	/// <returns>If the language is supported</returns>
	public bool IsLanguageSupportedEnglishName(string englishName)
	{
		if(availableLanguages == null)
		{
			Debug.LogError("LanguageManager is not initialized properly!");
			return false;	
		}
		
		return availableLanguages.cultureInfos.Find(info => info.englishName.ToLower() == englishName.ToLower()) != null;
	}


	
	/// <summary>
	/// Gets the culture info from the available languages list. If the language is not supported it will return null
	/// </summary>
	public SmartCultureInfo GetCultureInfo(string languageCode)
	{
		return availableLanguages.cultureInfos.Find(info => info.languageCode == languageCode);
	}

	/// <summary>
	/// Gets the name of the system language in an English name. 
	/// If its SystemLanguage.Unknown, a string with the value "Unknown" will be returned.
	/// </summary>
	public string GetSystemLanguageEnglishName()
	{
		return ApplicationExtensions.GetSystemLanguage();
	}

	/// <summary> Returns a list of all the supported languages in the project </summary>
	public List<SmartCultureInfo> GetSupportedLanguages()
	{
		if(availableLanguages == null)
		{
			Debug.LogError("LanguageManager is not initialized properly! Cannot find available languages!");
			return null;	
		}

		return availableLanguages.cultureInfos;
	}
	
	/// <summary>
	/// Returns a list of keys within a specified category.
	/// Example: If you have keys named My.Key, the category would be "My."
	/// </summary>
	/// <returns>A list of keys that starts with the current category key</returns>
	/// <param name="category">If you have keys named My.Key, the category would be "My."</param>
	public List<string> GetKeysWithinCategory(string category)
	{
		return languageDataHandler.GetKeysWithinCategory(category);
	}
	
	/// <summary>
	/// Returns a text value in the current language for the key. Returns null if nothing is found.
	/// </summary>
    public string GetTextValue(string key)
    {
		return languageDataHandler.GetTextValue(key);
    }
	
	/// <summary>
	/// Returns a text value in the current language for the key with plural forms. Returns null if nothing is found.
	/// </summary>
	/// <returns>The text value.</returns>
	public string GetTextValue(string key, int count)
	{
		return GetTextValue(PluralForms.GetPluralKey(languageDataHandler.LoadedCulture.languageCode, key, count));
	}
	
	/// <summary>
	/// Returns a text asset in the current language for the key with plural forms. Returns null if nothing is found.
	/// </summary>
	/// <returns>The text asset.</returns>
	public TextAsset GetTextAsset(string key, int count)
	{
		return GetTextAsset(PluralForms.GetPluralKey(languageDataHandler.LoadedCulture.languageCode, key, count));
	}
	
	/// <summary>
	/// Returns an audio clip in the current language for the key with plural forms. Returns null if nothing is found.
	/// </summary>
	/// <returns>The audio clip.</returns>
	public AudioClip GetAudioClip(string key, int count)
	{
		return GetAudioClip(PluralForms.GetPluralKey(languageDataHandler.LoadedCulture.languageCode, key, count));
	}
	
	/// <summary>
	/// Returns a prefab in the current language for the key with plural forms. Returns null if nothing is found.
	/// </summary>
	/// <returns>The prefab.</returns>
	public GameObject GetPrefab(string key, int count)
	{
		return GetPrefab(PluralForms.GetPluralKey(languageDataHandler.LoadedCulture.languageCode, key, count));
	}
	
	/// <summary>
	/// Returns a texture in the current language for the key with plural forms. Returns null if nothing is found.
	/// </summary>
	/// <returns>The texture.</returns>
	public Texture GetTexture(string key, int count)
	{
		return GetTexture(PluralForms.GetPluralKey(languageDataHandler.LoadedCulture.languageCode, key, count));
	}
	
	/// <summary>
	/// Returns a text value in the current language for the key with a custom plural form. Returns null if nothing is found.
	/// </summary>
	/// <returns>The text value.</returns>
	public string GetTextValue(string key, int count, Func<int, int> pluralForm)
	{
		return GetTextValue(PluralForms.GetPluralKey(key, count, pluralForm));
	}
	
	/// <summary>
	/// Returns a text asset in the current language for the key with a custom plural form. Returns null if nothing is found.
	/// </summary>
	/// <returns>The text asset.</returns>
	public TextAsset GetTextAsset(string key, int count, Func<int, int> pluralForm)
	{
		return GetTextAsset(PluralForms.GetPluralKey(key, count, pluralForm));
	}
	
	/// <summary>
	/// Returns a font in the current language for the key with a custom plural form. Returns null if nothing is found.
	/// </summary>
	/// <returns>The localized font</returns>
	public Font GetFont(string key, int count, Func<int, int> pluralForm)
	{
		return GetFont(PluralForms.GetPluralKey(key, count, pluralForm));
	}
	
	/// <summary>
	/// Returns an audio clip in the current language for the key with a custom plural form. Returns null if nothing is found.
	/// </summary>
	/// <returns>The audio clip.</returns>
	public AudioClip GetAudioClip(string key, int count, Func<int, int> pluralForm)
	{
		return GetAudioClip(PluralForms.GetPluralKey(key, count, pluralForm));
	}
	
	/// <summary>
	/// Returns a prefab in the current language for the key with a custom plural form. Returns null if nothing is found.
	/// </summary>
	/// <returns>The prefab.</returns>
	public GameObject GetPrefab(string key, int count, Func<int, int> pluralForm)
	{
		return GetPrefab(PluralForms.GetPluralKey(key, count, pluralForm));
	}
	
	/// <summary>
	/// Returns a texture in the current language for the key with a custom plural form. Returns null if nothing is found.
	/// </summary>
	/// <returns>The texture.</returns>
	public Texture GetTexture(string key, int count, Func<int, int> pluralForm)
	{
		return GetTexture(PluralForms.GetPluralKey(key, count, pluralForm));
	}
	
	/// <summary>
	/// Gets a text asset for the current language, returns null if nothing is found
	/// </summary>
	public TextAsset GetTextAsset(string key)
	{
		return languageDataHandler.GetAsset<TextAsset>(key);
	}
	
	/// <summary>
	/// Gets a Font for the current language, returns null if nothing is found
	/// </summary>
	public Font GetFont(string key)
	{
		return languageDataHandler.GetAsset<Font>(key);
	}
	
	/// <summary>
	/// Gets the audio clip for the current language, returns null if nothing is found
	/// </summary>
	public AudioClip GetAudioClip(string key)
	{
        return languageDataHandler.GetAsset<AudioClip>(key);
	}
	/// <summary>
	/// Gets the prefab game object for the current language, returns null if nothing is found
	/// </summary>
	public GameObject GetPrefab(string key)
	{
		return languageDataHandler.GetAsset<GameObject>(key);
	}
		
	/// <summary>
	/// Gets a texture for the current language, returns null if nothing is found
	/// </summary>
	public Texture GetTexture(string key)
	{
		return languageDataHandler.GetAsset<Texture>(key);
	}

	/// <summary>
	/// Returns whether or not the key is available in the system.
	/// </summary>
	public bool HasKey(string key)
	{
		return languageDataHandler.HasKey(key);
	}
	
	/// <summary>
	/// Gets the localized object from the localizedObjectDataBase
	/// </summary>
	private LocalizedObject GetLocalizedObject(string key)
	{
		return languageDataHandler.GetLocalizedObject(key);
	}
}
}//namespace SmartLocalization
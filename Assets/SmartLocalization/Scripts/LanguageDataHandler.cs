using UnityEngine;
using System.Collections.Generic;

namespace SmartLocalization{
internal class LanguageDataHandler
{
	SortedDictionary<string, LocalizedObject> loadedValuesDictionary;
	bool verboseLogging = false;
	SmartCultureInfo loadedCultureInfo;
	ILocalizedAssetLoader assetLoader;
	
	internal SmartCultureInfo LoadedCulture
	{
		get
		{
			return loadedCultureInfo;
		}
		set
		{
			loadedCultureInfo = value;
		}
	}
	
	internal bool VerboseLogging{
		get
		{
			return verboseLogging;
		}
		set
		{
			verboseLogging = value;
		}
	}
	
	internal SortedDictionary<string, LocalizedObject> LoadedValuesDictionary
	{
		get
		{
			return loadedValuesDictionary;
		}
		set
		{
			loadedValuesDictionary = value;
		}
	}
	
	internal ILocalizedAssetLoader AssetLoader
	{
		get
		{
			if(assetLoader == null)
			{
				assetLoader = new RuntimeLocalizedAssetLoader();
			}
			
			return assetLoader;
		}
		set
		{
			assetLoader = value;
		}
	}
	
	internal LanguageDataHandler()
	{
		loadedValuesDictionary = new SortedDictionary<string, LocalizedObject>();
	}
	
	internal bool Load(string resxData)
	{
		var languageValues = LoadLanguageDictionary(resxData);
		if(languageValues != null && languageValues.Count > 0)
		{
			loadedValuesDictionary = languageValues;
			
			if(verboseLogging)
			{
				Debug.Log("Successfully loaded language");
			}
			
			return true;
		}
		return false;
	}
	
	internal bool Append(string resxData)
	{
		var languageValues = LoadLanguageDictionary(resxData);
		if(languageValues != null && languageValues.Count > 0)
		{
			foreach(var pair in languageValues)
			{
				if(loadedValuesDictionary.ContainsKey(pair.Key))
				{
					loadedValuesDictionary[pair.Key] = pair.Value;
				}
				else
				{
					loadedValuesDictionary.Add(pair.Key, pair.Value);
				}
			}
			
			if(verboseLogging)
			{
				Debug.Log("Successfully appended language with " + languageValues.Count + " values");
			}
			
			return true;
		}
		return false;
	}
	
	internal List<string> GetKeysWithinCategory(string category)
	{
		if(string.IsNullOrEmpty(category) || loadedValuesDictionary == null)
		{
			return new List<string>();
		}
		
		var categoryList = new List<string>();
		foreach(var pair in loadedValuesDictionary)
		{
			if(pair.Key.StartsWith(category))
			{
				categoryList.Add(pair.Key);
			}
		}
		
		return categoryList;
	}
	
	internal List<string> GetAllKeys()
	{
		return new List<string>(loadedValuesDictionary.Keys);
	}
	
	internal LocalizedObject GetLocalizedObject(string key)
	{
		LocalizedObject localizedObject;
		loadedValuesDictionary.TryGetValue(key, out localizedObject);
		return localizedObject;
	}
	
	internal string GetTextValue(string key)
	{
		LocalizedObject localizedObject = GetLocalizedObject(key);
		
		if(localizedObject != null)
		{
			return localizedObject.TextValue;	
		}
			
		if(VerboseLogging)
		{
			Debug.LogError("LanguageManager.cs: Invalid Key:" + key + ". Could not get language value.");
		}
			
		return null;
	}
	
	internal T GetAsset<T>(string key) where T : UnityEngine.Object
	{
		LocalizedObject localizedObject = GetLocalizedObject(key);
		
		if(localizedObject != null)
		{
			return AssetLoader.LoadAsset<T>(key, CheckLanguageOverrideCode(localizedObject));
		}
		
		if(VerboseLogging)
		{
			Debug.LogError("Could not get asset with key: " + key + " as asset type:" + typeof(T).ToString());
		}
			
		return default (T);
	}
	
	internal bool HasKey(string key)
	{
		return GetLocalizedObject(key) != null;
	}
	
	SortedDictionary<string, LocalizedObject> LoadLanguageDictionary(string resxData)
	{
		if(string.IsNullOrEmpty(resxData))
		{
			return null;
		}
		
		return LanguageParser.LoadLanguage(resxData);
	}
	
	/// <summary>
	/// Helper method that checks and gets the language override code of the object is overridden.
	/// If it's not - it will return the currently loaded language.
	/// </summary>
	string CheckLanguageOverrideCode(LocalizedObject localizedObject)
	{
		if(localizedObject == null)
		{
			return loadedCultureInfo.languageCode;
		}
		
		string objectLanguage = localizedObject.OverrideLocalizedObject ? localizedObject.OverrideObjectLanguageCode : loadedCultureInfo.languageCode;
		if(string.IsNullOrEmpty(objectLanguage))
		{
			objectLanguage = loadedCultureInfo.languageCode;
		}
		
		return objectLanguage;
	}
}
}

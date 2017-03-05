//LanguageHandlerEditor.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//
namespace SmartLocalization.Editor
{
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using UnityEditor;
using System.Text;

/// <summary>
/// Utility class for handling language files in the editor
/// </summary>
public static class LanguageHandlerEditor 
{
#region Lookups

	/// <summary>
	/// Gets a SmartCultureInfoCollection with all the cultures not available in this workspace
	/// </summary>
	/// <param name="allCultures">The list of all the available cultures</param>
	/// <returns>A SmartCultureInfoCollection with all the cultures not available in this workspace</returns>
	public static SmartCultureInfoCollection GetNonAvailableLanguages(SmartCultureInfoCollection allCultures)
	{
		SmartCultureInfoCollection nonCreatedLanguages = new SmartCultureInfoCollection();
		
		foreach(SmartCultureInfo cultureInfo in allCultures.cultureInfos)
		{
			if(!FileUtility.Exists(LocalizationWorkspace.LanguageFilePath(cultureInfo.languageCode)))
			{
				nonCreatedLanguages.AddCultureInfo(cultureInfo);
			}
		}
		
		return nonCreatedLanguages;
	}

	/// <summary>
	/// Checks all the created languages and Saves the AvailableCultures xml.
	/// </summary>
	/// <param name="allCultures">A list of all the available cultures</param>
	/// <returns>A list of all the created languages</returns>
	public static SmartCultureInfoCollection CheckAndSaveAvailableLanguages(SmartCultureInfoCollection allCultures)
	{
		SmartCultureInfoCollection createdCultures = new SmartCultureInfoCollection();
		
		foreach(SmartCultureInfo cultureInfo in allCultures.cultureInfos)
		{
			if(FileUtility.Exists(LocalizationWorkspace.LanguageFilePath(cultureInfo.languageCode)))
			{
				createdCultures.AddCultureInfo(cultureInfo);
			}
		}

		createdCultures.Serialize(LocalizationWorkspace.AvailableCulturesFilePath());
		
		return createdCultures;
	}
	
	/// <summary>
	/// Returns a list of keys within a specified category.
	/// Example: If you have keys named My.Key, the category would be "My."
	/// </summary>
	/// <returns>A list of keys that starts with the current category key</returns>
	/// <param name="category">If you have keys named My.Key, the category would be "My."</param>
	public static List<string> GetKeysWithinCategory(string category)
	{
		var categoryList = new List<string>();
		var languageRootDict = LoadParsedLanguageFile(null, true);
		if(string.IsNullOrEmpty(category) || languageRootDict == null)
		{
			return categoryList;
		}
		
		foreach(var pair in languageRootDict)
		{
			if(pair.Key.StartsWith(category)){
				categoryList.Add(pair.Key);
			}
		}
		return categoryList;
	}

#endregion

#region Creating

	/// <summary>
	/// Creates the initial root language file
	/// </summary>
	public static void CreateRootResourceFile()
	{
		//Add a dummy value so that the user will see how everything works
		Dictionary<string,string> baseDictionary = new Dictionary<string, string>();
		baseDictionary.Add("MyFirst.Key", "MyFirstValue");
		
		SaveLanguageFile(baseDictionary, LocalizationWorkspace.RootLanguageFilePath());
	}

	/// <summary>
	/// Creates a new language
	/// </summary>
	/// <param name="languageName">The language code of the language to create</param>
	/// <param name="fromFile">Base language values to create a language from where each list of strings is a row.</param>
	public static void CreateNewLanguage(string languageName, List<List<string>> fromFile = null)
	{
		Dictionary<string,string> rootValues = LanguageHandlerEditor.LoadLanguageFile(null, true);
		
		//Copy the keys over to the new language
		Dictionary<string,string> baseDictionary = new Dictionary<string, string>();
		foreach(KeyValuePair<string, string> keyPair in rootValues)
		{
			baseDictionary.Add(keyPair.Key, "");
		}

		if (fromFile != null)
		{
			foreach (var row in fromFile)
			{
				if (row.Count != 2)
				{
					Debug.LogError("The CSV file is not in the correct format.");
					break;
				}
				baseDictionary[row[0]] = row[1];
			}
		}
		
		//Save the new language file
		SaveLanguageFile(baseDictionary, LocalizationWorkspace.LanguageFilePath(languageName));
	}

	/// <summary>
	/// Creates the serializable localization list from the parsed LocalizedObjects
	/// </summary>
	public static List<SerializableLocalizationObjectPair> CreateSerializableLocalizationList(Dictionary<string, LocalizedObject> languageValues)
	{
		var localizationList = new List<SerializableLocalizationObjectPair>();
		foreach(var languageValue in languageValues)
		{
			localizationList.Add(new SerializableLocalizationObjectPair(languageValue.Key, languageValue.Value));
		}
		return localizationList;
	}
#endregion

#region Saving / Updating
	/// <summary>
	/// Saves the root language file and updates all the available languages.
	/// </summary>
	public static void SaveRootLanguageFile(Dictionary<string,string> changedRootKeys, Dictionary<string,string> changedRootValues, SmartCultureInfoCollection availableCultures)
	{
		//The dictionary with all the final changes
		Dictionary<string,string> changedDictionary = new Dictionary<string, string>();
		
		foreach(KeyValuePair<string,string> changedKey in changedRootKeys)
		{
			if(changedKey.Key == changedKey.Value)
			{
				//The key is not changed, just add the key and the changed value to the new dictionary
				LanguageDictionaryHelper.AddNewKeyPersistent(changedDictionary, changedKey.Key, changedRootValues[changedKey.Key]);
			}
			else
			{
				//Add the new key along with the new changed value
				LanguageDictionaryHelper.AddNewKeyPersistent(changedDictionary, changedKey.Value, changedRootValues[changedKey.Key]);
			}
		}
		
		//Look if any keys were deleted,(so that we can delete the created files)
		List<string> deletedKeys = new List<string>();
		IEnumerable<string> originalKeys = LoadLanguageFile(null, true).Keys;
		foreach(string originalKey in originalKeys)
		{
			bool foundMatch = false;
			foreach(KeyValuePair<string,string> changedKey in changedRootKeys)
			{
				if(originalKey == changedKey.Key)
				{
					foundMatch = true;
					break;
				}
			}
			if(!foundMatch)
			{
				deletedKeys.Add(originalKey);
			}
		}
		
		//Save the language file
		SaveLanguageFile(changedDictionary, LocalizationWorkspace.RootLanguageFilePath());
		
		//Change all the key values for all the translated languages
		var changedCultureValues = new Dictionary<string, string>();
		foreach(var cultureInfo in availableCultures.cultureInfos)
		{
			var currentCultureValues = LoadLanguageFile(cultureInfo.languageCode, false);
			foreach(var changedKey in changedRootKeys)
			{
				string currentValue;
				currentCultureValues.TryGetValue(changedKey.Key, out currentValue);
				if(currentValue == null)
				{
					currentValue = "";
				}
				
				//If the key is changed, we need to change the asset names as well
				if(changedKey.Key != changedKey.Value && currentValue != "")
				{
					LocalizedObjectType originalType = LocalizedObject.GetLocalizedObjectType(changedKey.Key);
					LocalizedObjectType changedType = LocalizedObject.GetLocalizedObjectType(changedKey.Value);
					
					if(originalType != changedType)
					{
						//If the type is changed, then delete the asset and reset the value
						DeleteFileFromResources(changedKey.Key, cultureInfo);
						currentValue = "";
					}
					else
					{
						//just rename it otherwise
						RenameFileFromResources(changedKey.Key, changedKey.Value, cultureInfo);
					}
				}
				
				LanguageDictionaryHelper.AddNewKeyPersistent(changedCultureValues, changedKey.Value, currentValue);
			}
			
			//Save the language file
			SaveLanguageFile (changedCultureValues, LocalizationWorkspace.LanguageFilePath(cultureInfo.languageCode));
			changedCultureValues.Clear();
			
			//Remove all the deleted files associated with the deleted keys
			foreach(string deletedKey in deletedKeys)
			{
				Debug.Log("Deleted key!:" + deletedKey);
				DeleteFileFromResources(deletedKey, cultureInfo);
			}
		}
	}

	/// <summary>
	/// Saves a language file(.resx) at the specified path containing the values in the languageValueDictionary
	/// </summary>
	public static void SaveLanguageFile(Dictionary<string, string> languageValueDictionary, string filePath)
	{
		//Get resx header
		TextAsset emptyResourceFile = Resources.Load("EmptyResourceHeader") as TextAsset;
		string resxHeader = emptyResourceFile.text;
		
		using(XmlTextWriter xmlWriter = new XmlTextWriter(filePath,Encoding.UTF8))
		{
			xmlWriter.Formatting = Formatting.Indented;
			xmlWriter.Settings.Encoding = Encoding.UTF8;
			xmlWriter.Settings.Indent = true;
			xmlWriter.WriteStartDocument();
			xmlWriter.WriteStartElement("root");
			xmlWriter.WriteRaw(resxHeader); // Paste in the raw resx header
			xmlWriter.WriteString("\n");  
		
			//Copy the keys over to the new language
			foreach(KeyValuePair<string, string> keyPair in languageValueDictionary)
			{
				xmlWriter.WriteString("\t");
				xmlWriter.WriteStartElement("data");
				xmlWriter.WriteAttributeString("name", keyPair.Key);
				xmlWriter.WriteAttributeString("xml:space", "preserve");
				xmlWriter.WriteString("\n\t\t");
			
				xmlWriter.WriteStartElement("value");
				xmlWriter.WriteString(keyPair.Value);
				xmlWriter.WriteEndElement(); //value
				xmlWriter.WriteString("\n\t");
				xmlWriter.WriteEndElement(); //data
				xmlWriter.WriteString("\n");  
			}
		
			xmlWriter.WriteEndElement(); //root
			xmlWriter.WriteEndDocument();
		}
		
		//Update the assetfolders
		AssetDatabase.Refresh(ImportAssetOptions.Default);
	}

	public static void UpdateLanguageFile(string languageCode, List<List<string>> values)
	{
		Dictionary<string,string> languageItems = null;
		if(FileUtility.Exists(LocalizationWorkspace.LanguageFilePath(languageCode)))
		{
			languageItems = LanguageHandlerEditor.LoadLanguageFile(languageCode, false);
		}
		else
		{
			languageItems = new Dictionary<string, string>();
		}
		
		int updatedKeys = 0;
		foreach (List<string> row in values)
		{
			if (row.Count != 2)
			{
				continue;
			}
			string key = row[0].TrimStart('\r', '\n').TrimEnd('\r', '\n').Trim();
			string value = row[1];

			if (!languageItems.ContainsKey(key))
			{
				continue;
			}

			languageItems[key] = value;
			updatedKeys++;
		}

		LanguageHandlerEditor.SaveLanguageFile(languageItems, LocalizationWorkspace.LanguageFilePath(languageCode));
		Debug.Log("Updated language:" + languageCode + ", Keys updated:" + updatedKeys);
	}

	public static void BulkUpdateLanguageFiles(List<List<string>> values)
	{
		if(values.Count > 0)
		{
			var allLanguageData = new Dictionary<string,List<List<string>>>();
			int i = 0;
			//First row contains all the column names which represents languages
			List<string> allLanguages = values[i++];

			for(; i < values.Count; i++)
			{
				List<string> languageData = values[i];
				if(languageData.Count >= 2)
				{
					for(int y = 1; y < languageData.Count; y++)
					{
						if(y >= allLanguages.Count || allLanguages[y] == null || allLanguages[y] == string.Empty)
						{
							continue;
						}

						var currentKeyValueList = new List<string>();
						currentKeyValueList.Add(languageData[0]);
						currentKeyValueList.Add(languageData[y]);
						if(!allLanguageData.ContainsKey(allLanguages[y]))
						{
							var languageList = new List<List<string>>();
							languageList.Add(currentKeyValueList);
							allLanguageData.Add(allLanguages[y], languageList);
						}
						else
						{
							allLanguageData[allLanguages[y]].Add(currentKeyValueList);
						}
					}
				}
			}

			foreach(var languagePair in allLanguageData)
			{
				UpdateLanguageFile(languagePair.Key, languagePair.Value);
			}
		}
	}

#endregion

#region Loading

	/// <summary>
	/// Loads the parsed language file.(without the type identifiers)
	/// </summary>
	public static Dictionary<string, LocalizedObject> LoadParsedLanguageFile(string languageCode, bool isRoot)
	{
		string fileContents = string.Empty;
		string filePath = null;

		if(isRoot)
		{
			filePath = LocalizationWorkspace.ResourcesFolderFilePath() + "/" + 
				LocalizationWorkspace.rootLanguageName + LocalizationWorkspace.txtFileEnding;
		}
		else
		{
			filePath = LocalizationWorkspace.ResourcesFolderFilePath() + "/" + 
					LocalizationWorkspace.rootLanguageName + "." + languageCode + LocalizationWorkspace.txtFileEnding;
		}
					
		if(!FileUtility.ReadFromFile(filePath, out fileContents))
		{
			Debug.LogError("Failed to read language from file - " + filePath);
		}

	
		var loadedLanguageDB = LanguageParser.LoadLanguage(fileContents);
		
		return new Dictionary<string,LocalizedObject>(loadedLanguageDB);	
	}
	/// <summary>
	/// Loads the language file and returns the RAW values
	/// </summary>
	public static Dictionary<string,string> LoadLanguageFile(string languageCode, bool isRoot)
	{
		Dictionary<string, LocalizedObject> loadedLanguageDB = LoadParsedLanguageFile(languageCode, isRoot);
		Dictionary<string,string> rawValues = new Dictionary<string, string>();

		foreach(var pair in loadedLanguageDB)
		{
			rawValues.Add(pair.Value.GetFullKey(pair.Key), pair.Value.TextValue);
		}
		
		return rawValues;
	}

	/// <summary>
	/// Loads all the language files with their raw values
	/// </summary>
	/// <returns>A dictionary with all the language dictionaries. The language codes are being used as keys</returns>
	public static Dictionary<string,Dictionary<string,string>> LoadAllLanguageFiles()
	{
		var allLanguages = new Dictionary<string,Dictionary<string,string>>();
		var availableCultures = LanguageHandlerEditor.CheckAndSaveAvailableLanguages(SmartCultureInfoEx.Deserialize(LocalizationWorkspace.CultureInfoCollectionFilePath()));

		foreach(SmartCultureInfo info in availableCultures.cultureInfos)
		{
			allLanguages.Add(info.languageCode, LoadLanguageFile(info.languageCode, false));
		}

		return allLanguages;
	}

#endregion

#region Deleting
	
	/// <summary> Deletes the language. </summary>
	public static void DeleteLanguage(SmartCultureInfo cultureInfo)
	{
		string filePath = LocalizationWorkspace.LanguageFilePath(cultureInfo.languageCode);
		if(FileUtility.Exists(filePath))
		{
			FileUtility.Delete(filePath);
			FileUtility.Delete(filePath + LocalizationWorkspace.metaFileEnding);
		}
		//The text file
		filePath = LocalizationWorkspace.ResourcesFolderFilePath() + "/" + LocalizationWorkspace.rootLanguageName + "." + cultureInfo.languageCode + LocalizationWorkspace.txtFileEnding;
		if(FileUtility.Exists(filePath))
		{
			FileUtility.Delete(filePath);
			FileUtility.Delete(filePath + LocalizationWorkspace.metaFileEnding);
		}
		
		//The assets directory
		filePath = LocalizationWorkspace.LanguageRuntimeFolderPath(cultureInfo.languageCode);
		if(Directory.Exists(filePath))
		{
			Directory.Delete(filePath + "/", true);
			FileUtility.Delete(filePath + LocalizationWorkspace.metaFileEnding);
		}

		SmartCultureInfoCollection allCultures = SmartCultureInfoEx.Deserialize(LocalizationWorkspace.CultureInfoCollectionFilePath());
		LanguageHandlerEditor.CheckAndSaveAvailableLanguages(allCultures);
		AssetDatabase.Refresh();
	}

#endregion

#region Asset Loading / Moving / Deleting
	/// <summary> Loads all assets in language values if they have a valid file path </summary>
	public static void LoadAllAssets(List<SerializableLocalizationObjectPair> languageValues)
	{
		foreach(SerializableLocalizationObjectPair objectPair in languageValues)
		{
			string assetPath = null;
			if(GetAssetPathFromGUID(objectPair, out assetPath))
			{
				switch(objectPair.changedValue.ObjectType)
				{
					case LocalizedObjectType.AUDIO:
					objectPair.changedValue.ThisAudioClip = AssetDatabase.LoadAssetAtPath(assetPath, typeof(AudioClip)) as AudioClip;
					break;
					
					case LocalizedObjectType.GAME_OBJECT:
					objectPair.changedValue.ThisGameObject = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
					break;
						
					case LocalizedObjectType.TEXTURE:
					objectPair.changedValue.ThisTexture = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture)) as Texture;
					break;											
					case LocalizedObjectType.TEXT_ASSET:
					objectPair.changedValue.ThisTextAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
					break;
					case LocalizedObjectType.FONT:
					objectPair.changedValue.Font = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Font)) as Font;
					break;
				}
			}
		}
	}

	static bool GetAssetPathFromGUID(SerializableLocalizationObjectPair objectPair, out string assetPath)
	{
		assetPath = null;
		if(objectPair.changedValue.TextValue != null && objectPair.changedValue.TextValue != string.Empty)
		{
			assetPath = AssetDatabase.GUIDToAssetPath(objectPair.changedValue.TextValue);
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary> Copies the file into the resources folder. Naming the new asset to KEY </summary>
	public static string CopyFileIntoResources(SerializableLocalizationObjectPair objectPair, SmartCultureInfo thisCultureInfo)
	{
		if(!DirectoryUtility.CheckAndCreate(LocalizationWorkspace.LanguageRuntimeFolderPath(thisCultureInfo.languageCode)))
		{
			return "";
		}


		string newFileName = objectPair.keyValue;
		string filePath = string.Empty;
		string currentAssetPath = string.Empty;
		LocalizedObject objectToCopy = objectPair.changedValue;

		if(objectToCopy.ObjectType == LocalizedObjectType.AUDIO && objectToCopy.ThisAudioClip != null)
		{
			filePath = LocalizationWorkspace.LanguageAudioFolderPath(thisCultureInfo.languageCode);
			currentAssetPath = AssetDatabase.GetAssetPath(objectToCopy.ThisAudioClip);
		}
		else if(objectToCopy.ObjectType == LocalizedObjectType.TEXTURE && objectToCopy.ThisTexture != null)
		{
			filePath = LocalizationWorkspace.LanguageTexturesFolderPath(thisCultureInfo.languageCode);
			currentAssetPath = AssetDatabase.GetAssetPath(objectToCopy.ThisTexture);
		}
		else if(objectToCopy.ObjectType == LocalizedObjectType.GAME_OBJECT && objectToCopy.ThisGameObject != null)
		{
			filePath = LocalizationWorkspace.LanguagePrefabsFolderPath(thisCultureInfo.languageCode);
			currentAssetPath = AssetDatabase.GetAssetPath(objectToCopy.ThisGameObject);
		}
		else if(objectToCopy.ObjectType == LocalizedObjectType.TEXT_ASSET && objectToCopy.ThisTextAsset != null)
		{
			filePath = LocalizationWorkspace.LanguageTextAssetsFolderPath(thisCultureInfo.languageCode);
			currentAssetPath = AssetDatabase.GetAssetPath(objectToCopy.ThisTextAsset);
		}
		else if(objectToCopy.ObjectType == LocalizedObjectType.FONT && objectToCopy.Font != null)
		{
			filePath = LocalizationWorkspace.LanguageFontsFolderPath(thisCultureInfo.languageCode);
			currentAssetPath = AssetDatabase.GetAssetPath(objectToCopy.Font);
		}
		else
		{
			return string.Empty;
		}

		if(!DirectoryUtility.CheckAndCreate(filePath))
		{
			return "";
		}

		//Get the fileExtension of the asset
		string fileExtension = FileUtility.GetFileExtension(Application.dataPath + currentAssetPath);
		
		if(objectToCopy.ObjectType != LocalizedObjectType.GAME_OBJECT){
	
			//Copy or replace the file to the new path
			FileUtil.ReplaceFile(currentAssetPath, filePath + "/" + newFileName + fileExtension);
	
			string metaFile = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) + 
								currentAssetPath.Substring(0, currentAssetPath.Length - fileExtension.Length) + fileExtension + ".meta";
			if(File.Exists(metaFile))
			{
				FileUtil.ReplaceFile(metaFile, filePath + "/" + newFileName + fileExtension + ".meta");
			}
		}
		else{
			string relativePath = filePath + "/" + newFileName + fileExtension;
			relativePath = "Assets" + relativePath.Substring(Application.dataPath.Length);
			PrefabUtility.CreatePrefab(relativePath, objectToCopy.ThisGameObject);
		}

		return AssetDatabase.AssetPathToGUID(currentAssetPath);
	}
	
	/// <summary> Deletes the localized file from resources.</summary>
	public static void DeleteFileFromResources(string key, SmartCultureInfo cultureInfo)
	{
		string languageFolderPath = string.Empty;
		string cleanKey = LocalizedObject.GetCleanKey(key);
		LocalizedObjectType keyType = LocalizedObject.GetLocalizedObjectType(key);
		
		switch(keyType)
		{
			case LocalizedObjectType.GAME_OBJECT:
			languageFolderPath = LocalizationWorkspace.LanguagePrefabsFolderPathRelative(cultureInfo.languageCode) + "/" + cleanKey + LocalizationWorkspace.prefabFileEnding;
			break;
			
			case LocalizedObjectType.AUDIO:
			languageFolderPath = LocalizationWorkspace.LanguageAudioFolderPathRelative(cultureInfo.languageCode);
			break;
			
			case LocalizedObjectType.TEXTURE:
			languageFolderPath = LocalizationWorkspace.LanguageTexturesFolderPathRelative(cultureInfo.languageCode);
			break;
			case LocalizedObjectType.TEXT_ASSET:
			languageFolderPath = LocalizationWorkspace.LanguageTextAssetsFolderPathRelative(cultureInfo.languageCode);
			break;
			case LocalizedObjectType.FONT:
			languageFolderPath = LocalizationWorkspace.LanguageFontsFolderPathRelative(cultureInfo.languageCode);
			break;
		}

		if(keyType != LocalizedObjectType.GAME_OBJECT)
		{
			string fileExtension = FileUtility.GetFileExtension(cleanKey, languageFolderPath);
			languageFolderPath += "/" + cleanKey + fileExtension;
		}

		if(FileUtility.Exists(Application.dataPath + languageFolderPath))
		{
			AssetDatabase.DeleteAsset("Assets" + languageFolderPath);
		}
		AssetDatabase.Refresh();
	}

	/// <summary>Renames the localized file from resources.</summary>
	public static void RenameFileFromResources(string key, string newKey, SmartCultureInfo cultureInfo)
	{
		string languageFolderPath = null;
		LocalizedObjectType keyType = LocalizedObject.GetLocalizedObjectType(key);
		string cleanKey = LocalizedObject.GetCleanKey(key);
		string cleanNewKey = LocalizedObject.GetCleanKey(newKey);
		
		switch(keyType)
		{
			case LocalizedObjectType.GAME_OBJECT:
			languageFolderPath = LocalizationWorkspace.LanguagePrefabsFolderPathRelative(cultureInfo.languageCode) + "/" + cleanKey + LocalizationWorkspace.prefabFileEnding;
			break;
			
			case LocalizedObjectType.AUDIO:
			languageFolderPath = LocalizationWorkspace.LanguageAudioFolderPathRelative(cultureInfo.languageCode);
			break;
				
			case LocalizedObjectType.TEXTURE:
			languageFolderPath = LocalizationWorkspace.LanguageTexturesFolderPathRelative(cultureInfo.languageCode);
			break;
				
			case LocalizedObjectType.TEXT_ASSET:
			languageFolderPath = LocalizationWorkspace.LanguageTextAssetsFolderPathRelative(cultureInfo.languageCode);
			break;
			case LocalizedObjectType.FONT:
			languageFolderPath = LocalizationWorkspace.LanguageFontsFolderPathRelative(cultureInfo.languageCode);
			break;
		}

		if(keyType != LocalizedObjectType.GAME_OBJECT)
		{
			string fileExtension = FileUtility.GetFileExtension(cleanKey, languageFolderPath);
			languageFolderPath += "/" + cleanKey + fileExtension;
		}

		if(FileUtility.Exists(Application.dataPath + languageFolderPath))
		{
			AssetDatabase.RenameAsset("Assets" + languageFolderPath, cleanNewKey);
		}
		
		AssetDatabase.Refresh();
	}
#endregion
}
} //namespace SmartLocalization.Editor
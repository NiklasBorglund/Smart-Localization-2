//
//  TranslateLanguageWindow.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//


namespace SmartLocalization.Editor
{

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using SmartLocalization.ReorderableList;

[System.Serializable]
public class TranslateLanguageWindow : EditorWindow
{
	static readonly string CollapseMultilineSaveKey = "jTech_translate_collapse_multiline";

#region Members
	public IAutomaticTranslator automaticTranslator = null;

	[SerializeField]
	Dictionary<string,LocalizedObject> rootValues;	
	[SerializeField]
	List<SerializableLocalizationObjectPair> loadedLanguageValues = new List<SerializableLocalizationObjectPair>();	
	[SerializeField]
 	string thisLanguage;
	[SerializeField]
	SmartCultureInfo currentCultureInfo;
	[SerializeField]
	bool rootFileChanged = false;
	[SerializeField]
	Vector2 scrollPosition = Vector2.zero;
	[SerializeField]
	bool guiChanged = false;
	[SerializeField]
	bool canLanguageBeTranslated = false;
	[SerializeField]
	bool collapseMultilineFields = false;

	List<SmartCultureInfo> availableTranslateFromLanguages = new List<SmartCultureInfo>();
	string[] availableTranslateLangEnglishNames;
	int translateFromLanguageValue = 0;
	int oldTranslateFromLanguageValue = 0;
	string translateFromLanguage = "None";
	///<summary> The language dictionary to translate from</summary>
	Dictionary<string,LocalizedObject> translateFromDictionary;
	string searchText = "";
	LanguageSearchType searchType = LanguageSearchType.KEY;
	LanguageSortType sortType = LanguageSortType.KEY;
	
	List<string> otherAvailableLanguageCodes = new List<string>();
	string[] otherAvailableLanguageCodesArray;

	HOEditorUndoManager undoManager;

	SettingsMenuControl settingsContextMenu;
	SettingsListAdaptor settingsAdaptor;
	LocalizedObjectMenuControl localizedObjectContextMenu;
	LocalizedObjectListAdaptor localizedObjectAdaptor;
	List<string> settingsList = new List<string>();
	EditorColumns listColumns;
	bool shouldRepaint = false;

	FileSystemWatcher watcher;
	bool isWatchingFile = false;
	string fileBeingWatched = "";

#endregion
#region Initialization

	public void Initialize(SmartCultureInfo thisCultureInfo, bool forceNewLanguage = false)
	{
		if(thisCultureInfo != null)
		{
			if(undoManager == null)
			{
				// Instantiate Undo Manager
				undoManager = new HOEditorUndoManager(this, "Smart Localization - Translate Language Window");
			}

			if(thisCultureInfo != null)
			{
				bool newLanguage = thisCultureInfo != this.currentCultureInfo ? true : false;

				if(Application.isPlaying || forceNewLanguage)
				{
					newLanguage = true;
				}

				this.currentCultureInfo = thisCultureInfo;
				if(loadedLanguageValues == null || loadedLanguageValues.Count < 1 || newLanguage)
				{
					InitializeLanguage(thisCultureInfo, 
						LanguageHandlerEditor.LoadParsedLanguageFile(null, true), 
						LanguageHandlerEditor.LoadParsedLanguageFile(thisCultureInfo.languageCode, 
						false));
				}
			}

			settingsList.Clear();
			settingsList.Add("SETTINGS");
			settingsList.Add("CONVERTLINEBREAK");
			settingsList.Add("WATCHFILE");
			settingsList.Add("AUTOTRANSLATE");
			settingsList.Add("GENERAL");

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
                if(	EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebPlayer &&
				EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebPlayerStreamed)
#endif
			{
				settingsList.Add("SORT");
			}
			
			settingsList.Add("SEARCH");

			settingsAdaptor = new SettingsListAdaptor(settingsList,DrawSettingsItem, 20);
			settingsContextMenu = new SettingsMenuControl();

			listColumns = new EditorColumns(0.02f, true);
			listColumns.AddColumn("Copy", 0.1f);
			listColumns.AddColumn("Translate", 0.1f);
			listColumns.AddColumn("Key", 0.21f);
			listColumns.AddColumn("Comment", 0.21f);
			listColumns.AddColumn("Override", 0.07f);
			listColumns.AddColumn("Value", 0.25f);
			listColumns.RecalculateColumnWidths();

			if(EditorPrefs.HasKey(CollapseMultilineSaveKey))
			{
				collapseMultilineFields = EditorPrefs.GetBool(CollapseMultilineSaveKey);
			}

			GUIUtility.keyboardControl = 0;
			
			SmartCultureInfoCollection allCultures = SmartCultureInfoEx.Deserialize(LocalizationWorkspace.CultureInfoCollectionFilePath());
			SmartCultureInfoCollection availableCultures = LanguageHandlerEditor.CheckAndSaveAvailableLanguages(allCultures);
			otherAvailableLanguageCodes.Clear();
			otherAvailableLanguageCodesArray = null;
			foreach(SmartCultureInfo otherCulture in availableCultures.cultureInfos)
			{
				if(otherCulture.languageCode != thisCultureInfo.languageCode)
				{
					otherAvailableLanguageCodes.Add(otherCulture.languageCode);
				}
			}
			
			if(otherAvailableLanguageCodes.Count > 0)
			{
				otherAvailableLanguageCodesArray = otherAvailableLanguageCodes.ToArray();
			}
		}
	}

	public void InitializeTranslator(IAutomaticTranslator translator)
	{
		automaticTranslator = translator;

		if(automaticTranslator == null)
		{
			return;
		}

		if(automaticTranslator.IsInitialized)
		{
			//Check if the language can be translated
			canLanguageBeTranslated = false;
			CheckIfCanBeTranslated();

			if(translateFromDictionary != null)
			{
				translateFromDictionary.Clear();
				translateFromDictionary = null;
			}
		}
	}

	/// <summary>
	/// Initializes the Language
	/// </summary>
	public void InitializeLanguage(SmartCultureInfo info, Dictionary<string, LocalizedObject> rootValues, Dictionary<string, LocalizedObject> languageValues)
	{
		this.rootValues = rootValues;	
		this.loadedLanguageValues.Clear();
		this.loadedLanguageValues = LanguageHandlerEditor.CreateSerializableLocalizationList(languageValues);
		//Load assets
		LanguageHandlerEditor.LoadAllAssets(this.loadedLanguageValues);

		this.thisLanguage = (currentCultureInfo.englishName + " - " + currentCultureInfo.languageCode);
		rootFileChanged = false;
		
		SortLanguageValues(sortType);
		localizedObjectAdaptor = new LocalizedObjectListAdaptor(this.loadedLanguageValues, null, DrawLanguageValue, 17, true);
		localizedObjectContextMenu = new LocalizedObjectMenuControl();

		localizedObjectAdaptor.collapseMultiline = collapseMultilineFields;
	}

	public void ReloadLanguage()
	{
		InitializeLanguage(currentCultureInfo, LanguageHandlerEditor.LoadParsedLanguageFile(null, true), LanguageHandlerEditor.LoadParsedLanguageFile(currentCultureInfo.languageCode, false));
	}
#endregion
#region EditorWindow Overrides

	void OnEnable()
	{
		EditRootLanguageFileWindow.OnRootFileChanged += OnRootFileChanged;
		EditorApplication.update += Update;
		Initialize(currentCultureInfo);
	}

	void OnDisable()
	{
		EditRootLanguageFileWindow.OnRootFileChanged -= OnRootFileChanged;
		EditorApplication.update -= Update;
	}

	void Update()
	{
		if(shouldRepaint)
		{
			Repaint();
			shouldRepaint = false;
		}
	}

	void OnGUI()
	{
		if(LocalizationWindowUtility.ShouldShowWindow(true))
		{	
			if(Application.isPlaying)
			{
				if(currentCultureInfo == null || 
				 (LanguageManager.HasInstance && currentCultureInfo.languageCode != LanguageManager.Instance.CurrentlyLoadedCulture.languageCode))
				{
					if(LanguageManager.HasInstance)
					{
						SmartCultureInfo currentLanguage = LanguageManager.Instance.GetCultureInfo(LanguageManager.Instance.CurrentlyLoadedCulture.languageCode);
						if(currentLanguage == null)
						{
							return;
						}

						Initialize(currentLanguage);
					}
					else
					{
						GUILayout.Label ("There is no LanguageManager in the scene. Smart Localization Translation Window cannot be used at runtime without it.", EditorStyles.boldLabel);
						return;
					}
				}
				else if(rootValues == null && currentCultureInfo != null)
				{
					Initialize(currentCultureInfo);
				}
			}
			else if(rootValues == null)
			{
				if(currentCultureInfo != null)
				{
					Initialize(currentCultureInfo, true);
				}
				else
				{
					this.Close();
					return;
				}
			}

			if(!rootFileChanged)
			{
				DrawMainTranslationView();
			}
			else
			{
				//The root file did change, which means that you have to reload. A key might have changed
				//We can't have language files with different keys
				GUILayout.Label("The root file might have changed", EditorStyles.boldLabel);
				GUILayout.Label("The root file did save, which means that you have to reload. A key might have changed.", EditorStyles.miniLabel);
				GUILayout.Label("You can't have language files with different keys", EditorStyles.miniLabel);
				if(GUILayout.Button("Reload Language File"))
				{
					ReloadLanguage();
				}
			}
		}
	}

	/// <summary>
	/// Draws the settings window and the main translation view where all the keys are.
	/// </summary>
	void DrawMainTranslationView()
	{
		ReorderableListGUI.Title("Language - " + thisLanguage);
		EditorGUILayout.Space();

		settingsContextMenu.Draw(settingsAdaptor);

		GUILayout.Label("Language Values", EditorStyles.boldLabel);

		bool shouldRepaintColumns = listColumns.DrawColumns();
		if(shouldRepaintColumns)
		{
			shouldRepaint = true;
		}

		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
		localizedObjectContextMenu.Draw(localizedObjectAdaptor);
		EditorGUILayout.EndScrollView();

		if(guiChanged)
		{
			GUILayout.Label("- You have unsaved changes", EditorStyles.miniLabel);
		}

		//If any changes to the gui is made
		if(GUI.changed)
		{
			guiChanged = true;
		}

		GUILayout.Label("Save Changes", EditorStyles.boldLabel);
		GUILayout.Label("Remember to always press save when you have changed values", EditorStyles.miniLabel);
		if(GUILayout.Button("Save/Rebuild"))
		{
			SaveAndRebuild();
		}

		if(shouldRepaint)
		{
			Repaint();
			shouldRepaint = false;
		}
	}

	private void SaveAndRebuild()
	{
		//Copy everything into a dictionary
		Dictionary<string,string> newLanguageValues = new Dictionary<string, string>();
		foreach(var objectPair in loadedLanguageValues)
		{
			if(objectPair.changedValue.ObjectType == LocalizedObjectType.STRING)
			{
				newLanguageValues.Add(objectPair.changedValue.GetFullKey(objectPair.keyValue), objectPair.changedValue.TextValue);
			}
			else
			{
				//Delete the file in case there was a file there previously
				LanguageHandlerEditor.DeleteFileFromResources(objectPair.changedValue.GetFullKey(objectPair.keyValue), currentCultureInfo);

				//Store the path to the file
				string pathValue = string.Empty;
				if(objectPair.changedValue.OverrideLocalizedObject)
				{
					pathValue = "override=" + objectPair.changedValue.OverrideObjectLanguageCode;
				}
				else
				{
					pathValue = LanguageHandlerEditor.CopyFileIntoResources(objectPair, currentCultureInfo);
				}
				newLanguageValues.Add(objectPair.changedValue.GetFullKey(objectPair.keyValue), pathValue);
			}
		}
		LanguageHandlerEditor.SaveLanguageFile(newLanguageValues, LocalizationWorkspace.LanguageFilePath(currentCultureInfo.languageCode));
		guiChanged = false;
		GUIUtility.keyboardControl = 0;

		if(Application.isPlaying && LanguageManager.HasInstance)
		{
			LanguageManager.Instance.ChangeLanguage(currentCultureInfo.languageCode);
		}
	}

#endregion
#region Translation Check
	/// <summary> Checks if this language can be translated by the automatic translator </summary>
	private void CheckIfCanBeTranslated()
	{
		if(automaticTranslator != null && automaticTranslator.IsInitialized)
		{
			automaticTranslator.GetAvailableTranslationLanguages(OnGotAvailableLanguages);
		}
	}
#endregion
#region GUI Methods

	SerializableLocalizationObjectPair DrawLanguageValue(Rect position, SerializableLocalizationObjectPair item)
	{
		float fullWindowWidth = position.width + 28;
		position.width = fullWindowWidth;

		if(GUI.Button(listColumns.GetColumnPosition(position, "Copy"), "Copy Root"))
		{
			if(undoManager != null)
			{
				undoManager.ForceDirty();
			}

			item.changedValue = new LocalizedObject(rootValues[item.keyValue]);

			GUIUtility.keyboardControl = 0;
		}
		if(item.changedValue.ObjectType == LocalizedObjectType.STRING && canLanguageBeTranslated &&
			translateFromLanguageValue != 0 && 
			translateFromDictionary != null && translateFromDictionary[item.keyValue].TextValue != null &&
			translateFromDictionary[item.keyValue].TextValue != string.Empty)
		{
			if(GUI.Button(listColumns.GetColumnPosition(position, "Translate"), "Translate"))
			{
				automaticTranslator.TranslateText(OnTextTranslated, item.keyValue, translateFromDictionary[item.keyValue].TextValue, 
																				translateFromLanguage,currentCultureInfo.languageCode); 
			}
		}
		else
		{
			GUI.Label(listColumns.GetColumnPosition(position, "Translate"), "Translate");
		}

		EditorGUI.SelectableLabel(listColumns.GetColumnPosition(position, "Key"), item.keyValue);
		EditorGUI.SelectableLabel(listColumns.GetColumnPosition(position, "Comment"), rootValues[item.keyValue].TextValue);
		
		if(item.changedValue.ObjectType != LocalizedObjectType.STRING && otherAvailableLanguageCodes.Count > 0)
		{
			bool overrideLang = EditorGUI.Toggle(listColumns.GetColumnPosition(position, "Override"), item.changedValue.OverrideLocalizedObject);
			if(overrideLang != item.changedValue.OverrideLocalizedObject)
			{
				if(undoManager != null)
				{
					undoManager.ForceDirty();
				}
					
				item.changedValue.OverrideLocalizedObject = overrideLang;
				
				if(overrideLang)
				{
					item.changedValue.OverrideObjectLanguageCode = otherAvailableLanguageCodes[0];
				}
			}
		}
		else
		{
			GUI.Label(listColumns.GetColumnPosition(position, "Override"), "-");
		}

		Rect newPosition = listColumns.GetColumnPosition(position, "Value");	
	
		bool setDirty = false;
		if(!item.changedValue.OverrideLocalizedObject)
		{
			if(item.changedValue.ObjectType == LocalizedObjectType.STRING)
			{
				string newTextValue = EditorGUI.TextArea(newPosition, item.changedValue.TextValue);
				if(newTextValue != item.changedValue.TextValue)
				{
					setDirty = true;
					item.changedValue.TextValue = newTextValue;
				}
			}
			else if(item.changedValue.ObjectType == LocalizedObjectType.AUDIO)
			{
				AudioClip newAudioValue = (AudioClip)EditorGUI.ObjectField(newPosition,
																			item.changedValue.ThisAudioClip, 
																			typeof(AudioClip),false);
				if(newAudioValue != item.changedValue.ThisAudioClip)
				{
					setDirty = true;
					item.changedValue.ThisAudioClip = newAudioValue;
				}
			}
			else if(item.changedValue.ObjectType == LocalizedObjectType.GAME_OBJECT)
			{
				GameObject newGameObjectValue = (GameObject)EditorGUI.ObjectField(newPosition,
																		item.changedValue.ThisGameObject, 
																		typeof(GameObject),false);
	
				if(newGameObjectValue != item.changedValue.ThisGameObject)
				{
					setDirty = true;
					item.changedValue.ThisGameObject = newGameObjectValue;
				}
			}
			else if(item.changedValue.ObjectType == LocalizedObjectType.TEXTURE)
			{
				Texture newTextureValue = (Texture)EditorGUI.ObjectField(newPosition,
																			item.changedValue.ThisTexture, 
																			typeof(Texture),false);
				if(newTextureValue != item.changedValue.ThisTexture)
				{
					setDirty = true;
					item.changedValue.ThisTexture = newTextureValue;
				}
			}
			else if(item.changedValue.ObjectType == LocalizedObjectType.TEXT_ASSET)
			{
				TextAsset newTextAssetValue = (TextAsset)EditorGUI.ObjectField(newPosition,
																			item.changedValue.ThisTextAsset, 
																			typeof(TextAsset),false);
				if(newTextAssetValue != item.changedValue.ThisTextAsset)
				{
					setDirty = true;
					item.changedValue.ThisTextAsset = newTextAssetValue;
				}
			}
			else if(item.changedValue.ObjectType == LocalizedObjectType.FONT)
			{
				Font newFontValue = (Font)EditorGUI.ObjectField(newPosition,
                                                               item.changedValue.Font, 
                                                               typeof(Font),false);
				if(newFontValue != item.changedValue.Font)
				{
					setDirty = true;
					item.changedValue.Font = newFontValue;
				}
			}
		}
		else
		{
			if(otherAvailableLanguageCodes.Count > 0)
			{
				int selectedIndex = -1;
				for(int i = 0; i < otherAvailableLanguageCodes.Count; ++i)
				{
					if(otherAvailableLanguageCodes[i] == item.changedValue.OverrideObjectLanguageCode)
					{
						selectedIndex = i;
						break;
					}
				}
				
				if(selectedIndex == -1)
				{
					selectedIndex = 0;
					item.changedValue.OverrideObjectLanguageCode = otherAvailableLanguageCodes[selectedIndex];
					setDirty = true;
				}
				int newIndex = EditorGUI.Popup(newPosition, selectedIndex, otherAvailableLanguageCodesArray);
				
				if(newIndex != selectedIndex)
				{
					item.changedValue.OverrideObjectLanguageCode = otherAvailableLanguageCodes[newIndex];
					setDirty = true;
				}
			}
			else
			{
				//There are no languages to steal from, disable the override.
				item.changedValue.OverrideLocalizedObject = false;
				setDirty = true;
			}
		}

		if(undoManager != null && setDirty)
		{
			undoManager.ForceDirty();
		}
		return item;
	}
	
	string DrawSettingsItem(Rect position, string item)
	{
		switch(item)
		{
			case "SETTINGS":
				DrawSettingsField(position);
				break;
			case "AUTOTRANSLATE":
				DrawAutoTranslation(position);
				break;
			case "WATCHFILE":
				DrawWatchFile(position);
				break;
			case "SEARCH":
				DrawSearchField(position);
				break;
			case "GENERAL":
				DrawGeneralField(position);
				break;
			case "SORT":
				DrawSortField(position);
				break;
			case "CONVERTLINEBREAK":
				DrawLinebreakField(position);
				break;
		}
		return item;
	}

	void DrawGeneralField(Rect position)
	{
		float fullWindowWidth = position.width + 30;
		Rect newPosition = position;
		newPosition.width = fullWindowWidth * 0.99f;

		bool collapse = EditorGUI.Toggle(newPosition, "Collapse multiline fields", collapseMultilineFields);

		if(collapse != collapseMultilineFields)
		{
			collapseMultilineFields = collapse;
			localizedObjectAdaptor.collapseMultiline = collapse;

			EditorPrefs.SetBool(CollapseMultilineSaveKey, collapseMultilineFields);
		}
	}
	void DrawLinebreakField(Rect position)
	{
		float fullWindowWidth = position.width + 30;
		Rect newPosition = position;

		newPosition.width = fullWindowWidth * 0.99f;
		if(GUI.Button(newPosition,@"Convert all '\n' into line breaks"))
		{
			OnConvertLinebreaksClick();
		}
	}

	void DrawSortField(Rect position)
	{
		float fullWindowWidth = position.width + 30;
		Rect newPosition = position;

		newPosition.width = fullWindowWidth * 0.99f;
		LanguageSortType chosenSortType = (LanguageSortType)EditorGUI.EnumPopup(newPosition,"Sort by", sortType);

		if(chosenSortType != sortType)
		{
			sortType = chosenSortType;
			SortLanguageValues(sortType);
			GUIUtility.keyboardControl = 0;
		}
	}

	void DrawSearchField(Rect position)
	{
		float fullWindowWidth = position.width + 30;
		Rect newPosition = position;
		newPosition.width = fullWindowWidth * 0.69f;
		searchText = EditorGUI.TextField(newPosition, "Search", searchText);
		newPosition.x += fullWindowWidth * 0.7f;
		newPosition.width = fullWindowWidth * 0.29f;
		searchType = (LanguageSearchType)EditorGUI.EnumPopup(newPosition, searchType);

		localizedObjectAdaptor.SearchType = searchType;
		localizedObjectAdaptor.SearchLine = searchText;
	}

	void DrawWatchFile(Rect position)
	{
		float fullWindowWidth = position.width + 30;
		Rect newPosition = position;
		
		newPosition.width = fullWindowWidth * 0.99f;
		string watchName = "Watch file for changes.";
		if (isWatchingFile)
		{
			GUI.backgroundColor = Color.red;
			watchName = "Watching: " + fileBeingWatched;
		}
		if(GUI.Button(newPosition, watchName))
		{
			if (isWatchingFile)
			{
				StopWatchingFile();
			}
			else
			{
				OnWatchFileClick();
			}
		}
		GUI.backgroundColor = Color.white;
	}

	void DrawAutoTranslation(Rect position)
	{
		if(automaticTranslator == null || Application.isPlaying)
		{
			return;
		}

		float fullWindowWidth = position.width + 30;
		Rect newPosition = position;
		newPosition.width = fullWindowWidth;

		if(automaticTranslator.IsInitializing)
		{
			GUI.Label(newPosition, "Initializing...", EditorStyles.boldLabel);
		}
		else if(!automaticTranslator.IsInitialized)
		{
			if(!EditorPrefs.HasKey(SmartLocalizationWindow.MicrosoftTranslatorIDSaveKey) || 
				!EditorPrefs.HasKey(SmartLocalizationWindow.MicrosoftTranslatorSecretSaveKey) || 
				!EditorPrefs.HasKey(SmartLocalizationWindow.KeepAuthenticatedSaveKey))
			{
				GUI.Label(newPosition,"You have no saved Automatic Translator Settings!", EditorStyles.boldLabel);
			}
		}
		else if(automaticTranslator.IsInitialized)
		{
			if(canLanguageBeTranslated)
			{
				DrawAvailableAutoTranslationGUI(fullWindowWidth, newPosition);
			}
			else
			{
				DrawNotAvailableAutoTranslationGUI(fullWindowWidth, newPosition);
			}
		}
	}

	/// <summary>
	/// Draws the GUI for when the automatic translator is initialized and available for translation
	/// </summary>
	void DrawAvailableAutoTranslationGUI(float fullWindowWidth, Rect newPosition)
	{
		newPosition.width = fullWindowWidth * 0.69f;
		translateFromLanguageValue = EditorGUI.Popup(newPosition, "Translate From", translateFromLanguageValue, availableTranslateLangEnglishNames);
		newPosition.x += fullWindowWidth * 0.7f;

		if(oldTranslateFromLanguageValue != translateFromLanguageValue)
		{
			oldTranslateFromLanguageValue = translateFromLanguageValue;
			//The value have been changed, load the language file of the other language that you want to translate from
			//I load it like this to show the translate buttons only on the ones that can be translated i.e some values
			//in the "from" language could be an empty string - no use in translating that
			if(translateFromDictionary != null)
			{
				translateFromDictionary.Clear();
				translateFromDictionary = null;
			}
			if(translateFromLanguageValue != 0)
			{
				string englishName = availableTranslateLangEnglishNames[translateFromLanguageValue];
				foreach(SmartCultureInfo info in availableTranslateFromLanguages)
				{
					if(info.englishName == englishName)
					{
						translateFromDictionary = LanguageHandlerEditor.LoadParsedLanguageFile(info.languageCode, false);
						translateFromLanguage = info.languageCode;
						break;
					}
				}
			}
		}

		newPosition.width = fullWindowWidth * 0.14f;
		if(translateFromLanguageValue != 0)
		{
			if(GUI.Button(newPosition, "Translate all text"))
			{
				if(translateFromLanguageValue != 0)
					TranslateAllText();
			}
			newPosition.x += fullWindowWidth * 0.15f;
		}
		if(GUI.Button(newPosition, "Refresh"))
		{
			CheckIfCanBeTranslated();
		}
	}

	void DrawNotAvailableAutoTranslationGUI(float fullWindowWidth, Rect newPosition)
	{
		newPosition.width = fullWindowWidth * 0.49f;
		GUI.Label(newPosition, currentCultureInfo.englishName + " is not available for translation", EditorStyles.boldLabel);
		newPosition.x += fullWindowWidth * 0.5f;


		if(GUI.Button(newPosition, "Refresh"))
		{
			CheckIfCanBeTranslated();
			GUIUtility.keyboardControl = 0;
		}
	}

	void DrawSettingsField(Rect position)
	{
		float fullWindowWidth = position.width + 30;
		Rect newPosition = position;
		newPosition.width = fullWindowWidth * 0.99f;
		if(GUI.Button(newPosition, "Copy All Values From Root"))
		{
			OnCopyBaseValuesClick();
		}
	}

	void OnCopyBaseValuesClick()
	{
		if(undoManager != null)
		{
			undoManager.ForceDirty();
		}

		int count = 0;
		foreach(var rootValue in rootValues)
		{
			if(rootValue.Value.ObjectType == LocalizedObjectType.STRING)
			{
				loadedLanguageValues[count].changedValue.TextValue = rootValue.Value.TextValue;
			}
			count++;
		}
		GUIUtility.keyboardControl = 0;
	}


	void SortLanguageValues(LanguageSortType sortType)
	{
#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
		if(EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebPlayer &&
			EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebPlayerStreamed)
#endif
		{
			loadedLanguageValues.Sort((a, b) =>
			{
				switch(sortType)
				{
					case LanguageSortType.KEY:
						return a.keyValue.CompareTo(b.keyValue);
					case LanguageSortType.VALUE:
						return a.changedValue.TextValue.CompareTo(b.changedValue.TextValue);
					case LanguageSortType.TYPE:
						return a.changedValue.ObjectType.CompareTo(b.changedValue.ObjectType);
				}
				return a.keyValue.CompareTo(b.keyValue);
			});
		}
	}

#endregion

#region Automatic Translation

	void OnTranslatorInitialized(bool success)
	{
		if(success)
		{
			CheckIfCanBeTranslated();
		}
	}

	private void TranslateAllText()
	{
		var keys = new List<string>();
		var textsToTranslate = new List<string>();
		foreach(var stringPair in translateFromDictionary)
		{
			if(stringPair.Value.ObjectType == LocalizedObjectType.STRING &&
				stringPair.Value.TextValue != null && stringPair.Value.TextValue != string.Empty)
			{
				keys.Add(stringPair.Key);
				textsToTranslate.Add(stringPair.Value.TextValue);
			}
		}

		if(keys.Count > 0)
		{
			automaticTranslator.TranslateTextArray(OnTextArrayTranslated, keys, textsToTranslate, translateFromLanguage, currentCultureInfo.languageCode);
		}
	}

#endregion
#region Event Handlers

	void OnConvertLinebreaksClick()
	{
		foreach(var translationValue in loadedLanguageValues)
		{
			if(translationValue.changedValue.ObjectType == LocalizedObjectType.STRING && 
				translationValue.changedValue.TextValue != null && translationValue.changedValue.TextValue != string.Empty)
			{
				translationValue.changedValue.TextValue = translationValue.changedValue.TextValue.Replace(@"\n", "\n");
			}
		}

		GUIUtility.keyboardControl = 0;
	}
	
	void OnWatchedFileChanged(object source, FileSystemEventArgs e)
	{
		EditorThreadQueuer.QueueOnMainThread(() => {
			Console.WriteLine("File changed. Reloading...");
			List<List<string>> values = CSVParser.Read(e.FullPath, CSVParser.GetDelimiter(CSVParser.Delimiter.COMMA));
			LanguageHandlerEditor.UpdateLanguageFile(currentCultureInfo.languageCode, values);
			ReloadLanguage();
			if (Application.isPlaying && LanguageManager.HasInstance) {
				LanguageManager.Instance.ChangeLanguage(currentCultureInfo.languageCode);
			}
			Repaint();
		});
	}
	
	void OnWatchFileClick()
	{
		string file = EditorUtility.OpenFilePanel("Select CSV file.", "", "");
		if (file != null && file != "")
		{
			watcher = new FileSystemWatcher();
			watcher.Path = Path.GetDirectoryName(file);
			watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
				| NotifyFilters.FileName | NotifyFilters.DirectoryName;
			watcher.Filter = Path.GetFileName(file);
			watcher.Changed += new FileSystemEventHandler(OnWatchedFileChanged);
			watcher.EnableRaisingEvents = true;
			fileBeingWatched = file;
			isWatchingFile = true;
		}
	}
	
	void StopWatchingFile()
	{
		watcher = null;
		fileBeingWatched = "";
		isWatchingFile = false;
	}

	void OnRootFileChanged()
	{
		rootFileChanged = true;
	}

	void OnTextTranslated(bool success, string key, string translatedValue)
	{
		if(!success)
		{
			return;
		}

		foreach(var objectPair in loadedLanguageValues)
		{
			if(objectPair.keyValue == key)
			{
				objectPair.changedValue.TextValue = translatedValue;
				break;
			}
		}

		shouldRepaint = true;
	}

	void OnTextArrayTranslated(bool success, List<string> keys, List<string> translatedValues)
	{
		if(!success)
		{
			return;
		}

		for(int j = 0; j < keys.Count; j++)
		{
			foreach(var objectPair in loadedLanguageValues)
			{
				if(objectPair.keyValue == keys[j])
				{
					objectPair.changedValue.TextValue = translatedValues[j];
					break;
				}
			}
		}

		shouldRepaint = true;
	}

	void OnGotAvailableLanguages(bool success, List<string> availableLanguages)
	{
		if(!success)
		{
			return;
		}

		availableTranslateFromLanguages.Clear();
		//Clear the array
		if(availableTranslateLangEnglishNames != null)
		{
			Array.Clear(availableTranslateLangEnglishNames, 0, availableTranslateLangEnglishNames.Length);
			availableTranslateLangEnglishNames = null;
		}
		
		if(translateFromDictionary != null)
		{
			translateFromDictionary.Clear();
			translateFromDictionary = null;
		}
		translateFromLanguageValue = 0;
		oldTranslateFromLanguageValue = 0;

		List<string> englishNames = new List<string>();
		englishNames.Add("None");
		if(availableLanguages.Contains(currentCultureInfo.languageCode))
		{
			canLanguageBeTranslated = true;

			SmartCultureInfoCollection allCultures = SmartCultureInfoEx.Deserialize(LocalizationWorkspace.CultureInfoCollectionFilePath());
			SmartCultureInfoCollection availableCultures = LanguageHandlerEditor.CheckAndSaveAvailableLanguages(allCultures);

			foreach(SmartCultureInfo cultureInfo in availableCultures.cultureInfos)
			{
				if(cultureInfo.languageCode != currentCultureInfo.languageCode && availableLanguages.Contains(cultureInfo.languageCode))
				{
					availableTranslateFromLanguages.Add(cultureInfo);
					englishNames.Add(cultureInfo.englishName);
				}
			}
		}
		else
		{
			canLanguageBeTranslated = false;
		}
		availableTranslateLangEnglishNames = englishNames.ToArray();
	}

#endregion

	/// <summary> Shows the translate window window. </summary>
    public static TranslateLanguageWindow ShowWindow(SmartCultureInfo info, SmartLocalizationWindow smartLocWindow)
    {
		TranslateLanguageWindow translateLanguageWindow = (TranslateLanguageWindow)EditorWindow.GetWindow<TranslateLanguageWindow>("Translate Language",true,typeof(SmartLocalizationWindow));
		translateLanguageWindow.Initialize(info);
		return translateLanguageWindow;
    }
}
} //SmartLocalization.Editor

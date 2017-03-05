//
//  SmartLocalizationWindow.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//

namespace SmartLocalization.Editor
{

using UnityEngine;
using UnityEditor;
using SmartLocalization;
using System.Collections.Generic;
using SmartLocalization.ReorderableList;

[System.Serializable]
public class SmartLocalizationWindow : EditorWindow
{
#region Members
	
	public static readonly string MicrosoftTranslatorIDSaveKey = "cws_mtClientID";
	public static readonly string MicrosoftTranslatorSecretSaveKey = "cws_mtClientSecret";
	public static readonly string KeepAuthenticatedSaveKey = "cws_mtKeepAuthenticated";

	public IAutomaticTranslator automaticTranslator = null;
	[SerializeField]
	public TranslateLanguageWindow translateLanguageWindow = null;

	[SerializeField]
	SmartCultureInfoCollection allCultures = null;

	[SerializeField]
	SmartCultureInfoCollection availableCultures = null;

	[SerializeField]
	SmartCultureInfoCollection nonAvailableCultures = null;
	
	[SerializeField]
	Vector2 scrollPosition = Vector2.zero;

	[SerializeField]
	Vector2 createScrollPosition = Vector2.zero;

	[SerializeField]
	bool isInitialized = false;
	
	[SerializeField]
	HOEditorUndoManager undoManager = null;

	//Windows
	[SerializeField]
	EditRootLanguageFileWindow editRootWindow = null;

	[SerializeField]
	string mtClientID = string.Empty;

	[SerializeField]
	string mtClientSecret = string.Empty;

	[SerializeField]
	bool keepTranslatorAuthenticated = false;

	SmartCultureInfoMenuControl languageListContextMenu;
	SmartCultureInfoListAdaptor languageListAdaptor;
	CreateLanguageMenuControl createListContextMenu;
	CreateLanguageListAdaptor createListAdaptor;
	SettingsMenuControl settingsContextMenu;
	SettingsListAdaptor settingsAdaptor;

	[SerializeField]
	List<string> settingsList = new List<string>();

#endregion

#region Properties

	public SmartCultureInfoCollection AvailableCultures
	{
		get
		{
			return availableCultures;
		}
	}

#endregion

#region Initialization
	void Initialize()
	{	
		if(undoManager == null)
		{
			undoManager = new HOEditorUndoManager(this, "SmartLocalization-Main");
		}
		
		if(availableCultures == null)
		{
			allCultures = SmartCultureInfoEx.Deserialize(LocalizationWorkspace.CultureInfoCollectionFilePath());
			
			if(allCultures.version != SmartCultureInfoCollection.LatestVersion)
			{
				LocalizationWorkspace.GenerateCultureInfoCollection(allCultures);
				allCultures = SmartCultureInfoEx.Deserialize(LocalizationWorkspace.CultureInfoCollectionFilePath());
			}
			InitializeCultureCollections();
		}

		if(EditorPrefs.HasKey(MicrosoftTranslatorIDSaveKey) && EditorPrefs.HasKey(MicrosoftTranslatorSecretSaveKey) && EditorPrefs.HasKey(KeepAuthenticatedSaveKey))
		{
			mtClientID = EditorPrefs.GetString(MicrosoftTranslatorIDSaveKey);
			mtClientSecret = EditorPrefs.GetString(MicrosoftTranslatorSecretSaveKey);
			keepTranslatorAuthenticated = EditorPrefs.GetBool(KeepAuthenticatedSaveKey);
		}

		InitializeTranslator();

		settingsList.Clear();		
		settingsList.Add("SETTINGS");
		settingsList.Add("AUTOTRANSLATE");
		
		isInitialized = true;

		GUIUtility.keyboardControl = 0;
	}

	void InitializeTranslator()
	{
		if(automaticTranslator == null)
		{
			automaticTranslator = new MicrosoftAutomaticTranslator();

			if(keepTranslatorAuthenticated)
			{
				automaticTranslator.Initialize(OnTranslatorInitialized, mtClientID, mtClientSecret);
			}
		}
	}

	void OnInitializeCollectionsCallback()
	{
		InitializeCultureCollections();
	}
	
	public void InitializeCultureCollections(bool reloadAllCultures = false)
	{
		if(reloadAllCultures)
		{
			allCultures = SmartCultureInfoEx.Deserialize(LocalizationWorkspace.CultureInfoCollectionFilePath());
		}
		
		availableCultures = LanguageHandlerEditor.CheckAndSaveAvailableLanguages(allCultures);
		nonAvailableCultures = LanguageHandlerEditor.GetNonAvailableLanguages(allCultures);
		
		availableCultures.cultureInfos.Sort((a, b)=>
    	{
			return a.englishName.CompareTo(b.englishName);
		});
		nonAvailableCultures.cultureInfos.Sort((a, b) =>
		{
			return a.englishName.CompareTo(b.englishName);
		});

		availableCultures.cultureInfos.Insert(0, new SmartCultureInfo(string.Empty, "ROOT", "ROOT", false));

		languageListAdaptor = new SmartCultureInfoListAdaptor(availableCultures.cultureInfos,DrawAvailableLanguageItem, 28);
		languageListContextMenu = new SmartCultureInfoMenuControl();

		createListAdaptor = new CreateLanguageListAdaptor(nonAvailableCultures.cultureInfos,DrawCreateLanguageItem, 15);
		createListContextMenu = new CreateLanguageMenuControl();

		settingsAdaptor = new SettingsListAdaptor(settingsList,DrawSettingsItem, 110);
		settingsContextMenu = new SettingsMenuControl();
	}

#endregion

#region Mono

	void OnGUI()
	{
		if(LocalizationWindowUtility.ShouldShowWindow())
		{
			if(!isInitialized)
			{
				Initialize();
			}

			if(	createListContextMenu == null || 
				createListAdaptor == null || 
				settingsContextMenu == null ||
				settingsAdaptor == null ||
				languageListAdaptor == null ||
				languageListContextMenu == null)
			{
				InitializeCultureCollections(true);
			}

			undoManager.CheckUndo();

			//Show settings
			ReorderableListGUI.Title("Smart Localization");
			EditorGUILayout.Space();

			ShowCreateAndSettingsActions();

			ShowCreatedLanguages();

			undoManager.CheckDirty();
		}
	}
#endregion

#region GUI Sections
		
	void ShowCreateAndSettingsActions()
	{
		float maxWidth = position.width * 0.5f;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.BeginVertical();
		Rect positionCheck = EditorGUILayout.GetControlRect( GUILayout.MaxWidth(maxWidth));
		ReorderableListGUI.Title(positionCheck, "Add / Update Languages");
		createScrollPosition = GUILayout.BeginScrollView(createScrollPosition, GUILayout.MaxHeight(350),  GUILayout.MaxWidth(maxWidth));
		createListContextMenu.Draw(createListAdaptor);
		GUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		EditorGUILayout.Space();

		EditorGUILayout.BeginVertical();
		positionCheck = EditorGUILayout.GetControlRect( GUILayout.MaxWidth(maxWidth));
		ReorderableListGUI.Title(positionCheck, "Settings");
		settingsContextMenu.Draw(settingsAdaptor);
		EditorGUILayout.EndVertical();

		EditorGUILayout.EndHorizontal();
	}

	public string DrawSettingsItem(Rect position, string label)
	{	
		if(label == "SETTINGS")
		{
			DrawSettingsActions(position);
		}
		else
		{
			DrawAutoTranslateOptions(position);
		}
		return label;
	}
	
	public void DrawSettingsActions(Rect position)
	{
		float fullWindowWidth = position.width + 30;
		float controlHeight = position.height * 0.16f;
		Rect newPosition = position;
		newPosition.width = fullWindowWidth;
		newPosition.height = controlHeight;
		GUI.Label(newPosition, "Settings", EditorStyles.boldLabel);
		if(GUI.Button(newPosition, "Create new culture"))
		{
			CreateLanguageWindow.ShowWindow(this);
		}
		newPosition.y += controlHeight;
		
		if(GUI.Button(newPosition, "Export All Languages"))
		{
			BulkUpdateWindow.ShowWindow(BulkUpdateWindow.BulkUpdateMethod.Export, this);
		}
		newPosition.y += controlHeight;
		
		if(GUI.Button(newPosition, "Import All Languages"))
		{
			BulkUpdateWindow.ShowWindow(BulkUpdateWindow.BulkUpdateMethod.Import, this);
		}
	}


	public void DrawAutoTranslateOptions(Rect position)
	{
		float fullWindowWidth = position.width + 30;
		Rect newPosition = position;
		newPosition.width = fullWindowWidth;
		GUI.Label(newPosition, "Automatic Translation - Microsoft", EditorStyles.boldLabel);

		float controlHeight = position.height * 0.15f;
		newPosition.y += controlHeight;
		newPosition.height = controlHeight;
		newPosition.width = position.width;
		string clientID = EditorGUI.TextField(newPosition,"Client ID", mtClientID);
		newPosition.y += controlHeight;
		string clientSecret = EditorGUI.TextField(newPosition, "Client Secret", mtClientSecret);
		newPosition.y += controlHeight;
		bool keepAuthenticated = EditorGUI.Toggle(newPosition, "Keep Authenticated", keepTranslatorAuthenticated);

		newPosition.y += controlHeight;
		if(automaticTranslator != null)
		{
			if(automaticTranslator.IsInitializing)
			{
				GUI.Label (newPosition, "Authenticating...", EditorStyles.boldLabel);
			}
			else if(!automaticTranslator.IsInitialized)
			{
				newPosition.width = 100;
				if(keepAuthenticated && automaticTranslator.InitializationDidExpire)
				{
					automaticTranslator.Initialize(OnTranslatorInitialized, mtClientID, mtClientSecret);
				}
				else if(GUI.Button(newPosition, "Authenticate"))
				{
					automaticTranslator.Initialize(OnTranslatorInitialized, mtClientID, mtClientSecret);
				}
			}
		}
		else
		{
			InitializeTranslator();
		}

		if(clientID != mtClientID || clientSecret != mtClientSecret || keepAuthenticated != keepTranslatorAuthenticated)
		{
			mtClientID = clientID;
			mtClientSecret = clientSecret;
			keepTranslatorAuthenticated = keepAuthenticated;
			SaveMicrosoftTranslatorSettings();
		}
	}

	public SmartCultureInfo DrawCreateLanguageItem(Rect position, SmartCultureInfo info)
	{		
		float fullWindowWidth = position.width + 30;
		Rect newPosition = position;
		newPosition.width = fullWindowWidth * 0.5f;
		GUI.Label(newPosition, info.englishName + " - " + info.languageCode);

		float buttonWidth = fullWindowWidth * 0.2f;
		newPosition.width = buttonWidth;
		newPosition.x = fullWindowWidth - newPosition.width;

		if(GUI.Button(newPosition,"Create"))
		{
			OnCreateLanguageClick(info);
		}
		newPosition.x -= buttonWidth;
		if(GUI.Button(newPosition,"Import"))
		{
			LanguageImportWindow.ShowWindow(info, OnInitializeCollectionsCallback);
		}
		return info;
	}

	public SmartCultureInfo DrawAvailableLanguageItem(Rect position, SmartCultureInfo info)
	{
		if(info.englishName != "ROOT")
		{
			float fullWindowWidth = position.width;
			Rect newPosition = position;
			newPosition.width = fullWindowWidth * 0.4f;
			GUI.Label(position, info.englishName + " - " + info.languageCode);

			float buttonWidth = fullWindowWidth * 0.2f;
			buttonWidth = Mathf.Clamp(buttonWidth, 70, 120);

			newPosition.width = buttonWidth;
			newPosition.x = fullWindowWidth - buttonWidth;

			if(GUI.Button(newPosition,"Update"))
			{
				LanguageUpdateWindow.ShowWindow(info, this);
			}
			newPosition.x -= buttonWidth;
			if(GUI.Button(newPosition,"Export"))
			{
				LanguageExportWindow.ShowWindow(info);
			}
			newPosition.x -= buttonWidth;
			if(GUI.Button(newPosition,"Translate"))
			{
				OnTranslateButtonClick(info);
			}
		}
		else
		{
			position.width += 28;
			if(GUI.Button(position,"Edit Root Language File"))
			{
				OnRootEditClick();
			}
		}
		return info;
	}

	void ShowCreatedLanguages()
	{
		if(languageListContextMenu == null || languageListAdaptor == null)
		{
			this.Close();
		}

		ReorderableListGUI.Title("Created Languages");
		EditorGUILayout.Space();
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		languageListContextMenu.Draw(languageListAdaptor);
		GUILayout.EndScrollView();
	}

#endregion

#region Translator Settings (Microsoft Translator)
	/// <summary>
	/// Saves the microsoft translator settings in EditorPrefs
	/// Keys = cws_mtCliendID, cws_mtCliendSecret
	/// </summary>
	private void SaveMicrosoftTranslatorSettings()
	{
		EditorPrefs.SetString(MicrosoftTranslatorIDSaveKey, mtClientID);
		EditorPrefs.SetString(MicrosoftTranslatorSecretSaveKey, mtClientSecret);
		EditorPrefs.SetBool(KeepAuthenticatedSaveKey, keepTranslatorAuthenticated);
	}

	void OnTranslatorInitialized(bool success){}
#endregion

#region Event Handlers

	void OnCreateLanguageClick(SmartCultureInfo info)
	{
		SmartCultureInfo chosenCulture = allCultures.FindCulture(info);

		if(chosenCulture == null)
		{
			Debug.LogError("The language: " + info.englishName + " could not be created");
			return;
		}
		LanguageHandlerEditor.CreateNewLanguage(chosenCulture.languageCode);

		InitializeCultureCollections();
	}
	
	void OnRootEditClick()
	{
		ShowRootEditWindow(LanguageHandlerEditor.LoadParsedLanguageFile(null, true));
	}
		
	void OnDeleteLanguageClick(SmartCultureInfo cultureInfo)
	{
		LanguageHandlerEditor.DeleteLanguage(cultureInfo);
	}
	
	void OnTranslateButtonClick(SmartCultureInfo info)
	{
		//Open language edit window
		ShowTranslateWindow(info);
	}

#endregion
	
#region Show Windows
	void ShowRootEditWindow(Dictionary<string,LocalizedObject> rootValues)
	{
		if(editRootWindow == null)
		{
			editRootWindow = EditRootLanguageFileWindow.ShowWindow();
			editRootWindow.Show();
		}
		else
		{
            editRootWindow = EditRootLanguageFileWindow.ShowWindow();
            editRootWindow.SetRootValues(rootValues);
			editRootWindow.Show();
		}
	}
	void ShowTranslateWindow(SmartCultureInfo info)
	{
        translateLanguageWindow = TranslateLanguageWindow.ShowWindow(info, this);
        translateLanguageWindow.InitializeTranslator(automaticTranslator);
        translateLanguageWindow.Show();
	}
	
	//Show window
    [MenuItem("Window/Smart Localization")]
    public static void ShowWindow()
    {
        var smartLocalizationWindow = (SmartLocalizationWindow)EditorWindow.GetWindow(typeof(SmartLocalizationWindow));      
#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0
		smartLocalizationWindow.title = "Smart Localization";
#else
		smartLocalizationWindow.titleContent.text = "Smart Localization";
#endif	
    }
#endregion
}
} //namespace SmartLocalization.Editor

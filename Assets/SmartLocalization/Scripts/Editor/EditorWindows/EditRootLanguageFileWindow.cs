//
// EditRootLanguageFileWindow.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//

namespace SmartLocalization.Editor
{

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using SmartLocalization.ReorderableList;

/// <summary> Serializable string pair. Helper class for a string pair that is serializeable(for the undo functionality)</summary>
[System.Serializable]
public class SerializableStringPair
{
	public string originalValue;
	public string changedValue;
	public SerializableStringPair(){}
	public SerializableStringPair(string originalValue, string changedValue)
	{
		this.originalValue = originalValue;
		this.changedValue = changedValue;
	}
}
/// <summary>Serializable localization object pair. Helper class for a string pair that is serializeable(for the undo functionality)</summary>
[System.Serializable]
public class SerializableLocalizationObjectPair
{
	public string keyValue;
	public LocalizedObject changedValue;

	public SerializableLocalizationObjectPair(){}
	public SerializableLocalizationObjectPair(string keyValue, LocalizedObject changedValue)
	{
		this.keyValue = keyValue;
		this.changedValue = changedValue;
	}
}

[System.Serializable]
public class EditRootLanguageFileWindow : EditorWindow
{
	public static Action OnRootFileChanged = null;

	static readonly string CollapseMultilineSaveKey = "jTech_root_collapse_multiline";

#region Members
	/// <summary>Containing the original keys and the changes to them, if any.</summary>
	[SerializeField]
	List<SerializableStringPair> changedRootKeys = new List<SerializableStringPair>();
	/// <summary>Containing the original values and any changes to them</summary>
	[SerializeField]
	List<SerializableLocalizationObjectPair> changedRootValues = new List<SerializableLocalizationObjectPair>();	
	/// <summary>The scroll view position</summary>
	[SerializeField]
	Vector2 scrollPosition = Vector2.zero;
	/// <summary>Did the GUI change?</summary>
	[SerializeField]
	bool guiChanged = false;
	/// <summary>Search field.</summary>
	[SerializeField]
	string searchText = "";
	/// <summary>The Undo Manager</summary>
	HOEditorUndoManager undoManager;
	/// <summary>The parsed root values. This is used to check root key duplicates</summary>
	[SerializeField]
	public Dictionary<string, LocalizedObject> parsedRootValues = new Dictionary<string, LocalizedObject>();
	/// <summary>The key types</summary>
	[SerializeField]
	string[] keyTypes;
	[SerializeField]
	bool collapseMultilineFields = false;

	SettingsMenuControl settingsContextMenu;
	SettingsListAdaptor settingsAdaptor;
	LocalizedObjectMenuControl localizedObjectContextMenu;
	LocalizedObjectListAdaptor localizedObjectAdaptor;
	List<string> settingsList = new List<string>();
	EditorColumns listColumns;
	LanguageSearchType searchType = LanguageSearchType.KEY;

#endregion
#region Initialization
	void Initialize()
	{
		if(undoManager == null)
		{
			// Instantiate Undo Manager
			undoManager = new HOEditorUndoManager(this, "Smart Localization - Edit Root Language Window");
		}
		if(keyTypes == null)
		{
			//Get the different key types
			List<string> enumNames = new List<string>(Enum.GetNames(typeof(LocalizedObjectType)));
			if(enumNames.Contains("INVALID"))
			{
				enumNames.Remove("INVALID");
			}
			keyTypes = enumNames.ToArray();
		}

		if(changedRootKeys == null)
		{
			changedRootKeys = new List<SerializableStringPair>();
		}
		if(changedRootValues == null)
		{
			changedRootValues = new List<SerializableLocalizationObjectPair>();
		}
		if(parsedRootValues == null)
		{
			parsedRootValues = new Dictionary<string, LocalizedObject>();
		}

		if(parsedRootValues.Count < 1)
		{
			SetRootValues(LanguageHandlerEditor.LoadParsedLanguageFile(null, true));
		}

		settingsList.Clear();
		settingsList.Add("CONVERTLINEBREAK");
		settingsList.Add("GENERAL");
		settingsList.Add("SEARCH");

		settingsAdaptor = new SettingsListAdaptor(settingsList,DrawSettingsItem, 20);
		settingsContextMenu = new SettingsMenuControl();

		localizedObjectAdaptor = new LocalizedObjectListAdaptor(changedRootValues, changedRootKeys, DrawRootPair, 17);

		if(localizedObjectContextMenu != null)
			localizedObjectContextMenu.ClearEvents();

		localizedObjectContextMenu = new LocalizedObjectMenuControl(OnKeyDeleted, OnKeyAdded);

		listColumns = new EditorColumns(0.1f);
		listColumns.AddColumn("Type", 0.1f);
		listColumns.AddColumn("Key", 0.45f);
		listColumns.AddColumn("Comment", 0.45f);
		listColumns.RecalculateColumnWidths();

		if(EditorPrefs.HasKey(CollapseMultilineSaveKey))
		{
			collapseMultilineFields = EditorPrefs.GetBool(CollapseMultilineSaveKey);
		}

		GUIUtility.keyboardControl = 0;
	}


	/// <summary>
	/// Sets the dictionaries necessary to change them within the extension
	/// </summary>
	/// <param name='rootValues'>
	/// Root values.
	/// </param>
	public void SetRootValues(Dictionary<string, LocalizedObject> rootValues)
	{
		changedRootValues.Clear();
		changedRootKeys.Clear();
		parsedRootValues.Clear();

		foreach(var rootValue in rootValues)
		{
			changedRootKeys.Add(new SerializableStringPair(rootValue.Key, rootValue.Key));
			changedRootValues.Add(new SerializableLocalizationObjectPair(rootValue.Key, rootValue.Value));

			LocalizedObject copyObject = new LocalizedObject();
			copyObject.ObjectType = rootValue.Value.ObjectType;
			copyObject.TextValue = rootValue.Value.TextValue;
			parsedRootValues.Add(rootValue.Key, copyObject);
		}

		searchText = "";
	}
#endregion

#region Editor Window Overrides

	void OnEnable()
	{
		Initialize();
	}

	void OnFocus()
	{
		Initialize();
	}

	void OnProjectChange()
	{
		Initialize();
	}

	string DrawSettingsItem(Rect position, string item)
	{
		float fullWindowWidth = position.width + 30;
		Rect newPosition = position;
		newPosition.width = fullWindowWidth * 0.99f;

		if(item == "GENERAL")
		{
			bool collapse = EditorGUI.Toggle(newPosition, "Collapse multiline fields", collapseMultilineFields);

			if(collapse != collapseMultilineFields)
			{
				collapseMultilineFields = collapse;
				localizedObjectAdaptor.collapseMultiline = collapse;

				EditorPrefs.SetBool(CollapseMultilineSaveKey, collapseMultilineFields);
			}
		}
		else if(item == "SEARCH")
		{
			newPosition.width = fullWindowWidth * 0.69f;
			searchText = EditorGUI.TextField(newPosition, "Search", searchText);
			newPosition.x += fullWindowWidth * 0.7f;
			newPosition.width = fullWindowWidth * 0.29f;
			searchType = (LanguageSearchType)EditorGUI.EnumPopup(newPosition, searchType);

			localizedObjectAdaptor.SearchType = searchType;
			localizedObjectAdaptor.SearchLine = searchText;
		}
		else if(item == "CONVERTLINEBREAK")
		{
			if(GUI.Button(newPosition,@"Convert all '\n' into line breaks"))
			{
				OnConvertLinebreaksClick();
			}
		}

		return item;
	}

	SerializableLocalizationObjectPair DrawRootPair(Rect position, SerializableLocalizationObjectPair item)
	{
		LocalizedObjectType newChangedType = (LocalizedObjectType)EditorGUI.Popup(
			listColumns.GetColumnPosition(position, "Type"), 
			(int)item.changedValue.ObjectType, 
			keyTypes);

		int index = changedRootValues.IndexOf(item);
		SerializableStringPair rootValue = changedRootKeys[index];
		string newChangedValue = EditorGUI.TextField(listColumns.GetColumnPosition(position, "Key"), rootValue.changedValue);
		string newTextValue = EditorGUI.TextArea(listColumns.GetColumnPosition(position, "Comment"), item.changedValue.TextValue);

		if(	newChangedType != changedRootValues[index].changedValue.ObjectType ||
			newChangedValue != rootValue.changedValue ||
					newTextValue != changedRootValues[index].changedValue.TextValue)
			{
				undoManager.ForceDirty();
				changedRootValues[index].changedValue.ObjectType = newChangedType;
				rootValue.changedValue = newChangedValue;
				changedRootValues[index].changedValue.TextValue = newTextValue;
			}

		return item;	
	}

	void OnGUI()
	{
		if(	localizedObjectContextMenu == null || 
			localizedObjectAdaptor == null || 
			settingsContextMenu == null ||
			settingsAdaptor == null)
		{
			Initialize();
		}


		if(LocalizationWindowUtility.ShouldShowWindow())
		{
			ReorderableListGUI.Title("Edit Root Values");
			EditorGUILayout.Space();

			settingsContextMenu.Draw(settingsAdaptor);

			bool shouldRepaint = listColumns.DrawColumns();
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
			localizedObjectContextMenu.Draw(localizedObjectAdaptor);
			EditorGUILayout.EndScrollView();
			
			if(guiChanged)
			{
				GUILayout.Label ("- You have unsaved changes", EditorStyles.miniLabel);
			}
			
			//If any changes to the gui is made
			if(GUI.changed)
			{
				guiChanged = true;
			}
			
			GUILayout.Label ("Save Changes", EditorStyles.boldLabel);
			GUILayout.Label ("Remember to always press save when you have changed values", EditorStyles.miniLabel);
			if(GUILayout.Button("Save Root Language File"))
			{
				SaveRootLanguageFile();
				guiChanged = false;
				GUIUtility.keyboardControl = 0;
			}

			if(shouldRepaint)
			{
				Repaint();
			}		
		}
	}

	void SaveRootLanguageFile()
	{
		var changeNewRootKeys = new Dictionary<string, string>();
		var changeNewRootValues = new Dictionary<string, string>();

		for(int i = 0; i < changedRootKeys.Count; i++)
		{
			SerializableStringPair rootKey = changedRootKeys[i];
			SerializableLocalizationObjectPair rootValue = changedRootValues[i];
			//Check for possible duplicates and rename them
			string newKeyValue = LanguageDictionaryHelper.AddNewKeyPersistent(changeNewRootKeys, 
				                                                                  rootKey.originalValue, 
				                                                                  rootValue.changedValue.GetFullKey(rootKey.changedValue));

			//Check for possible duplicates and rename them(same as above)
			LanguageDictionaryHelper.AddNewKeyPersistent(changeNewRootValues, newKeyValue, rootValue.changedValue.TextValue);
		}

		//Add the full values before saving
		var changeNewRootKeysToSave = new Dictionary<string, string>();
		var changeNewRootValuesToSave = new Dictionary<string, string>();

		foreach(var rootKey in changeNewRootKeys)
		{
			LocalizedObject thisLocalizedObject = parsedRootValues[rootKey.Key];
			changeNewRootKeysToSave.Add(thisLocalizedObject.GetFullKey(rootKey.Key), rootKey.Value);
			changeNewRootValuesToSave.Add(thisLocalizedObject.GetFullKey(rootKey.Key), changeNewRootValues[rootKey.Key]);
		}



		SmartCultureInfoCollection allCultures = SmartCultureInfoEx.Deserialize(LocalizationWorkspace.CultureInfoCollectionFilePath());
		LanguageHandlerEditor.SaveRootLanguageFile(changeNewRootKeysToSave, changeNewRootValuesToSave, LanguageHandlerEditor.CheckAndSaveAvailableLanguages(allCultures));

		//Fire the root language changed event
		if(OnRootFileChanged != null)
		{
			OnRootFileChanged();
		}

		//Reload the window(in case of duplicate keys)
		SetRootValues(LanguageHandlerEditor.LoadParsedLanguageFile(null, true));
	}

#endregion

#region Event Handlers

	void OnConvertLinebreaksClick()
	{
		foreach(var translationValue in changedRootValues)
		{
			if(translationValue.changedValue.TextValue != null && translationValue.changedValue.TextValue != string.Empty)
			{
				translationValue.changedValue.TextValue = translationValue.changedValue.TextValue.Replace(@"\n", "\n");
			}
		}

		GUIUtility.keyboardControl = 0;
	}

	void OnKeyDeleted(int deletedIndex)
	{
		parsedRootValues.Remove(changedRootKeys[deletedIndex].originalValue);
		changedRootKeys.RemoveAt(deletedIndex);
	}

	void OnKeyAdded(int addedIndex)
	{
		LocalizedObject dummyObject = new LocalizedObject();
		dummyObject.ObjectType = LocalizedObjectType.STRING;
		dummyObject.TextValue = "New Value";

		string addedKey = LanguageDictionaryHelper.AddNewKeyPersistent(parsedRootValues, "New Key", dummyObject);

		LocalizedObject copyObject = new LocalizedObject();
		copyObject.ObjectType = LocalizedObjectType.STRING;
		copyObject.TextValue = "New Value";

		changedRootKeys.Add(new SerializableStringPair(addedKey, addedKey));
		changedRootValues[addedIndex] = new SerializableLocalizationObjectPair(addedKey, copyObject);
	}

#endregion

#region Show Window
    public static EditRootLanguageFileWindow ShowWindow()
    {
		var rootEditWindow = (EditRootLanguageFileWindow)EditorWindow.GetWindow<EditRootLanguageFileWindow>("Edit Root Language File",true);

		return rootEditWindow;
	}
#endregion
}
} //SmartLocalization.Editor

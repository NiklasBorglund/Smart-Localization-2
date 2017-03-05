//LanguageImportWindow.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//

namespace SmartLocalization.Editor
{
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
	
[System.Serializable]
public class LanguageUpdateWindow : EditorWindow
{
#region Members
	
	CSVParser.Delimiter delimiter = CSVParser.Delimiter.COMMA;
	SmartCultureInfo chosenCulture;
	SmartLocalizationWindow parentWindow;
	
	int chosenFileFormat = 0;
	
	static readonly string		csvFileEnding = ".csv";
	static readonly string		xlsFileEnding = ".xls";
	
	static readonly string[]	availableFileFormats = {".csv", ".xls"};
	
#endregion
	
#region Initialization
	
	void Initialize(SmartCultureInfo chosenCulture, SmartLocalizationWindow parentWindow)
	{
		this.chosenCulture = chosenCulture;
		this.parentWindow = parentWindow;
		
		if(chosenFileFormat >= availableFileFormats.Length)
		{
			chosenFileFormat = 0;
		}
	}
	
#endregion
	
#region GUI Methods
	
	void OnGUI()
	{
		if(LocalizationWindowUtility.ShouldShowWindow())
		{
			GUILayout.Label ("Update Language from file", EditorStyles.boldLabel);
			GUILayout.Label ("Language to Update: " + chosenCulture.englishName + " - " + chosenCulture.languageCode);
			chosenFileFormat = EditorGUILayout.Popup("File Format", chosenFileFormat, availableFileFormats);
			
			if(availableFileFormats[chosenFileFormat] == csvFileEnding)
			{
				delimiter = (CSVParser.Delimiter)EditorGUILayout.EnumPopup("Delimiter",delimiter);
			}
			
			if(GUILayout.Button("Update"))
			{
				OnUpdateClicked();
			}
		}
	}
	
#endregion


#region Event Handlers
	void OnUpdateClicked()
	{
		string file = EditorUtility.OpenFilePanel("Select Update file.", "", "");
		if (file != null && file != "")
		{
			if(availableFileFormats[chosenFileFormat] == csvFileEnding)
			{
				UpdateFromCSV(file);
				this.Close();
			}
			else if(availableFileFormats[chosenFileFormat] == xlsFileEnding)
			{
				UpdateFromXLS(file);
				this.Close();
			}
			else
			{
				Debug.LogError("Unsupported file format! Cannot export file!");
			}
		}
		else{
			Debug.Log("Failed to export language");
		}
	}

#endregion

#region Helper Methods

	void UpdateFromCSV(string chosenUpdateFile)
	{
		LanguageHandlerEditor.UpdateLanguageFile(chosenCulture.languageCode, CSVParser.Read(chosenUpdateFile, CSVParser.GetDelimiter(delimiter)));
		
		if(parentWindow.translateLanguageWindow != null)
		{
			parentWindow.translateLanguageWindow.ReloadLanguage();
		}
	}
	
	void UpdateFromXLS(string chosenUpdateFile)
	{
		var values = XLSExporter.Read(chosenUpdateFile);
		LanguageHandlerEditor.UpdateLanguageFile(chosenCulture.languageCode, values);
		
		if(parentWindow.translateLanguageWindow != null)
		{
			parentWindow.translateLanguageWindow.ReloadLanguage();
		}
	}

#endregion
	
#region Show Windows
	public static LanguageUpdateWindow ShowWindow(SmartCultureInfo info, SmartLocalizationWindow parentWindow)
	{
		LanguageUpdateWindow languageUpdateWindow = (LanguageUpdateWindow)EditorWindow.GetWindow<LanguageUpdateWindow>("Update");
		languageUpdateWindow.Initialize(info, parentWindow);
		
		return languageUpdateWindow;
	}
#endregion
}
}
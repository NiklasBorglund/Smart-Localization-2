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
public class LanguageImportWindow : EditorWindow
{
#region Members

	public CSVParser.Delimiter delimiter = CSVParser.Delimiter.COMMA;
	public SmartCultureInfo chosenCulture = null;
	public Action creationDelegate = null;
	int chosenFileFormat = 0;
	
	static readonly string		csvFileEnding = ".csv";
	static readonly string		xlsFileEnding = ".xls";


	static readonly string[]	availableFileFormats = {".csv", ".xls"};

#endregion

#region Initialization

	void OnDestroy()
	{
		creationDelegate = null;
	}

	void Initialize(SmartCultureInfo cultureInfo, Action creationDelegate = null)
	{
		if(creationDelegate != null)
		{
			this.creationDelegate = creationDelegate;
		}
		chosenCulture = cultureInfo;
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
			if(chosenCulture == null)
			{
				this.Close();
			}

			GUILayout.Label ("Import Language", EditorStyles.boldLabel);
			GUILayout.Label ("Language to Import: " + chosenCulture.englishName + " - " + chosenCulture.languageCode);
			
			chosenFileFormat = EditorGUILayout.Popup("File Format", chosenFileFormat, availableFileFormats);
			
			if(availableFileFormats[chosenFileFormat] == csvFileEnding)
			{
				delimiter = (CSVParser.Delimiter)EditorGUILayout.EnumPopup("Delimiter",delimiter);
			}

			if(GUILayout.Button("Import"))
			{
				OnImportClicked();
			}
		}
	}

#endregion

#region Event Handlers
	void OnImportClicked()
	{
		string file = EditorUtility.OpenFilePanel("Select Import file.", "", "");
		if (file != null && file != "")
		{
			if(availableFileFormats[chosenFileFormat] == csvFileEnding)
			{
				ImportFromCSV(file);
				this.Close();
			}
			else if(availableFileFormats[chosenFileFormat] == xlsFileEnding)
			{
				ImportFromXLS(file);
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

	void ImportFromCSV(string chosenImportFile)
	{
		List<List<string>> values = CSVParser.Read(chosenImportFile, CSVParser.GetDelimiter(delimiter));
		
		if(chosenCulture == null)
		{
			Debug.LogError("The language: " + chosenCulture.englishName + " could not be created");
			this.Close();
			return;
		}
		LanguageHandlerEditor.CreateNewLanguage(chosenCulture.languageCode, values);
		
		if(creationDelegate != null)
		{
			creationDelegate();
			creationDelegate = null;
		}
	}
	
	void ImportFromXLS(string chosenImportFile)
	{
		List<List<string>> values =	XLSExporter.Read(chosenImportFile);
		if(chosenCulture == null)
		{
			Debug.LogError("The language: " + chosenCulture.englishName + " could not be created");
			this.Close();
			return;
		}
		LanguageHandlerEditor.CreateNewLanguage(chosenCulture.languageCode, values);
		
		if(creationDelegate != null)
		{
			creationDelegate();
			creationDelegate = null;
		}
	}

#endregion

#region Show Windows
	public static LanguageImportWindow ShowWindow(SmartCultureInfo chosenCulture, Action creationDelegate)
    {
		LanguageImportWindow languageImportWindow = (LanguageImportWindow)EditorWindow.GetWindow<LanguageImportWindow>("CSV Import");
		languageImportWindow.Initialize(chosenCulture, creationDelegate);
		return languageImportWindow;
	}
#endregion
}
}
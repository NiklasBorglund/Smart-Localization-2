//LanguageExportWindow.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//

namespace SmartLocalization.Editor
{
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class LanguageExportWindow : EditorWindow
{
#region Members

	public SmartCultureInfo chosenCulture = null;
	public CSVParser.Delimiter delimiter = CSVParser.Delimiter.COMMA;
	int chosenFileFormat = 0;
	
	static readonly string		csvFileEnding = ".csv";
	static readonly string		xlsFileEnding = ".xls";

	static readonly string[]	availableFileFormats = {".csv", ".xls"};

#endregion

#region Initialization

void Initialize(SmartCultureInfo chosenCulture)
{
	this.chosenCulture = chosenCulture;
	
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

		GUILayout.Label ("Export Language", EditorStyles.boldLabel);
		GUILayout.Label ("Language to Export: " + chosenCulture.englishName + " - " + chosenCulture.languageCode);
		chosenFileFormat = EditorGUILayout.Popup("File Format", chosenFileFormat, availableFileFormats);
		
		if(availableFileFormats[chosenFileFormat] == csvFileEnding)
		{
			delimiter = (CSVParser.Delimiter)EditorGUILayout.EnumPopup("Delimiter", delimiter);
		}

		if(GUILayout.Button("Export"))
		{
			OnExportClicked();
		}
	}
}

#endregion

#region Event Handlers

void OnExportClicked()
{
	string folder = EditorUtility.OpenFolderPanel("Select folder to save to.", "", "");
	if (folder != null && folder != string.Empty)
	{
		if(availableFileFormats[chosenFileFormat] == csvFileEnding)
		{
			ExportToCSV(folder);
			this.Close();
		}
		else if(availableFileFormats[chosenFileFormat] == xlsFileEnding)
		{
			ExportToXLS(folder);
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

void ExportToCSV(string chosenExportFolder)
{
	string name = chosenCulture.englishName + " - " + chosenCulture.languageCode + ".csv";
	var input = new List<List<string>>();
	Dictionary<string, LocalizedObject> languageItems = LanguageHandlerEditor.LoadParsedLanguageFile(chosenCulture.languageCode, false);
	foreach (var item in languageItems)
	{
		var row = new List<string>();
		row.Add(item.Key);
		row.Add(item.Value.TextValue);
		input.Add(row);
	}
	CSVParser.Write(chosenExportFolder + "/" + name, CSVParser.GetDelimiter(delimiter), input);
}

void ExportToXLS(string chosenExportFolder)
{
	string name = chosenCulture.englishName + " - " + chosenCulture.languageCode + xlsFileEnding;

	XLSExporter.Write(chosenExportFolder + "/" + name, chosenCulture.englishName, 
			LanguageHandlerEditor.LoadLanguageFile(chosenCulture.languageCode, false));
}

#endregion

#region Show Windows
public static LanguageExportWindow ShowWindow(SmartCultureInfo chosenCulture)
{
	LanguageExportWindow thisWindow = (LanguageExportWindow)EditorWindow.GetWindow<LanguageExportWindow>("Export");
	thisWindow.Initialize(chosenCulture);
	return thisWindow;
}
#endregion
}
}

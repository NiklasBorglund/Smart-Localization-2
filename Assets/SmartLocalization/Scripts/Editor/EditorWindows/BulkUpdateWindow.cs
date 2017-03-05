// BulkUpdateWindow.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//

namespace SmartLocalization.Editor
{
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class BulkUpdateWindow : EditorWindow
{
	public enum BulkUpdateMethod
	{
		Import,
		Export,
	}

	static readonly string		exportFileName	= "Languages";
	static readonly string		csvFileEnding = ".csv";
	static readonly string		xlsFileEnding = ".xls";

	static readonly string[]	availableFileFormats = {".csv", ".xls"};

	[SerializeField]
	BulkUpdateMethod updateMedhod = BulkUpdateMethod.Export;
	CSVParser.Delimiter delimiter = CSVParser.Delimiter.COMMA;
	SmartLocalizationWindow parentWindow = null;
	int chosenFileFormat = 0;

	public void Initialize(BulkUpdateMethod updateMethod, SmartLocalizationWindow parentWindow)
	{
		this.updateMedhod = updateMethod;
		this.parentWindow = parentWindow;
	}

#region GUI Methods

	void OnGUI()
	{
		if(this.parentWindow == null)
		{
			this.Close();
		}

		if(LocalizationWindowUtility.ShouldShowWindow())
		{
			if(updateMedhod == BulkUpdateMethod.Import)
			{
				GUILayout.Label ("Import all languages from single file", EditorStyles.boldLabel);
				ShowCommonGUI();
				ShowImportGUI();
			}
			else
			{
				GUILayout.Label ("Export all languages to single file", EditorStyles.boldLabel);
				ShowCommonGUI();
				ShowExportGUI();
			}
		}
	}

	void ShowImportGUI()
	{
		if(GUILayout.Button("Import"))
		{
			if(availableFileFormats[chosenFileFormat] == csvFileEnding)
			{
				string file = EditorUtility.OpenFilePanel("Select CSV file.", "", "");
				if (file != null && file != "")
				{
					var values = CSVParser.Read(file, CSVParser.GetDelimiter(delimiter));
					if(values.Count > 0)
					{
						LanguageHandlerEditor.BulkUpdateLanguageFiles(values);
					}
				}
				this.Close();
			}
			else if(availableFileFormats[chosenFileFormat] == xlsFileEnding)
			{
				string file = EditorUtility.OpenFilePanel("Select XLS file.", "", "");
				if (file != null && file != "")
				{
					var values = XLSExporter.Read(file);
					if(values.Count > 0)
					{
						LanguageHandlerEditor.BulkUpdateLanguageFiles(values);
					}
				}
				this.Close();
			}
			else
			{
				Debug.LogError("BulkUpdateWindow: Unsupported import format!");
			}

			if(parentWindow.translateLanguageWindow != null)
			{
				parentWindow.translateLanguageWindow.ReloadLanguage();
			}
		}
	}

	void ShowExportGUI()
	{
		if(GUILayout.Button("Export"))
		{
			string folderPath = EditorUtility.OpenFolderPanel("Select folder to save to.", "", "");
			if(availableFileFormats[chosenFileFormat] == csvFileEnding)
			{
				string fullPath = folderPath + "/" + exportFileName + csvFileEnding;
				CSVParser.Write(fullPath, CSVParser.GetDelimiter(delimiter),
					new List<string>(LanguageHandlerEditor.LoadLanguageFile(null, true).Keys), LanguageHandlerEditor.LoadAllLanguageFiles());

				Debug.Log("Exported CSV file to " + fullPath);
				this.Close();
			}
			else if(availableFileFormats[chosenFileFormat] == xlsFileEnding)
			{
				string fullPath = folderPath + "/" + exportFileName + xlsFileEnding;
				XLSExporter.Write(fullPath, "Languages",
					new List<string>(LanguageHandlerEditor.LoadLanguageFile(null, true).Keys), LanguageHandlerEditor.LoadAllLanguageFiles());

				Debug.Log("Exported XLS file to " + fullPath);
				this.Close();
			}
			else
			{
				Debug.LogError("BulkUpdateWindow: Unsupported export format!");
			}
		}
	}

	void ShowCommonGUI()
	{
		chosenFileFormat = EditorGUILayout.Popup("File Format", chosenFileFormat, availableFileFormats);

		if(availableFileFormats[chosenFileFormat] == csvFileEnding)
			ShowCSVOptions();
	}

	void ShowCSVOptions()
	{
		delimiter = (CSVParser.Delimiter)EditorGUILayout.EnumPopup("Delimiter", delimiter);
	}

#endregion

#region Show Windows
	public static BulkUpdateWindow ShowWindow(BulkUpdateMethod updateMethod, SmartLocalizationWindow parentWindow)
	{
		BulkUpdateWindow thisWindow = (BulkUpdateWindow)EditorWindow.GetWindow<BulkUpdateWindow>("Update Languages");
		thisWindow.Initialize(updateMethod, parentWindow);
		
		return thisWindow;
	}
#endregion
}
}
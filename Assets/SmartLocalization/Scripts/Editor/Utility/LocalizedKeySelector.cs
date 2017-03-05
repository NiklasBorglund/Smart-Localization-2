//
// LocalizedKeySelector.cs
//
//
// Written by Niklas Borglund and Jakob Hillerström
//

namespace SmartLocalization.Editor
{

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// A class providing a simple GUI for selecting a key from the root.
/// </summary>
public static class LocalizedKeySelector 
{
	private static List<string> parsedRootValues = new List<string>();
	private static LocalizedObjectType loadedObjectType;

	/// <summary>If the SelectKeyGUI should continously refresh the key list. </summary>
	public static bool autoRefresh = false;


	/// <summary>
	/// Call this from OnInspectorGUI in your own editor class. It will create buttons and a selectable popup with all the keys available.
	/// </summary>
	/// <param name="currentKey">The currently chosen key</param>
	/// <param name="sort">If the select key GUI should filter keys with a certain type</param>
	/// <param name="sortType">If sort is true, this is the type of keys that will be shown.</param>
	/// <returns>The currently chosen key</returns>
	public static string SelectKeyGUI(string currentKey, bool sort = false, LocalizedObjectType sortType = LocalizedObjectType.INVALID)
	{
		if(!ShouldShowKeySelector())
		{
			return currentKey;
		}

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Smart Localization",EditorStyles.boldLabel);
		if (GUILayout.Button("Open", GUILayout.Width(50)))
        {
			SmartLocalizationWindow.ShowWindow();
		}
		EditorGUILayout.EndHorizontal();

		if(autoRefresh || parsedRootValues.Count == 0 || sortType != loadedObjectType)
		{
			RefreshList(sort, sortType);
		}

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Sort Mode: ",EditorStyles.miniLabel, GUILayout.Width(55));
		if(sort)
		{
			GUILayout.Label(sortType.ToString() + " only.",EditorStyles.miniLabel);
		}
		else
		{
			GUILayout.Label("Showing ALL types",EditorStyles.miniLabel);
		}
		EditorGUILayout.EndHorizontal();
		

		int index = parsedRootValues.IndexOf(currentKey);
		index = Mathf.Max(0, index);
		int newIndex = index;
		newIndex = EditorGUILayout.Popup(index, parsedRootValues.ToArray());
			
		if(newIndex != index)
		{
			currentKey = parsedRootValues[newIndex];
		}
			
		if(!autoRefresh && GUILayout.Button("Refresh list", GUILayout.Width(70)))
		{
			RefreshList(sort, sortType);
		}
		
		
		return currentKey;
	}

	/// <summary>
	/// Returns if the key selector should be shown.
	/// </summary>
	/// <returns>Returns if the key selector should be shown.</returns>
	public static bool ShouldShowKeySelector()
	{
		if(Application.isPlaying)
		{
			GUILayout.Label("Feature not available in play mode.",EditorStyles.miniLabel);
			return false;
		}

		if(!LocalizationWorkspace.Exists())
		{
			GUILayout.Label("There is no Smart Localization workspace created",EditorStyles.miniLabel);
			//There is no language created
			if (GUILayout.Button("Create Workspace"))
        	{
				LocalizationWorkspace.Create();
			}
			return false;
		}

		return true;
	}
	
	/// <summary>
	/// Refreshes the list containing the selectable keys
	/// </summary>
	/// <param name="sort">If the refreshed list should be sorted and filtered with a certain key type</param>
	/// <param name="sortType">The key type to use as filter</param>
	public static void RefreshList(bool sort, LocalizedObjectType sortType)
	{
		if(!Application.isPlaying)
		{
			parsedRootValues.Clear();
	
			Dictionary<string, LocalizedObject> values = LanguageHandlerEditor.LoadParsedLanguageFile(null, true);
			if(sort)
			{
				loadedObjectType = sortType;
				foreach(KeyValuePair<string, LocalizedObject> pair in values)
				{
					if(pair.Value.ObjectType == sortType)
					{
						parsedRootValues.Add(pair.Key);	
					}
				}
			}
			else
			{
				//Use invalid for showing all
				loadedObjectType = LocalizedObjectType.INVALID;
				
				parsedRootValues.AddRange(values.Keys);
			}
			
			if(parsedRootValues.Count > 0)
			{
				parsedRootValues.Insert(0,"--- No key selected ---");
			}
			else
			{
				parsedRootValues.Add("--- No localized keys available ---");
			}
		}
	}
}
} //namespace SmartLocalization.Editor

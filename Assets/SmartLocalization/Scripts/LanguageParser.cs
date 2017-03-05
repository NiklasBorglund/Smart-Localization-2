//LanguageParser.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//

namespace SmartLocalization
{
using UnityEngine;
using System.Xml;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Helper class that parses a Smart Localization language file at runtime.
/// </summary>
public static class LanguageParser 
{
	static string xmlHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<root>";


#region Language Loading

	/// <summary>
	/// Loads and initializes a language file
	/// </summary>
	public static SortedDictionary<string, LocalizedObject> LoadLanguage(string languageDataInResX)
	{
		if(languageDataInResX == null || languageDataInResX == "")
		{
			Debug.LogError("Cannot load language file - languageDataInResX is null!");
			return null;
		}

		SortedDictionary<string, LocalizedObject> loadedLanguageDictionary = new SortedDictionary<string, LocalizedObject>();

		string resxDocument = languageDataInResX;
		int index = resxDocument.IndexOf("</xsd:schema>");

		//13 == Length of "</xsd:schema>"
		index += 13;
		string xmlDocument = resxDocument.Substring(index);

		//add the header to the document
		xmlDocument = xmlHeader + xmlDocument;
			
		//Create the xml file with the new reduced resx document
		using(StringReader stringReader = new StringReader(xmlDocument))
		{
			using(XmlReader reader = XmlReader.Create(stringReader))
			{
				ReadElements(reader, loadedLanguageDictionary);
			}	
		}

		return loadedLanguageDictionary;
	}

	static void ReadElements(XmlReader reader, SortedDictionary<string, LocalizedObject> loadedLanguageDictionary)
	{
		while (reader.Read())
		{
			if(reader.NodeType == XmlNodeType.Element && reader.Name == "data")
			{
				ReadData(reader, loadedLanguageDictionary);
			}
		}
	}

	static void ReadData(XmlReader reader, SortedDictionary<string, LocalizedObject> loadedLanguageDictionary)
	{
		string key = string.Empty;	
		string value = string.Empty;
		
		if (reader.HasAttributes)
		{
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "name")
				{
					key = reader.Value;
				}
			}
		}
		
		//Move back to the element
		reader.MoveToElement();
		
		//Read the child nodes
		if (reader.ReadToDescendant("value"))
		{
			do
			{
				value = reader.ReadElementContentAsString();
			}
			while (reader.ReadToNextSibling("value"));
		}
		
		//Add the localized parsed values to the localizedObjectDict
		LocalizedObject newLocalizedObject = new LocalizedObject();
		newLocalizedObject.ObjectType = LocalizedObject.GetLocalizedObjectType(key);
		newLocalizedObject.TextValue = value;
		if(newLocalizedObject.ObjectType != LocalizedObjectType.STRING && newLocalizedObject.TextValue != null && newLocalizedObject.TextValue.StartsWith("override="))
		{
			newLocalizedObject.OverrideLocalizedObject = true;
			newLocalizedObject.OverrideObjectLanguageCode = newLocalizedObject.TextValue.Substring("override=".Length);
		}
		loadedLanguageDictionary.Add(LocalizedObject.GetCleanKey(key, newLocalizedObject.ObjectType), newLocalizedObject); 
	}

#endregion
}
}// namespace SmartLocalization

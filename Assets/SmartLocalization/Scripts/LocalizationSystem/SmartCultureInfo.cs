// SmartCultureInfo.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//

namespace SmartLocalization
{
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

/// <summary>
/// A Serializeable Collection containing a list of Smart Culture Infos
/// </summary>
[XmlRoot("SmartCultureCollections"), System.Serializable]
public class SmartCultureInfoCollection
{
#region Members

	public const int LatestVersion = 4;

	[XmlElement(ElementName = "version")]
	public int version = 0;

	[XmlArray("CultureInfos"),XmlArrayItem("CultureInfo")]
	public List<SmartCultureInfo> cultureInfos = new List<SmartCultureInfo>();

#endregion

#region Add CultureInfo

	/// <summary>
	/// Removes a culture info from the collection
	/// </summary>
	/// <param name="cultureInfo">The reference to the culture info to remove</param>
	public void RemoveCultureInfo(SmartCultureInfo cultureInfo)
	{
		if(cultureInfo == null)
		{
			Debug.LogError("Cannot remove a SmartCultureInfo that's null!");
			return;
		}

		if(!cultureInfos.Remove(cultureInfo))
		{
			Debug.LogError("Failed to remove cultureInfo!");
		}
	}

	/// <summary>
	/// Adds a culture info to the collection
	/// </summary>
	/// <param name="cultureInfo">The culture info to add</param>
	public void AddCultureInfo(SmartCultureInfo cultureInfo)
	{
		if(cultureInfo == null)
		{
			Debug.LogError("Cannot add a SmartCultureInfo that's null!");
			return;
		}

		cultureInfos.Add(cultureInfo);
	}

#endregion

#region Serialization

	/// <summary>
	/// Deserializes and creates a SmartCultureInfoCollection from a text asset.
	/// </summary>
	/// <param name="xmlFile">The textasset containing the serialized xml data</param>
	/// <returns>The deserialized SmartCultureInfoCollection</returns>
	public static SmartCultureInfoCollection Deserialize(TextAsset xmlFile)
	{
		return SmartCultureInfoCollectionDeserializer.Deserialize(xmlFile);
	}

#endregion

#region Lookups

	/// <summary>
	/// Finds a culture info from the list. Compares the English name and the language code
	/// </summary>
	/// <param name="cultureInfo">The culture info to find</param>
	/// <returns>The culture info, returns null if not found</returns>
	public SmartCultureInfo FindCulture(SmartCultureInfo cultureInfo)
	{
		if(cultureInfo == null)
		{
			return null;
		}

		return cultureInfos.Find(c => 
								(c.englishName.ToLower() == cultureInfo.englishName.ToLower()) &&
								(c.languageCode.ToLower() == cultureInfo.languageCode.ToLower()));
	}

	/// <summary>
	/// Finds a culture info from the list by the ISO-639 language code.
	/// </summary>
	/// <param name="languageCode">The ISO-639 language code</param>
	/// <returns>The found SmartCultureInfo. Returns null if nothing was found.</returns>
	public SmartCultureInfo FindCulture(string languageCode)
	{
		if(string.IsNullOrEmpty(languageCode))
		{
			return null;
		}

		return cultureInfos.Find(c => 
								(c.languageCode.ToLower() == languageCode.ToLower()));
	}

	/// <summary>
	/// Checks if a specific culture info is in this collection
	/// </summary>
	/// <param name="cultureInfo">The culture info to check</param>
	/// <returns>If the specified culture info was in the collection</returns>
	public bool IsCultureInCollection(SmartCultureInfo cultureInfo)
	{
		return FindCulture(cultureInfo) != null;
	}

#endregion
}

/// <summary>
/// A serializable class containing the basic information about a culture.
/// </summary>
[System.Serializable]
public class SmartCultureInfo
{
	/// <summary> The language code of the culture. (e.g. sv, en, de, fr)</summary>
	public string languageCode 	= null;
	/// <summary> The English name of the culture</summary>
	public string englishName	= null;
	/// <summary> The native name of the culture</summary>
	public string nativeName	= null;
	/// <summary> If the language is written and read from right to left </summary>
	public bool isRightToLeft = false;

	/// <summary> Creates a new instance of SmartCultureInfo</summary>
	public SmartCultureInfo(){}
	/// <summary>
	/// Creates a new instance of SmartCultureInfo
	/// </summary>
	/// <param name="languageCode">The language code of the culture. (e.g. sv, en, de, fr)</param>
	/// <param name="englishName">The English name of the culture</param>
	/// <param name="nativeName">The native name of the culture</param>
	public SmartCultureInfo(string languageCode, string englishName, string nativeName, bool isRightToLeft)
	{
		this.languageCode = languageCode;
		this.englishName = englishName;
		this.nativeName = nativeName;
		this.isRightToLeft = isRightToLeft;
	}
	
	public override string ToString()
	{
			return string.Format (@"[SmartCultureInfo LanguageCode=""{0}"" EnglishName={1} NativeName={2} IsRightToLeft={3}]",
									languageCode, englishName, nativeName, isRightToLeft.ToString());
	}
}
}
//LanguageDictionaryHelper.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//
namespace SmartLocalization.Editor
{
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Helper methods for language values using parsed & raw language dictionaries 
/// </summary>
public static class LanguageDictionaryHelper
{
	/// <summary>
	/// Adds a new key to a dictionary<string,string> and does not stop until a unique key is found
	/// </summary>
	public static string AddNewKeyPersistent(Dictionary<string,string> languageDictionary, string desiredKey, string newValue)
	{
		LocalizedObjectType keyType = LocalizedObject.GetLocalizedObjectType(desiredKey);
		//Clean the key from unwanted type identifiers
		//Nothing will happen to a regular string, since a string doesn't have an identifier
		desiredKey = LocalizedObject.GetCleanKey(desiredKey, keyType);

		bool newKeyFound = false;
		int count = 0;
		string newKeyName = desiredKey;
		while(!newKeyFound)
		{
			if(!languageDictionary.ContainsKey(newKeyName))
			{
				bool duplicateFound = false;
				foreach(KeyValuePair<string,string> stringPair in languageDictionary)
				{
					string cleanKey = LocalizedObject.GetCleanKey(stringPair.Key);
					if(cleanKey == newKeyName)
					{
						duplicateFound = true;
						break;
					}
				}
				if(!duplicateFound)
				{
					languageDictionary.Add(LocalizedObject.GetFullKey(newKeyName, keyType), newValue);
					newKeyFound = true;
					return desiredKey;
				}
				else
				{
					newKeyName = desiredKey + count;
					count++;
				}
			}
			else
			{
				newKeyName = desiredKey + count;
				count++;
			}
		}
		Debug.Log("Duplicate keys in dictionary was found! - renaming key:" + desiredKey + " to:" + newKeyName);
		return newKeyName;
	}
	/// <summary>
	/// Adds a new key to a dictionary<string,LocalizedObject> and does not stop until a unique key is found
	/// </summary>
	public static string AddNewKeyPersistent(Dictionary<string, LocalizedObject> languageDictionary, string desiredKey, LocalizedObject newValue)
	{
		if(!languageDictionary.ContainsKey(desiredKey))
		{
			languageDictionary.Add(desiredKey, newValue);
			return desiredKey;
		}
		else
		{
			bool newKeyFound = false;
			int count = 0;
			while(!newKeyFound)
			{
				if(!languageDictionary.ContainsKey(desiredKey + count))
				{
					languageDictionary.Add(desiredKey + count, newValue);
					newKeyFound = true;
				}
				else
				{
					count++;
				}
			}
			Debug.LogWarning("Duplicate keys in dictionary was found! - renaming key:" + desiredKey + " to:" + (desiredKey + count));
			return (desiredKey + count);
		}
	}	
}
}//namespace SmartLocalization.Editor
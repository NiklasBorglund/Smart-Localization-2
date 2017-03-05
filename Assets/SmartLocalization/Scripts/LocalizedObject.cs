//
// LocalizedObject.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//

namespace SmartLocalization
{
using UnityEngine;

/// <summary>
/// The type of the localized object
/// </summary>
public enum LocalizedObjectType
{
	STRING = 0,
	GAME_OBJECT = 1,
	AUDIO = 2,
	TEXTURE = 3,
	TEXT_ASSET = 4,
	FONT = 5,
	INVALID,
}

/// <summary>
/// The localized object, used to determine what type the string value from the .resx file is
/// </summary>
[System.Serializable]
public class LocalizedObject
{
#region Members

	/// <summary>
	/// All keys that are not a string will begin with this identifier
	/// for example - if the localized object is an audio file, the key
	/// will begin with <type=AUDIO>
	/// regular text strings will not begin with any type of identifier
	/// </summary>
	public static readonly string keyTypeIdentifier = "[type=";
	static readonly string endBracket = "]";


	/// <summary> The type of the localizedObject.</summary>
	[SerializeField]
	LocalizedObjectType objectType = LocalizedObjectType.STRING;
		
	/// <summary>
	/// The text value of the object, all objects are converted into string values in the end, whether it's a 
	/// file path or a simple string value
	/// </summary>
	[SerializeField]
	string textValue;

	/// <summary>
	/// Variable if this object type is GAME_OBJECT
	/// </summary>
	[SerializeField]
	GameObject thisGameObject;

	/// <summary>
	/// Variable if this object type is AUDIO
	/// </summary>
	[SerializeField]
	AudioClip thisAudioClip;

	/// <summary>
	/// Variable if this object type is TEXTURE
	/// </summary>
	[SerializeField]
	Texture thisTexture;

	/// <summary>
	/// Variable if this object type is TEXT_ASSET
	/// </summary>
	[SerializeField]
	TextAsset thisTextAsset;
	
	/// <summary>
	/// Variable if this object type is FONT
	/// </summary>
	[SerializeField]
	Font font;
	
	/// <summary>
	/// If this object should use an asset from a different language.
	/// </summary>
	[SerializeField]
	bool overrideLocalizedObject = false;
	
	/// <summary>
	/// If overrideLocalizedObject is true, this is the language code that it will use to get the other language.
	/// </summary>
	[SerializeField]
	string overrideObjectLanguageCode = null;

#endregion

#region Properties

	/// <summary> Gets or sets the LocalizedObjectType of the object.</summary>
	public LocalizedObjectType ObjectType
	{
		get
		{
			return objectType;	
		}
		set
		{
			objectType = value;	
		}
	}

	/// <summary>
	/// Gets or sets the text value.
	/// </summary>
	/// <value>
	/// The text value of the object, all objects are converted into string values in the end, whether it's a
	/// file path or a simple string value.
	/// </value>
	public string TextValue
	{
		get
		{
		 	return textValue;	
		}
		set
		{
			textValue = value;	
		}
	}

	/// <summary>
	/// Variable if this object type is GAME_OBJECT. Gets or sets the this game object.
	/// </summary>
	/// <value>
	/// The this game object.
	/// </value>
	public GameObject ThisGameObject
	{
		get
		{
			return thisGameObject;	
		}
		set
		{
			thisGameObject = value;	
		}
	}	

	/// <summary>
	/// Variable if this object type is AUDIO. Gets or sets the this AudioClip.
	/// </summary>
	/// <value>
	/// This AudioClip.
	/// </value>
	public AudioClip ThisAudioClip
	{
		get
		{
			return thisAudioClip;	
		}
		set
		{
			thisAudioClip = value;	
		}
	}
	
	/// <summary>
	/// Variable if this object type is TEXTURE. Gets or sets the this Texture.
	/// Can be used with MovieTexture(Pro-only feature)
	/// </summary>
	public Texture ThisTexture
	{
		get
		{
			return thisTexture;	
		}
		set
		{
			thisTexture = value;	
		}
	}
	
	/// <summary>
	/// Gets or sets if this object should use an asset from a different language.
	/// </summary>
	public bool OverrideLocalizedObject
	{
		get
		{
			return overrideLocalizedObject;	
		}
		set
		{
			overrideLocalizedObject = value;
			
			if(!overrideLocalizedObject)
			{
				overrideObjectLanguageCode = null;
			}
			else{
				ThisAudioClip = null;
				ThisTexture = null;
				ThisGameObject = null;
				ThisTexture = null;
				ThisTextAsset = null;
				Font = null;
			}
		}
	}
	
	/// <summary>
	/// If overrideLocalizedObject is true, this gets or sets the language code that it will use to get the other language.
	/// </summary>
	public string OverrideObjectLanguageCode
	{
		get
		{
			return overrideObjectLanguageCode;
		}
		set
		{
			overrideObjectLanguageCode = value;
			if(overrideLocalizedObject){
				textValue = "override=" + overrideObjectLanguageCode;
			}
		}
	}
	
	/// <summary>
	/// Variable if this object type is TEXT_ASSET. Gets or sets the this TextAsset.
	/// </summary>
	public TextAsset ThisTextAsset
	{
		get
		{
			return thisTextAsset;	
		}
		set
		{
			thisTextAsset = value;	
		}
	}
	
	/// <summary>
	/// Variable if this object type is FONT. Gets or sets the this Font.
	/// </summary>
	public Font Font
	{
		get
		{
			return font;	
		}
		set
		{
			font = value;	
		}
	}

#endregion

	/// <summary>
	/// Creates a new Localized Object
	/// </summary>
	public LocalizedObject(){}

	/// <summary>
	/// Creates a new Localized Object from another
	/// </summary>
	/// <param name="other">The object to copy</param>
	public LocalizedObject(LocalizedObject other)
	{
		if(other != null)
		{
			objectType = other.ObjectType;
			textValue = other.TextValue;
			overrideLocalizedObject = other.OverrideLocalizedObject;
		}
	}

	/// <summary>
	/// Gets the LocalizedObjectType of the localized object.
	/// </summary>
	/// <returns>
	/// The LocalizedObjectType of the key value
	/// </returns>
	/// <param name='key'>
	/// The language key from the .resx file
	/// </param>
	public static LocalizedObjectType GetLocalizedObjectType(string key)
	{
		if(key.StartsWith(keyTypeIdentifier)) //Check if it is something else than a string value
		{
			if(key.StartsWith(keyTypeIdentifier + "AUDIO" +  endBracket)) // it's an audio file
			{
				return LocalizedObjectType.AUDIO;
			}
			else if(key.StartsWith(keyTypeIdentifier + "GAME_OBJECT" +  endBracket)) // it's a game object(prefab)
			{
				return LocalizedObjectType.GAME_OBJECT;
			}
			else if(key.StartsWith(keyTypeIdentifier + "TEXTURE" +  endBracket)) // it's a texture
			{
				return LocalizedObjectType.TEXTURE;
			}
			else if(key.StartsWith(keyTypeIdentifier + "TEXT_ASSET" +  endBracket)) // it's a text file
			{
				return LocalizedObjectType.TEXT_ASSET;
			}
			else if(key.StartsWith(keyTypeIdentifier + "FONT" +  endBracket)) // it's a font file
			{
				return LocalizedObjectType.FONT;
			}
			else //Something is wrong with the syntax
			{
				Debug.LogError("LocalizedObject.cs: ERROR IN SYNTAX of key:" + key + ", setting object type to STRING");
				return LocalizedObjectType.STRING;
			}
		}
		else //If not - it's a string value
		{
			return LocalizedObjectType.STRING;
		}
	}	
	/// <summary>
	/// Gets the clean key. i.e the key without a type identifier "<type=TEXTURE>" etc.
	/// </summary>
	/// <returns>
	/// The clean key without the type identifier
	/// </returns>
	/// <param name='key'>
	/// The key language value
	/// </param>
	/// <param name='objectType'>
	/// The LocalizedObjectType of the key
	/// </param>
	public static string GetCleanKey(string key, LocalizedObjectType objectType)
	{
		int identifierLength = (keyTypeIdentifier + GetLocalizedObjectTypeStringValue(objectType) + endBracket).Length;
		if(objectType == LocalizedObjectType.STRING)
		{
			return key;
		}
		else if(objectType == LocalizedObjectType.AUDIO ||
				objectType == LocalizedObjectType.GAME_OBJECT ||
				objectType == LocalizedObjectType.TEXTURE)
		{
			return key.Substring(identifierLength);	
		}
		else if (objectType == LocalizedObjectType.TEXT_ASSET ||
				 objectType == LocalizedObjectType.FONT)
		{
			return key.Substring(identifierLength);	
		}
		//if none of the above, return the key and send error message
		Debug.LogError("LocalizedObject.GetCleanKey(key) error!, object type is unknown! objectType:" + (int)objectType);
		return key;
	}
	/// <summary>
	/// Gets the clean key. i.e the key without a type identifier "[type=TEXTURE]" etc.
	/// </summary>
	/// <returns>
	/// The clean key without the type identifier
	/// </returns>
	/// <param name='key'>
	/// The full key with the type identifier
	/// </param>
	public static string GetCleanKey(string key)
	{
		LocalizedObjectType objectType = GetLocalizedObjectType(key);
		return GetCleanKey(key, objectType);
	}
	/// <summary>
	/// Gets the full key value with identifiers and everything
	/// </summary>
	/// <returns>
	/// The full key.
	/// </returns>
	/// <param name='parsedKey'>
	/// Parsed key. (Clean key originally from GetCleanKey)
	/// </param>
	public string GetFullKey(string parsedKey)
	{
		if(objectType == LocalizedObjectType.STRING) 
		{
			//No identifier is returned for a string
			return parsedKey;	
		}
		
		return (keyTypeIdentifier + GetLocalizedObjectTypeStringValue(objectType) + endBracket + parsedKey);
	}
	/// <summary>
	/// Gets the full key value with identifiers and everything
	/// </summary>
	/// <returns>
	/// The full key.
	/// </returns>
	/// <param name='parsedKey'>
	/// Parsed key. (Clean key originally from GetCleanKey)
	/// </param>
	public static string GetFullKey(string parsedKey, LocalizedObjectType objectType)
	{
		if(objectType == LocalizedObjectType.STRING) 
		{
			//No identifier is returned for a string
			return parsedKey;	
		}
		
		return (keyTypeIdentifier + GetLocalizedObjectTypeStringValue(objectType) + endBracket + parsedKey);
	}

	/// <summary>
	/// Performs a manual ToString() on a LocalizedObjectType enum since ToString() is not 
	/// allowed on WebPlayer platforms.
	/// </summary>
	/// <param name="objectType">The enumeration to get the string value from.</param>
	/// <returns>The string value of the localized object type enumeration</returns>
	public static string GetLocalizedObjectTypeStringValue(LocalizedObjectType objectType)
	{
		switch(objectType)
		{
			case LocalizedObjectType.AUDIO:
				return "AUDIO";
			case LocalizedObjectType.GAME_OBJECT:
				return "GAME_OBJECT";
			case LocalizedObjectType.STRING:
				return "STRING";
			case LocalizedObjectType.TEXTURE:
				return "TEXTURE";
			case LocalizedObjectType.TEXT_ASSET:
				return "TEXT_ASSET";
			case LocalizedObjectType.FONT:
				return "FONT";
			default:
				return "STRING";
		}
	}
}
} //namespace SmartLocalization

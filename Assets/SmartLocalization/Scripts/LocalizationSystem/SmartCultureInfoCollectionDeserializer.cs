// SmartCultureInfoCollectionDeserializer.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//

namespace SmartLocalization
{
using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;

internal static class SmartCultureInfoCollectionDeserializer 
{
	public static SmartCultureInfoCollection Deserialize(TextAsset xmlFile)
	{
		if(xmlFile == null)
		{
			return null;
		}

		SmartCultureInfoCollection newCollection = new SmartCultureInfoCollection();
		using(StringReader stringReader = new StringReader(xmlFile.text))
		{
			using(XmlReader reader = XmlReader.Create(stringReader))
			{
				ReadElements(reader, newCollection);
			}	
		}			
		return newCollection;
	}

	static void ReadElements(XmlReader reader, SmartCultureInfoCollection newCollection)
	{
		while (reader.Read())
		{
			if(reader.NodeType == XmlNodeType.Element && reader.Name == "CultureInfo")
			{
				ReadData(reader, newCollection);
			}
		}
	}
	
	static void ReadData(XmlReader reader, SmartCultureInfoCollection newCollection)
	{
		string languageCode = string.Empty;
		string englishName = string.Empty;
		string nativeName = string.Empty;
		bool isRightToLeft = false;
	
		//Read the child nodes
		if (reader.ReadToDescendant("languageCode"))
		{
			languageCode = reader.ReadElementContentAsString();
		}

		if(reader.ReadToNextSibling("englishName"))
		{
			englishName = reader.ReadElementContentAsString();
		}

		if(reader.ReadToNextSibling("nativeName"))
		{
			nativeName = reader.ReadElementContentAsString();
		}
			
		if(reader.ReadToNextSibling("isRightToLeft"))
		{
			isRightToLeft = reader.ReadElementContentAsBoolean();
		}
	

		newCollection.AddCultureInfo(new SmartCultureInfo(languageCode, englishName, nativeName, isRightToLeft));
	}

}
}
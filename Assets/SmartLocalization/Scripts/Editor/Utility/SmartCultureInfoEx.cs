using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Text;

namespace SmartLocalization.Editor
{
public static class SmartCultureInfoEx
{
	/// <summary>
	/// Extension method to deserialize a SmartCultureInfoCollection
	/// </summary>
	/// <param name="fullPath">The full path where the serialized SmartCultureInfoCollection can be found</param>
	/// <returns>The deserialized SmartCultureInfoCollection</returns>
	public static SmartCultureInfoCollection Deserialize(string fullPath)
	{
		var serializer = new XmlSerializer(typeof(SmartCultureInfoCollection));
		SmartCultureInfoCollection deserializedCulture = null;
		using(var stream = new FileStream(fullPath, FileMode.Open))
		{
			deserializedCulture = serializer.Deserialize(stream) as SmartCultureInfoCollection;
		}
		
		return deserializedCulture;
	}

	/// <summary>
	/// Extension method to serialize a SmartCultureInfoCollection to a given file path using XMLSerializer.
	/// </summary>
	/// <param name="info">The collection to serialize</param>
	/// <param name="fullPath">The full path where the collection will be saved</param>
	public static void Serialize(this SmartCultureInfoCollection info, string fullPath)
	{
		var serializer = new XmlSerializer(typeof(SmartCultureInfoCollection));
			
		using(XmlTextWriter writer = new XmlTextWriter(fullPath,Encoding.UTF8))
		{
			writer.Formatting = Formatting.Indented;
			serializer.Serialize(writer, info);
		}
	}
}
}

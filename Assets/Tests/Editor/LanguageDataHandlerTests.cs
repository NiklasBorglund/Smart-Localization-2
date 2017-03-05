using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Text;

namespace SmartLocalization.Editor
{
[TestFixture]
public class LanguageDataHandlerTests 
{
	static readonly string resxTemplateDataStart = 
			"<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
			"<root>\n" +
  			"<xsd:schema>\n" +
  			"</xsd:schema>\n";
  			
  	static readonly string resxTemplateDataEnd = "</root>";

	[Test]
	public void GetTextValue_Success()
	{
		var languageDataHandler = GetLanguageDataHandler();
		Assert.IsTrue(languageDataHandler.Load(GetTestData("TestKey", "TestValue", LocalizedObjectType.STRING)));
		Assert.AreEqual("TestValue", languageDataHandler.GetTextValue("TestKey"));
	}
	
	[Test]
	public void GetTextValue_Failure()
	{
		var languageDataHandler = GetLanguageDataHandler();
		Assert.IsTrue(languageDataHandler.Load(GetTestData("TestKey", "TestValue", LocalizedObjectType.STRING)));
		Assert.AreEqual(null, languageDataHandler.GetTextValue("TestKeyNOTEXIST"));
	}
	
	[Test]
	public void TestHasKey()
	{
		var languageDataHandler = GetLanguageDataHandler();
		Assert.IsTrue(languageDataHandler.Load(GetTestData("TestKey", "TestValue", LocalizedObjectType.STRING)));
		Assert.IsTrue(languageDataHandler.HasKey("TestKey"));
		Assert.IsFalse(languageDataHandler.HasKey("TestKeyNOTEXIST"));
	}
	
	[Test]
	public void GetAllKeys()
	{
		var languageDataHandler = GetLanguageDataHandler();
		languageDataHandler.Load(GetTestData("First", string.Empty, LocalizedObjectType.STRING));
		languageDataHandler.Append(GetTestData("Second", string.Empty, LocalizedObjectType.STRING));
		languageDataHandler.Append(GetTestData("Third", string.Empty, LocalizedObjectType.STRING));
		var categoryKeys = languageDataHandler.GetAllKeys();
		Assert.IsNotNull(categoryKeys);
		Assert.That(categoryKeys.Count == 3);
		Assert.That(categoryKeys.Contains("First"));
		Assert.That(categoryKeys.Contains("Second"));
		Assert.That(categoryKeys.Contains("Third"));
	}
	
	[Test]
	public void GetNonExistingCategory()
	{
		var languageDataHandler = GetLanguageDataHandler();
		Assert.IsTrue(languageDataHandler.Load(GetTestData("TestKey", "TestValue", LocalizedObjectType.STRING)));
		var categoryKeys = languageDataHandler.GetKeysWithinCategory("NOTEXISTING");
		Assert.IsNotNull(categoryKeys);
		Assert.That(categoryKeys.Count == 0);
	}
	
	[Test]
	public void GetCategoryKeys()
	{
		var languageDataHandler = GetLanguageDataHandler();
		languageDataHandler.Load(GetTestData("SameCategory.First", string.Empty, LocalizedObjectType.STRING));
		languageDataHandler.Append(GetTestData("SameCategory.Second", string.Empty, LocalizedObjectType.STRING));
		languageDataHandler.Append(GetTestData("OtherCategory.First", string.Empty, LocalizedObjectType.STRING));
		var categoryKeys = languageDataHandler.GetKeysWithinCategory("SameCategory");
		Assert.IsNotNull(categoryKeys);
		Assert.That(categoryKeys.Count == 2);
		Assert.That(categoryKeys.Contains("SameCategory.First"));
		Assert.That(categoryKeys.Contains("SameCategory.Second"));
		Assert.IsFalse(categoryKeys.Contains("OtherCategory.First"));
	}
	
	[Test]
	public void TestAppendLanguage_Overwrite()
	{
		var languageDataHandler = GetLanguageDataHandler();
		Assert.IsTrue(languageDataHandler.Load(GetTestData("TestKey", "TestValue", LocalizedObjectType.STRING)));
		languageDataHandler.Append(GetTestData("TestKey", "TestValueOVERWRITE", LocalizedObjectType.STRING));
		Assert.AreEqual("TestValueOVERWRITE", languageDataHandler.GetTextValue("TestKey"));
	}
	
	[Test]
	public void TestAppendLanguage()
	{
		var languageDataHandler = GetLanguageDataHandler();
		Assert.IsTrue(languageDataHandler.Load(GetTestData("TestKey", "TestValue", LocalizedObjectType.STRING)));
		Assert.AreEqual(null, languageDataHandler.GetTextValue("TestKey2"));
		languageDataHandler.Append(GetTestData("TestKey2", "TestValue2", LocalizedObjectType.STRING));
		Assert.AreEqual("TestValue2", languageDataHandler.GetTextValue("TestKey2"));
	}
	
	[Test]
	[ExpectedException(typeof(AssetSuccessfullyLoadedException))] //Maybe not the best way to test - but it is indeed a way ;)
	public void GetAudio_Success()
	{
		var languageDataHandler = GetLanguageDataHandler();
		((MockLocalizedAssetLoader)languageDataHandler.AssetLoader).TestScenario = MockLocalizedAssetLoader.Scenario.ThrowSuccessfullyLoadedExceptions;
		Assert.IsTrue(languageDataHandler.Load(GetTestData("TestKey", "TestValue", LocalizedObjectType.AUDIO)));
		languageDataHandler.GetAsset<AudioClip>("TestKey");		
	}
	
	[Test]
	[ExpectedException(typeof(AssetSuccessfullyLoadedException))]
	public void GetFont_Success()
	{
		var languageDataHandler = GetLanguageDataHandler();
		((MockLocalizedAssetLoader)languageDataHandler.AssetLoader).TestScenario = MockLocalizedAssetLoader.Scenario.ThrowSuccessfullyLoadedExceptions;
		Assert.IsTrue(languageDataHandler.Load(GetTestData("TestKey", "TestValue", LocalizedObjectType.FONT)));
		languageDataHandler.GetAsset<Font>("TestKey");		
	}
	
	[Test]
	[ExpectedException(typeof(AssetSuccessfullyLoadedException))]
	public void GetGameObject_Success()
	{
		var languageDataHandler = GetLanguageDataHandler();
		((MockLocalizedAssetLoader)languageDataHandler.AssetLoader).TestScenario = MockLocalizedAssetLoader.Scenario.ThrowSuccessfullyLoadedExceptions;
		Assert.IsTrue(languageDataHandler.Load(GetTestData("TestKey", "TestValue", LocalizedObjectType.GAME_OBJECT)));
		languageDataHandler.GetAsset<GameObject>("TestKey");		
	}
	
	[Test]
	[ExpectedException(typeof(AssetSuccessfullyLoadedException))]
	public void GetTextAsset_Success()
	{
		var languageDataHandler = GetLanguageDataHandler();
		((MockLocalizedAssetLoader)languageDataHandler.AssetLoader).TestScenario = MockLocalizedAssetLoader.Scenario.ThrowSuccessfullyLoadedExceptions;
		Assert.IsTrue(languageDataHandler.Load(GetTestData("TestKey", "TestValue", LocalizedObjectType.TEXT_ASSET)));
		languageDataHandler.GetAsset<TextAsset>("TestKey");		
	}
	
	[Test]
	[ExpectedException(typeof(AssetSuccessfullyLoadedException))]
	public void GetTexture_Success()
	{
		var languageDataHandler = GetLanguageDataHandler();
		((MockLocalizedAssetLoader)languageDataHandler.AssetLoader).TestScenario = MockLocalizedAssetLoader.Scenario.ThrowSuccessfullyLoadedExceptions;
		Assert.IsTrue(languageDataHandler.Load(GetTestData("TestKey", "TestValue", LocalizedObjectType.TEXTURE)));
		languageDataHandler.GetAsset<Texture>("TestKey");		
	}
	
	[Test]
	public void GetAudio_KeyNotExist()
	{
		var languageDataHandler = GetLanguageDataHandler();
		Assert.IsTrue(languageDataHandler.Load(GetTestData("TestKey", "TestValue", LocalizedObjectType.AUDIO)));
		Assert.IsNull(languageDataHandler.GetAsset<AudioClip>("TestKeyNOTEXIST"));		
	}
	
	[Test]
	public void GetFont_KeyNotExist()
	{
		var languageDataHandler = GetLanguageDataHandler();
		Assert.IsTrue(languageDataHandler.Load(GetTestData("TestKey", "TestValue", LocalizedObjectType.FONT)));
		Assert.IsNull(languageDataHandler.GetAsset<Font>("TestKeyNOTEXIST"));		
	}
	
	[Test]
	public void GetGameObject_KeyNotExist()
	{
		var languageDataHandler = GetLanguageDataHandler();
		Assert.IsTrue(languageDataHandler.Load(GetTestData("TestKey", "TestValue", LocalizedObjectType.GAME_OBJECT)));
		Assert.IsNull(languageDataHandler.GetAsset<GameObject>("TestKeyNOTEXIST"));		
	}
	
	[Test]
	public void GetTextAsset_KeyNotExist()
	{
		var languageDataHandler = GetLanguageDataHandler();
		Assert.IsTrue(languageDataHandler.Load(GetTestData("TestKey", "TestValue", LocalizedObjectType.TEXT_ASSET)));
		Assert.IsNull(languageDataHandler.GetAsset<TextAsset>("TestKeyNOTEXIST"));		
	}
	
	[Test]
	public void GetTexture_KeyNotExist()
	{
		var languageDataHandler = GetLanguageDataHandler();
		Assert.IsTrue(languageDataHandler.Load(GetTestData("TestKey", "TestValue", LocalizedObjectType.TEXTURE)));
		Assert.IsNull(languageDataHandler.GetAsset<Texture>("TestKeyNOTEXIST"));		
	}
	
	string GetTestData(string testKey, string testValue, LocalizedObjectType objectType)
	{
		StringBuilder resxData = new StringBuilder(resxTemplateDataStart);
		resxData.Append("<data name=\"");
		resxData.Append(LocalizedObject.GetFullKey(testKey, objectType));
		resxData.Append("\" xml:space=\"preserve\">\n");
		resxData.Append("<value>");
		resxData.Append(testValue);
		resxData.Append("</value>\n");		
		resxData.Append("</data>\n");
		resxData.Append(resxTemplateDataEnd);
		
		return resxData.ToString();
	}
	
	LanguageDataHandler GetLanguageDataHandler()
	{
		LanguageDataHandler dataHandler = new LanguageDataHandler();
		dataHandler.LoadedCulture = new SmartCultureInfo("en", "English", "English", false);
		dataHandler.AssetLoader = new MockLocalizedAssetLoader();
		
		return dataHandler;
	}
}
}

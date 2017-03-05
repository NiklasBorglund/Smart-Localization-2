using NUnit.Framework;
using System;
using UnityEngine;

namespace SmartLocalization.Editor
{
[TestFixture]
public class ApplicationExtensionTests 
{
	[Test]
	public void TestGetSystemLanguage()
	{
		foreach (var systemLanguage in Enum.GetValues(typeof(SystemLanguage))) 
		{
			string systemLanguageString = ApplicationExtensions.GetStringValueOfSystemLanguage((SystemLanguage)systemLanguage);
			Assert.AreEqual(systemLanguage.ToString(), systemLanguageString);
		}
	}

}
}
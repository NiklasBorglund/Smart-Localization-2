using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;
using System;

namespace SmartLocalization.Editor
{
[TestFixture]
public class XLSExporterTests
{
	public static readonly Dictionary<string,string> testData = new Dictionary<string, string>()
	{
		{"TestKey", "TestValue"},
		{"TestKey2", "TestValue2"}
	};

	[Test]
	[ExpectedException(typeof(ArgumentNullException))]
	public void CreateWorkbookFromDictWithNullSheetName_ExpectedException()
	{
		XLSExporter.CreateWorkbookFromDictionary(null, testData);
	}

	[Test]
	[ExpectedException(typeof(ArgumentNullException))]
	public void CreateWorkbookFromDictWithNullData_ExpectedException()
	{
		XLSExporter.CreateWorkbookFromDictionary("MySheet", null);
	}

	[Test]
	[ExpectedException(typeof(ArgumentNullException))]
	public void CreateWorkbookFromMultipleDictsWithNullSheetName_ExpectedException()
	{
		var dictionaries = new Dictionary<string, Dictionary<string, string>>();
		dictionaries.Add("en", testData);
		XLSExporter.CreateWorkbookFromMultipleDictionaries(null, new List<string>(), dictionaries);
	}

	[Test]
	[ExpectedException(typeof(ArgumentNullException))]
	public void CreateWorkbookFromMultipleDictsWithNullKeys_ExpectedException()
	{
		var dictionaries = new Dictionary<string, Dictionary<string, string>>();
		dictionaries.Add("en", testData);
		XLSExporter.CreateWorkbookFromMultipleDictionaries("MySheet", null, dictionaries);
	}

	[Test]
	[ExpectedException(typeof(ArgumentNullException))]
	public void CreateWorkbookFromMultipleDictsWithNullValues_ExpectedException()
	{
		XLSExporter.CreateWorkbookFromMultipleDictionaries("MySheet", new List<string>(), null);
	}

	[Test]	
	public void CreateWorkbookFromDict()
	{
		var workbook = XLSExporter.CreateWorkbookFromDictionary("MySheet", testData);
		Assert.AreEqual(1, workbook.NumberOfSheets);
		var sheet = workbook.GetSheetAt(0);
		Assert.AreEqual("MySheet", sheet.SheetName);
		Assert.AreEqual("TestKey", sheet.GetRow(0).GetCell(0).StringCellValue);
		Assert.AreEqual("TestValue", sheet.GetRow(0).GetCell(1).StringCellValue);
		Assert.AreEqual("TestKey2", sheet.GetRow(1).GetCell(0).StringCellValue);
		Assert.AreEqual("TestValue2", sheet.GetRow(1).GetCell(1).StringCellValue);
	}

	[Test]	
	public void CreateWorkbookFromMultipleDicts()
	{
		var dictionaries = new Dictionary<string, Dictionary<string, string>>();
		dictionaries.Add("en", testData);
		dictionaries.Add("sv", testData);
		var workbook = XLSExporter.CreateWorkbookFromMultipleDictionaries("MySheet", new List<string>(testData.Keys), dictionaries);
		Assert.AreEqual(1, workbook.NumberOfSheets);

        var sheet = workbook.GetSheetAt(0);
        Assert.AreEqual("MySheet", sheet.SheetName);
		Assert.AreEqual(string.Empty, sheet.GetRow(0).GetCell(0).StringCellValue);
		Assert.AreEqual("en", sheet.GetRow(0).GetCell(1).StringCellValue);
		Assert.AreEqual("sv", sheet.GetRow(0).GetCell(2).StringCellValue);
		Assert.AreEqual("TestKey", sheet.GetRow(1).GetCell(0).StringCellValue);
		Assert.AreEqual("TestKey2", sheet.GetRow(2).GetCell(0).StringCellValue);
		Assert.AreEqual("TestValue", sheet.GetRow(1).GetCell(1).StringCellValue);
		Assert.AreEqual("TestValue", sheet.GetRow(1).GetCell(2).StringCellValue);
		Assert.AreEqual("TestValue2", sheet.GetRow(2).GetCell(1).StringCellValue);
		Assert.AreEqual("TestValue2", sheet.GetRow(2).GetCell(2).StringCellValue);
	}
}
}
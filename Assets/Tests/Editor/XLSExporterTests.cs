using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace SmartLocalization.Editor
{
	[TestFixture]
	public class XLSExporterTests
	{
		public static readonly Dictionary<string, string> testData = new Dictionary<string, string>()
		{ { "TestKey", "TestValue" }, { "TestKey2", "TestValue2" }
		};

		[Test]
		public void CreateWorkbookFromDictWithNullSheetName_ExpectedException()
		{
			Assert.That(()=> XLSExporter.CreateWorkbookFromDictionary(null, testData), Throws.ArgumentNullException);
		}

		[Test]
		public void CreateWorkbookFromDictWithNullData_ExpectedException()
		{
			Assert.That(()=> XLSExporter.CreateWorkbookFromDictionary("MySheet", null), Throws.ArgumentNullException);
		}

		[Test]
		public void CreateWorkbookFromMultipleDictsWithNullSheetName_ExpectedException()
		{
			var dictionaries = new Dictionary<string, Dictionary<string, string>>();
			dictionaries.Add("en", testData);
			Assert.That(()=> XLSExporter.CreateWorkbookFromMultipleDictionaries(null, new List<string>(), dictionaries), Throws.ArgumentNullException);
		}

		[Test]
		public void CreateWorkbookFromMultipleDictsWithNullKeys_ExpectedException()
		{
			var dictionaries = new Dictionary<string, Dictionary<string, string>>();
			dictionaries.Add("en", testData);
			Assert.That(()=> XLSExporter.CreateWorkbookFromMultipleDictionaries("MySheet", null, dictionaries), Throws.ArgumentNullException);
		}

		[Test]
		public void CreateWorkbookFromMultipleDictsWithNullValues_ExpectedException()
		{
			Assert.That(()=> XLSExporter.CreateWorkbookFromMultipleDictionaries("MySheet", new List<string>(), null), Throws.ArgumentNullException);
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
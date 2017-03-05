using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;


namespace SmartLocalization.Editor{
	[TestFixture]
	public class CSVParserTests {
		static string TestCSVDataWithComma = "\"TestKey\",\"TestValue\"";
		static string TestCSVDataWithSemiColon = "\"TestKey\";\"TestValue\"";
		static string TestCSVDataWithTab = "\"TestKey\"\t\"TestValue\"";
		static string TestCSVDataWithVerticalBar = "\"TestKey\"|\"TestValue\"";
		static string TestCSVDataWithCaret = "\"TestKey\"^\"TestValue\"";
		static string TestCSVDataWithQuoteMarks = "\"TestKey\",\"TestValue\"\"\"\"\"";
		
		List<List<string>> GetCsvWriteTestData(string testKey, string testValue)
		{
			List<List<string>> csvData = new List<List<string>>();
			List<string> firstLine = new List<string>();
			firstLine.Add(testKey);
			firstLine.Add(testValue);
			csvData.Add(firstLine);
			return csvData;
		}
		
		[Test]
		public void TestCSVWriteWithComma_Success()
		{
			var testData = GetCsvWriteTestData("TestKey", "TestValue");
			string csv = CSVParser.WriteToString(CSVParser.GetDelimiter(CSVParser.Delimiter.COMMA), testData);
			var actualData = CSVParser.ReadFromString(csv, CSVParser.GetDelimiter(CSVParser.Delimiter.COMMA));
			Assert.AreEqual(testData[0][0], actualData[0][0]);
			Assert.AreEqual(testData[0][1], actualData[0][1]);
		}
		
		[Test]
		public void TestCSVReadWithWrongDelimiter_Failure()
		{
			var result = CSVParser.ReadFromString(TestCSVDataWithCaret, CSVParser.GetDelimiter(CSVParser.Delimiter.COMMA));
			Assert.AreEqual(1, result.Count);
			Assert.AreNotEqual(2, result[0].Count);
			Assert.AreNotEqual("TestKey", result[0][0]);
		}
		
		[Test]
		public void TestCSVWriteWithSemiColon_Success()
		{
			var testData = GetCsvWriteTestData("TestKey", "TestValue");
			string csv = CSVParser.WriteToString(CSVParser.GetDelimiter(CSVParser.Delimiter.SEMI_COLON), testData);
			var actualData = CSVParser.ReadFromString(csv, CSVParser.GetDelimiter(CSVParser.Delimiter.SEMI_COLON));
			Assert.AreEqual(testData[0][0], actualData[0][0]);
			Assert.AreEqual(testData[0][1], actualData[0][1]);
		}
		
		[Test]
		public void TestCSVWriteWithTab_Success()
		{
			var testData = GetCsvWriteTestData("TestKey", "TestValue");
			string csv = CSVParser.WriteToString(CSVParser.GetDelimiter(CSVParser.Delimiter.TAB), testData);
			var actualData = CSVParser.ReadFromString(csv, CSVParser.GetDelimiter(CSVParser.Delimiter.TAB));
			Assert.AreEqual(testData[0][0], actualData[0][0]);
			Assert.AreEqual(testData[0][1], actualData[0][1]);
		}
		
		[Test]
		public void TestCSVWriteWithVerticalBar_Success()
		{
			var testData = GetCsvWriteTestData("TestKey", "TestValue");
			string csv = CSVParser.WriteToString(CSVParser.GetDelimiter(CSVParser.Delimiter.VERTICAL_BAR), testData);
			var actualData = CSVParser.ReadFromString(csv, CSVParser.GetDelimiter(CSVParser.Delimiter.VERTICAL_BAR));
			Assert.AreEqual(testData[0][0], actualData[0][0]);
			Assert.AreEqual(testData[0][1], actualData[0][1]);
		}
		
		[Test]
		public void TestCSVWriteWithCaret_Success()
		{
			var testData = GetCsvWriteTestData("TestKey", "TestValue");
			string csv = CSVParser.WriteToString(CSVParser.GetDelimiter(CSVParser.Delimiter.CARET), testData);
			var actualData = CSVParser.ReadFromString(csv, CSVParser.GetDelimiter(CSVParser.Delimiter.CARET));
			Assert.AreEqual(testData[0][0], actualData[0][0]);
			Assert.AreEqual(testData[0][1], actualData[0][1]);
		}
		
		[Test]
		public void TestCSVReadWithComma_Success()
		{
			var result = CSVParser.ReadFromString(TestCSVDataWithComma, CSVParser.GetDelimiter(CSVParser.Delimiter.COMMA));
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(2, result[0].Count);
			Assert.AreEqual("TestKey", result[0][0]);
			Assert.AreEqual("TestValue", result[0][1]);
		}
		
		[Test]
		public void TestCSVReadWithSemiColon_Success()
		{
			var result = CSVParser.ReadFromString(TestCSVDataWithSemiColon, CSVParser.GetDelimiter(CSVParser.Delimiter.SEMI_COLON));
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(2, result[0].Count);
			Assert.AreEqual("TestKey", result[0][0]);
			Assert.AreEqual("TestValue", result[0][1]);
		}
		
		[Test]
		public void TestCSVReadWithTab_Success()
		{
			var result = CSVParser.ReadFromString(TestCSVDataWithTab, CSVParser.GetDelimiter(CSVParser.Delimiter.TAB));
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(2, result[0].Count);
			Assert.AreEqual("TestKey", result[0][0]);
			Assert.AreEqual("TestValue", result[0][1]);
		}
		
		[Test]
		public void TestCSVReadWithVerticalBar_Success()
		{
			var result = CSVParser.ReadFromString(TestCSVDataWithVerticalBar, CSVParser.GetDelimiter(CSVParser.Delimiter.VERTICAL_BAR));
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(2, result[0].Count);
			Assert.AreEqual("TestKey", result[0][0]);
			Assert.AreEqual("TestValue", result[0][1]);
		}
		
		[Test]
		public void TestCSVReadWithCaret_Success()
		{
			var result = CSVParser.ReadFromString(TestCSVDataWithCaret, CSVParser.GetDelimiter(CSVParser.Delimiter.CARET));
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(2, result[0].Count);
			Assert.AreEqual("TestKey", result[0][0]);
			Assert.AreEqual("TestValue", result[0][1]);
		}
		
		[Test]
		public void TestCSVReadWithExtraQuoteMarks_Success()
		{
			var result = CSVParser.ReadFromString(TestCSVDataWithQuoteMarks, CSVParser.GetDelimiter(CSVParser.Delimiter.COMMA));
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(2, result[0].Count);
			Assert.AreEqual("TestKey", result[0][0]);
			Assert.AreEqual("TestValue\"\"", result[0][1]);
		}
		
		[Test]
		[ExpectedException(typeof(System.ArgumentException))]
		public void TestCSVReadEmptyContent_ArgumentException()
		{
			CSVParser.ReadFromString(null, CSVParser.GetDelimiter(CSVParser.Delimiter.COMMA));
		}
		
		[Test]
		[ExpectedException(typeof(System.IO.FileNotFoundException))]
		public void TestCSVRead_FileNotFoundException()
		{
			CSVParser.Read(string.Empty, CSVParser.GetDelimiter(CSVParser.Delimiter.COMMA));
		}
	}
}

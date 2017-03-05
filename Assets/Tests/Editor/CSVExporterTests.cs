using NUnit.Framework;
using System;
using UnityEngine;

namespace SmartLocalization.Editor
{
[TestFixture]
public class CSVExporterTests 
{


#pragma warning disable 618		
	[Test]
	public void TestObsoleteGetDelimiter()
	{	
		Assert.AreEqual(CSVExporter.GetDelimiter(CSVDelimiter.COMMA), CSVParser.GetDelimiter(CSVParser.Delimiter.COMMA));
		Assert.AreEqual(CSVExporter.GetDelimiter(CSVDelimiter.SEMI_COLON), CSVParser.GetDelimiter(CSVParser.Delimiter.SEMI_COLON));
		Assert.AreEqual(CSVExporter.GetDelimiter(CSVDelimiter.TAB), CSVParser.GetDelimiter(CSVParser.Delimiter.TAB));
		Assert.AreEqual(CSVExporter.GetDelimiter(CSVDelimiter.VERTICAL_BAR), CSVParser.GetDelimiter(CSVParser.Delimiter.VERTICAL_BAR));
		Assert.AreEqual(CSVExporter.GetDelimiter(CSVDelimiter.CARET), CSVParser.GetDelimiter(CSVParser.Delimiter.CARET));
	}
#pragma warning restore 618
}
}
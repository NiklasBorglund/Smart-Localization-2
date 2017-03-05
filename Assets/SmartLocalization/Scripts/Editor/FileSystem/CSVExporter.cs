// CSVExporter.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SmartLocalization.Editor
{
	/// <summary>
	/// The delimiter type for CSV
	/// </summary>
	[System.Obsolete("use CSVParser.Delimiter")]
	public enum CSVDelimiter
	{
		/// <summary> COMMA</summary>
		COMMA,
		/// <summary> SEMI_COLON</summary>
		SEMI_COLON,
		/// <summary> TAB</summary>
		TAB,
		/// <summary> VERTICAL_BAR</summary>
		VERTICAL_BAR,
		/// <summary> CARET</summary>
		CARET,
	}

	/// <summary>
	/// A CSV Exporter
	/// </summary>
	public static class CSVExporter
	{
		/// <summary>
		/// Gets the actual delimiter char based on the CSVDelimiter type
		/// </summary>
		/// <param name="delimiter">The delimiter type</param>
		/// <returns>the delimiter</returns>
		[System.Obsolete("use CSVParser.GetDelimiter")]
		public static char GetDelimiter(CSVDelimiter delimiter)
		{
			return CSVParser.GetDelimiter((CSVParser.Delimiter)delimiter);
		}

		/// <summary>
		/// Write the csv to file
		/// </summary>
		/// <param name="path">The destination path</param>
		/// <param name="delimiter">The delimiter to separate values with</param>
		/// <param name="input">The Values</param>
		[System.Obsolete("use CSVParser.Write")]
		public static void WriteCSV(string path, char delimiter, List<List<string>> input)
		{
			CSVParser.Write(path, delimiter, input);
		}

		/// <summary>
		/// Write a combined CSV to File
		/// </summary>
		/// <param name="path">The destination path</param>
		/// <param name="delimiter">The delimiter to separate values with</param>
		[System.Obsolete("use CSVParser.Write")]
		public static void WriteCSV(string path, char delimiter, List<string> keys, Dictionary<string, Dictionary<string, string>> languages)
		{
			CSVParser.Write(path, delimiter, keys, languages);
		}	
		
		/// <summary>
		/// Read a csv file
		/// </summary>
		/// <param name="path">The path to the file</param>
		/// <param name="delimiter">The delimiter used in the file</param>
		/// <returns>The parsed csv values</returns>
		[System.Obsolete("use CSVParser.Read")]
		public static List<List<string>> ReadCSV(string path, char delimiter)
		{
			return CSVParser.Read(path, delimiter);
		}
	}
}


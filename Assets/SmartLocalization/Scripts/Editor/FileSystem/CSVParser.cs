// CSVParser.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//
using System.Collections.Generic;
using System.Text;
using System.IO;
using CsvHelper;

namespace SmartLocalization.Editor
{	
	/// <summary>
	/// A CSV Parser
	/// </summary>
	public static class CSVParser
	{
		/// <summary>
		/// The delimiter type for CSV
		/// </summary>
		public enum Delimiter
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
		/// Gets the actual delimiter char based on the CSVDelimiter type
		/// </summary>
		/// <param name="delimiter">The delimiter type</param>
		/// <returns>the delimiter</returns>
		public static char GetDelimiter(Delimiter delimiter)
		{
			switch(delimiter)
			{
			case Delimiter.COMMA:
				return ',';
			case Delimiter.SEMI_COLON:
				return ';';
			case Delimiter.TAB:
				return '\t';
			case Delimiter.VERTICAL_BAR:
				return '|';
			case Delimiter.CARET:
				return '^';
			default:
				return ',';
			}
		}
		
		/// <summary>
		/// Write the csv to file
		/// </summary>
		/// <param name="path">The destination path</param>
		/// <param name="delimiter">The delimiter to separate values with</param>
		/// <param name="input">The Values</param>
		public static void Write(string path, char delimiter, List<List<string>> input)
		{
			var csvContent = WriteToString(delimiter, input);
			FileUtility.WriteToFile(path, csvContent);
		}
		
		/// <summary>
		/// Write a combined CSV to File
		/// </summary>
		/// <param name="path">The destination path</param>
		/// <param name="delimiter">The delimiter to separate values with</param>
		public static void Write(string path, char delimiter, List<string> keys, Dictionary<string, Dictionary<string, string>> languages)
		{
			var csvContent = WriteToString(delimiter, keys, languages);
			FileUtility.WriteToFile(path, csvContent);
		}
		
		internal static string WriteToString(char delimiter, List<List<string>> input)
		{
			string csvContent = null;
			using (var sw = new StringWriter())
			{
				var csv = new CsvWriter(sw);
				csv.Configuration.Delimiter = delimiter.ToString();
				foreach (List<string> list in input)
				{
					foreach (string value in list)
					{
						csv.WriteField(value);
					}
					csv.NextRecord();
				}
				csvContent = sw.ToString();
			}
			return csvContent;
		}
		
		internal static string WriteToString(char delimiter, List<string> keys, Dictionary<string, Dictionary<string, string>> languages)
		{
			string csvContent = null;
			keys.Sort();
			using (var sw = new StringWriter())
			{
				var csv = new CsvWriter(sw);
				csv.Configuration.Delimiter = delimiter.ToString();
				csv.WriteField("");
				foreach (var language in languages.Keys)
				{
					csv.WriteField(language);
				}
				
				csv.NextRecord();
				
				foreach (var key in keys)
				{
					csv.WriteField(key);
					
					foreach (var pair in languages)
					{
						csv.WriteField(pair.Value.ContainsKey(key) ? pair.Value[key] : "");
					}
					
					csv.NextRecord();
				}
				csvContent = sw.ToString();
			}
			return csvContent;
		}
		
		/// <summary>
		/// Read a csv file
		/// </summary>
		/// <param name="path">The path to the file</param>
		/// <param name="delimiter">The delimiter used in the file</param>
		/// <returns>The parsed csv values</returns>
		public static List<List<string>> Read(string path, char delimiter)
		{
			string csvContent = null;
			if(FileUtility.Exists(path))
			{
				FileUtility.ReadFromFile(path, out csvContent);
				return ReadFromString(csvContent, delimiter);
			}
			
			throw new System.IO.FileNotFoundException("CSV file does not exist!", path);
		}
		
		internal static List<List<string>> ReadFromString(string csvContent, char delimiter)
		{
			if(string.IsNullOrEmpty(csvContent))
			{
				throw new System.ArgumentException("Empty CSV content. Cannot read CSV", "csvContent");
			}
		
			var rows = new List<List<string>>();
			using(StringReader reader = new StringReader(csvContent))
			{
				var config = new CsvHelper.Configuration.CsvConfiguration();
				config.Delimiter = delimiter.ToString();
				var parser = new CsvParser(reader, config);
				
				while (true)
				{
					var row = parser.Read();
					if( row == null )
					{
						break;
					}
					rows.Add(new List<string>(row));
				}
			}
			
			return rows;
		}
	}
}


// XLSExporter.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//

namespace SmartLocalization.Editor
{
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using System.IO;
    using ExcelLibrary.SpreadSheet;
    using UnityEngine;

    public static class XLSExporter 
    {
	    public static void Write(string path, string sheetName, Dictionary<string,string> values)
	    {
            var workbook = CreateWorkbookFromDictionary(sheetName, values);
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                {
                    workbook.Write(stream);
                }
            }
            catch (System.Exception exception)
            {
                Debug.LogError(exception.Message);
            }
        }

        public static void Write(string path, string sheetName, List<string> keys, Dictionary<string, Dictionary<string, string>> languages)
	    {
            var workbook = CreateWorkbookFromMultipleDictionaries(sheetName, keys, languages);
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                {
                    workbook.Write(stream);
                }
            }
            catch(System.Exception exception)
            {
                Debug.LogError(exception.Message);
            }
        }

        internal static NPOI.SS.UserModel.IWorkbook CreateWorkbookFromDictionary(string sheetName, Dictionary<string, string> values)
        {
            if (string.IsNullOrEmpty(sheetName))
            {
                throw new ArgumentNullException("sheetName");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            NPOI.SS.UserModel.IWorkbook workbook = new NPOI.HSSF.UserModel.HSSFWorkbook();
            var worksheet = workbook.CreateSheet(sheetName);
            int column = 0;
            int row = 0;
            foreach (var pair in values)
            {
                var currentRow = worksheet.CreateRow(row);
                var leftCell = currentRow.CreateCell(column);
                leftCell.SetCellValue(pair.Key);
                var rightCell = currentRow.CreateCell(column +1);
                rightCell.SetCellValue(pair.Value);
                row++;
            }

            return workbook;
        }

        internal static NPOI.SS.UserModel.IWorkbook CreateWorkbookFromMultipleDictionaries(string sheetName, List<string> keys, Dictionary<string, Dictionary<string, string>> languages)
        {
            if (string.IsNullOrEmpty(sheetName))
            {
                throw new ArgumentNullException("sheetName");
            }
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }
            if (languages == null)
            {
                throw new ArgumentNullException("languages");
            }

            keys.Sort();
            NPOI.SS.UserModel.IWorkbook workbook = new NPOI.HSSF.UserModel.HSSFWorkbook();
            var worksheet = workbook.CreateSheet(sheetName);
            int column = 0;
            int row = 0;

            var firstRow = worksheet.CreateRow(row++);
            var firstCell = firstRow.CreateCell(column);
            firstCell.SetCellValue(string.Empty);
            foreach (string key in keys)
            {
                var currentRow = worksheet.CreateRow(row++);
                var currentCell = currentRow.CreateCell(column);
                currentCell.SetCellValue(key);
            }

            column++;
            foreach (var pair in languages)
            {
                row = 0;
                var currentRow = worksheet.GetRow(row++);
                var currentCell = currentRow.CreateCell(column);
                currentCell.SetCellValue(pair.Key);
                foreach (string key in keys)
                {
                    if (pair.Value.ContainsKey(key))
                    {
                        currentRow = worksheet.GetRow(row++);
                        currentCell = currentRow.CreateCell(column);
                        currentCell.SetCellValue(pair.Value[key]);
                    }
                    else
                    {
                        currentRow = worksheet.GetRow(row++);
                        currentCell = currentRow.CreateCell(column);
                        currentCell.SetCellValue(string.Empty);
                    }
                }

                column++;
            }
            return workbook;
        }

#if !UNITY_EDITOR_OSX
        /// <summary>
        ///	Read an xls file from the specified path
        /// </summary>
        /// <param name="path">The path to the xls file</param>
        public static List<List<string>> Read(string path)
	    {
		    var returnValues = new List<List<string>>();
		    try
		    {
			    Workbook book = Workbook.Load(path);
			    Worksheet sheet = book.Worksheets[0];
			    int maxColumns = sheet.Cells.GetRow(0).LastColIndex;
			    for (int rowIndex = sheet.Cells.FirstRowIndex; 
					    rowIndex <= sheet.Cells.LastRowIndex; rowIndex++)
			     {
				     Row row = sheet.Cells.GetRow(rowIndex);
				 
				     List<string> currentRowList = new List<string>();

				     for(int columnIndex = 0; columnIndex <= maxColumns; columnIndex++)
				     {
					     Cell currentCell = row.GetCell(columnIndex);
					     if(currentCell != null)
					     {
						     currentRowList.Add(currentCell.StringValue);
					     }
					     else
					     { 
						     currentRowList.Add(string.Empty);
					     }
				     }

				     returnValues.Add(currentRowList);
			     }
		    }
		    catch(System.Exception ex)
		    {
			    UnityEngine.Debug.LogError("Failed to read xls file at path: " + path + ", Error: " + ex.Message);
		    }

		    return returnValues;
	    }
#else
        /// <summary>
        ///	Read an xls file from the specified path
        /// </summary>
        /// <param name="path">The path to the xls file</param>
        public static List<List<string>> Read(string path)
        {
            var returnValues = new List<List<string>>();
            try
            {
                NPOI.SS.UserModel.IWorkbook book;
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    book = new NPOI.HSSF.UserModel.HSSFWorkbook(stream);
                }

                NPOI.SS.UserModel.ISheet sheet = book.GetSheetAt(0);
    
                for (int rowIndex = sheet.FirstRowNum;
                    rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);

                    List<string> currentRowList = new List<string>();

                    for (int cellIndex = 0; cellIndex < row.LastCellNum; cellIndex++)
                    {
                        var cell = row.GetCell(cellIndex);
                        if (cell != null)
                        {
                            currentRowList.Add(cell.StringCellValue);
                        }
                        else
                        {
                            currentRowList.Add(string.Empty);
                        }
                    }
                    returnValues.Add(currentRowList);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("Failed to read xls file at path: " + path + ", Error: " + ex.Message);
            }

            return returnValues;
        }
#endif
    }
}
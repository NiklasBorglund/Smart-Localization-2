// FileUtility.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//
using UnityEngine;
using System.IO;

namespace SmartLocalization.Editor
{
/// <summary>
/// Utility class for handling files
/// </summary>
public class FileUtility
{
	#region Lookups
	/// <summary>
	/// Checks if a file exists
	/// </summary>
	/// <param name="fullPath">The path to the file</param>
	/// <returns>If the file exists</returns>
	public static bool Exists(string fullPath)
	{
		return File.Exists(fullPath);
	}

	/// <summary>
	/// Gets the file extension for the file at the specified path
	/// </summary>
	/// <param name='fileName'>
	/// The file name without the extension. If the full name is for example hello.png, this parameter
	/// should be only "hello"
	/// </param>
	/// <param name='relativeFolderPath'>
	/// The relative path to the folder containing the asset file
	/// relativeFolderPath should be relative to the project folder. Like: "Assets/MyTextures/".
	/// </param>
	public static string GetFileExtension(string fileName, string relativeFolderPath)
	{
		string fullFolderPath = Application.dataPath + relativeFolderPath;
		
		
		if(!DirectoryUtility.Exists(fullFolderPath))
		{
			return string.Empty;
		}

		string[] assetsInFolder = DirectoryUtility.GetFiles(fullFolderPath);
		
		foreach(string asset in assetsInFolder)
		{
			if(!asset.EndsWith(".meta")) //If this is not a .meta file
			{
				string currentFileName = RemoveExtension(asset);
				if(fileName == currentFileName)
				{
					return GetFileExtension(asset);
				}
			}
		}
		
		return string.Empty;
	}
	
		
	/// <summary>
	/// Gets a file extension of a file
	/// </summary>
	/// <param name="fullPath">The full path to the file</param>
	/// <returns>The file extension of a file</returns>
	public static string GetFileExtension(string fullPath)
	{
		return Path.GetExtension(fullPath);
	}

	/// <summary> Appends the relativePath to Application.dataPath </summary>
	public static bool ExistsRelative(string relativePath)
	{
		return Exists(Application.dataPath + relativePath);
	}

	
	/// <summary>
	/// Returns the file name without the extension
	/// </summary>
	/// <param name="fullPath">The full path to the file</param>
	/// <returns>The file name without the extension</returns>
	public static string RemoveExtension(string fullPath)
	{
		return Path.GetFileNameWithoutExtension(fullPath);
	}
	
	#endregion

	#region File Actions /Create / Delete / Read

	/// <summary>
	/// Deletes a file
	/// </summary>
	/// <param name="fullPath">The full path to the file to delete</param>
	/// <returns>If the file was deleted</returns>
	public static bool Delete(string fullPath)
	{
		try
		{
			File.Delete(fullPath);
			return true;
		}
		catch(System.Exception ex)
		{
			Debug.LogError("Failed to delete file! error:" + ex.Message);
			return false;
		}
	}

	/// <summary>
	/// Writes string data to a file
	/// </summary>
	/// <param name="fullPath">The full path to the file to write to</param>
	/// <param name="data">The string data to write</param>
	/// <returns>If the writing operation was a success</returns>
	public static bool WriteToFile(string fullPath, string data)
	{
		try
		{
			File.WriteAllText(fullPath, data);
			return true;
		}
		catch(System.Exception ex)
		{
			Debug.LogError("Error! Could not save to file! error -" + ex.Message);
			return false;
		}
	}

	/// <summary>
	/// Reads string data from a file
	/// </summary>
	/// <param name="fullPath">The full path to the file to read from</param>
	/// <param name="data">The data that was read from the file</param>
	/// <returns>If the operation was a success</returns>
	public static bool ReadFromFile(string fullPath, out string data)
	{
		if(!Exists(fullPath))
		{
			data = null;
			Debug.LogError("FileUtility.cs: File to read from does not exist!");
			return false;
		}

		try
		{
			data = File.ReadAllText(fullPath);
			return true;
		}
		catch(System.Exception ex)
		{
			data = null;
			Debug.LogError("FileUtility.cs: Could not load from file! error -" + ex.Message);
			return false;
		}
	}
	#endregion
}
}//SmartLocalization.Editor

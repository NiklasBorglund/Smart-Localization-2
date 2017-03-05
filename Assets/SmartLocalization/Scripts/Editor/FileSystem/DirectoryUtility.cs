//DirectoryUtility.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//
using UnityEngine;
using System.IO;

namespace SmartLocalization.Editor
{
/// <summary>
/// Utility class for handling Directories
/// </summary>
public class DirectoryUtility 
{
	/// <summary>
	/// Checks if a directory exists
	/// </summary>
	/// <param name="fullPath">The full path to the directory</param>
	/// <returns>If the directory exists</returns>
	public static bool Exists(string fullPath)
	{
		return Directory.Exists(fullPath);
	}

	/// <summary> Checks if a directory exists. Appends the relativePath to Application.dataPath </summary>
	public static bool ExistsRelative(string relativePath)
	{
		return Exists(Application.dataPath + relativePath);
	}

	/// <summary> Returns the names of files(including their paths) in the directory </summary>
	public static string[] GetFiles(string fullPath)
	{
		return Directory.GetFiles(fullPath);
	}

	/// <summary> Appends the relativePath to Application.dataPath </summary>
	public static string[] GetFilesRelative(string relativePath)
	{
		return GetFiles(Application.dataPath + relativePath);
	}
	
	public static void DeleteAllFilesAndFolders(string folderPath, bool recursive = true, bool isTop = true)
	{
		if(!Exists(folderPath))
		{
			return;
		}
		
		foreach(string file in GetFiles(folderPath))
		{
			File.Delete(file);
		}
		
		if(recursive)
		{
			foreach(string dir in Directory.GetDirectories(folderPath))
			{
				DeleteAllFilesAndFolders(dir, recursive, false);
			}
		}
		
		if(!isTop)
		{
			Directory.Delete(folderPath);
		}
	}

	/// <summary>
	/// Creates a directory
	/// </summary>
	/// <param name="fullPath">The path where the directory should be created</param>
	/// <returns>If the directory was created</returns>
	public static bool Create(string fullPath)
	{
		try
		{
			Directory.CreateDirectory(fullPath);
			return true;
		}
		catch(System.Exception ex)
		{
			Debug.LogError("Failed to create directory! error - " + ex.Message);
			return false;
		}
	}
	/// <summary>
	/// Checks the folder and creates it if it does not exist, only returns false if the directory failed to create
	/// </summary>
	public static bool CheckAndCreate(string fullPath)
	{
		if(!Exists(fullPath))
		{
			return Create(fullPath);
		}
		return true;
	}

	/// <summary> Appends the relativePath to Application.dataPath </summary>
	public static bool CreateRelative(string relativePath)
	{
		return Create(Application.dataPath + relativePath);
	}

}
}//JaneTools.Editor.FileSystem
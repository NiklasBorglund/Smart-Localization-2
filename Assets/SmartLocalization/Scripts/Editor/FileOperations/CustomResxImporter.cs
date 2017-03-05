//
//  CustomResxImporter.cs
//
// Creates or rewrites a .txt file for each .resx file in a subfolder called 
// GeneratedAssets whenever the .resx changes
//
// Written by Niklas Borglund and Jakob Hillerström
//

namespace SmartLocalization.Editor
{

using UnityEditor;
using System.IO;

/// <summary>
/// Resx importer that detects if a resx file changed or was added to the project
/// </summary>
public class CustomResxImporter : AssetPostprocessor 
{
	/// <summary>
	/// Checks .resx files and converts them into text assets that can be used at runtime
	/// </summary>
    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
		//Only use this if there's a localization system created
		if(!LocalizationWorkspace.Exists())
		{
			return;
		}

        foreach (string asset in importedAssets)
        {
            if (asset.EndsWith(LocalizationWorkspace.resXFileEnding))
            {
				string newFileName = LocalizationWorkspace.ResourcesFolderFilePath() + "/" + Path.GetFileNameWithoutExtension(asset) + LocalizationWorkspace.txtFileEnding;

				if(!DirectoryUtility.CheckAndCreate(LocalizationWorkspace.ResourcesFolderFilePath()))
				{
					return;
				}
				
				//Delete the file if it already exists
				if(FileUtility.Exists(newFileName))
				{
					FileUtility.Delete(newFileName);
				}
				
				string fileData = "";
                using(StreamReader reader = new StreamReader(asset))
				{
	                fileData = reader.ReadToEnd();
				}
				

				FileUtility.WriteToFile(newFileName, fileData);

				SmartCultureInfoCollection allCultures = SmartCultureInfoEx.Deserialize(LocalizationWorkspace.CultureInfoCollectionFilePath());
				LanguageHandlerEditor.CheckAndSaveAvailableLanguages(allCultures);

                AssetDatabase.Refresh(ImportAssetOptions.Default);
            }
        }
    }

}
} //namespace SmartLocalization.Editor

using UnityEngine;

namespace SmartLocalization{
internal class RuntimeLocalizedAssetLoader : ILocalizedAssetLoader 
{
	static readonly System.Type GameObjectType = typeof(GameObject);
	static readonly System.Type AudioClipType = typeof(AudioClip);
	static readonly System.Type TextureType = typeof(Texture);
	static readonly System.Type TextAssetType = typeof(TextAsset);
	static readonly System.Type FontType = typeof(Font);

	public T LoadAsset<T>(string assetKey, string languageCode) where T : UnityEngine.Object
	{
		var loadedObject = Resources.Load(GetAssetFolderPath(typeof(T), languageCode) + "/" + assetKey);
		if(loadedObject != null){	
			return (T)loadedObject;
		}
		return default(T);
	}
	
	string GetAssetFolderPath(System.Type assetType, string languageCode)
	{
		if(assetType == GameObjectType)
		{
			return LanguageRuntimeData.PrefabsFolderPath(languageCode);
		}
		else if(assetType == AudioClipType)
		{
			return LanguageRuntimeData.AudioFilesFolderPath(languageCode);
		}
		else if(assetType == TextureType)
		{
			return LanguageRuntimeData.TexturesFolderPath(languageCode);
		}
		else if(assetType == TextAssetType)
		{
			return LanguageRuntimeData.TextAssetsFolderPath(languageCode);
		}
		else if(assetType == FontType)
		{
			return LanguageRuntimeData.FontsFolderPath(languageCode);
		}			
		
		return string.Empty;
	}
}
}
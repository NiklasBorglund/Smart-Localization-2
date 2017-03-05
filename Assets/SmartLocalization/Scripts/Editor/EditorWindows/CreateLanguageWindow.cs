// CreateLanguageWindow.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//

namespace SmartLocalization.Editor
{
using UnityEditor;
using UnityEngine;

public class CreateLanguageWindow : EditorWindow
{
	[SerializeField]
	string englishName  = null;
	[SerializeField]
	string languageCode = null;
	[SerializeField]
	string nativeName	= null;
	[SerializeField]
	bool isRightToLeft	= false;

	bool	showHelpMessage = false;
	string  helpMessage		= null;
	MessageType helpMessageType = MessageType.Info;
	SmartLocalizationWindow parentWindow = null;

	void OnGUI()
	{
		if(LocalizationWindowUtility.ShouldShowWindow())
		{
			GUILayout.Label ("Create a new culture info", EditorStyles.boldLabel);

			languageCode = EditorGUILayout.TextField("Language Code", languageCode);
			if(languageCode != null)
				languageCode = languageCode.RemoveWhitespace();

			englishName = EditorGUILayout.TextField("English Name", englishName);
			nativeName = EditorGUILayout.TextField("Native Name", nativeName);
			isRightToLeft = EditorGUILayout.Toggle("Is Right To Left", isRightToLeft);

			if(GUILayout.Button("Create"))
			{
				SmartCultureInfo newInfo = new SmartCultureInfo();
				newInfo.languageCode = languageCode;
				newInfo.englishName = englishName.Trim();
				newInfo.nativeName = nativeName.Trim();
				newInfo.isRightToLeft = isRightToLeft;

				SmartCultureInfoCollection allCultures = SmartCultureInfoEx.Deserialize(LocalizationWorkspace.CultureInfoCollectionFilePath());
				if(!allCultures.IsCultureInCollection(newInfo))
				{
					allCultures.AddCultureInfo(newInfo);
					allCultures.Serialize(LocalizationWorkspace.CultureInfoCollectionFilePath());
					LanguageHandlerEditor.CheckAndSaveAvailableLanguages(allCultures);

					showHelpMessage = true;
					helpMessageType = MessageType.Info;
					helpMessage = string.Format("Successfully created language!\n Language Code: {0}\n English Name:{1}\n Native Name:{2}\n IsRightToLeft:{3}",
												newInfo.languageCode, newInfo.englishName, newInfo.nativeName, newInfo.isRightToLeft);
												
					if(parentWindow != null)
					{
						parentWindow.InitializeCultureCollections(true);
					}
					
					this.Close();
				}
				else
				{
					SmartCultureInfo conflictingCulture = allCultures.FindCulture(newInfo);
					string conflictingVariable = null;

					if(conflictingCulture.languageCode.ToLower() == newInfo.languageCode.ToLower())
					{
						conflictingVariable = "Language Code:" + newInfo.languageCode;
					}
					else if(conflictingCulture.englishName.ToLower() == newInfo.englishName.ToLower())
					{
						conflictingVariable = "English Name:" + newInfo.englishName;
					}

					showHelpMessage = true;
					helpMessageType = MessageType.Error;
					helpMessage = string.Format("Failed to create language!\n Conflicting variable - {0}\n\n",
												conflictingVariable);

					helpMessage += string.Format("Conflicting Culture \n Language Code: {0}\n English Name:{1}\n Native Name:{2}",
												conflictingCulture.languageCode, conflictingCulture.englishName, conflictingCulture.nativeName);
				}
			}

			if(showHelpMessage)
			{
				EditorGUILayout.HelpBox(helpMessage, helpMessageType);
			}
		}
	}


#region Show Window
	public static CreateLanguageWindow ShowWindow(SmartLocalizationWindow parentWindow)
	{
		CreateLanguageWindow thisWindow = (CreateLanguageWindow)EditorWindow.GetWindow<CreateLanguageWindow>("New Language");
		thisWindow.parentWindow = parentWindow;
		return thisWindow;
	}
#endregion
	}
}
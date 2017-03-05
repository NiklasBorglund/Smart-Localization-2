namespace SmartLocalization.Editor
{
using UnityEngine;
using System.Collections;
using SmartLocalization.ReorderableList;
using UnityEditor;

internal class CreateLanguageMenuControl : ReorderableListControl  
{
	static GUIContent commandCreate = new GUIContent("Create");
	static GUIContent commandImport = new GUIContent("Import from CSV");

	public CreateLanguageMenuControl() : base(ReorderableListFlags.HideAddButton | ReorderableListFlags.DisableContextMenu){}


	//Nothing in here is used ATM, the context menu is disabled
	protected override void AddItemsToMenu(GenericMenu menu, int itemIndex, IReorderableListAdaptor adaptor) 
	{
		menu.AddItem(commandCreate, false, defaultContextHandler, commandCreate);
		menu.AddItem(commandImport, false, defaultContextHandler, commandImport);
	}

	protected override bool HandleCommand(string commandName, int itemIndex,IReorderableListAdaptor adaptor) 
	{
		CreateLanguageListAdaptor smartAdaptor = adaptor as CreateLanguageListAdaptor;

		if(smartAdaptor == null)
		{
			return false;
		}

		switch (commandName) 
		{
			case "Create":
				OnCreateClick(smartAdaptor.GetCultureInfo(itemIndex));
				return true;
			case "Import from CSV":
				OnImportClick(smartAdaptor.GetCultureInfo(itemIndex));
				return true;
		}

		return false;
	}

	void OnCreateClick(SmartCultureInfo info)
	{

	}

	void OnImportClick(SmartCultureInfo info)
	{

	}
}
}

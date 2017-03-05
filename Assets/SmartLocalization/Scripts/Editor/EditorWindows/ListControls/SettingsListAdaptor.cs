namespace SmartLocalization.Editor
{
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmartLocalization.ReorderableList;

internal class SettingsListAdaptor : GenericListAdaptor<string> 
{
	 public SettingsListAdaptor(List<string> list, ReorderableListControl.ItemDrawer<string> itemDrawer, float itemHeight)
		: base(list, itemDrawer, itemHeight) 
	{

	}

	public override void DrawItem(Rect position, int index) 
	{
		base.DrawItem(position, index);
	}

	public override bool CanDrag(int index) 
	{
		return false;
	}

	public override bool CanRemove(int index)
	{
		return false;
	}

	public override float GetItemHeight(int index)
	{
		return base.GetItemHeight(index);
	}
}
}

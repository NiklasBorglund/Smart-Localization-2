
namespace SmartLocalization.Editor
{
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmartLocalization.ReorderableList;

internal class SmartCultureInfoListAdaptor : GenericListAdaptor<SmartCultureInfo> 
{
	 public SmartCultureInfoListAdaptor(List<SmartCultureInfo> list, ReorderableListControl.ItemDrawer<SmartCultureInfo> itemDrawer, float itemHeight)
		: base(list, itemDrawer, itemHeight) 
	{

	}

	public override void DrawItem(Rect position, int index) 
	{
		base.DrawItem(position, index);
	}

	public SmartCultureInfo GetCultureInfo(int itemIndex)
	{
		return this[itemIndex];
	}

	public override bool CanDrag(int index) 
	{
		return false;
	}

	public override bool CanRemove(int index)
	{
		return IsRoot(GetCultureInfo(index)) ? false : true;
	}

	bool IsRoot(SmartCultureInfo info)
	{
		return info.englishName == "ROOT";
	}

}
}
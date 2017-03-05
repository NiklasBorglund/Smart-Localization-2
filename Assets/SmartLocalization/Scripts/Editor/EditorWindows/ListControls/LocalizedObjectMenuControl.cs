
namespace SmartLocalization.Editor
{
using UnityEngine;
using System.Collections;
using SmartLocalization.ReorderableList;
using UnityEditor;
using System;

internal class LocalizedObjectMenuControl : ReorderableListControl 
{
	Action<int> onItemRemoved = null;
	Action<int>	onItemAdded = null;

	public LocalizedObjectMenuControl(Action<int> onItemRemoved, Action<int> onItemAdded) : base(ReorderableListFlags.DisableContextMenu)
	{
		this.onItemRemoved = onItemRemoved;
		this.onItemAdded = onItemAdded;
	}

	public LocalizedObjectMenuControl() : base(ReorderableListFlags.HideAddButton | ReorderableListFlags.DisableContextMenu){}

	public void ClearEvents()
	{
		onItemRemoved = null;
	}

	protected override void OnItemInserted(ItemInsertedEventArgs args)
	{
		base.OnItemInserted(args);

		if(onItemAdded != null)
		{
			onItemAdded(args.itemIndex);
		}
	}

	protected override void OnItemRemoving(ItemRemovingEventArgs args)
	{
		base.OnItemRemoving(args);

		if(onItemRemoved != null)
		{
			onItemRemoved(args.itemIndex);
		}
	}
}
}


namespace SmartLocalization.Editor
{
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmartLocalization.ReorderableList;

internal class LocalizedObjectListAdaptor : GenericListAdaptor<SerializableLocalizationObjectPair>  
{
	public bool collapseMultiline = false;
	string searchLine = string.Empty;
	LanguageSearchType searchType = LanguageSearchType.KEY;
	List<SerializableStringPair> changedRootKeys = null;
	bool isTranslationView = false;
	
	Dictionary<int,bool> searchCache = new Dictionary<int,bool>();
	
	public string SearchLine 
	{
		get
		{
			return searchLine;
		}
		set
		{
			if(searchLine != value)
			{
				ClearSearchCache();
			}
			searchLine = value;
		}
	}
	public LanguageSearchType SearchType
	{
		get
		{
			return searchType;
		}
		set
		{
			if(searchType != value)
			{
				ClearSearchCache();
			}
			searchType = value;
		}
	}


	 public LocalizedObjectListAdaptor(List<SerializableLocalizationObjectPair> list, 
		 List<SerializableStringPair> changedRootKeys, 
		 ReorderableListControl.ItemDrawer<SerializableLocalizationObjectPair> itemDrawer, 
		 float itemHeight, 
		 bool isTranslationView = false)
		: base(list, itemDrawer, itemHeight) 
	{
		 this.changedRootKeys = changedRootKeys;
		 this.isTranslationView = isTranslationView;
	}

	public SerializableLocalizationObjectPair GetObjectPair(int itemIndex)
	{
		return this[itemIndex];
	}

	public string GetCurrentKey(int index)
	{
		if(!isTranslationView)
		{
			if(changedRootKeys == null)
			{
				return string.Empty;
			}

			return changedRootKeys[index].changedValue;
		}
		else
		{
			return GetObjectPair(index).keyValue;
		}
	}

	public override void DrawItem(Rect position, int index) 
	{
		if(IsWithinSearch(index))
		{
			base.DrawItem(position, index);
		}
	}

	public override float GetItemHeight(int index)
	{
		SerializableLocalizationObjectPair currentObjectPair = GetObjectPair(index);

		if(IsWithinSearch(index))
		{
			if(currentObjectPair.changedValue.ObjectType == LocalizedObjectType.STRING)
			{
				if(collapseMultiline)
				{
					return base.GetItemHeight(index);
				}

				var textDimensions = GUI.skin.label.CalcSize(new GUIContent(currentObjectPair.changedValue.TextValue));				
				float currentHeight = base.GetItemHeight(index);

				return Mathf.Max(textDimensions.y, currentHeight);
			}
			else
			{
				return base.GetItemHeight(index);
			}
		}
		else
		{
			return 0;
		}
	}

	public override bool CanDrag(int index) 
	{
		return false;
	}

	public override bool CanRemove(int index)
	{
		if(isTranslationView)
		{
			return false;
		}
		else
		{
			return base.CanRemove(index);
		}
	}

	public override bool CanDraw(int index)
	{
		if(IsWithinSearch(index))
		{
			return base.CanDraw(index);
		}
		else 
		{
			return false;
		}
	}
	
	void ClearSearchCache()
	{
		searchCache.Clear();
	}

	bool IsWithinSearch(int index)
	{
		if(searchLine == string.Empty || searchLine == null)
		{
			return true;
		}
		
		if(searchCache.ContainsKey(index))
		{
			return searchCache[index];
		}
		else
		{
			string key = null;
	
			if(searchType == LanguageSearchType.KEY)
			{
				key = GetCurrentKey(index);
			}
			else if(searchType == LanguageSearchType.VALUE)
			{
				key = GetObjectPair(index).changedValue.TextValue;
			}
	
			if(key != null)
			{
				bool isWithinSearch = key.ToLower().Contains(searchLine.ToLower());
				searchCache.Add(index, isWithinSearch);
				return isWithinSearch;
			}
			else
			{
				return false;
			}
		}
	}
}
}
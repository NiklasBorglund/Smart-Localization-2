
namespace SmartLocalization.Editor
{
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using SmartLocalization.ReorderableList.Internal;

internal class EditorColumns  
{
	class DraggableColumnDivider
	{
		public Rect position;
		public Vector2 currentPosition;
		public Vector2 startPosition;
		public float newSizeValue = 0;
		public float startValue = 0.1f;
		public float previousWindowSize = 0;
		public bool isDragging = false;
	}

	GUIStyle defaultTitleStyle = null;
	Dictionary<string, float> columns = new Dictionary<string,float>();
	Dictionary<string, DraggableColumnDivider> columnDividers = new Dictionary<string,DraggableColumnDivider>();
	float minColumnSize = 0.1f;
	int maxColumns = 10;
	bool fullListRow = false;
	bool isCurrentlyDragging = false;

	float MaxColumnSize
	{
		get
		{
			return Mathf.Max(minColumnSize, minColumnSize + (1.0f - (columns.Count * minColumnSize)));
		}
	}

	public EditorColumns(float minColumnSize, bool fullListRow = false)
	{
		this.minColumnSize = minColumnSize;
		this.fullListRow = fullListRow;
		maxColumns = Mathf.FloorToInt(1.0f / (float)minColumnSize);

		defaultTitleStyle = new GUIStyle();
		defaultTitleStyle.border = new RectOffset(2, 2, 2, 1);
		defaultTitleStyle.margin = new RectOffset(5, 5, 5, 0);
		defaultTitleStyle.padding = new RectOffset(5, 5, 0, 0);
		defaultTitleStyle.alignment = TextAnchor.MiddleLeft;
		defaultTitleStyle.normal.background = ReorderableListResources.texTitleBackground;
		defaultTitleStyle.normal.textColor = EditorGUIUtility.isProSkin
			? new Color(0.8f, 0.8f, 0.8f)
			: new Color(0.2f, 0.2f, 0.2f);
	}

	public bool AddColumn(string title, float desiredColumnWidth)
	{
		if(columns.ContainsKey(title) || columns.Count >= maxColumns)
		{
			return false;
		}

		columns.Add(title, desiredColumnWidth);
		columnDividers.Add(title, new DraggableColumnDivider());
		return true;
	}

	public bool RemoveColumn(string title)
	{
		if(!columns.ContainsKey(title))
		{
			return false;
		}

		columns.Remove(title);
		columnDividers.Remove(title);
		RecalculateColumnWidths();
		return true;
	}

	public bool DrawColumns()
	{
		string titles = string.Empty;
		foreach(var pair in columns)
		{
			titles += pair.Key + " ";
		}

		Rect position = GUILayoutUtility.GetRect(new GUIContent(titles), defaultTitleStyle);
		position.height += 6;

		if (Event.current.type == EventType.Repaint)
		{
			//Draw background
			defaultTitleStyle.Draw(position, string.Empty, false, false, false, false);

			Rect newPosition = position;
			float fullPositionWidth = position.width;

			if(!fullListRow)
			{
				fullPositionWidth -= 34;
			}
			else
			{
				fullPositionWidth -= 9;
			}
			

			newPosition.y += 2;
			newPosition.x += 5;

			int count = 0;
			foreach(var pair in columns)
			{
				newPosition.width = (fullPositionWidth * (pair.Value - 0.01f));
				GUI.Label(newPosition, pair.Key);
				float x = newPosition.x;

				if(count < columns.Count - 1)
				{
					newPosition.width = (fullPositionWidth * minColumnSize);
					newPosition.x =  x + (fullPositionWidth * (pair.Value - 0.01f));
					GUI.Label(newPosition,"|");
				}

				DraggableColumnDivider divider = columnDividers[pair.Key];
				divider.position = newPosition;
				divider.position.x -= divider.position.width * 0.5f;
				divider.currentPosition = new Vector2(divider.position.x, divider.position.y);
				//divider.startPosition = divider.currentPosition;
				divider.newSizeValue = pair.Value;
				divider.previousWindowSize = position.width;

				newPosition.x = x + (fullPositionWidth * pair.Value);

				count++;
			}
		}
		return CheckDividerDragging(position);
	}

	bool CheckDividerDragging(Rect fullRect)
	{

		Vector2 mousePosition = Event.current.mousePosition;
		if (Event.current.type == EventType.MouseUp)
		{
			isCurrentlyDragging = false;

			foreach(var pair in columnDividers)
			{
				pair.Value.isDragging = false;
			}
		}
		else if (Event.current.type == EventType.MouseDown && !isCurrentlyDragging)
		{
			foreach(var pair in columnDividers)
			{
				if(pair.Value.position.Contains(mousePosition))
				{
					pair.Value.isDragging = true;
					isCurrentlyDragging = true;
					pair.Value.startPosition = mousePosition;
					pair.Value.startValue = columns[pair.Key];
					Event.current.Use();
					break;
				}
			}
		}

		if (!isCurrentlyDragging)
		{
			return false;
		}

		List<string> keys = new List<string>(columnDividers.Keys);
		foreach(string key in keys)
		{
			var value = columnDividers[key];
			float currentValue = value.startValue;
			if(value.isDragging)
			{
				value.currentPosition = mousePosition;

				float changeInPixels = (value.currentPosition.x - value.startPosition.x);
				if(changeInPixels != 0)
				{
					bool isNegative = Mathf.Sign(changeInPixels) < 0 ? true : false;
					changeInPixels = Mathf.Abs(changeInPixels);

					float relativeChange = (changeInPixels / value.previousWindowSize);
					if(isNegative)
					{
						changeInPixels = currentValue - relativeChange;
					}
					else
					{
						changeInPixels = currentValue + relativeChange;
					}

					value.newSizeValue = Mathf.Clamp(changeInPixels, minColumnSize, MaxColumnSize);
				}
			}
		}


		if(isCurrentlyDragging)
		{
			float fullWidth = 1.0f;
			bool dragObjectFound = false;
			for(int i = 0; i < keys.Count; i++)
			{
				string key = keys[i];
				DraggableColumnDivider divider = columnDividers[key];

				if(divider.isDragging || dragObjectFound)
				{
					dragObjectFound = true;
					int objectsLeft = Mathf.Max(0, keys.Count - (i + 1));
					if(objectsLeft ==  0)
					{
						columns[key] = fullWidth;
						divider.newSizeValue = columns[key];
					}
					else if(fullWidth - (divider.newSizeValue + (objectsLeft * minColumnSize)) < 0)
					{
						columns[key] = fullWidth - (objectsLeft * minColumnSize);
						divider.newSizeValue = columns[key];
					}
					else
					{
						columns[key] = divider.newSizeValue;
					}
				}
				else
				{
					columns[key] = Mathf.Max(minColumnSize, divider.newSizeValue);
				}

				fullWidth -= columns[key];
			}
			RecalculateColumnWidths();
			return true;
		}
		return false;
	}


	public Rect GetColumnPosition(Rect fullRow, string title)
	{
		Rect newPosition = fullRow;
		foreach(var pair in columns)
		{
			newPosition.width = (fullRow.width * (pair.Value - 0.01f));

			if(pair.Key == title)
			{
				break;
			}

			newPosition.x += (fullRow.width * pair.Value);
		}

		return newPosition;
	}

	public void RecalculateColumnWidths()
	{
		float fullWidth = 1.0f;
		int count = 0;
		List<string> keys = new List<string>(columns.Keys);
		foreach(string key in keys)
		{
			count++;

			int objectsLeft = Mathf.Max(0, keys.Count - count);
			if(objectsLeft == 0)
			{
				columns[key] = fullWidth;
			}
			else if(fullWidth - (columns[key] + (objectsLeft * minColumnSize)) < 0)
			{
				columns[key] = fullWidth - (objectsLeft * minColumnSize);
			}

			fullWidth -= columns[key];
		}
	}
}
}
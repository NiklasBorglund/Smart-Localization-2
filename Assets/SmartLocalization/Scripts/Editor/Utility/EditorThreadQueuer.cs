using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace SmartLocalization.Editor
{
	public class EditorThreadQueuer 
	{
		static EditorThreadQueuer _instance = null;
		
		static EditorThreadQueuer Instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = new EditorThreadQueuer();
				}
					
				return _instance;
			}
		}
		
		IList<Action> queuedActions = new List<Action>();
		IList<Action> runningActions = new List<Action>();
		bool isRegisteredToUpdate = false;
		object listLock = new object();
		
		void RegisterToEditorUpdate()
		{
			if(!isRegisteredToUpdate)
			{
				EditorApplication.update += Update;
				isRegisteredToUpdate = true;
			}
		}
		
		void UnregisterToEditorUpdate()
		{
			if(isRegisteredToUpdate)
			{
				EditorApplication.update -= Update;
				isRegisteredToUpdate = false;
			}
		}
		
		void RunOnMainThread(Action action)
		{
			lock(listLock)
			{
				queuedActions.Add(action);
				RegisterToEditorUpdate();	
			}
		}
		
		void Update()
		{
			lock(listLock)
			{
				for(int i = 0; i < queuedActions.Count;++i)
				{
					runningActions.Add(queuedActions[i]);
				}
				queuedActions.Clear();
				UnregisterToEditorUpdate();
			}
			
			for(int i = 0; i < runningActions.Count;++i)
			{
				try
				{
					runningActions[i]();
				}
				catch(System.Exception ex)
				{
					Debug.LogError(ex.Message);
				}
			}
			runningActions.Clear();
		}
		
		public static void QueueOnMainThread(Action action)
		{
			Instance.RunOnMainThread(action);
		}
			
	}
}

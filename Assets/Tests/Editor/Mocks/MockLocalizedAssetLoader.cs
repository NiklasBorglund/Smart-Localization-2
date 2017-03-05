using UnityEngine;
using System.Collections;
using System;

namespace SmartLocalization.Editor{

public class AssetSuccessfullyLoadedException : Exception
{
	
}

public class MockLocalizedAssetLoader : ILocalizedAssetLoader 
{
	public enum Scenario
	{
		ReturnNull,
		ThrowSuccessfullyLoadedExceptions,
	}
	
	Scenario testScenario = Scenario.ReturnNull;

	public Scenario TestScenario {
		get {
			return testScenario;
		}
		set {
			testScenario = value;
		}
	}	

	public T LoadAsset<T> (string assetKey, string languageCode) where T : UnityEngine.Object
	{
		switch(testScenario)
		{
			case Scenario.ThrowSuccessfullyLoadedExceptions:
			{
				throw new AssetSuccessfullyLoadedException();
			}
			case Scenario.ReturnNull:
			default:
			return null;
		}
	}
}
}

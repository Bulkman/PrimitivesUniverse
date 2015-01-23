using System;
using UnityEngine;

public static class GlobalData
{
	public static bool FirstGeneration = true;

	public static TreeNode<UniverseData> CurrentUniverse = null;

	static GlobalData()
	{
		LeanTween.init();
	}

}


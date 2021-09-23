using System;
using UnityEngine;

public class TheOneAndOnly : MonoBehaviour
{
	public TheOneAndOnly()
	{
	}

	private void Awake()
	{
		TheOneAndOnly[] array = (TheOneAndOnly[])UnityEngine.Object.FindObjectsOfType(typeof(TheOneAndOnly));
		if (1 < array.Length)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}
}

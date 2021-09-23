using System;
using UnityEngine;

public class EditorDebugObj : MonoBehaviour
{
	public EditorDebugObj()
	{
	}

	private void Awake()
	{
		if (Application.isEditor)
		{
			LNG.Init(this.m_debugLanguage);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public string m_debugLanguage = "English";
}

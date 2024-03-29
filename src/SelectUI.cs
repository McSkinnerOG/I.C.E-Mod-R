﻿using System;
using UnityEngine;

public class SelectUI : MonoBehaviour
{
	public SelectUI()
	{
	}

	private void OnGUI()
	{
		float num = (float)Screen.width / 2f;
		float num2 = (float)Screen.height / 2f;
		if (GUI.Button(new Rect(num - 50f, num2 - 12f, 100f, 20f), "Start Server"))
		{
			LidgrenServer lidgrenServer = (LidgrenServer)UnityEngine.Object.FindObjectOfType(typeof(LidgrenServer));
			lidgrenServer.enabled = true;
			Application.LoadLevel("Level");
		}
		if (GUI.Button(new Rect(num - 50f, num2 + 12f, 100f, 20f), "Start Client"))
		{
			LidgrenClient lidgrenClient = (LidgrenClient)UnityEngine.Object.FindObjectOfType(typeof(LidgrenClient));
			lidgrenClient.enabled = true;
			Application.LoadLevel("Level");
		}
	}
}

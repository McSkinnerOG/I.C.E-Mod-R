using System;
using UnityEngine;

public class SwitchView : MonoBehaviour
{
	public SwitchView()
	{
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(this.pos.x * (float)Screen.width, this.pos.y * (float)Screen.height, this.size.x * (float)Screen.width, this.size.y * (float)Screen.height), "Camera01"))
		{
			this.DisableCam();
			this.mainCam.enabled = true;
		}
		if (GUI.Button(new Rect(this.pos.x * (float)Screen.width + 200f, this.pos.y * (float)Screen.height, this.size.x * (float)Screen.width, this.size.y * (float)Screen.height), "Camera02"))
		{
			this.DisableCam();
			this.subCam01.enabled = true;
		}
		if (GUI.Button(new Rect(this.pos.x * (float)Screen.width + 400f, this.pos.y * (float)Screen.height, this.size.x * (float)Screen.width, this.size.y * (float)Screen.height), "Camera03"))
		{
			this.DisableCam();
			this.subCam02.enabled = true;
		}
	}

	private void DisableCam()
	{
		this.mainCam.enabled = false;
		this.subCam01.enabled = false;
		this.subCam02.enabled = false;
	}

	public Camera mainCam;

	public Camera subCam01;

	public Camera subCam02;

	public Vector2 pos;

	public Vector2 size;
}

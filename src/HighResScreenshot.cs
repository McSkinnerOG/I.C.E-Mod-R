using System;
using UnityEngine;

public class HighResScreenshot : MonoBehaviour
{
	public HighResScreenshot()
	{
	}

	public static string GetScreenShotName(int width, int height)
	{
		return string.Format("{0}/../screenshots/screen_{1}x{2}_{3}.png", new object[]
		{
			Application.dataPath,
			width,
			height,
			DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
		});
	}

	public void TakeHiResShot()
	{
		this.takeHiResShot = 1;
	}

	private void LateUpdate()
	{
		if (Input.GetKeyDown("k"))
		{
			this.takeHiResShot = 1;
		}
		if (this.takeHiResShot == 1)
		{
			this.takeHiResShot++;
			Application.CaptureScreenshot(HighResScreenshot.GetScreenShotName(Screen.width * 2, Screen.height * 2), 2);
			Debug.Break();
		}
		else if (this.takeHiResShot == 2)
		{
			this.takeHiResShot++;
			Application.CaptureScreenshot(HighResScreenshot.GetScreenShotName(Screen.width * 2, Screen.height * 2), 2);
			Debug.Break();
		}
		else if (this.takeHiResShot == 3)
		{
			this.takeHiResShot++;
			Application.CaptureScreenshot(HighResScreenshot.GetScreenShotName(Screen.width * 2, Screen.height * 2), 2);
			Debug.Break();
		}
	}

	public Camera cam;

	private int takeHiResShot;
}

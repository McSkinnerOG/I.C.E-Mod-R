using System;
using UnityEngine;

public class GUI3dMaster : MonoBehaviour
{
	public GUI3dMaster()
	{
	}

	private void Update()
	{
		this.m_buttonClickedName = string.Empty;
		this.m_buttonRightClickedName = string.Empty;
		Vector3 vector = (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1)) ? Vector3.zero : Input.mousePosition;
		if (Vector3.zero != vector)
		{
			Ray ray = Camera.main.ScreenPointToRay(vector);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, 100f, this.m_guiLayer))
			{
				if (Input.GetMouseButtonDown(0))
				{
					this.m_buttonClickedName = raycastHit.transform.name;
				}
				if (Input.GetMouseButtonDown(1))
				{
					this.m_buttonRightClickedName = raycastHit.transform.name;
				}
				raycastHit.transform.SendMessage("Animate", SendMessageOptions.DontRequireReceiver);
				if (null != base.audio)
				{
					base.audio.Play();
				}
			}
		}
	}

	public string GetClickedButtonName()
	{
		return this.m_buttonClickedName;
	}

	public string GetRightClickedButtonName()
	{
		return this.m_buttonRightClickedName;
	}

	public float GetRatioMultiplier()
	{
		float num = (float)this.m_designResX / (float)this.m_designResY;
		float num2 = (float)Screen.width / (float)Screen.height;
		return num2 / num;
	}

	public int m_designResX = 9;

	public int m_designResY = 16;

	private int m_guiLayer = 288;

	private string m_buttonClickedName = string.Empty;

	private string m_buttonRightClickedName = string.Empty;
}

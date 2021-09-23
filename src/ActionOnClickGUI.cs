using System;
using UnityEngine;

public class ActionOnClickGUI : MonoBehaviour
{
	public ActionOnClickGUI()
	{
	}

	private void Start()
	{
		this.m_guimaster = (GUI3dMaster)UnityEngine.Object.FindObjectOfType(typeof(GUI3dMaster));
		this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
	}

	private void Update()
	{
		if (null != this.m_client && this.m_client.GetHealth() == 0f)
		{
			this.m_wasDeadFlag = true;
		}
		else if (this.m_wasDeadFlag)
		{
			this.m_button.SetActive(true);
			this.m_wasDeadFlag = false;
		}
	}

	private void LateUpdate()
	{
		if (Time.timeSinceLevelLoad < 1f)
		{
			return;
		}
		if (null != this.m_guimaster)
		{
			string clickedButtonName = this.m_guimaster.GetClickedButtonName();
			if (string.Empty != clickedButtonName && null != this.m_button && this.m_button.name == clickedButtonName)
			{
				this.m_button.SetActive(false);
			}
		}
	}

	public GameObject m_button;

	private GUI3dMaster m_guimaster;

	private LidClient m_client;

	private bool m_wasDeadFlag;
}

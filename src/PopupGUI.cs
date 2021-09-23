using System;
using UnityEngine;

public class PopupGUI : MonoBehaviour
{
	public PopupGUI()
	{
	}

	private void Start()
	{
		this.m_guimaster = (GUI3dMaster)UnityEngine.Object.FindObjectOfType(typeof(GUI3dMaster));
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			this.ShowGui(false, string.Empty);
		}
	}

	private void LateUpdate()
	{
		if (null != this.m_guimaster && this.m_guiParent.activeSelf)
		{
			string clickedButtonName = this.m_guimaster.GetClickedButtonName();
			if (string.Empty != clickedButtonName)
			{
				if ("btn_quit_yes" == clickedButtonName)
				{
					this.m_saidYesFlag = true;
					this.ShowGui(false, string.Empty);
				}
				else if ("btn_quit_no" == clickedButtonName)
				{
					this.ShowGui(false, string.Empty);
				}
			}
		}
	}

	public int ShowGui(bool a_show, string a_caption = "")
	{
		if (a_show)
		{
			this.m_sessionId++;
			this.m_saidYesFlag = false;
		}
		this.m_guiParent.SetActive(a_show);
		this.m_caption.text = a_caption;
		return this.m_sessionId;
	}

	public bool IsActive()
	{
		return this.m_guiParent.activeSelf;
	}

	public int GetSessionId()
	{
		return this.m_sessionId;
	}

	public GameObject m_guiParent;

	public TextMesh m_caption;

	[HideInInspector]
	public bool m_saidYesFlag;

	private int m_sessionId;

	private GUI3dMaster m_guimaster;
}

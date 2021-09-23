using System;
using UnityEngine;

public class QuitGameGUI : MonoBehaviour
{
	public QuitGameGUI()
	{
	}

	private void Start()
	{
		this.m_guimaster = (GUI3dMaster)UnityEngine.Object.FindObjectOfType(typeof(GUI3dMaster));
		this.m_msgBar = UnityEngine.Object.FindObjectOfType<MessageBarGUI>();
	}

	private void Update()
	{
		if (this.m_openWithEsc && Input.GetKeyDown(KeyCode.Escape))
		{
			this.ShowGui(true);
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
					if (!this.IsBattleLogging())
					{
						TheOneAndOnly theOneAndOnly = (TheOneAndOnly)UnityEngine.Object.FindObjectOfType(typeof(TheOneAndOnly));
						UnityEngine.Object.DestroyImmediate(theOneAndOnly.gameObject);
						Application.LoadLevel(0);
					}
				}
				else if ("btn_quit_no" == clickedButtonName)
				{
					this.ShowGui(false);
				}
			}
		}
	}

	private bool IsBattleLogging()
	{
		bool flag = this.m_cantLogoutTime > Time.time;
		if (flag && null != this.m_msgBar)
		{
			int num = (int)(this.m_cantLogoutTime - Time.time + 0.5f);
			this.m_msgBar.DisplayMessage(LNG.Get("CANT_LOGOUT_DURING_BATTLE").Replace("%1", num.ToString()), 1000);
		}
		return flag;
	}

	public void ShowGui(bool a_show)
	{
		this.m_guiParent.SetActive(a_show);
	}

	public GameObject m_guiParent;

	public bool m_openWithEsc = true;

	[HideInInspector]
	public float m_cantLogoutTime = -99999f;

	private GUI3dMaster m_guimaster;

	private MessageBarGUI m_msgBar;
}

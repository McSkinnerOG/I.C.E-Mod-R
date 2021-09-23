using System;
using UnityEngine;

public class MessageBarGUI : MonoBehaviour
{
	public MessageBarGUI()
	{
	}

	private void Start()
	{
		this.DisplayMessage(LNG.Get("PRESS_H_FOR_HELP"), 100);
	}

	private void Update()
	{
		if (Time.time > this.m_disappearTime && this.m_disappearTime > 0f)
		{
			this.SetVisibility(false);
			this.m_disappearTime = 0f;
		}
	}

	private void SetVisibility(bool a_visible)
	{
		if (null != this.m_bar)
		{
			this.m_bar.SetActive(a_visible);
		}
		if (null != this.m_text)
		{
			this.m_text.gameObject.SetActive(a_visible);
		}
	}

	public bool DisplayMessage(string a_msg, int a_prio = 100)
	{
		bool result = false;
		if (Time.time > this.m_disappearTime || this.m_curPrio <= a_prio)
		{
			this.SetVisibility(true);
			if (null != this.m_text)
			{
				this.m_text.text = a_msg;
			}
			this.m_curPrio = a_prio;
			this.m_disappearTime = Time.time + this.m_displayDuration;
			result = true;
		}
		return result;
	}

	public GameObject m_bar;

	public TextMesh m_text;

	public float m_displayDuration = 8f;

	private float m_disappearTime;

	private int m_curPrio;
}

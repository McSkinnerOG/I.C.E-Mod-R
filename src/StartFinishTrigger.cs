using System;
using UnityEngine;

public class StartFinishTrigger : MonoBehaviour
{
	public StartFinishTrigger()
	{
	}

	private void OnTriggerEnter()
	{
		if (this.m_startTime != 0f)
		{
			this.m_timesDisplay.text = this.m_timesDisplay.text + "\n" + (Time.time - this.m_startTime).ToString();
		}
		this.m_startTime = Time.time;
	}

	private void Update()
	{
	}

	public GUIText m_timesDisplay;

	private float m_startTime;
}

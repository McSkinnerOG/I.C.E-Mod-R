using System;
using UnityEngine;

public class GameInit : MonoBehaviour
{
	public GameInit()
	{
	}

	private void Start()
	{
		Time.timeScale = this.m_timeScale;
	}

	public float m_timeScale = 1f;
}

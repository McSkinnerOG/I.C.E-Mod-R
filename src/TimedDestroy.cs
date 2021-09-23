using System;
using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
	public TimedDestroy()
	{
	}

	private void Start()
	{
		this.m_dieTime = Time.time + this.m_destroyAfter;
	}

	private void Update()
	{
		if (Time.time > this.m_dieTime)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public float m_destroyAfter = 60f;

	private float m_dieTime;
}

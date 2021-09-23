using System;
using UnityEngine;

public class Bounce : MonoBehaviour
{
	public Bounce()
	{
	}

	private void Update()
	{
		float num = Mathf.Abs(Mathf.Sin(Time.timeSinceLevelLoad * this.m_speed)) * this.m_addToScale;
		base.transform.localScale = new Vector3(this.m_startScale + num, 1f, this.m_startScale + num);
	}

	public float m_speed = 1f;

	public float m_startScale = 1f;

	public float m_addToScale = 0.05f;
}

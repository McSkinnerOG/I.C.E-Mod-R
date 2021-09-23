using System;
using UnityEngine;

public class MouseWalkIndicator : MonoBehaviour
{
	public MouseWalkIndicator()
	{
	}

	private void Start()
	{
		this.m_mat = base.transform.renderer.material;
	}

	private void Update()
	{
		Color color = this.m_mat.GetColor("_TintColor");
		if (base.transform.position != this.m_lastPos)
		{
			this.m_lastPos = base.transform.position;
			color.a = this.m_startAlpha;
		}
		if (0f < color.a)
		{
			color.a -= Time.deltaTime * this.m_speed;
			if (0f >= color.a)
			{
				base.transform.position = Vector3.up * 1000f;
				this.m_lastPos = base.transform.position;
				color.a = 0f;
			}
		}
		if (color != this.m_mat.GetColor("_TintColor"))
		{
			this.m_mat.SetColor("_TintColor", color);
		}
	}

	public float m_startAlpha = 0.3f;

	public float m_speed = 0.5f;

	private Material m_mat;

	private Vector3 m_lastPos = Vector3.zero;
}

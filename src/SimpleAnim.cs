using System;
using UnityEngine;

public class SimpleAnim : MonoBehaviour
{
	public SimpleAnim()
	{
	}

	private void Start()
	{
		this.m_progress = UnityEngine.Random.Range(0f, 1f);
		this.m_startVars = new float[base.transform.childCount];
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (null != child)
			{
				this.m_startVars[i] = child.localPosition.y;
			}
		}
	}

	private void Update()
	{
		this.m_progress += Time.deltaTime * this.m_speed;
		if (1f < this.m_progress)
		{
			this.m_progress -= 1f;
		}
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (null != child)
			{
				float num = this.m_progress + (float)i * 0.05f;
				if (1f < num)
				{
					num -= 1f;
				}
				Vector3 localPosition = child.localPosition;
				localPosition.y = this.m_startVars[i] + FastSin.Get(num * 3.1415927f) * this.m_animMoveDelta;
				child.localPosition = localPosition;
			}
		}
	}

	public float m_speed = 1f;

	public float m_animMoveDelta = 0.4f;

	private float m_progress;

	private float[] m_startVars;
}

using System;
using UnityEngine;

public class TreeAnimation : MonoBehaviour
{
	public TreeAnimation()
	{
	}

	private void OnEnable()
	{
		this.m_animProgress = 0f;
	}

	private void OnDisable()
	{
		if (null != this.m_tree)
		{
			this.m_tree.localRotation = Quaternion.identity;
			this.m_tree.renderer.enabled = true;
		}
	}

	private void Update()
	{
		if (this.m_animProgress != -1f)
		{
			if (this.m_animProgress < 2f)
			{
				this.m_animProgress += Time.deltaTime;
				float num = 1f + FastSin.Get(4.712389f + Mathf.Clamp01(this.m_animProgress) * 0.5f * 3.1415927f);
				this.m_tree.localRotation = Quaternion.Euler(num * -85f, 0f, 0f);
			}
			else
			{
				this.m_tree.renderer.enabled = false;
				this.m_animProgress = -1f;
			}
		}
	}

	public Transform m_tree;

	private float m_animProgress;
}

using System;
using UnityEngine;

public class ChangeRenderQueue : MonoBehaviour
{
	public ChangeRenderQueue()
	{
	}

	private void Awake()
	{
		if (this.m_withChildren)
		{
			Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
			if (componentsInChildren != null)
			{
				foreach (Renderer renderer in componentsInChildren)
				{
					if (null != renderer && null != renderer.material)
					{
						renderer.material.renderQueue += this.m_queueChange;
					}
				}
			}
		}
		else if (null != base.renderer && null != base.renderer.material)
		{
			base.renderer.material.renderQueue += this.m_queueChange;
		}
	}

	public int m_queueChange = 1;

	public bool m_withChildren;
}

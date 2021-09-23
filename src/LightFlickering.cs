using System;
using UnityEngine;

public class LightFlickering : MonoBehaviour
{
	public LightFlickering()
	{
	}

	private void FixedUpdate()
	{
		if (null != base.light)
		{
			base.light.intensity = UnityEngine.Random.Range(this.m_minIntensity, this.m_maxIntensity);
		}
	}

	public float m_minIntensity = 1f;

	public float m_maxIntensity = 1.6f;
}

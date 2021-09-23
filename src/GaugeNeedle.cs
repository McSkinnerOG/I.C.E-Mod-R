using System;
using UnityEngine;

public class GaugeNeedle : MonoBehaviour
{
	public GaugeNeedle()
	{
	}

	private void Update()
	{
		base.transform.rotation = Quaternion.Euler(0f, 0f, (this.m_input - 0.6f) / 0.79999995f * -360f);
	}

	private const float c_minInput = 0.6f;

	private const float c_maxInput = 1.4f;

	[Range(0.6f, 1.4f)]
	public float m_input = 0.6f;
}

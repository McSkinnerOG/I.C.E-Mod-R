using System;
using UnityEngine;

public class RandomTreeColor : MonoBehaviour
{
	public RandomTreeColor()
	{
	}

	private void Start()
	{
		base.renderer.material.color = new Color(UnityEngine.Random.Range(this.m_fromColor.r, this.m_toColor.r), UnityEngine.Random.Range(this.m_fromColor.g, this.m_toColor.g), UnityEngine.Random.Range(this.m_fromColor.b, this.m_toColor.b));
	}

	public Color m_fromColor = Color.black;

	public Color m_toColor = Color.white;
}

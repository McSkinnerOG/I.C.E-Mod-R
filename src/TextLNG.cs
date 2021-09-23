using System;
using UnityEngine;
using UnityEngine.UI;

public class TextLNG : MonoBehaviour
{
	public TextLNG()
	{
	}

	private void Start()
	{
		this.TranslateText();
	}

	public void TranslateText()
	{
		string text = LNG.Get(this.m_lngKey);
		Text component = base.GetComponent<Text>();
		if (null != component)
		{
			component.text = text;
		}
		else
		{
			TextMesh component2 = base.GetComponent<TextMesh>();
			if (null != component2)
			{
				component2.text = text;
			}
		}
	}

	public string m_lngKey = string.Empty;
}

using System;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSetter : MonoBehaviour
{
	public LanguageSetter()
	{
	}

	private void Awake()
	{
		string @string = PlayerPrefs.GetString("prefLang", "English");
		this.SetLanguage(@string);
	}

	public void SetLanguage(string a_lng)
	{
		LNG.Init(a_lng);
		PlayerPrefs.SetString("prefLang", a_lng);
		if (null != this.m_curLngTxt)
		{
			this.m_curLngTxt.text = a_lng;
		}
		TextLNG[] array = UnityEngine.Object.FindObjectsOfType<TextLNG>();
		foreach (TextLNG textLNG in array)
		{
			if (null != textLNG)
			{
				textLNG.TranslateText();
			}
		}
	}

	public Text m_curLngTxt;
}

using System;
using UnityEngine;

public class ChatLabel : MonoBehaviour
{
	public ChatLabel()
	{
	}

	private void Start()
	{
		if (this.m_dropShadow)
		{
			this.CreateShadow();
		}
	}

	private void LateUpdate()
	{
		if (null != this.m_text)
		{
			if (Time.time > this.m_textDisappearTime && this.m_textDisappearTime > 0f)
			{
				this.m_text.gameObject.SetActive(false);
				this.m_textDisappearTime = 0f;
			}
			this.m_text.transform.rotation = this.m_localRot;
		}
	}

	public void SetText(string a_text, bool a_stayForever = false)
	{
		if (null == this.m_text)
		{
			this.m_text = base.GetComponent<TextMesh>();
		}
		string text = string.Empty;
		if (a_text.Length > 50)
		{
			string[] array = a_text.Split(new char[]
			{
				' '
			});
			int num = array.Length / 2 - 1;
			for (int i = 0; i < array.Length; i++)
			{
				text += array[i];
				if (i == num)
				{
					text += "\n";
				}
				else
				{
					text += " ";
				}
			}
		}
		else
		{
			text = a_text;
		}
		if (null != this.m_text)
		{
			this.m_text.text = text;
			if (null != this.m_shadowText)
			{
				this.m_shadowText.text = text;
			}
			this.m_text.transform.rotation = this.m_localRot;
			this.m_text.gameObject.SetActive(true);
			this.m_textDisappearTime = ((!a_stayForever) ? (Time.time + 4f + (float)text.Length * 0.25f) : 0f);
		}
	}

	private void CreateShadow()
	{
		if (null != this.m_shadowText)
		{
			UnityEngine.Object.DestroyImmediate(this.m_shadowText.gameObject);
		}
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(base.gameObject, base.transform.position, base.transform.rotation);
		UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<ChatLabel>());
		gameObject.transform.parent = base.transform;
		gameObject.renderer.material.color = this.m_shadowColor;
		gameObject.transform.localPosition = this.m_shadowOffset;
		this.m_shadowText = gameObject.GetComponent<TextMesh>();
	}

	public bool m_dropShadow;

	public Color m_shadowColor = Color.black;

	public Vector3 m_shadowOffset = new Vector3(-1f, -2f, 0f);

	private Quaternion m_localRot = Quaternion.Euler(55f, 0f, 0f);

	private TextMesh m_shadowText;

	private TextMesh m_text;

	private float m_textDisappearTime;
}

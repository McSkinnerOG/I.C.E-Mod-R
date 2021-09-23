using System;
using UnityEngine;

public class ChatGUI : MonoBehaviour
{
	public ChatGUI()
	{
	}

	private void Start()
	{
		int num = (int)((float)Screen.width * this.m_length);
		int num2 = 25;
		int num3 = (int)((float)Screen.width * this.m_xOffset);
		int num4 = (int)((float)Screen.height * this.m_yOffset);
		this.m_chatRect = new Rect((float)num3, (float)num4, (float)num, (float)num2);
		this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
	}

	private void OnGUI()
	{
		if (this.m_showTextInput || Time.timeSinceLevelLoad < 1f)
		{
			GUI.SetNextControlName("chatInput");
			this.m_chatString = GUI.TextField(this.m_chatRect, this.m_chatString, 110);
		}
		if (Event.current.type == EventType.KeyUp)
		{
			if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
			{
				if (this.m_showTextInput)
				{
					this.m_chatString = this.m_chatString.Replace("\\", string.Empty);
					if (this.m_chatString.Length > 0)
					{
						if (null != this.m_client)
						{
							this.m_client.SendChatMsg(this.m_chatString, true);
						}
						this.m_chatString = string.Empty;
					}
					this.m_showTextInput = false;
				}
				else
				{
					GUI.FocusControl("chatInput");
					this.m_showTextInput = true;
				}
				Event.current.Use();
			}
			if (Event.current.keyCode == KeyCode.Escape)
			{
				this.m_chatString = string.Empty;
				this.m_showTextInput = false;
				Event.current.Use();
			}
		}
	}

	private float m_xOffset = 0.035f;

	private float m_yOffset = 0.92f;

	private float m_length = 0.45f;

	private Rect m_chatRect = default(Rect);

	private string m_chatString = string.Empty;

	private bool m_showTextInput;

	private LidClient m_client;
}

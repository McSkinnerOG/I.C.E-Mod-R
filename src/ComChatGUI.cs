using System;
using System.Collections;
using UnityEngine;

public class ComChatGUI : MonoBehaviour
{
	public ComChatGUI()
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static ComChatGUI()
	{
	}

	private void Start()
	{
		float num = 1.7777778f / ((float)Screen.width / (float)Screen.height);
		float num2 = ((float)Screen.width / (float)Screen.height / 1.7777778f + 1f) * 0.5f;
		int num3 = (int)((float)Screen.width * this.m_length * num);
		int num4 = 25;
		int num5 = (int)((float)Screen.width * this.m_xOffset * num2);
		int num6 = (int)((float)Screen.height * this.m_yOffset);
		this.m_chatRect = new Rect((float)num5, (float)num6, (float)num3, (float)num4);
		this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
		this.m_qmunicator = base.GetComponent<QmunicatorGUI>();
		this.m_unreadMsgIndicator.SetActive(false);
		this.m_unreadMsgIndicator2.SetActive(false);
	}

	private void OnGUI()
	{
		this.m_isActive = this.m_chatText.gameObject.activeInHierarchy;
		if (this.m_qmunicator.IsActive(false) && this.m_isActive)
		{
			if (this.m_unreadMsgIndicator.activeSelf)
			{
				this.m_unreadMessages = 0;
				this.m_unreadMsgIndicator.SetActive(false);
				this.m_unreadMsgIndicator2.SetActive(false);
			}
			GUI.SetNextControlName("chatInputCom");
			this.m_chatString = GUI.TextField(this.m_chatRect, this.m_chatString, 100);
			if (Event.current.type == EventType.KeyUp)
			{
				if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
				{
					this.m_chatString = this.m_chatString.Replace("\\", string.Empty);
					if (this.m_chatString.Length > 0)
					{
						if (!this.IsSpam(this.m_chatString))
						{
							if (null != this.m_client)
							{
								this.m_client.SendChatMsg(this.m_chatString, false);
							}
							else
							{
								this.AddString(this.m_chatString);
							}
						}
						else
						{
							this.AddString("System§ " + LNG.Get("ANTI_SPAM_CHAT"));
						}
						this.m_chatString = string.Empty;
					}
					else
					{
						GUI.FocusControl((!("chatInputCom" == GUI.GetNameOfFocusedControl())) ? "chatInputCom" : string.Empty);
					}
					Event.current.Use();
				}
				if (Event.current.keyCode == KeyCode.Escape)
				{
					GUI.FocusControl(string.Empty);
					this.m_chatString = string.Empty;
					Event.current.Use();
				}
			}
		}
	}

	private bool IsSpam(string a_str)
	{
		bool flag = Time.time < this.m_nextChatTime;
		if (!flag)
		{
			Hashtable hashtable = new Hashtable();
			foreach (object obj in ComChatGUI.m_chatEntries)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				float num = (float)dictionaryEntry.Key;
				if (Time.time - num < 10f)
				{
					hashtable.Add(dictionaryEntry.Key, dictionaryEntry.Value);
				}
			}
			ComChatGUI.m_chatEntries = hashtable;
			if (4 < ComChatGUI.m_chatEntries.Count)
			{
				this.m_spamCounter++;
				flag = true;
				this.m_nextChatTime = Time.time + 10f * (float)this.m_spamCounter;
			}
			else
			{
				ComChatGUI.m_chatEntries.Add(Time.time, a_str);
			}
		}
		return flag;
	}

	private void DeleteOldestChatEntry()
	{
		if (this.m_chatAsStr.Length > 0)
		{
			int num = this.m_chatAsStr.IndexOf('\n');
			if (num == -1)
			{
				this.m_chatAsStr = string.Empty;
			}
			else
			{
				this.m_chatAsStr = this.m_chatAsStr.Substring(this.m_chatAsStr.IndexOf('\n') + 1);
			}
			if (null != this.m_chatText)
			{
				this.m_chatText.text = this.m_chatAsStr;
			}
		}
	}

	public void AddString(string a_str)
	{
		a_str = this.CropStr(a_str, 55);
		string text = string.Empty;
		string[] array = a_str.Split(new char[]
		{
			'§'
		});
		if (0 < array.Length && !this.m_playersOnlineGui.IsMuted(array[0]))
		{
			if (1 < array.Length)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (i == 0)
					{
						text = text + "<color=\"#aaaaaa\">" + array[i] + ":</color>";
					}
					else
					{
						text += array[i];
					}
				}
			}
			else
			{
				text = a_str;
			}
			this.m_chatAsStr = this.m_chatAsStr + "\n" + text;
			while (this.m_chatAsStr.Split(new char[]
			{
				'\n'
			}).Length - 1 > this.m_maxChatLines)
			{
				this.DeleteOldestChatEntry();
			}
			if (null != this.m_chatText)
			{
				this.m_chatText.text = this.m_chatAsStr;
			}
			if (!this.m_qmunicator.IsActive(false) || !this.m_isActive)
			{
				this.m_unreadMsgIndicator.SetActive(true);
				this.m_unreadMsgIndicator2.SetActive(true);
				this.m_unreadMessages = Mathf.Min(this.m_unreadMessages + 1, 99);
				this.m_unreadMsgText.text = this.m_unreadMessages.ToString();
				this.m_unreadMsgText2.text = this.m_unreadMessages.ToString();
			}
			if (null != base.audio && this.m_unreadMessages == 1)
			{
				base.audio.Play();
			}
		}
	}

	private string CropStr(string a_text, int a_newLineAfter = 55)
	{
		int num = a_newLineAfter / 2;
		string text = a_text;
		if (text.Length > a_newLineAfter)
		{
			string[] array = text.Split(new char[]
			{
				'\n'
			});
			text = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = string.Empty;
				string[] array2 = array[i].Split(new char[]
				{
					' '
				});
				for (int j = 0; j < array2.Length; j++)
				{
					string text3 = array2[j];
					if (num < text3.Length)
					{
						text3 = text3.Substring(0, num);
					}
					text2 = text2 + text3 + " ";
					int num2 = (j + 1 >= array2.Length) ? 0 : (array2[j + 1].Length / 2);
					if (text2.Length + num2 > a_newLineAfter)
					{
						text = text + text2 + ((j + 1 >= array2.Length) ? string.Empty : "\n");
						text2 = string.Empty;
					}
				}
				text = text + text2 + "\n";
			}
		}
		if (text.EndsWith("\n"))
		{
			text = text.Substring(0, text.Length - 1);
		}
		return text;
	}

	public TextMesh m_chatText;

	public int m_maxChatLines = 8;

	public PlayersOnlineGui m_playersOnlineGui;

	public float m_xOffset = 0.36f;

	public float m_yOffset = 0.75f;

	public float m_length = 0.47f;

	public GameObject m_unreadMsgIndicator;

	public TextMesh m_unreadMsgText;

	public GameObject m_unreadMsgIndicator2;

	public TextMesh m_unreadMsgText2;

	private Rect m_chatRect = default(Rect);

	private static Hashtable m_chatEntries = new Hashtable();

	private float m_nextChatTime;

	private int m_spamCounter;

	private string m_chatString = string.Empty;

	private string m_chatAsStr = string.Empty;

	private int m_unreadMessages;

	private LidClient m_client;

	private QmunicatorGUI m_qmunicator;

	private bool m_isActive;
}

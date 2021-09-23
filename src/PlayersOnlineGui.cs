using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class PlayersOnlineGui : MonoBehaviour
{
	public PlayersOnlineGui()
	{
	}

	private void Start()
	{
		this.m_guimaster = UnityEngine.Object.FindObjectOfType<GUI3dMaster>();
		this.m_playerEntityTemplate.SetActive(false);
	}

	private int UpdateList()
	{
		if (null != this.m_playerEntityTemplate)
		{
			for (int i = 0; i < 36; i++)
			{
				if (null != this.m_playerEntities[i])
				{
					UnityEngine.Object.Destroy(this.m_playerEntities[i].gameObject);
					this.m_playerEntities[i] = null;
				}
			}
			for (int j = 0; j < 36; j++)
			{
				int num = j + this.m_page * 36;
				if (num >= this.m_charData.Count)
				{
					break;
				}
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_playerEntityTemplate);
				gameObject.SetActive(true);
				this.m_playerEntities[j] = gameObject.transform;
				gameObject.transform.parent = this.m_playerEntityTemplate.transform.parent;
				gameObject.transform.localPosition = new Vector3(-0.216f + (float)(j / 12) * 0.19f, 0.15f - (float)(j % 12) * 0.028f, 0f);
				gameObject.transform.localRotation = Quaternion.identity;
				TextMesh componentInChildren = gameObject.GetComponentInChildren<TextMesh>();
				if (null != componentInChildren)
				{
					componentInChildren.text = this.m_charData[num].name;
					if (16 < componentInChildren.text.Length)
					{
						componentInChildren.text = componentInChildren.text.Substring(0, 16) + "...";
					}
					if (this.m_invitedSteamIds.Contains(this.m_charData[num].aid))
					{
						componentInChildren.text = "<color=\"#66bb66\">" + componentInChildren.text + "</color>";
					}
					else if (this.m_mutedSteamIds.Contains(this.m_charData[num].aid))
					{
						componentInChildren.text = "<color=\"#bb6666\">" + componentInChildren.text + "</color>";
					}
				}
				GUI3dButton componentInChildren2 = gameObject.GetComponentInChildren<GUI3dButton>();
				if (null != componentInChildren2)
				{
					componentInChildren2.name = "btnpe_" + this.m_charData[num].aid;
				}
			}
		}
		return this.m_charData.Count;
	}

	public bool IsMuted(string a_name)
	{
		for (int i = 0; i < this.m_mutedSteamIds.Count; i++)
		{
			for (int j = 0; j < this.m_charData.Count; j++)
			{
				if (this.m_mutedSteamIds[i] == this.m_charData[j].aid && a_name == this.m_charData[j].name)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void OpenAsPlayerInvitation()
	{
		this.m_invitePartyMode = true;
		base.gameObject.SetActive(true);
	}

	private void OnEnable()
	{
		this.m_page = 0;
		this.m_invitedSteamIds.Clear();
		this.m_client = UnityEngine.Object.FindObjectOfType<LidClient>();
		if (null != this.m_client)
		{
			this.m_charData.Clear();
			for (int i = 0; i < this.m_client.m_playerData.Length; i++)
			{
				if (this.m_client.m_playerData[i].name != null && 1 < this.m_client.m_playerData[i].name.Length)
				{
					this.m_charData.Add(this.m_client.m_playerData[i]);
				}
			}
		}
		else if (this.m_charData.Count == 0)
		{
			for (int j = 0; j < 90; j++)
			{
				CharData item = default(CharData);
				item.name = "Ethan " + j + " the very very great!";
				item.aid = (ulong)((long)(1337 + j));
				this.m_charData.Add(item);
			}
		}
		this.UpdateList();
		if (null != this.m_pageText)
		{
			this.m_pageText.text = "Page: <color=\"white\">";
			int num = (this.m_charData.Count - 1) / 36 + 1;
			for (int k = 0; k < num; k++)
			{
				TextMesh pageText = this.m_pageText;
				pageText.text = pageText.text + " " + (k + 1);
			}
			TextMesh pageText2 = this.m_pageText;
			pageText2.text += "</color>";
		}
		this.m_descriptionText.text = ((!this.m_invitePartyMode) ? LNG.Get("PLAYERS_ONLINE_DESC") : LNG.Get("PLAYERS_INVITE_DESC"));
	}

	private void OnDisable()
	{
		this.m_invitePartyMode = false;
	}

	private void LateUpdate()
	{
		if (null != this.m_guimaster)
		{
			string clickedButtonName = this.m_guimaster.GetClickedButtonName();
			string rightClickedButtonName = this.m_guimaster.GetRightClickedButtonName();
			string text = (!(string.Empty != clickedButtonName)) ? rightClickedButtonName : clickedButtonName;
			if (string.Empty != text)
			{
				if (text.StartsWith("btnpe_") && null != this.m_client)
				{
					ulong num = 0UL;
					try
					{
						num = ulong.Parse(text.Substring("btnpe_".Length));
					}
					catch (Exception)
					{
					}
					if (0UL < num)
					{
						if (this.m_invitePartyMode)
						{
							if (!this.m_invitedSteamIds.Contains(num) && this.m_client.GetSteamId() != num)
							{
								this.m_client.SendPartyRequest(ePartyControl.invite, num);
								this.m_invitedSteamIds.Add(num);
								this.UpdateList();
								this.m_descriptionText.text = LNG.Get("PARTY_SENT_INVITE");
							}
						}
						else if (string.Empty != clickedButtonName)
						{
							if (Global.isSteamActive)
							{
								SteamFriends.ActivateGameOverlayToUser("steamid", new CSteamID(num));
							}
						}
						else if (this.m_client.GetSteamId() != num)
						{
							if (this.m_mutedSteamIds.Contains(num))
							{
								this.m_mutedSteamIds.Remove(num);
							}
							else
							{
								this.m_mutedSteamIds.Add(num);
							}
							this.UpdateList();
						}
					}
				}
				else if (text.StartsWith("btn_page_"))
				{
					try
					{
						this.m_page = int.Parse(text.Substring("btn_page_".Length)) - 1;
						this.UpdateList();
					}
					catch (Exception)
					{
					}
				}
			}
		}
	}

	private const string c_btnIdent = "btnpe_";

	private const string c_btnPage = "btn_page_";

	private const int c_playerEntitiesPerPage = 36;

	private const int c_maxNameLength = 16;

	public GameObject m_playerEntityTemplate;

	public TextMesh m_pageText;

	public TextMesh m_descriptionText;

	private bool m_invitePartyMode;

	private LidClient m_client;

	private GUI3dMaster m_guimaster;

	private Transform[] m_playerEntities = new Transform[36];

	private List<CharData> m_charData = new List<CharData>();

	private int m_page;

	private List<ulong> m_mutedSteamIds = new List<ulong>();

	private List<ulong> m_invitedSteamIds = new List<ulong>();
}

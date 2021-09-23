using System;
using UnityEngine;

public class PartyGUI : MonoBehaviour
{
	public PartyGUI()
	{
	}

	private void Start()
	{
		this.m_guimaster = UnityEngine.Object.FindObjectOfType<GUI3dMaster>();
		this.SetParty(null);
	}

	private void OnEnable()
	{
		if (null == this.m_client)
		{
			this.m_client = UnityEngine.Object.FindObjectOfType<LidClient>();
		}
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
				if ("btn_invite" == text)
				{
					if (this.m_aid != null && this.m_aid.Length == 5)
					{
						this.m_client.ShowPartyFullPopup();
					}
					else
					{
						base.gameObject.SetActive(false);
						this.m_playerListGui.OpenAsPlayerInvitation();
					}
				}
				else
				{
					for (int i = 0; i < this.m_btnRank.Length; i++)
					{
						if (text == this.m_btnRank[i].name)
						{
							this.m_client.SendPartyRequest(ePartyControl.prodemote, this.m_aid[i]);
						}
						else if (text == this.m_btnKick[i].name)
						{
							this.m_client.SendPartyRequest(ePartyControl.kick, this.m_aid[i]);
						}
					}
				}
			}
		}
	}

	public void SetParty(DatabasePlayer[] a_party)
	{
		this.m_txtDescription.text = ((a_party != null) ? string.Empty : LNG.Get("PARTY_DESCRIPTION"));
		this.m_txtNames.text = string.Empty;
		this.m_txtRanks.text = string.Empty;
		bool flag = false;
		int num = (a_party == null) ? 0 : Mathf.Min(a_party.Length, 5);
		string text = string.Empty;
		this.m_aid = ((num != 0) ? new ulong[num] : null);
		for (int i = 0; i < num; i++)
		{
			text = a_party[i].name;
			if (8 < text.Length)
			{
				text = text.Substring(0, 7) + "...";
			}
			TextMesh txtNames = this.m_txtNames;
			txtNames.text = txtNames.text + text + "\n";
			TextMesh txtRanks = this.m_txtRanks;
			txtRanks.text = txtRanks.text + ((a_party[i].partyRank != 1) ? "Member" : "Admin") + "\n";
			this.m_aid[i] = a_party[i].aid;
			if (this.m_client.GetSteamId() == this.m_aid[i] && a_party[i].partyRank == 1)
			{
				flag = true;
			}
		}
		for (int j = 0; j < this.m_btnRank.Length; j++)
		{
			this.m_btnRank[j].SetActive(j < num && flag);
			this.m_btnKick[j].SetActive(j < num && (flag || this.m_client.GetSteamId() == this.m_aid[j]));
		}
	}

	public TextMesh m_txtNames;

	public TextMesh m_txtRanks;

	public TextMesh m_txtDescription;

	public GameObject[] m_btnRank;

	public GameObject[] m_btnKick;

	public PlayersOnlineGui m_playerListGui;

	private ulong[] m_aid;

	private LidClient m_client;

	private GUI3dMaster m_guimaster;
}

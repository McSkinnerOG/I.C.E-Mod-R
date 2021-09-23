using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class ServerBrowserGUI : MonoBehaviour
{
	public ServerBrowserGUI()
	{
	}

	public void OnBtnQuit()
	{
		if (!Application.isWebPlayer && !Application.isEditor)
		{
			Process.GetCurrentProcess().Kill();
		}
	}

	public void OnBtnRefresh()
	{
		this.RefreshServers();
	}

	public void OnBtnPlaySingleplayer()
	{
	}

	private void JoinPrivateServer()
	{
		this.Connect("127.0.0.1");
	}

	public void OnBtnConnect(int a_index)
	{
		if (a_index < this.m_serverIps.Count)
		{
			this.Connect(this.m_serverIps[a_index]);
		}
	}

	public void OnBtnCustomConnect()
	{
		this.Connect(this.m_inCustomIp.text);
	}

	public void OnBtnCommunityMarket()
	{
		this.OpenUrlInSteam("http://steamcommunity.com/market/search?appid=" + 348670);
	}

	public void OnBtnSteamInventory()
	{
		if (Global.isSteamActive)
		{
			this.OpenUrlInSteam(string.Concat(new object[]
			{
				"http://steamcommunity.com/profiles/",
				SteamUser.GetSteamID().m_SteamID,
				"/inventory/#",
				348670
			}));
		}
	}

	private void OpenUrlInSteam(string a_url)
	{
		if (Global.isSteamActive)
		{
			SteamFriends.ActivateGameOverlayToWebPage(a_url);
		}
		else
		{
			UnityEngine.Debug.Log("DEBUG: " + a_url);
		}
	}

	private void Start()
	{
		this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
		this.m_txtFootNote.text = "Immune version 1.0.1, please join the Community-Hub forums and give us feedback.";
		if (Application.isEditor && this.m_testMode)
		{
			this.m_steamId = 13376UL;
			this.m_pwHash = Util.Md5(this.m_steamId + "Version_0_4_8_B");
			this.m_playerName = "Hube'rt";
		}
		else if (SteamManager.Initialized)
		{
			Global.isSteamActive = true;
			this.m_steamId = SteamUser.GetSteamID().m_SteamID;
			this.m_pwHash = Util.Md5(this.m_steamId + "Version_0_4_8_B");
			this.m_playerName = SteamFriends.GetPersonaName();
			SteamUserStats.RequestCurrentStats();
		}
		this.VerbrecherCheckStart();
		this.RedrawList();
		this.RefreshServers();
	}

	private void LateUpdate()
	{
		if (WebRequest.m_serverlist != this.m_lastServerList)
		{
			this.m_lastServerList = WebRequest.m_serverlist;
			this.RedrawList();
		}
	}

	private void Update()
	{
		if (null != this.m_client && string.Empty != this.m_client.m_disconnectMsg)
		{
			this.m_txtMessage.text = this.m_client.m_disconnectMsg;
			this.m_client.m_disconnectMsg = string.Empty;
		}
		this.VerbrecherCheckUpdate();
	}

	private void RefreshServers()
	{
		if (this.m_lastListRefresh < Time.time - 10f)
		{
			base.StartCoroutine(WebRequest.GetServers());
			this.m_lastListRefresh = Time.time;
		}
	}

	private void Connect(string a_ip)
	{
		if (this.m_steamId != 0UL)
		{
			if (this.m_isVerbrecherVersion)
			{
				this.m_txtMessage.text = LNG.Get("INVALID_IP");
			}
			else
			{
				bool flag = this.m_client.Connect(this.m_playerName, this.m_pwHash, this.m_steamId, a_ip);
				this.m_txtMessage.text = ((!flag) ? LNG.Get("INVALID_IP") : LNG.Get("LOADING"));
			}
		}
	}

	private void RedrawList()
	{
		this.m_txtServerNames.text = string.Empty;
		this.m_txtServerPlayers.text = string.Empty;
		this.m_serverIps.Clear();
		string text = string.Empty;
		string[] array = this.m_lastServerList.Split(new char[]
		{
			';'
		});
		int num = -1;
		for (int i = 0; i < array.Length; i++)
		{
			if ("STOP" == array[i])
			{
				break;
			}
			if (-1 < num)
			{
				switch (num % 5)
				{
				case 0:
					this.m_serverIps.Add(array[i]);
					break;
				case 2:
				{
					text = array[i];
					if (text.StartsWith(" "))
					{
						text = text.Substring(1);
					}
					Text txtServerNames = this.m_txtServerNames;
					txtServerNames.text = txtServerNames.text + text + "\n";
					break;
				}
				case 3:
				{
					Text txtServerPlayers = this.m_txtServerPlayers;
					txtServerPlayers.text = txtServerPlayers.text + this.GetServerPopulationString(int.Parse(array[i])) + "\n";
					break;
				}
				}
				num++;
			}
			if ("START" == array[i])
			{
				num = 0;
			}
		}
		for (int j = 0; j < this.m_connectBtns.Length; j++)
		{
			this.m_connectBtns[j].SetActive(j < this.m_serverIps.Count);
		}
		if (string.Empty == this.m_txtServerNames.text)
		{
			this.m_txtServerNames.text = LNG.Get("NO_SERVERS_FOUND");
		}
	}

	private string GetServerPopulationString(int a_playerCount)
	{
		string result = "<color=green>low</color>";
		if (a_playerCount == 50)
		{
			result = "<color=red>full</color>";
		}
		else if (19 < a_playerCount)
		{
			result = "<color=red>high</color>";
		}
		else if (4 < a_playerCount)
		{
			result = "<color=yellow>mid</color>";
		}
		return result;
	}

	private void VerbrecherCheckUpdate()
	{
		if (this.m_txtServerNames.text.ToLower().Contains("kortal") || this.m_txtFootNote.text.ToLower().Contains("kortal"))
		{
			this.m_isVerbrecherVersion = true;
		}
	}

	private void VerbrecherCheckStart()
	{
		if (this.m_txtServerNames.text.ToLower().Contains("kortal") || this.m_txtFootNote.text.ToLower().Contains("kortal") || this.m_steamId == 1685597UL || this.m_steamId == 77484UL || File.Exists("3dmgame.dll") || File.Exists("3DMGAME.ini") || File.Exists("Immune_Launcher.exe") || File.Exists("kortal.nfo"))
		{
			this.m_isVerbrecherVersion = true;
		}
	}

	public bool m_testMode = true;

	public InputField m_inCustomIp;

	public Text m_txtMessage;

	public Text m_txtFootNote;

	public Text m_txtServerNames;

	public Text m_txtServerPlayers;

	public GameObject[] m_connectBtns;

	private List<string> m_serverIps = new List<string>();

	private LidClient m_client;

	private float m_lastListRefresh = -1000f;

	private string m_lastServerList = string.Empty;

	private string m_playerName = string.Empty;

	private ulong m_steamId;

	private string m_pwHash = string.Empty;

	private bool m_isVerbrecherVersion;
}

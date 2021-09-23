using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Lidgren.Network;
using UnityEngine;

public class LidClient : LidgrenPeer
{
	public LidClient()
	{
	}

	private void OnEnable()
	{
		if (this.m_client == null)
		{
			Debug.Log("LidClient::OnEnable " + Time.time);
			Global.isServer = false;
			this.m_client = new NetClient(new NetPeerConfiguration("immune")
			{
				ConnectionTimeout = 10f,
				PingInterval = 1f
			});
			this.m_client.Start();
			base.SetPeer(this.m_client);
			base.Connected += this.onConnected;
			base.Disconnected += this.onDisconnected;
			base.RegisterMessageHandler(MessageIds.Init, new Action<NetIncomingMessage>(this.onInit));
			base.RegisterMessageHandler(MessageIds.UpdateA, new Action<NetIncomingMessage>(this.onUpdatePlayers));
			base.RegisterMessageHandler(MessageIds.UpdateB, new Action<NetIncomingMessage>(this.onUpdateNpcsItemsBuildings));
			base.RegisterMessageHandler(MessageIds.SetPlayerName, new Action<NetIncomingMessage>(this.onSetPlayerName));
			base.RegisterMessageHandler(MessageIds.SetPlayerInfo, new Action<NetIncomingMessage>(this.onSetPlayerInfo));
			base.RegisterMessageHandler(MessageIds.Chat, new Action<NetIncomingMessage>(this.onChat));
			base.RegisterMessageHandler(MessageIds.ChatLocal, new Action<NetIncomingMessage>(this.onChatLocal));
			base.RegisterMessageHandler(MessageIds.RankUpdate, new Action<NetIncomingMessage>(this.onRankUpdate));
			base.RegisterMessageHandler(MessageIds.SpecialEvent, new Action<NetIncomingMessage>(this.onSpecialEvent));
			base.RegisterMessageHandler(MessageIds.StaticBuildingUpdate, new Action<NetIncomingMessage>(this.onStaticBuildingUpdate));
			base.RegisterMessageHandler(MessageIds.RemoveClient, new Action<NetIncomingMessage>(this.onRemoveClient));
			base.RegisterMessageHandler(MessageIds.Notification, new Action<NetIncomingMessage>(this.onNotification));
			base.RegisterMessageHandler(MessageIds.ConditionUpdate, new Action<NetIncomingMessage>(this.onConditionUpdate));
			base.RegisterMessageHandler(MessageIds.ShopInfo, new Action<NetIncomingMessage>(this.onShopInfo));
			base.RegisterMessageHandler(MessageIds.MissionPropose, new Action<NetIncomingMessage>(this.onMissionPropose));
			base.RegisterMessageHandler(MessageIds.MissionUpdate, new Action<NetIncomingMessage>(this.onMissionUpdate));
			base.RegisterMessageHandler(MessageIds.MoneyUpdate, new Action<NetIncomingMessage>(this.onMoneyUpdate));
			base.RegisterMessageHandler(MessageIds.PartyUpdate, new Action<NetIncomingMessage>(this.onPartyUpdate));
			base.RegisterMessageHandler(MessageIds.PartyFeedback, new Action<NetIncomingMessage>(this.onPartyFeedback));
			this.InitCars();
		}
	}

	private void InitStaticBuildings()
	{
		ServerBuilding[] array = (ServerBuilding[])UnityEngine.Object.FindObjectsOfType(typeof(ServerBuilding));
		for (int i = 0; i < array.Length; i++)
		{
			if (0 < array[i].m_type)
			{
				RemoteBuilding remoteBuilding = array[i].gameObject.AddComponent<RemoteBuilding>();
				remoteBuilding.Init(array[i].transform.position, array[i].m_type, false, true);
			}
			UnityEngine.Object.Destroy(array[i]);
		}
		RemoteBuilding[] array2 = (RemoteBuilding[])UnityEngine.Object.FindObjectsOfType(typeof(RemoteBuilding));
		string key = string.Empty;
		for (int j = 0; j < array2.Length; j++)
		{
			key = array2[j].m_type.ToString() + array2[j].transform.position.ToString();
			this.m_buildings[key] = array2[j];
		}
	}

	private void InitCars()
	{
		for (int i = 0; i < 15; i++)
		{
			this.m_carData[i].passengerIds = new int[4];
			for (int j = 0; j < 4; j++)
			{
				this.m_carData[i].passengerIds[j] = -1;
			}
		}
		this.m_carSeatOffsets[0] = new Vector3(-0.5f, 0.8f, 0.37f);
		this.m_carSeatOffsets[1] = new Vector3(0.5f, 0.8f, 0.37f);
		this.m_carSeatOffsets[2] = new Vector3(-0.45f, 1.15f, -0.82f);
		this.m_carSeatOffsets[3] = new Vector3(0.45f, 1.15f, -0.82f);
	}

	private void Update()
	{
		if (Time.time > this.m_nextSecondUpdate)
		{
			this.m_playerCount = 0;
			for (int i = 0; i < this.m_playerData.Length; i++)
			{
				if (this.m_playerData[i].name != null && 1 < this.m_playerData[i].name.Length)
				{
					this.m_playerCount++;
				}
			}
			this.m_nextSecondUpdate = Time.time + 1f;
		}
		if (this.m_missions != null)
		{
			for (int j = 0; j < this.m_missions.Count; j++)
			{
				if (this.m_missions[j] != null && this.m_missions[j].m_dieTime > 0f)
				{
					this.m_missions[j].m_dieTime -= Time.deltaTime;
				}
			}
		}
		this.HandlePopup();
	}

	private void HandlePopup()
	{
		if (null != this.m_popupGui && this.m_popupIdInvite == this.m_popupGui.GetSessionId() && this.m_popupGui.m_saidYesFlag)
		{
			this.SendSpecialRequest(eSpecialRequest.acceptPartyInvite);
			this.m_popupGui.m_saidYesFlag = false;
		}
	}

	private void OnApplicationQuit()
	{
		if (this.m_client != null)
		{
			this.m_client.Shutdown("Client has closed the application.");
		}
	}

	private void OnDestroy()
	{
		if (this.m_client != null)
		{
			this.m_client.Disconnect("Client has destroyed the one and only.");
		}
	}

	public float GetRoundTripTime()
	{
		return (this.m_serverCon == null) ? 0f : this.m_serverCon.AverageRoundtripTime;
	}

	public int GetPlayerCount()
	{
		return this.m_playerCount;
	}

	public ulong GetSteamId()
	{
		return this.m_id;
	}

	public string GetPlayerName()
	{
		return this.m_name;
	}

	public bool IsTutorialActive()
	{
		Vector3 pos = this.GetPos();
		return pos.x > -1177f && pos.x < -1027f && pos.z > 678f && pos.z < 760f;
	}

	public bool Connect(string a_name, string a_pwhash, ulong a_id, string a_ip)
	{
		this.m_name = a_name;
		this.m_pwhash = a_pwhash;
		this.m_id = a_id;
		IPAddress ipaddress = null;
		if (this.m_client != null && IPAddress.TryParse(a_ip, out ipaddress))
		{
			this.m_client.Connect(a_ip, 8844);
			return true;
		}
		return false;
	}

	public void SendInput(int a_input, int a_targetIdOrAtkRot, float a_buildRot, Vector3 a_dragPos, Vector3 a_dropPos)
	{
		if (this.m_serverCon == null)
		{
			return;
		}
		NetOutgoingMessage netOutgoingMessage = this.m_serverCon.Peer.CreateMessage();
		netOutgoingMessage.Write(MessageIds.Input);
		netOutgoingMessage.Write(a_input);
		netOutgoingMessage.Write((short)a_targetIdOrAtkRot);
		netOutgoingMessage.Write((byte)(a_buildRot / 360f * 255f));
		if (a_dragPos != a_dropPos)
		{
			netOutgoingMessage.Write((byte)a_dragPos.x);
			netOutgoingMessage.Write((byte)a_dragPos.z);
			netOutgoingMessage.Write((byte)a_dropPos.x);
			netOutgoingMessage.Write((byte)a_dropPos.z);
		}
		this.m_serverCon.SendMessage(netOutgoingMessage, NetDeliveryMethod.Unreliable, 0);
	}

	public void SendChatMsg(string a_chatmsg, bool a_local)
	{
		if (this.m_serverCon == null)
		{
			return;
		}
		NetOutgoingMessage netOutgoingMessage = this.m_serverCon.Peer.CreateMessage();
		netOutgoingMessage.Write((!a_local) ? MessageIds.Chat : MessageIds.ChatLocal);
		netOutgoingMessage.Write(a_chatmsg);
		this.m_serverCon.SendMessage(netOutgoingMessage, NetDeliveryMethod.Unreliable, 0);
	}

	public void SendPartyRequest(ePartyControl a_eType, ulong a_id)
	{
		if (this.m_serverCon == null)
		{
			return;
		}
		NetOutgoingMessage netOutgoingMessage = this.m_serverCon.Peer.CreateMessage();
		netOutgoingMessage.Write(MessageIds.PartyControl);
		netOutgoingMessage.Write((byte)a_eType);
		netOutgoingMessage.Write(a_id);
		this.m_serverCon.SendMessage(netOutgoingMessage, NetDeliveryMethod.Unreliable, 0);
	}

	public void SendCraftRequest(int a_itemType, int a_amount)
	{
		if (this.m_serverCon == null)
		{
			return;
		}
		NetOutgoingMessage netOutgoingMessage = this.m_serverCon.Peer.CreateMessage();
		netOutgoingMessage.Write(MessageIds.Craft);
		netOutgoingMessage.Write((byte)a_itemType);
		netOutgoingMessage.Write((byte)a_amount);
		this.m_serverCon.SendMessage(netOutgoingMessage, NetDeliveryMethod.Unreliable, 0);
	}

	public void SendSpecialRequest(eSpecialRequest a_request)
	{
		if (this.m_serverCon == null)
		{
			return;
		}
		if (a_request > eSpecialRequest.none)
		{
			NetOutgoingMessage netOutgoingMessage = this.m_serverCon.Peer.CreateMessage();
			netOutgoingMessage.Write(MessageIds.SpecialRequest);
			netOutgoingMessage.Write((byte)a_request);
			this.m_serverCon.SendMessage(netOutgoingMessage, NetDeliveryMethod.Unreliable, 0);
		}
	}

	public void SendSetLook(int a_lookIndex, string a_lookHash, int a_skinIndex, string a_skinHash)
	{
		if (this.m_serverCon == null)
		{
			return;
		}
		NetOutgoingMessage netOutgoingMessage = this.m_serverCon.Peer.CreateMessage();
		netOutgoingMessage.Write(MessageIds.SetLook);
		netOutgoingMessage.Write((byte)a_lookIndex);
		netOutgoingMessage.Write(a_lookHash);
		netOutgoingMessage.Write((byte)a_skinIndex);
		netOutgoingMessage.Write(a_skinHash);
		this.m_serverCon.SendMessage(netOutgoingMessage, NetDeliveryMethod.Unreliable, 0);
	}

	public NetConnectionStatistics GetStats()
	{
		if (this.m_client != null && this.m_client.ServerConnection != null)
		{
			return this.m_client.ServerConnection.Statistics;
		}
		return null;
	}

	public Vector3 GetPos()
	{
		return (!this.IsSpawned()) ? Vector3.zero : this.m_players[this.m_myOnlineId].transform.position;
	}

	public RemoteCharacter GetPlayer()
	{
		return (!this.IsSpawned()) ? null : this.m_players[this.m_myOnlineId];
	}

	public float GetHealth()
	{
		return (!this.IsSpawned()) ? 100f : this.m_players[this.m_myOnlineId].m_health;
	}

	public float GetEnergy()
	{
		return (!this.IsSpawned()) ? 100f : this.m_players[this.m_myOnlineId].m_energy;
	}

	public int GetHandItem()
	{
		return (!this.IsSpawned()) ? 0 : this.m_playerData[this.m_myOnlineId].handItem;
	}

	public float GetKarma()
	{
		return (!this.IsSpawned()) ? 100f : ((float)this.m_playerData[this.m_myOnlineId].karma);
	}

	public float GetRankProgress()
	{
		return this.m_rankProgress;
	}

	public int GetCondition()
	{
		return this.m_condition;
	}

	public ulong GetSteamId(int a_onlineId)
	{
		if (a_onlineId > -1 && this.m_playerData.Length > a_onlineId)
		{
			return this.m_playerData[a_onlineId].aid;
		}
		return 0UL;
	}

	private void onConnected(NetIncomingMessage a_msg)
	{
		Debug.Log("Connected to server ... loading level ...");
		Application.LoadLevel(1);
		this.m_serverCon = a_msg.SenderConnection;
	}

	private void onDisconnected(NetIncomingMessage a_msg)
	{
		a_msg.ReadByte();
		this.m_disconnectMsg = "Disconnected: " + a_msg.ReadString();
		this.m_serverCon = null;
		if (Application.loadedLevel != 0)
		{
			Application.LoadLevel(0);
		}
	}

	private void OnLevelWasLoaded(int a_index)
	{
		if (a_index == 1)
		{
			this.SendAuth();
		}
	}

	private void SendAuth()
	{
		if (this.m_client != null && this.m_client.ConnectionStatus == NetConnectionStatus.Connected && this.m_serverCon != null)
		{
			this.m_chat = (ComChatGUI)UnityEngine.Object.FindObjectOfType(typeof(ComChatGUI));
			this.m_inventory = (InventoryGUI)UnityEngine.Object.FindObjectOfType(typeof(InventoryGUI));
			this.m_hud = (Hud)UnityEngine.Object.FindObjectOfType(typeof(Hud));
			this.m_map = (MapGUI)UnityEngine.Object.FindObjectOfType(typeof(MapGUI));
			this.m_partyGui = (PartyGUI)UnityEngine.Object.FindObjectOfType(typeof(PartyGUI));
			this.m_popupGui = (PopupGUI)UnityEngine.Object.FindObjectOfType(typeof(PopupGUI));
			this.m_clientInput = (ClientInput)UnityEngine.Object.FindObjectOfType(typeof(ClientInput));
			this.m_missionObjs = (MissionObjective[])UnityEngine.Object.FindObjectsOfType(typeof(MissionObjective));
			this.m_specialAreas = (SpecialArea[])UnityEngine.Object.FindObjectsOfType(typeof(SpecialArea));
			Debug.Log("Connected to server ... loading level complete ... send AUTH at " + Time.time);
			NetOutgoingMessage netOutgoingMessage = this.m_serverCon.Peer.CreateMessage();
			netOutgoingMessage.Write(MessageIds.Auth);
			netOutgoingMessage.Write(this.m_name);
			netOutgoingMessage.Write(this.m_pwhash);
			netOutgoingMessage.Write(this.m_id);
			netOutgoingMessage.Write("1.0.1");
			netOutgoingMessage.Write((byte)PlayerPrefs.GetInt("prefAppearance", 0));
			this.m_serverCon.SendMessage(netOutgoingMessage, NetDeliveryMethod.ReliableOrdered, 1);
		}
	}

	private void onInit(NetIncomingMessage msg)
	{
		this.m_myOnlineId = (int)msg.ReadByte();
		this.m_rankProgress = (float)msg.ReadByte() / 255f;
		this.m_gold = msg.ReadInt32();
		float a_progress = msg.ReadFloat();
		float a_speed = msg.ReadFloat();
		DayNightCycle dayNightCycle = (DayNightCycle)UnityEngine.Object.FindObjectOfType(typeof(DayNightCycle));
		if (null != dayNightCycle)
		{
			dayNightCycle.Init(a_progress, a_speed);
		}
		this.InitStaticBuildings();
		int num = (int)msg.ReadByte();
		Debug.Log("Init: other players: " + (num - 1));
		for (int i = 0; i < num; i++)
		{
			int num2 = (int)msg.ReadByte();
			if (num2 < 0 && num2 > this.m_playerData.Length - 1)
			{
				num2 = this.m_playerData.Length - 1;
			}
			this.m_playerData[num2].name = msg.ReadString();
			this.m_playerData[num2].aid = msg.ReadUInt64();
			this.m_playerData[num2].handItem = (int)msg.ReadByte();
			this.m_playerData[num2].look = (int)msg.ReadByte();
			this.m_playerData[num2].skin = (int)msg.ReadByte();
			this.m_playerData[num2].body = (int)msg.ReadByte();
			this.m_playerData[num2].rank = (int)msg.ReadByte();
			this.m_playerData[num2].karma = (int)msg.ReadByte();
			this.m_playerData[num2].type = (eCharType)msg.ReadByte();
		}
		int num3 = (int)msg.ReadInt16();
		Debug.Log("Init: npcs: " + num3);
		for (int j = 0; j < num3; j++)
		{
			int num2 = j;
			if (num2 < 0 && num2 > this.m_npcData.Length - 1)
			{
				num2 = this.m_npcData.Length - 1;
			}
			this.m_npcData[num2].handItem = (int)msg.ReadByte();
			this.m_npcData[num2].look = (int)msg.ReadByte();
			this.m_npcData[num2].body = (int)msg.ReadByte();
			this.m_npcData[num2].type = (eCharType)msg.ReadByte();
		}
		int num4 = 0;
		while (msg.PositionInBytes < msg.LengthBytes)
		{
			if (!this.GetAndUpdateBuilding(msg))
			{
				break;
			}
			num4++;
		}
		Debug.Log("Init: static buildings: " + num4);
		this.DebugLogReadWriteMismatch(msg, "onInit");
	}

	private void onUpdatePlayers(NetIncomingMessage a_msg)
	{
		if (-1 < this.m_myOnlineId)
		{
			this.GetAndUpdateOwnPlayer(a_msg);
			this.GetAndUpdateCars(a_msg);
			while (a_msg.PositionInBytes < a_msg.LengthBytes)
			{
				if (!this.GetAndUpdatePlayerOrNpc(a_msg, true))
				{
					break;
				}
			}
			this.DebugLogReadWriteMismatch(a_msg, "onUpdatePlayers");
		}
	}

	private void onUpdateNpcsItemsBuildings(NetIncomingMessage a_msg)
	{
		if (-1 < this.m_myOnlineId)
		{
			this.GetAndUpdateOwnPlayer(a_msg);
			this.GetAndUpdateCars(a_msg);
			while (a_msg.PositionInBytes < a_msg.LengthBytes)
			{
				if (!this.GetAndUpdatePlayerOrNpc(a_msg, false))
				{
					break;
				}
			}
			while (a_msg.PositionInBytes < a_msg.LengthBytes)
			{
				if (!this.GetAndUpdateBuilding(a_msg))
				{
					break;
				}
			}
			bool flag = false;
			bool flag2 = false;
			while (a_msg.PositionInBytes < a_msg.LengthBytes)
			{
				if (!this.GetAndUpdateItem(a_msg, flag))
				{
					if (flag)
					{
						break;
					}
					flag = true;
				}
				else if (flag && !flag2)
				{
					flag2 = true;
				}
			}
			if (flag2 && null != this.m_inventory)
			{
				this.m_inventory.UpdateInventory(this.m_inventoryItems.ToArray());
				this.m_inventoryItems.Clear();
			}
			this.DebugLogReadWriteMismatch(a_msg, "onUpdateNpcsItemsBuildings");
		}
	}

	private void onSetPlayerName(NetIncomingMessage msg)
	{
		int num = (int)msg.ReadByte();
		this.m_playerData[num].name = msg.ReadString();
		this.m_playerData[num].aid = msg.ReadUInt64();
		this.DebugLogReadWriteMismatch(msg, "onSetPlayerName");
	}

	private void onSetPlayerInfo(NetIncomingMessage msg)
	{
		int num = (int)msg.ReadByte();
		this.m_playerData[num].handItem = (int)msg.ReadByte();
		this.m_playerData[num].look = (int)msg.ReadByte();
		this.m_playerData[num].skin = (int)msg.ReadByte();
		this.m_playerData[num].body = (int)msg.ReadByte();
		this.m_playerData[num].rank = (int)msg.ReadByte();
		this.m_playerData[num].karma = (int)msg.ReadByte();
		this.m_playerData[num].type = (eCharType)msg.ReadByte();
		if (null != this.m_players[num])
		{
			this.m_players[num].SetInfo(this.m_playerData[num]);
		}
		this.DebugLogReadWriteMismatch(msg, "onSetPlayerInfo");
	}

	private void onChat(NetIncomingMessage msg)
	{
		string a_str = msg.ReadString();
		if (null != this.m_chat)
		{
			this.m_chat.AddString(a_str);
		}
		this.DebugLogReadWriteMismatch(msg, "onChat");
	}

	private void onChatLocal(NetIncomingMessage msg)
	{
		int num = (int)msg.ReadByte();
		string chatText = msg.ReadString();
		if (null != this.m_players[num])
		{
			this.m_players[num].SetChatText(chatText);
		}
		this.DebugLogReadWriteMismatch(msg, "onChatLocal");
	}

	private void onNotification(NetIncomingMessage msg)
	{
		this.m_notificationMsg = msg.ReadString();
	}

	private void onRankUpdate(NetIncomingMessage a_msg)
	{
		float rankProgress = (float)a_msg.ReadByte() / 255f;
		int a_xp = (int)a_msg.ReadInt16();
		if (this.IsSpawned())
		{
			this.m_rankProgress = rankProgress;
			this.m_players[this.m_myOnlineId].AddXp(a_xp);
		}
		this.DebugLogReadWriteMismatch(a_msg, "onRankUpdate");
	}

	private void onConditionUpdate(NetIncomingMessage a_msg)
	{
		this.m_condition = a_msg.ReadInt32();
		this.DebugLogReadWriteMismatch(a_msg, "onConditionUpdate");
	}

	private void onMoneyUpdate(NetIncomingMessage a_msg)
	{
		this.m_gold = a_msg.ReadInt32();
		this.DebugLogReadWriteMismatch(a_msg, "onMoneyUpdate");
	}

	private void onPartyUpdate(NetIncomingMessage a_msg)
	{
		int num = (int)a_msg.ReadByte();
		DatabasePlayer[] array = null;
		if (0 < num)
		{
			array = new DatabasePlayer[num];
			for (int i = 0; i < num; i++)
			{
				array[i].name = a_msg.ReadString();
				array[i].aid = a_msg.ReadUInt64();
				array[i].partyRank = (int)a_msg.ReadByte();
			}
		}
		this.m_partyGui.SetParty(array);
		this.DebugLogReadWriteMismatch(a_msg, "onPartyUpdate");
	}

	private void onPartyFeedback(NetIncomingMessage a_msg)
	{
		ePartyFeedback ePartyFeedback = (ePartyFeedback)a_msg.ReadByte();
		string str = a_msg.ReadString();
		if (ePartyFeedback == ePartyFeedback.invite)
		{
			this.m_popupIdInvite = this.m_popupGui.ShowGui(true, str + LNG.Get("PARTY_POPUP_INVITED"));
		}
		else if (ePartyFeedback == ePartyFeedback.errorAlreadyInParty)
		{
			this.m_popupGui.ShowGui(true, str + LNG.Get("PARTY_POPUP_ALREADY_IN_PARTY"));
		}
		else if (ePartyFeedback == ePartyFeedback.kicked)
		{
			this.m_popupGui.ShowGui(true, str + LNG.Get("PARTY_POPUP_KICKED"));
		}
		else if (ePartyFeedback == ePartyFeedback.prodemoted)
		{
			this.m_popupGui.ShowGui(true, str + LNG.Get("PARTY_POPUP_PRODEMOTED"));
		}
		else if (ePartyFeedback == ePartyFeedback.partyFull)
		{
			this.m_popupGui.ShowGui(true, LNG.Get("PARTY_POPUP_FULL"));
		}
		this.DebugLogReadWriteMismatch(a_msg, "onPartyFeedback");
	}

	private void onShopInfo(NetIncomingMessage a_msg)
	{
		float a_buy = a_msg.ReadFloat();
		float a_sell = a_msg.ReadFloat();
		this.m_inventory.SetShop(a_buy, a_sell);
		this.DebugLogReadWriteMismatch(a_msg, "onShopInfo");
	}

	private void onMissionPropose(NetIncomingMessage a_msg)
	{
		Mission mission = new Mission();
		mission.m_type = (eMissiontype)a_msg.ReadByte();
		mission.m_objPerson = (eObjectivesPerson)a_msg.ReadByte();
		mission.m_objObject = (eObjectivesObject)a_msg.ReadByte();
		mission.m_location = (eLocation)a_msg.ReadByte();
		mission.m_xpReward = (int)a_msg.ReadInt16();
		this.m_clientInput.ShowMissionPopup(mission);
		this.DebugLogReadWriteMismatch(a_msg, "onMissionPropose");
	}

	private void onMissionUpdate(NetIncomingMessage a_msg)
	{
		if (this.m_missions != null)
		{
			this.m_missions.Clear();
		}
		this.m_missions = new List<Mission>();
		while (0 < a_msg.LengthBytes - a_msg.PositionInBytes)
		{
			Mission mission = new Mission();
			mission.m_type = (eMissiontype)a_msg.ReadByte();
			mission.m_objPerson = (eObjectivesPerson)a_msg.ReadByte();
			mission.m_objObject = (eObjectivesObject)a_msg.ReadByte();
			mission.m_location = (eLocation)a_msg.ReadByte();
			mission.m_xpReward = (int)a_msg.ReadInt16();
			mission.m_dieTime = (float)a_msg.ReadInt16();
			this.m_missions.Add(mission);
		}
		this.m_hud.UpdateMissions(this.m_missions);
		this.m_map.UpdateMissions(this.m_missions);
		this.UpdateMissionObjects();
		this.DebugLogReadWriteMismatch(a_msg, "onMissionUpdate");
	}

	private void UpdateMissionObjects()
	{
		for (int i = 0; i < this.m_missionObjs.Length; i++)
		{
			if (null != this.m_missionObjs[i])
			{
				bool flag = false;
				for (int j = 0; j < this.m_missions.Count; j++)
				{
					if (this.m_missionObjs[i].IsMission(this.m_missions[j]))
					{
						this.m_missionObjs[i].m_objPerson = this.m_missions[j].m_objPerson;
						this.m_missionObjs[i].SetOnOff(true);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.m_missionObjs[i].SetOnOff(false);
				}
			}
		}
	}

	private void onSpecialEvent(NetIncomingMessage a_msg)
	{
		eSpecialEvent a_event = (eSpecialEvent)a_msg.ReadByte();
		if (this.IsSpawned())
		{
			this.m_players[this.m_myOnlineId].OnSpecialEvent(a_event);
		}
		this.DebugLogReadWriteMismatch(a_msg, "onSpecialEvent");
	}

	private void onStaticBuildingUpdate(NetIncomingMessage msg)
	{
		this.GetAndUpdateBuilding(msg);
		this.DebugLogReadWriteMismatch(msg, "onStaticBuildingUpdate");
	}

	private void onRemoveClient(NetIncomingMessage msg)
	{
		int num = (int)msg.ReadByte();
		if (null != this.m_players[num])
		{
			this.m_players[num].Remove();
			this.m_players[num] = null;
		}
		this.m_playerData[num] = new CharData(eCharType.ePlayer);
		this.DebugLogReadWriteMismatch(msg, "onRemoveClient");
	}

	private void DebugLogReadWriteMismatch(NetIncomingMessage a_msg, string a_identifier)
	{
		int num = a_msg.LengthBytes - a_msg.PositionInBytes;
		if (num > 0)
		{
			Debug.Log(string.Concat(new object[]
			{
				"WRITE READ MISMATCH: ",
				a_identifier,
				" bytes left: ",
				num,
				" ",
				Time.time
			}));
		}
	}

	private void GetAndUpdateOwnPlayer(NetIncomingMessage a_msg)
	{
		byte b = a_msg.ReadByte();
		CharAnim2.ePose a_anim = ((b & 128) != 128) ? CharAnim2.ePose.eStand : CharAnim2.ePose.eAttack;
		float a_health = (float)(b & 127);
		byte b2 = a_msg.ReadByte();
		this.m_isInVehicle = (128 == (b2 & 128));
		float a_energy = (float)(b2 & 127);
		if (!this.m_isInVehicle)
		{
			Vector3 zero = Vector3.zero;
			zero.x = (float)a_msg.ReadInt16() * 0.1f;
			zero.z = (float)a_msg.ReadInt16() * 0.1f;
			float a_rot = (float)a_msg.ReadByte() / 255f * 360f;
			this.UpdateOrSpawnCharacter(this.m_myOnlineId, zero, a_rot, eCharType.ePlayer, a_anim, a_health, a_energy);
		}
		else if (this.IsSpawned())
		{
			this.m_players[this.m_myOnlineId].RefreshStatus(a_health, a_energy);
		}
	}

	private void GetAndUpdateCars(NetIncomingMessage a_msg)
	{
		while (a_msg.PositionInBytes < a_msg.LengthBytes)
		{
			if (!this.GetAndUpdateCar(a_msg))
			{
				break;
			}
		}
	}

	private bool GetAndUpdatePlayerOrNpc(NetIncomingMessage a_msg, bool a_isPlayer)
	{
		Vector3 zero = Vector3.zero;
		int num = (int)a_msg.ReadInt16();
		if (num == 32767)
		{
			return false;
		}
		zero.x = (float)a_msg.ReadInt16() * 0.1f;
		zero.z = (float)a_msg.ReadInt16() * 0.1f;
		float a_rot = (float)a_msg.ReadByte() / 255f * 360f;
		byte b = a_msg.ReadByte();
		int a_anim = ((b & 128) != 128) ? 0 : 1;
		float a_health = (float)(b & 127);
		this.UpdateOrSpawnCharacter(num, zero, a_rot, (!a_isPlayer) ? this.m_npcData[num].type : eCharType.ePlayer, (CharAnim2.ePose)a_anim, a_health, 100f);
		return true;
	}

	private bool GetAndUpdateCar(NetIncomingMessage a_msg)
	{
		bool flag = false;
		Vector3 zero = Vector3.zero;
		int num = (int)a_msg.ReadByte();
		if (num == 255)
		{
			return false;
		}
		zero.x = (float)a_msg.ReadInt16() * 0.1f;
		zero.z = (float)a_msg.ReadInt16() * 0.1f;
		float num2 = (float)a_msg.ReadByte() / 255f * 360f;
		byte b = a_msg.ReadByte();
		bool flag2 = 128 == (b & 128);
		float a_health = (float)(b & 127);
		for (int i = 0; i < 4; i++)
		{
			this.m_carData[num].passengerIds[i] = (int)(a_msg.ReadByte() - 1);
			flag |= (this.m_myOnlineId == this.m_carData[num].passengerIds[i]);
			if (-1 < this.m_carData[num].passengerIds[i])
			{
				this.UpdateOrSpawnCharacter(this.m_carData[num].passengerIds[i], zero + Quaternion.Euler(0f, num2, 0f) * this.m_carSeatOffsets[i], num2, (!flag2) ? eCharType.ePlayer : this.m_npcData[this.m_carData[num].passengerIds[i]].type, CharAnim2.ePose.eSit, 9999999f, 9999999f);
			}
		}
		this.UpdateOrSpawnCharacter(num, zero, num2, eCharType.eCar, CharAnim2.ePose.eStand, a_health, 100f);
		return true;
	}

	private bool GetAndUpdateItem(NetIncomingMessage a_msg, bool a_isInventory)
	{
		Vector3 zero = Vector3.zero;
		int num = (int)a_msg.ReadByte();
		if (num == 255)
		{
			return false;
		}
		zero.x = (float)a_msg.ReadInt16() * 0.1f;
		zero.z = (float)a_msg.ReadInt16() * 0.1f;
		int a_amount = (int)a_msg.ReadByte();
		this.UpdateOrSpawnItem(num, zero, a_amount, a_isInventory);
		return true;
	}

	private bool GetAndUpdateBuilding(NetIncomingMessage a_msg)
	{
		Vector3 zero = Vector3.zero;
		int num = (int)a_msg.ReadByte();
		if (num == 255)
		{
			return false;
		}
		zero.x = (float)a_msg.ReadInt16() * 0.1f;
		zero.z = (float)a_msg.ReadInt16() * 0.1f;
		int num2 = (int)a_msg.ReadByte();
		bool a_isMine = 128 == (num2 & 128);
		float a_health = Mathf.Clamp((float)(num2 >> 5 & 3) / 3f * 100f + 1f, 0f, 100f);
		float a_rot = (float)(num2 & 31) / 31f * 360f;
		this.UpdateOrSpawnBuilding(num, zero, a_rot, a_health, a_isMine);
		return true;
	}

	private void UpdateOrSpawnCharacter(int a_onlineId, Vector3 a_pos, float a_rot, eCharType a_type, CharAnim2.ePose a_anim, float a_health, float a_energy = 100f)
	{
		RemoteCharacter[] array;
		if (a_type == eCharType.ePlayer || a_type == eCharType.ePlayerFemale)
		{
			array = this.m_players;
		}
		else if (a_type == eCharType.eCar)
		{
			array = this.m_cars;
		}
		else
		{
			array = this.m_npcs;
		}
		if (a_onlineId > -1 && a_onlineId < array.Length)
		{
			bool flag = array != null && null == array[a_onlineId];
			if (!flag || 0f < a_health)
			{
				CharData[] array2 = null;
				if (a_type == eCharType.ePlayer || a_type == eCharType.ePlayerFemale)
				{
					array2 = this.m_playerData;
				}
				else if (a_type != eCharType.eCar)
				{
					array2 = this.m_npcData;
				}
				if (flag)
				{
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_remoteCharPrefab);
					array[a_onlineId] = gameObject.GetComponent<RemoteCharacter>();
					array[a_onlineId].Spawn(a_onlineId, a_type, (a_type == eCharType.ePlayer || a_type == eCharType.ePlayerFemale) && this.m_myOnlineId == a_onlineId);
					if (array2 != null)
					{
						array[a_onlineId].SetInfo(array2[a_onlineId]);
					}
					if (a_type == eCharType.ePlayer || a_type == eCharType.ePlayerFemale)
					{
						gameObject.name = string.Concat(new object[]
						{
							"player_",
							array2[a_onlineId].name,
							"_",
							a_onlineId
						});
						if (this.m_myOnlineId == a_onlineId)
						{
							BirdCam birdCam = (BirdCam)UnityEngine.Object.FindObjectOfType(typeof(BirdCam));
							birdCam.m_target = gameObject.transform;
							AudioListener audioListener = (AudioListener)UnityEngine.Object.FindObjectOfType(typeof(AudioListener));
							if (null != audioListener)
							{
								UnityEngine.Object.Destroy(audioListener);
							}
							gameObject.AddComponent<AudioListener>();
						}
					}
					else if (a_type != eCharType.eCar)
					{
						gameObject.name = "npc_" + a_onlineId;
					}
					else
					{
						gameObject.name = "car_" + a_onlineId;
					}
				}
				array[a_onlineId].Refresh(a_pos, a_rot, a_anim, a_health, a_energy);
			}
		}
	}

	private void UpdateOrSpawnItem(int a_type, Vector3 a_pos, int a_amount, bool a_isInventory)
	{
		string text = a_type.ToString() + a_pos.ToString();
		RemoteItem remoteItem = (!this.m_worldItems.Contains(text)) ? null : ((RemoteItem)this.m_worldItems[text]);
		if (a_isInventory || null == remoteItem)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_itemPrefab);
			gameObject.name = "item_" + text;
			remoteItem = gameObject.GetComponent<RemoteItem>();
			remoteItem.Init(a_pos, a_type, a_amount, a_isInventory);
			if (a_isInventory)
			{
				a_pos.x = Mathf.Round(a_pos.x);
				a_pos.z = Mathf.Round(a_pos.z);
				this.m_inventoryItems.Add(remoteItem);
			}
			else
			{
				this.m_worldItems[text] = remoteItem;
			}
		}
		remoteItem.Refresh();
	}

	private void UpdateOrSpawnBuilding(int a_type, Vector3 a_pos, float a_rot, float a_health, bool a_isMine)
	{
		if (0f < a_health)
		{
			string text = a_type.ToString() + a_pos.ToString();
			RemoteBuilding remoteBuilding = (!this.m_buildings.Contains(text)) ? null : ((RemoteBuilding)this.m_buildings[text]);
			if (null == remoteBuilding)
			{
				this.RemoveNullBuildingsFromList();
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_buildingPrefab);
				gameObject.name = "building_" + text;
				remoteBuilding = gameObject.GetComponent<RemoteBuilding>();
				remoteBuilding.Init(a_pos, a_type, a_isMine, false);
				this.m_buildings[text] = remoteBuilding;
			}
			remoteBuilding.Refresh(a_rot, a_health);
		}
	}

	private void RemoveNullBuildingsFromList()
	{
		foreach (object obj in this.m_buildings)
		{
			DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
			RemoteBuilding y = (RemoteBuilding)dictionaryEntry.Value;
			if (null == y)
			{
				this.m_buildings.Remove(dictionaryEntry.Key);
				break;
			}
		}
	}

	public Vector3 GetNearbyExplosion(Vector3 a_pos)
	{
		foreach (object obj in this.m_buildings)
		{
			RemoteBuilding remoteBuilding = (RemoteBuilding)((DictionaryEntry)obj).Value;
			if (null != remoteBuilding && remoteBuilding.IsExploding())
			{
				float sqrMagnitude = (a_pos - remoteBuilding.transform.position).sqrMagnitude;
				if (sqrMagnitude < 36f)
				{
					return remoteBuilding.transform.position;
				}
			}
		}
		return Vector3.zero;
	}

	public Mission GetMission(int a_index)
	{
		return (this.m_missions == null || this.m_missions.Count <= a_index || a_index <= -1) ? null : this.m_missions[a_index];
	}

	public RemoteItem GetNearestItem(Vector3 a_pos)
	{
		float num = 9999999f;
		RemoteItem result = null;
		foreach (object obj in this.m_worldItems)
		{
			RemoteItem remoteItem = (RemoteItem)((DictionaryEntry)obj).Value;
			if (null != remoteItem)
			{
				float sqrMagnitude = (a_pos - remoteItem.transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = remoteItem;
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	public RemoteBuilding GetNearestResource(Vector3 a_pos)
	{
		float num = 9999999f;
		RemoteBuilding result = null;
		foreach (object obj in this.m_buildings)
		{
			RemoteBuilding remoteBuilding = (RemoteBuilding)((DictionaryEntry)obj).Value;
			if (null != remoteBuilding && Buildings.IsResource(remoteBuilding.m_type))
			{
				float sqrMagnitude = (a_pos - remoteBuilding.transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = remoteBuilding;
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	public RemoteCharacter GetNearestNpc(Vector3 a_pos)
	{
		float num = 9999999f;
		RemoteCharacter result = null;
		for (int i = 0; i < this.m_npcs.Length; i++)
		{
			if (null != this.m_npcs[i] && this.m_npcs[i].IsNpc() && this.m_npcs[i].IsVisible())
			{
				float sqrMagnitude = (a_pos - this.m_npcs[i].transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = this.m_npcs[i];
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	public RemoteCharacter GetNearestCharacter(Vector3 a_pos, bool a_cars = false)
	{
		RemoteCharacter[] array = (!a_cars) ? this.m_players : this.m_cars;
		float num = 9999999f;
		RemoteCharacter result = null;
		for (int i = 0; i < array.Length; i++)
		{
			if (null != array[i] && array[i].IsVisible())
			{
				float sqrMagnitude = (a_pos - array[i].transform.position).sqrMagnitude;
				if (sqrMagnitude < num && sqrMagnitude > 0.01f)
				{
					result = array[i];
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	public bool IsSpawned()
	{
		return this.IsSpawned(this.m_myOnlineId);
	}

	public bool IsSpawned(int a_onlineId)
	{
		return this.m_players != null && this.m_players.Length > a_onlineId && a_onlineId > -1 && null != this.m_players[a_onlineId] && null != this.m_players[a_onlineId].transform;
	}

	public int GetRank()
	{
		return (-1 >= this.m_myOnlineId) ? 0 : this.m_playerData[this.m_myOnlineId].rank;
	}

	public int GetGoldCount()
	{
		return this.m_gold;
	}

	public bool IsInVehicle()
	{
		return this.m_isInVehicle;
	}

	public void ShowPartyFullPopup()
	{
		this.m_popupGui.ShowGui(true, LNG.Get("PARTY_POPUP_FULL"));
	}

	public bool IsValidBuildPos(Vector3 a_pos, int a_buildingType)
	{
		foreach (object obj in this.m_buildings)
		{
			RemoteBuilding remoteBuilding = (RemoteBuilding)((DictionaryEntry)obj).Value;
			if (null != remoteBuilding && !remoteBuilding.m_isStatic)
			{
				float sqrMagnitude = (remoteBuilding.transform.position - a_pos).sqrMagnitude;
				if ((!remoteBuilding.m_isMine && Buildings.IsDoor(remoteBuilding.m_type) && sqrMagnitude < 25f) || sqrMagnitude < 0.20249999f)
				{
					return false;
				}
			}
		}
		if (!Buildings.IsHarmless(a_buildingType))
		{
			foreach (SpecialArea specialArea in this.m_specialAreas)
			{
				if (null != specialArea && (specialArea.m_type == eAreaType.noBuilding || specialArea.m_type == eAreaType.noPvp) && (specialArea.transform.position - a_pos).sqrMagnitude < specialArea.m_radius * specialArea.m_radius)
				{
					return false;
				}
			}
		}
		return Buildings.IsDoor(a_buildingType) || false == Raycaster.BuildingSphereCast(a_pos);
	}

	public GameObject m_remoteCharPrefab;

	public GameObject m_itemPrefab;

	public GameObject m_buildingPrefab;

	[HideInInspector]
	public string m_disconnectMsg = string.Empty;

	[HideInInspector]
	public string m_notificationMsg = string.Empty;

	private NetClient m_client;

	private NetConnection m_serverCon;

	private string m_name = string.Empty;

	private string m_pwhash = string.Empty;

	private ulong m_id;

	private int m_myOnlineId = -1;

	private float m_rankProgress;

	private bool m_isInVehicle;

	private int m_condition;

	private int m_gold;

	private Hashtable m_worldItems = new Hashtable();

	private Hashtable m_buildings = new Hashtable();

	private List<RemoteItem> m_inventoryItems = new List<RemoteItem>();

	private List<Mission> m_missions;

	private ComChatGUI m_chat;

	private InventoryGUI m_inventory;

	private Hud m_hud;

	private MapGUI m_map;

	private PartyGUI m_partyGui;

	private PopupGUI m_popupGui;

	private int m_popupIdInvite = -1;

	private ClientInput m_clientInput;

	private RemoteCharacter[] m_npcs = new RemoteCharacter[1024];

	private RemoteCharacter[] m_players = new RemoteCharacter[50];

	private RemoteCharacter[] m_cars = new RemoteCharacter[15];

	private CharData[] m_npcData = new CharData[1024];

	[HideInInspector]
	public CharData[] m_playerData = new CharData[50];

	private CarData[] m_carData = new CarData[15];

	private SpecialArea[] m_specialAreas;

	private MissionObjective[] m_missionObjs;

	private Vector3[] m_carSeatOffsets = new Vector3[4];

	private float m_nextSecondUpdate;

	private int m_playerCount;
}

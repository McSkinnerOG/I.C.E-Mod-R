using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using UnityEngine;

public class LidServer : LidgrenPeer
{
	public LidServer()
	{
	}

	private void OnEnable()
	{
		Global.isServer = true;
		QualitySettings.SetQualityLevel(0);
		QualitySettings.vSyncCount = 0;
		this.m_sql = (SQLThreadManager)UnityEngine.Object.FindObjectOfType(typeof(SQLThreadManager));
		this.m_sql.enabled = true;
		Application.LoadLevel(1);
	}

	private void StartServer()
	{
		if (this.m_server == null)
		{
			this.m_server = new NetServer(new NetPeerConfiguration("immune")
			{
				Port = 8844,
				MaximumConnections = 50,
				ConnectionTimeout = 10f,
				PingInterval = 1f
			});
			this.m_server.Start();
			base.SetPeer(this.m_server);
			base.Connected += this.onConnected;
			base.Disconnected += this.onDisconnected;
			base.RegisterMessageHandler(MessageIds.Auth, new Action<NetIncomingMessage>(this.onAuth));
			base.RegisterMessageHandler(MessageIds.Input, new Action<NetIncomingMessage>(this.onInput));
			base.RegisterMessageHandler(MessageIds.Craft, new Action<NetIncomingMessage>(this.onCraft));
			base.RegisterMessageHandler(MessageIds.Chat, new Action<NetIncomingMessage>(this.onChat));
			base.RegisterMessageHandler(MessageIds.ChatLocal, new Action<NetIncomingMessage>(this.onChatLocal));
			base.RegisterMessageHandler(MessageIds.SpecialRequest, new Action<NetIncomingMessage>(this.onSpecialRequest));
			base.RegisterMessageHandler(MessageIds.SetLook, new Action<NetIncomingMessage>(this.onSetLook));
			base.RegisterMessageHandler(MessageIds.PartyControl, new Action<NetIncomingMessage>(this.onPartyControl));
		}
	}

	private void Init()
	{
		this.m_npcs = (ServerNpc[])UnityEngine.Object.FindObjectsOfType(typeof(ServerNpc));
		this.m_shopContainers = (ShopContainer[])UnityEngine.Object.FindObjectsOfType(typeof(ShopContainer));
		this.m_vehicles = (ServerVehicle[])UnityEngine.Object.FindObjectsOfType(typeof(ServerVehicle));
		this.m_spawnPoints = (SpawnPos[])UnityEngine.Object.FindObjectsOfType(typeof(SpawnPos));
		this.m_specialAreas = (SpecialArea[])UnityEngine.Object.FindObjectsOfType(typeof(SpecialArea));
		this.m_tutorial = (ServerTutorial)UnityEngine.Object.FindObjectOfType(typeof(ServerTutorial));
		this.m_repairNpcs = (RepairingNpc[])UnityEngine.Object.FindObjectsOfType(typeof(RepairingNpc));
		this.m_missionMan = (MissionManager)UnityEngine.Object.FindObjectOfType(typeof(MissionManager));
		this.m_buildingMan = (BuildingManager)UnityEngine.Object.FindObjectOfType(typeof(BuildingManager));
		Renderer[] array = (Renderer[])UnityEngine.Object.FindObjectsOfType(typeof(Renderer));
		foreach (Renderer renderer in array)
		{
			renderer.enabled = false;
		}
		SkinnedMeshRenderer[] array3 = (SkinnedMeshRenderer[])UnityEngine.Object.FindObjectsOfType(typeof(SkinnedMeshRenderer));
		foreach (SkinnedMeshRenderer renderer2 in array3)
		{
			renderer2.enabled = false;
		}
		this.m_staticBuildings = (ServerBuilding[])UnityEngine.Object.FindObjectsOfType(typeof(ServerBuilding));
		this.StartServer();
		this.m_maxPartyId = this.m_sql.GetMaxPartyId();
		this.m_sql.RequestBuildings();
		if (this.m_vehicles != null)
		{
			for (int k = 0; k < this.m_vehicles.Length; k++)
			{
				this.m_vehicles[k].m_id = k;
			}
		}
		if (Environment.CommandLine.Contains("-name"))
		{
			string[] array5 = Environment.CommandLine.Split(new char[]
			{
				'-'
			});
			for (int l = 0; l < array5.Length; l++)
			{
				if (array5[l].StartsWith("name"))
				{
					this.m_serverName = array5[l].Substring(4);
					break;
				}
			}
		}
	}

	private void OnApplicationQuit()
	{
		QualitySettings.SetQualityLevel(5);
		if (this.m_server != null)
		{
			if (string.Empty != this.m_serverName)
			{
				base.StartCoroutine(WebRequest.DeleteServer(this.m_server.Configuration.Port));
			}
			this.m_server.Shutdown("Server has shutdown.");
		}
	}

	private void Update()
	{
		if (Application.loadedLevel == 1)
		{
			if (!this.m_inited)
			{
				this.Init();
				this.m_inited = true;
			}
			this.m_dayNightCycle += Time.deltaTime * this.m_dayNightCycleSpeed;
			if (this.m_dayNightCycle > 1f)
			{
				this.m_dayNightCycle -= 1f;
			}
			this.HandleDatabaseAnswers();
			if (Time.time > this.m_nextItemUpdate)
			{
				this.UpdateItems();
				this.m_nextItemUpdate = Time.time + 1.17f;
			}
			this.UpdatePlayers();
			if (Time.time > this.m_nextPlayerDbWriteTime)
			{
				if (this.m_server.ConnectionsCount > 0)
				{
					this.m_sql.SavePlayers(this.m_players, this.m_server.ConnectionsCount);
				}
				this.m_nextPlayerDbWriteTime = Time.time + this.m_playerDbWriteIntervall;
			}
			if (Time.time > this.m_nextServerListUpdate)
			{
				if (string.Empty != this.m_serverName)
				{
					base.StartCoroutine(WebRequest.UpdateServer(this.m_server.Configuration.Port, this.m_serverName, this.m_server.ConnectionsCount));
				}
				this.m_nextServerListUpdate = Time.time + 60.34f;
			}
			if (Time.time > this.m_serverRestartTime)
			{
				Application.Quit();
				this.m_serverRestartTime = Time.time + 999f;
			}
			else if (Time.time > this.m_serverRestartTime - (float)this.m_restartMinutes * 60f)
			{
				this.SendNotification(LNG.Get("SERVER_RESTART_X_MINUTES").Replace("{1}", this.m_restartMinutes.ToString()));
				this.m_restartMinutes--;
			}
		}
	}

	private void onConnected(NetIncomingMessage a_msg)
	{
		a_msg.SenderConnection.Tag = -1;
	}

	private void onDisconnected(NetIncomingMessage a_msg)
	{
		bool flag = this.m_shutdownIfEmpty && 0 == this.m_server.ConnectionsCount;
		if (a_msg != null && a_msg.SenderConnection != null && a_msg.SenderConnection.Tag != null)
		{
			ServerPlayer player = this.GetPlayer((int)a_msg.SenderConnection.Tag);
			if (player != null)
			{
				if (Time.time > player.m_cantLogoutTime || flag)
				{
					this.DisconnectPlayer(player);
				}
				else
				{
					player.m_disconnectTime = player.m_cantLogoutTime;
				}
			}
		}
		if (flag)
		{
			Application.Quit();
		}
	}

	private void DisconnectPlayer(ServerPlayer a_player)
	{
		if (a_player != null)
		{
			if (a_player.m_partyId != 0 && (!this.m_partys.Contains(a_player.m_partyId) || 2 > ((List<DatabasePlayer>)this.m_partys[a_player.m_partyId]).Count))
			{
				a_player.m_partyId = 0;
				a_player.m_partyRank = 0;
				if (this.m_partys.Contains(a_player.m_partyId))
				{
					this.m_partys.Remove(a_player.m_partyId);
				}
			}
			this.m_sql.SavePlayer(a_player, false);
			ServerVehicle vehicle = a_player.GetVehicle();
			if (null != vehicle && !a_player.ExitVehicle(false))
			{
				vehicle.DestroyCarAndForceExitPassengers();
			}
			NetOutgoingMessage netOutgoingMessage = this.m_server.CreateMessage();
			netOutgoingMessage.Write(MessageIds.RemoveClient);
			netOutgoingMessage.Write((byte)a_player.m_onlineId);
			this.m_server.SendToAll(netOutgoingMessage, NetDeliveryMethod.ReliableOrdered);
			this.DeletePlayer(a_player.m_onlineId);
		}
	}

	private void onAuth(NetIncomingMessage msg)
	{
		string a_name = msg.ReadString();
		string a = msg.ReadString();
		ulong num = msg.ReadUInt64();
		string text = msg.ReadString();
		eCharType eCharType = (eCharType)msg.ReadByte();
		if (eCharType != eCharType.ePlayer && eCharType != eCharType.ePlayerFemale)
		{
			eCharType = eCharType.ePlayer;
		}
		if (text != "1.0.1")
		{
			msg.SenderConnection.Disconnect("Version conflict! Client version: " + text + " Server version: 1.0.1");
		}
		else if (0UL < num && a == Util.Md5(num + "Version_0_4_8_B"))
		{
			bool flag = false;
			for (int i = 0; i < this.m_players.Length; i++)
			{
				if (this.m_players[i] != null && num == this.m_players[i].m_accountId)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				msg.SenderConnection.Disconnect("Auth Problem: Error 2");
			}
			else
			{
				bool flag2 = false;
				for (int j = 0; j < this.m_players.Length; j++)
				{
					if (this.m_players[j] == null)
					{
						this.m_players[j] = new ServerPlayer(a_name, num, j, eCharType, msg.SenderConnection, this.m_sql, this, this.m_buildingMan, this.m_missionMan);
						flag2 = true;
						break;
					}
				}
				if (flag2)
				{
					this.m_sql.RequestPlayer(num);
				}
				else
				{
					Debug.Log("LidServer.cs: ERROR: Couldn't find free array element to place new player on?!");
					msg.SenderConnection.Disconnect("Server is full.");
				}
			}
		}
		else
		{
			msg.SenderConnection.Disconnect("Auth Problem: Error 1");
		}
	}

	private void onInput(NetIncomingMessage msg)
	{
		ServerPlayer player = this.GetPlayer((int)msg.SenderConnection.Tag);
		if (player != null && player.IsSpawned())
		{
			player.m_cantLogoutTime = Time.time + 0.5f;
			int num = msg.ReadInt32();
			int num2 = (int)msg.ReadInt16();
			float a_buildRotation = (float)msg.ReadByte() / 255f * 360f;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			if (msg.LengthBytes > 8)
			{
				zero.x = (float)msg.ReadByte();
				zero.z = (float)msg.ReadByte();
				zero2.x = (float)msg.ReadByte();
				zero2.z = (float)msg.ReadByte();
			}
			bool a_use = 1 == (num & 1);
			bool flag = 1 == (num >> 1 & 1);
			bool flag2 = 1 == (num >> 2 & 1);
			bool a_targetIsPlayer = 1 == (num >> 3 & 1);
			bool flag3 = 1 == (num >> 4 & 1);
			float num3 = (float)(num >> 8 & 255) * 0.01f - 1f;
			float num4 = (float)(num >> 16 & 255) * 0.01f - 1f;
			if (Mathf.Abs(num3) < 0.01f)
			{
				num3 = 0f;
			}
			if (Mathf.Abs(num4) < 0.01f)
			{
				num4 = 0f;
			}
			float a_attackRot = -1f;
			if (!flag2)
			{
				if (flag3)
				{
					a_attackRot = (float)num2 / 255f * 360f;
				}
				num2 = -1;
			}
			if ((num3 != 0f || num4 != 0f || flag) && (player.m_freeWorldContainer != null || player.m_persistentContainer != null || null != player.m_shopContainer))
			{
				player.ResetContainers();
			}
			player.AssignInput(num3, num4, a_use, flag, a_buildRotation);
			this.HandlePlayerInput(player, a_use, flag, a_targetIsPlayer, num2, a_attackRot, zero, zero2);
		}
	}

	private void onChat(NetIncomingMessage msg)
	{
		ServerPlayer player = this.GetPlayer((int)msg.SenderConnection.Tag);
		if (player != null && player.IsSpawned())
		{
			string text = msg.ReadString();
			if (string.Empty != text)
			{
				if (text.StartsWith(":#~"))
				{
					text = player.m_name + " just opened a case and received: \n<color=\"red\">" + text.Substring(3) + "</color>";
				}
				else
				{
					text = player.m_name + "§ " + text.Replace("<", string.Empty).Replace(">", string.Empty);
				}
				NetOutgoingMessage netOutgoingMessage = msg.SenderConnection.Peer.CreateMessage();
				netOutgoingMessage.Write(MessageIds.Chat);
				netOutgoingMessage.Write(text);
				this.m_server.SendToAll(netOutgoingMessage, NetDeliveryMethod.Unreliable);
			}
		}
	}
	public void SendMessageToPlayerLocal(string text, ServerPlayer player, NetIncomingMessage msg)
	{
		NetOutgoingMessage netOutgoingMessage = msg.SenderConnection.Peer.CreateMessage();
		netOutgoingMessage.Write(MessageIds.ChatLocal);
		netOutgoingMessage.Write((byte)player.m_onlineId);
		netOutgoingMessage.Write(text);
		m_server.SendMessage(netOutgoingMessage, player.m_connection, NetDeliveryMethod.Unreliable);
	}
	private void onChatLocal(NetIncomingMessage msg)
	{
		ServerPlayer player = this.GetPlayer((int)msg.SenderConnection.Tag);
		if (player != null && player.IsSpawned())
		{
			string text = msg.ReadString();
			if (string.Empty != text)
			{
				if (text.StartsWith("/"))
				{
					this.HandleChatCommand(text, player);
				}
				else
				{
					NetOutgoingMessage netOutgoingMessage = msg.SenderConnection.Peer.CreateMessage();
					netOutgoingMessage.Write(MessageIds.ChatLocal);
					netOutgoingMessage.Write((byte)player.m_onlineId);
					netOutgoingMessage.Write(text);
					this.m_server.SendToAll(netOutgoingMessage, NetDeliveryMethod.Unreliable);
				}
			}
		}
	}

	private void onSpecialRequest(NetIncomingMessage msg)
	{
		ServerPlayer player = this.GetPlayer((int)msg.SenderConnection.Tag);
		eSpecialRequest eSpecialRequest = (eSpecialRequest)msg.ReadByte();
		if (player != null && player.IsSpawned())
		{
			if (eSpecialRequest == eSpecialRequest.repairItem)
			{
				DatabaseItem itemFromPos = player.m_inventory.GetItemFromPos(0f, 0f);
				for (int i = 0; i < this.m_repairNpcs.Length; i++)
				{
					Vector3 position = this.m_repairNpcs[i].transform.position;
					Vector3 position2 = player.GetPosition();
					if (Mathf.Abs(position.x - position2.x) < 1.4f && Mathf.Abs(position.z - position2.z) < 1.4f && itemFromPos.type != 0 && Items.HasCondition(itemFromPos.type) && itemFromPos.amount < 100)
					{
						int num = (int)(1f + Items.GetValue(itemFromPos.type, 100) * 0.01f * (float)(100 - itemFromPos.amount));
						num = (int)((float)num * this.m_repairNpcs[i].m_priceMultip + 0.5f);
						if (num <= player.m_inventory.GetItemAmountByType(254))
						{
							player.m_inventory.DeclineItemAmountByType(254, num);
							player.m_inventory.RepairHandItem();
							player.m_updateContainersFlag = true;
							this.SendMoneyUpdate(player);
						}
						break;
					}
				}
			}
			else if (eSpecialRequest == eSpecialRequest.acceptMission)
			{
				this.m_missionMan.AcceptMission(player);
			}
			else if (eSpecialRequest == eSpecialRequest.solveMission)
			{
				this.m_missionMan.SolveMission(player, false);
			}
			else if (eSpecialRequest == eSpecialRequest.acceptPartyInvite && this.m_partys.Contains(player.m_partyInviteId))
			{
				List<DatabasePlayer> list = (List<DatabasePlayer>)this.m_partys[player.m_partyInviteId];
				if (5 > list.Count)
				{
					player.m_partyId = player.m_partyInviteId;
					player.m_partyInviteId = 0;
					player.m_partyRank = 0;
					this.m_sql.SavePlayer(player, true);
					list.Add(new DatabasePlayer(player.m_accountId, player.m_name, player.m_pid, 0f, 0f, 100, 100, 100, 0, 0, 0, 0, 0)
					{
						partyId = player.m_partyInviteId,
						partyRank = player.m_partyRank
					});
					this.UpdateParty(list);
				}
				else
				{
					this.SendPartyFeedback(player, ePartyFeedback.partyFull, string.Empty);
				}
			}
		}
	}

	private void onSetLook(NetIncomingMessage msg)
	{
		ServerPlayer player = this.GetPlayer((int)msg.SenderConnection.Tag);
		int num = (int)msg.ReadByte();
		string a = msg.ReadString();
		int num2 = (int)msg.ReadByte();
		string a2 = msg.ReadString();
		if (player != null && player.IsSpawned() && a == Util.GetItemDefHash(num, player.m_accountId) && a2 == Util.GetItemDefHash(num2, player.m_accountId))
		{
			player.m_lookIndex = num;
			player.m_skinIndex = num2;
			player.m_updateInfoFlag = true;
		}
	}

	private void onPartyControl(NetIncomingMessage msg)
	{
		ServerPlayer player = this.GetPlayer((int)msg.SenderConnection.Tag);
		ePartyControl ePartyControl = (ePartyControl)msg.ReadByte();
		ulong num = msg.ReadUInt64();
		if (ePartyControl == ePartyControl.invite)
		{
			if (player.m_partyId == 0)
			{
				player.m_partyId = ++this.m_maxPartyId;
				player.m_partyRank = 1;
				this.m_sql.SavePlayer(player, true);
				List<DatabasePlayer> list = new List<DatabasePlayer>(1);
				list.Add(new DatabasePlayer(player.m_accountId, string.Empty, 0, 0f, 0f, 100, 100, 100, 0, 0, 0, 0, 0)
				{
					pid = player.m_pid,
					name = player.m_name,
					partyId = player.m_partyId,
					partyRank = player.m_partyRank
				});
				this.m_partys[player.m_partyId] = list;
				this.SendPartyUpdate(player, list.ToArray());
			}
			ServerPlayer playerByAid = this.GetPlayerByAid(num);
			int partyId = playerByAid.m_partyId;
			if (playerByAid != null && (partyId == 0 || !this.m_partys.Contains(partyId) || 2 > ((List<DatabasePlayer>)this.m_partys[partyId]).Count))
			{
				playerByAid.m_partyInviteId = player.m_partyId;
				this.SendPartyFeedback(playerByAid, ePartyFeedback.invite, player.m_name);
			}
			else
			{
				this.SendPartyFeedback(player, ePartyFeedback.errorAlreadyInParty, playerByAid.m_name);
			}
		}
		else if (player != null && player.IsSpawned() && Time.time > player.m_nextPartyActionTime && player.m_partyId != 0 && this.m_partys.Contains(player.m_partyId) && (player.m_partyRank == 1 || (ePartyControl == ePartyControl.kick && player.m_accountId == num)))
		{
			player.m_nextPartyActionTime = Time.time + 0.5f;
			List<DatabasePlayer> list2 = (List<DatabasePlayer>)this.m_partys[player.m_partyId];
			for (int i = 0; i < list2.Count; i++)
			{
				if (num == list2[i].aid)
				{
					DatabasePlayer databasePlayer = list2[i];
					ServerPlayer playerByAid2 = this.GetPlayerByAid(databasePlayer.aid);
					if (ePartyControl == ePartyControl.kick)
					{
						databasePlayer.partyId = 0;
						databasePlayer.partyRank = 0;
						list2.RemoveAt(i);
						if (playerByAid2 != null)
						{
							playerByAid2.m_partyId = 0;
							playerByAid2.m_partyRank = 0;
							this.SendPartyUpdate(playerByAid2, null);
							if (player.m_accountId != num)
							{
								this.SendPartyFeedback(playerByAid2, ePartyFeedback.kicked, player.m_name);
							}
						}
					}
					else if (ePartyControl == ePartyControl.prodemote)
					{
						databasePlayer.partyRank = ((databasePlayer.partyRank != 0) ? 0 : 1);
						list2[i] = databasePlayer;
						if (playerByAid2 != null)
						{
							playerByAid2.m_partyRank = databasePlayer.partyRank;
							if (player.m_accountId != num)
							{
								this.SendPartyFeedback(playerByAid2, ePartyFeedback.prodemoted, player.m_name);
							}
						}
					}
					this.m_sql.SavePartyPlayer(databasePlayer);
					this.UpdateParty(list2);
					break;
				}
			}
		}
	}

	private void onCraft(NetIncomingMessage msg)
	{
		ServerPlayer player = this.GetPlayer((int)msg.SenderConnection.Tag);
		if (player != null && player.IsSpawned())
		{
			int num = player.m_inventory.CraftItem((int)msg.ReadByte(), (int)msg.ReadByte());
			player.AddXp((int)((float)num * 1.0001f));
			player.ResetContainers();
		}
	}

	private void HandleDatabaseAnswers()
	{
		DatabaseBuilding[] array = this.m_sql.PopRequestedBuildings();
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				this.m_buildingMan.CreateBuilding(array[i].type, array[i].GetPos(), array[i].pid, array[i].rot, array[i].health, false);
			}
		}
		DatabasePlayer[] array2 = this.m_sql.PopRequestedPlayers();
		if (array2 != null)
		{
			for (int j = 0; j < array2.Length; j++)
			{
				ServerPlayer a_player = this.SpawnPlayer(array2[j]);
				if (0 < array2[j].partyId)
				{
					if (this.m_partys.Contains(array2[j].partyId))
					{
						List<DatabasePlayer> list = (List<DatabasePlayer>)this.m_partys[array2[j].partyId];
						if (list != null && 0 < list.Count)
						{
							this.SendPartyUpdate(a_player, list.ToArray());
						}
					}
					else
					{
						this.m_sql.RequestParty(array2[j].partyId);
					}
				}
			}
		}
		array2 = this.m_sql.PopRequestedParty();
		if (array2 != null && 0 < array2.Length)
		{
			List<DatabasePlayer> list2 = new List<DatabasePlayer>(array2.Length);
			list2.AddRange(array2);
			this.m_partys[array2[0].partyId] = list2;
			this.UpdateParty(list2);
		}
		DatabaseItem[] array3 = this.m_sql.PopRequestedItems();
		if (array3 != null)
		{
			for (int k = 0; k < array3.Length; k++)
			{
				if (array3[k].cid == 0)
				{
					this.m_freeWorldItems.Add(array3[k]);
				}
				else if (this.m_sql.IsInventoryContainer(array3[k].cid))
				{
					for (int l = 0; l < this.m_players.Length; l++)
					{
						if (this.m_players[l] != null && array3[k].cid == this.m_sql.PidToCid(this.m_players[l].m_pid))
						{
							this.m_players[l].m_inventory.UpdateOrCreateItem(array3[k]);
							this.m_players[l].m_updateInfoFlag |= (array3[k].x < 1f);
							this.m_players[l].m_updateContainersFlag = true;
							break;
						}
					}
				}
				else
				{
					Lootbox lootbox = this.m_buildingMan.AddItemToLootContainer(array3[k]);
					if (null != lootbox)
					{
						for (int m = 0; m < this.m_players.Length; m++)
						{
							if (this.m_players[m] != null && this.m_players[m].m_persistentContainer == lootbox.m_container)
							{
								this.m_players[m].m_updateContainersFlag = true;
							}
						}
					}
				}
			}
		}
	}

	private void UpdateParty(List<DatabasePlayer> a_party)
	{
		if (a_party != null && 0 < a_party.Count)
		{
			int partyId = a_party[0].partyId;
			for (int i = 0; i < this.m_players.Length; i++)
			{
				if (this.m_players[i] != null && this.m_players[i].IsSpawned() && this.m_players[i].m_partyId == partyId)
				{
					this.SendPartyUpdate(this.m_players[i], a_party.ToArray());
				}
			}
		}
	}

	private void HandlePlayerInput(ServerPlayer a_player, bool a_use, bool a_attack, bool a_targetIsPlayer, int a_targetOnlineId, float a_attackRot, Vector3 a_dragPos, Vector3 a_dropPos)
	{
		Transform victim = null;
		if (!a_player.IsDead())
		{
			bool flag = a_use && a_player.HasUseChanged();
			if (null == a_player.GetVehicle())
			{
				if (flag && !this.TryEnterVehicle(a_player) && this.PickupItem(a_player, null).type == 0 && !this.OpenShopContainer(a_player) && !this.m_buildingMan.UseBuilding(a_player, a_player.GetPosition() + a_player.GetForward(), false) && !this.m_missionMan.RequestMission(a_player))
				{
					this.m_missionMan.SolveMission(a_player, true);
				}
				if (a_attack && a_targetOnlineId != -1)
				{
					if (a_targetIsPlayer)
					{
						ServerPlayer player = this.GetPlayer(a_targetOnlineId);
						victim = ((player == null || !player.IsSpawned()) ? null : player.GetTransform());
					}
					else
					{
						ServerNpc npc = this.GetNpc(a_targetOnlineId);
						victim = ((!(null != npc)) ? null : npc.transform);
					}
				}
			}
			else if (flag)
			{
				a_player.ExitVehicle(false);
			}
			if (a_dragPos != a_dropPos)
			{
				this.HandlePlayerDragDrop(a_player, a_dragPos, a_dropPos);
			}
		}
		a_player.SetVictim(victim);
		a_player.SetRotation(a_attackRot);
	}

	private void HandlePlayerDragDrop(ServerPlayer a_player, Vector3 a_dragPos, Vector3 a_dropPos)
	{
		bool flag = false;
		Vector3 vector = a_dragPos;
		Vector3 a_dropPos2 = a_dropPos;
		if ((a_dropPos - new Vector3(252f, 0f, 252f)).sqrMagnitude < 0.1f)
		{
			flag = this.BuySellItem(vector, a_player);
		}
		else
		{
			bool flag2 = (a_dropPos - new Vector3(253f, 0f, 253f)).sqrMagnitude < 0.1f;
			bool flag3 = (a_dropPos - new Vector3(254f, 0f, 254f)).sqrMagnitude < 0.1f;
			ItemContainer itemContainer = a_player.m_inventory;
			ItemContainer itemContainer2 = null;
			ItemContainer itemContainer3 = null;
			bool flag4 = false;
			if (a_player.m_persistentContainer != null)
			{
				itemContainer3 = a_player.m_persistentContainer;
				flag4 = false;
			}
			else if (a_player.m_freeWorldContainer != null)
			{
				itemContainer3 = a_player.m_freeWorldContainer;
				flag4 = true;
			}
			if (itemContainer3 != null)
			{
				if (a_dragPos.x > 5f)
				{
					itemContainer = itemContainer3;
					itemContainer2 = a_player.m_inventory;
				}
				else
				{
					itemContainer2 = itemContainer3;
				}
				for (int i = 0; i < this.m_players.Length; i++)
				{
					if (this.m_players[i] != null)
					{
						bool flag5 = (flag4 && this.m_players[i].m_freeWorldContainer == a_player.m_freeWorldContainer) || (!flag4 && this.m_players[i].m_persistentContainer == a_player.m_persistentContainer);
						if (flag5)
						{
							this.m_players[i].m_updateContainersFlag = true;
						}
					}
				}
			}
			if (flag3)
			{
				itemContainer.SplitItem(vector);
			}
			else if (flag2)
			{
				flag = itemContainer.EatItem(vector, a_player);
			}
			else
			{
				DatabaseItem item = itemContainer.DragDrop(vector, a_dropPos2, itemContainer2, a_player.GetPosition());
				if (0 < item.type)
				{
					this.m_freeWorldItems.Add(item);
				}
				else if (itemContainer2 != null)
				{
					this.SendMoneyUpdate(a_player);
				}
				flag = true;
			}
			if (flag4 && itemContainer3 != null && itemContainer3.m_items.Count < 1)
			{
				this.DeleteFreeWorldItem(itemContainer3.m_position, true);
			}
		}
		if (flag && (vector.x < 1f || a_dropPos2.x < 1f))
		{
			this.SendPlayerInfo(a_player);
		}
		a_player.m_updateContainersFlag = true;
	}

	private bool BuySellItem(Vector3 a_pos, ServerPlayer a_player)
	{
		bool flag = false;
		if (a_player != null && a_player.m_inventory != null && null != a_player.m_shopContainer)
		{
			ShopContainer shopContainer = a_player.m_shopContainer;
			if (null != shopContainer && shopContainer.m_container != null)
			{
				if (a_pos.x < 6f)
				{
					DatabaseItem itemFromPos = a_player.m_inventory.GetItemFromPos(a_pos.x, a_pos.z);
					if (0 < itemFromPos.type)
					{
						a_player.m_inventory.DeleteItem(a_pos.x, a_pos.z);
						int i = (int)(Items.GetValue(itemFromPos.type, itemFromPos.amount) * shopContainer.m_sellPriceMuliplier + 0.5f);
						while (i > 0)
						{
							int a_amount = (i <= 254) ? i : 254;
							i -= 254;
							DatabaseItem a_item = new DatabaseItem(254, 0f, 0f, a_amount, false, 0, 0);
							if (!a_player.m_inventory.CollectItem(a_item, true, default(Vector3)))
							{
								this.CreateFreeWorldItem(254, a_amount, a_player.GetPosition());
							}
						}
						if (shopContainer.m_container.HasFreeSlots())
						{
							shopContainer.m_container.CollectItem(itemFromPos, false, default(Vector3));
						}
						flag = true;
					}
				}
				else
				{
					DatabaseItem itemFromPos2 = shopContainer.m_container.GetItemFromPos(a_pos.x, a_pos.z);
					if (0 < itemFromPos2.type)
					{
						int num = (int)(Items.GetValue(itemFromPos2.type, itemFromPos2.amount) * shopContainer.m_buyPriceMuliplier + 0.5f);
						if (num <= a_player.m_inventory.GetItemAmountByType(254))
						{
							a_player.m_inventory.DeclineItemAmountByType(254, num);
							shopContainer.m_container.DeleteItem(a_pos.x, a_pos.z);
							if (!a_player.m_inventory.CollectItem(itemFromPos2, true, default(Vector3)))
							{
								this.CreateFreeWorldItem(itemFromPos2.type, itemFromPos2.amount, a_player.GetPosition());
							}
							flag = true;
						}
					}
				}
			}
		}
		if (flag)
		{
			this.SendMoneyUpdate(a_player);
		}
		return flag;
	}

	private void SendPlayerInfo(ServerPlayer a_player)
	{
		NetOutgoingMessage netOutgoingMessage = this.m_server.CreateMessage();
		netOutgoingMessage.Write(MessageIds.SetPlayerInfo);
		netOutgoingMessage.Write((byte)a_player.m_onlineId);
		this.AddPlayerInfoToMsg(netOutgoingMessage, a_player);
		this.m_server.SendToAll(netOutgoingMessage, NetDeliveryMethod.Unreliable);
	}

	private ServerPlayer SpawnPlayer(DatabasePlayer a_dbplayer)
	{
		ServerPlayer serverPlayer = null;
		for (int i = 0; i < this.m_players.Length; i++)
		{
			if (this.m_players[i] != null && this.m_players[i].m_accountId == a_dbplayer.aid)
			{
				serverPlayer = this.m_players[i];
				break;
			}
		}
		if (serverPlayer != null && serverPlayer.m_onlineId != -1)
		{
			serverPlayer.Spawn(this.m_controlledCharPrefab, a_dbplayer);
			this.SendPlayerAndNpcData(serverPlayer);
			this.m_missionMan.UpdatePlayer(serverPlayer);
			NetOutgoingMessage netOutgoingMessage = this.m_server.CreateMessage();
			netOutgoingMessage.Write(MessageIds.SetPlayerName);
			netOutgoingMessage.Write((byte)serverPlayer.m_onlineId);
			netOutgoingMessage.Write(serverPlayer.m_name);
			netOutgoingMessage.Write(serverPlayer.m_accountId);
			this.m_server.SendToAll(netOutgoingMessage, NetDeliveryMethod.ReliableOrdered);
			this.m_sql.RequestContainer(this.m_sql.PidToCid(serverPlayer.m_pid));
		}
		else
		{
			Debug.Log("LidServer.cs: Error: Can't init/spawn player because it's null or onlineId is -1.");
		}
		return serverPlayer;
	}

	private void UpdatePlayers()
	{
		for (int i = 0; i < this.m_players.Length; i++)
		{
			if (this.m_players[i] != null && this.m_players[i].IsSpawned())
			{
				if (0f < this.m_players[i].m_disconnectTime)
				{
					if (Time.time > this.m_players[i].m_disconnectTime)
					{
						this.DisconnectPlayer(this.m_players[i]);
					}
				}
				else
				{
					if (this.m_players[i].m_nextUpdate < Time.time)
					{
						this.m_players[i].Progress(this.m_updateIntervall);
						bool flag = 0 == ++this.m_players[i].m_updateCount % 2;
						NetOutgoingMessage netOutgoingMessage = this.m_players[i].m_connection.Peer.CreateMessage();
						netOutgoingMessage.Write((!flag) ? MessageIds.UpdateB : MessageIds.UpdateA);
						this.AddOwnPlayerToMsg(netOutgoingMessage, this.m_players[i]);
						this.AddVehiclesToMsg(netOutgoingMessage, this.m_players[i]);
						if (flag)
						{
							this.AddPlayersToMsg(netOutgoingMessage, this.m_players[i]);
						}
						else
						{
							this.UpdatePlayerInventory(this.m_players[i]);
							this.AddNpcsItemsBuildingsToMsg(netOutgoingMessage, this.m_players[i]);
						}
						this.m_players[i].m_connection.SendMessage(netOutgoingMessage, NetDeliveryMethod.Unreliable, 0);
						this.m_players[i].m_nextUpdate = Time.time + this.m_updateIntervall;
					}
					if (this.m_players[i].m_updateInfoFlag)
					{
						this.m_players[i].m_updateInfoFlag = false;
						this.SendPlayerInfo(this.m_players[i]);
					}
				}
			}
		}
	}

	private void UpdatePlayerInventory(ServerPlayer a_player)
	{
		if (a_player == null || a_player.m_inventory == null)
		{
			return;
		}
		for (int i = 0; i < a_player.m_inventory.m_items.Count; i++)
		{
			if (a_player.m_inventory.m_items[i].flag == eDbAction.delete || a_player.m_inventory.m_items[i].flag == eDbAction.update)
			{
				a_player.m_updateContainersFlag = true;
				this.m_sql.SaveItem(a_player.m_inventory.m_items[i]);
				if (a_player.m_inventory.m_items[i].flag == eDbAction.delete)
				{
					a_player.m_inventory.m_items.RemoveAt(i);
					break;
				}
				DatabaseItem value = a_player.m_inventory.m_items[i];
				value.flag = eDbAction.none;
				a_player.m_inventory.m_items[i] = value;
			}
		}
	}

	private void UpdateItems()
	{
		float time = Time.time;
		for (int i = 0; i < this.m_freeWorldItems.Count; i++)
		{
			if (time > this.m_freeWorldItems[i].dropTime + 6f && this.m_freeWorldItems[i].amount == 1)
			{
				int num = -1;
				if (this.m_freeWorldItems[i].type == 2)
				{
					num = 60;
				}
				if (this.m_freeWorldItems[i].type == 1)
				{
					num = 61;
				}
				if (num != -1 && this.m_buildingMan.CreateBuilding(num, this.m_freeWorldItems[i].GetPos(), 0, 0f, 100, true))
				{
					this.DeleteFreeWorldItem(i);
					i--;
				}
			}
		}
	}

	private void AddVehiclesToMsg(NetOutgoingMessage a_msg, ServerPlayer a_player)
	{
		Vector3 position = a_player.GetPosition();
		for (int i = 0; i < this.m_vehicles.Length; i++)
		{
			if (null != this.m_vehicles[i] && Mathf.Abs(this.m_vehicles[i].transform.position.x - position.x) < 22f && Mathf.Abs(this.m_vehicles[i].transform.position.z - position.z) < 19f)
			{
				this.AddVehicleToMsg(a_msg, i, this.m_vehicles[i]);
			}
		}
		a_msg.Write(byte.MaxValue);
	}

	private void AddPlayersToMsg(NetOutgoingMessage a_msg, ServerPlayer a_player)
	{
		if (a_player != null && a_player.m_connection != null)
		{
			Vector3 position = a_player.GetPosition();
			for (int i = 0; i < this.m_players.Length; i++)
			{
				if (this.m_players[i] != null && this.m_players[i].IsSpawned() && null == this.m_players[i].GetVehicle() && a_player.m_pid != this.m_players[i].m_pid && this.m_players[i].m_onlineId != -1 && Mathf.Abs(this.m_players[i].GetPosition().x - position.x) < 22f && Mathf.Abs(this.m_players[i].GetPosition().z - position.z) < 19f)
				{
					this.AddPlayerOrNpcToMsg(a_msg, this.m_players[i].m_onlineId, this.m_players[i].GetPosition().x, this.m_players[i].GetPosition().z, this.m_players[i].GetRotation(), this.m_players[i].IsAttacking(), this.m_players[i].GetHealth());
				}
			}
		}
		a_msg.Write(short.MaxValue);
	}

	private void AddNpcsItemsBuildingsToMsg(NetOutgoingMessage a_msg, ServerPlayer a_player)
	{
		if (a_player != null && a_player.m_connection != null)
		{
			Vector3 position = a_player.GetPosition();
			for (int i = 0; i < this.m_npcs.Length; i++)
			{
				if (null != this.m_npcs[i] && Mathf.Abs(this.m_npcs[i].transform.position.x - position.x) < 22f && Mathf.Abs(this.m_npcs[i].transform.position.z - position.z) < 19f)
				{
					this.AddPlayerOrNpcToMsg(a_msg, i, this.m_npcs[i].transform.position.x, this.m_npcs[i].transform.position.z, this.m_npcs[i].transform.rotation.eulerAngles.y, eBodyBaseState.attacking == this.m_npcs[i].GetBodyState(), this.m_npcs[i].GetHealth());
				}
			}
			a_msg.Write(short.MaxValue);
			for (int j = 0; j < this.m_buildingMan.m_buildings.Count; j++)
			{
				if (null != this.m_buildingMan.m_buildings[j] && Mathf.Abs(this.m_buildingMan.m_buildings[j].transform.position.x - position.x) < 22f && Mathf.Abs(this.m_buildingMan.m_buildings[j].transform.position.z - position.z) < 19f)
				{
					this.AddBuildingToMsg(a_msg, this.m_buildingMan.m_buildings[j], a_player.m_pid == this.m_buildingMan.m_buildings[j].GetOwnerId());
				}
			}
			a_msg.Write(byte.MaxValue);
			for (int k = 0; k < this.m_freeWorldItems.Count; k++)
			{
				if (Mathf.Abs(this.m_freeWorldItems[k].x - position.x) < 22f && Mathf.Abs(this.m_freeWorldItems[k].y - position.z) < 19f)
				{
					this.AddItemToMsg(a_msg, this.m_freeWorldItems[k]);
				}
			}
			a_msg.Write(byte.MaxValue);
			this.AddRequestedItemsToMsg(a_msg, a_player);
			a_msg.Write(byte.MaxValue);
		}
	}

	private void AddRequestedItemsToMsg(NetOutgoingMessage a_msg, ServerPlayer a_player)
	{
		bool flag = false;
		if (a_player.m_updateContainersFlag)
		{
			for (int i = 0; i < a_player.m_inventory.m_items.Count; i++)
			{
				if (0 < a_player.m_inventory.m_items[i].iid)
				{
					this.AddItemToMsg(a_msg, a_player.m_inventory.m_items[i]);
					flag = true;
				}
			}
			ItemContainer itemContainer = null;
			if (a_player.m_freeWorldContainer != null)
			{
				itemContainer = a_player.m_freeWorldContainer;
			}
			else if (a_player.m_persistentContainer != null)
			{
				itemContainer = a_player.m_persistentContainer;
			}
			else if (null != a_player.m_shopContainer)
			{
				itemContainer = a_player.m_shopContainer.m_container;
			}
			if (itemContainer != null)
			{
				if (0 < itemContainer.m_items.Count)
				{
					for (int j = 0; j < itemContainer.m_items.Count; j++)
					{
						this.AddItemToMsg(a_msg, itemContainer.m_items[j]);
					}
				}
				else
				{
					DatabaseItem a_item = new DatabaseItem(0, 0f, 0f, 1, false, 0, 0);
					a_item.x += 6f;
					this.AddItemToMsg(a_msg, a_item);
				}
				flag = true;
			}
			if (!flag)
			{
				DatabaseItem a_item2 = new DatabaseItem(0, 0f, 0f, 1, false, 0, 0);
				this.AddItemToMsg(a_msg, a_item2);
			}
			a_player.m_updateContainersFlag = false;
		}
	}

	private void AddOwnPlayerToMsg(NetOutgoingMessage a_msg, ServerPlayer a_player)
	{
		int num = ((!a_player.IsAttacking()) ? 0 : 128) | ((int)a_player.GetHealth() & 127);
		int num2 = ((!(null != a_player.GetVehicle())) ? 0 : 128) | ((int)a_player.GetEnergy() & 127);
		a_msg.Write((byte)num);
		a_msg.Write((byte)num2);
		if (null == a_player.GetVehicle())
		{
			a_msg.Write((short)(a_player.GetPosition().x * 10.00001f));
			a_msg.Write((short)(a_player.GetPosition().z * 10.00001f));
			a_msg.Write((byte)(a_player.GetRotation() / 360f * 255f));
		}
	}

	private void AddPlayerOrNpcToMsg(NetOutgoingMessage a_msg, int a_id, float a_x, float a_z, float a_rotation, bool a_isAttacking, float a_health)
	{
		int num = ((!a_isAttacking) ? 0 : 128) | ((int)a_health & 127);
		a_msg.Write((short)a_id);
		a_msg.Write((short)(a_x * 10.00001f));
		a_msg.Write((short)(a_z * 10.00001f));
		a_msg.Write((byte)(a_rotation / 360f * 255f));
		a_msg.Write((byte)num);
	}

	private void AddItemToMsg(NetOutgoingMessage a_msg, DatabaseItem a_item)
	{
		a_msg.Write((byte)a_item.type);
		a_msg.Write((short)(a_item.x * 10.00001f));
		a_msg.Write((short)(a_item.y * 10.00001f));
		a_msg.Write((byte)a_item.amount);
	}

	private void AddBuildingToMsg(NetOutgoingMessage a_msg, ServerBuilding a_building, bool a_isPlayersBuilding)
	{
		float num = a_building.transform.rotation.eulerAngles.y / 360f;
		int num2 = ((int)(a_building.GetState() * 3f + 0.5f) << 5 & 96) | ((int)(num * 31f) & 31);
		if (a_isPlayersBuilding)
		{
			num2 |= 128;
		}
		a_msg.Write((byte)a_building.m_type);
		a_msg.Write((short)(a_building.transform.position.x * 10.00001f));
		a_msg.Write((short)(a_building.transform.position.z * 10.00001f));
		a_msg.Write((byte)num2);
	}

	private void AddVehicleToMsg(NetOutgoingMessage a_msg, int a_id, ServerVehicle a_vehicle)
	{
		int num = ((!a_vehicle.IsNpcControlled()) ? 0 : 128) | ((int)a_vehicle.GetHealth() & 127);
		a_msg.Write((byte)a_id);
		a_msg.Write((short)(a_vehicle.transform.position.x * 10.00001f));
		a_msg.Write((short)(a_vehicle.transform.position.z * 10.00001f));
		a_msg.Write((byte)(a_vehicle.transform.rotation.eulerAngles.y / 360f * 255f));
		a_msg.Write((byte)num);
		for (int i = 0; i < 4; i++)
		{
			a_msg.Write((byte)(a_vehicle.m_data.passengerIds[i] + 1));
		}
	}

	private void AddPlayerInfoToMsg(NetOutgoingMessage a_msg, ServerPlayer a_player)
	{
		int num = (a_player.m_inventory == null) ? 0 : a_player.m_inventory.GetItemFromPos(0f, 0f).type;
		int lookIndex = a_player.m_lookIndex;
		int skinIndex = a_player.m_skinIndex;
		int num2 = (a_player.m_inventory == null) ? 0 : a_player.m_inventory.GetItemFromPos(0f, 2f).type;
		a_msg.Write((byte)num);
		a_msg.Write((byte)lookIndex);
		a_msg.Write((byte)skinIndex);
		a_msg.Write((byte)num2);
		a_msg.Write((byte)a_player.GetRank());
		a_msg.Write((byte)a_player.GetKarma());
		a_msg.Write((byte)a_player.m_charType);
	}

	private ServerNpc GetNpc(int a_onlineId)
	{
		if (a_onlineId < 0 || a_onlineId > this.m_npcs.Length)
		{
			return null;
		}
		return this.m_npcs[a_onlineId];
	}

	private ServerPlayer GetPlayer(int a_onlineId)
	{
		if (a_onlineId < 0 || a_onlineId > this.m_players.Length)
		{
			return null;
		}
		return this.m_players[a_onlineId];
	}

	private void DeletePlayer(int a_onlineId)
	{
		if (a_onlineId >= 0 && a_onlineId < this.m_players.Length)
		{
			this.m_players[a_onlineId].Remove();
			this.m_players[a_onlineId] = null;
		}
	}

	private void DeleteFreeWorldItem(Vector3 a_pos, bool a_containerOnly = false)
	{
		for (int i = 0; i < this.m_freeWorldItems.Count; i++)
		{
			if ((this.m_freeWorldItems[i].GetPos() - a_pos).sqrMagnitude < 0.1f && (!a_containerOnly || Items.IsContainer(this.m_freeWorldItems[i].type)))
			{
				this.DeleteFreeWorldItem(i);
				break;
			}
		}
	}

	private void DeleteFreeWorldItem(int a_index)
	{
		if (Items.IsContainer(this.m_freeWorldItems[a_index].type))
		{
			string key = this.m_freeWorldItems[a_index].GetPos().ToString();
			ItemContainer itemContainer = (ItemContainer)this.m_freeWorldContainers[key];
			for (int i = 0; i < this.m_players.Length; i++)
			{
				if (this.m_players[i] != null && itemContainer != null && this.m_players[i].m_freeWorldContainer == itemContainer)
				{
					this.m_players[i].ResetContainers();
				}
			}
			this.m_freeWorldContainers.Remove(key);
		}
		this.m_freeWorldItems.RemoveAt(a_index);
	}

	private void SendPlayerAndNpcData(ServerPlayer a_player)
	{
		NetOutgoingMessage netOutgoingMessage = a_player.m_connection.Peer.CreateMessage();
		netOutgoingMessage.Write(MessageIds.Init);
		netOutgoingMessage.Write((byte)a_player.m_onlineId);
		netOutgoingMessage.Write((byte)(a_player.GetRankProgress() * 255f));
		netOutgoingMessage.Write(a_player.m_gold);
		netOutgoingMessage.Write(this.m_dayNightCycle);
		netOutgoingMessage.Write(this.m_dayNightCycleSpeed);
		int num = 0;
		for (int i = 0; i < this.m_server.ConnectionsCount; i++)
		{
			int a_id = (int)this.m_server.Connections[i].Tag;
			if (this.IsValidPlayer(a_id))
			{
				num++;
			}
		}
		netOutgoingMessage.Write((byte)num);
		for (int j = 0; j < this.m_server.ConnectionsCount; j++)
		{
			int num2 = (int)this.m_server.Connections[j].Tag;
			if (this.IsValidPlayer(num2))
			{
				netOutgoingMessage.Write((byte)num2);
				netOutgoingMessage.Write(this.m_players[num2].m_name);
				netOutgoingMessage.Write(this.m_players[num2].m_accountId);
				this.AddPlayerInfoToMsg(netOutgoingMessage, this.m_players[num2]);
			}
		}
		netOutgoingMessage.Write((short)this.m_npcs.Length);
		for (int k = 0; k < this.m_npcs.Length; k++)
		{
			if (null != this.m_npcs[k])
			{
				netOutgoingMessage.Write((byte)this.m_npcs[k].GetHandItem());
				netOutgoingMessage.Write((byte)this.m_npcs[k].GetLookItem());
				netOutgoingMessage.Write((byte)this.m_npcs[k].GetBodyItem());
				netOutgoingMessage.Write((byte)this.m_npcs[k].m_npcType);
			}
		}
		if (this.m_staticBuildings != null && 0 < this.m_staticBuildings.Length)
		{
			for (int l = 0; l < this.m_staticBuildings.Length; l++)
			{
				if (null != this.m_staticBuildings[l] && this.m_staticBuildings[l].m_type > 0 && this.m_staticBuildings[l].m_type < 255)
				{
					this.AddBuildingToMsg(netOutgoingMessage, this.m_staticBuildings[l], false);
				}
			}
		}
		netOutgoingMessage.Write(byte.MaxValue);
		a_player.m_connection.SendMessage(netOutgoingMessage, NetDeliveryMethod.ReliableOrdered, 1);
	}

	private bool IsValidPlayer(int a_id)
	{
		return -1 < a_id && a_id < this.m_players.Count<ServerPlayer>() && null != this.m_players[a_id];
	}

	private void HandleChatCommand(string a_chatText, ServerPlayer a_player)
	{
		string[] array = a_chatText.Split(new char[]
		{
			' '
		});
		if ("/suicide" == array[0] || "/kill" == array[0])
		{
			a_player.ChangeHealthBy(-10000f);
		}
		else if ("/login" == array[0] && array.Length > 1 && ConfigFile.GetVar("adminpassword", "4544") == array[1])
		{
			a_player.m_isAdmin = true;
			Debug.Log(string.Concat(new object[]
			{
				a_player.m_name,
				" (Steam ID: ",
				a_player.m_accountId,
				") just logged in as admin"
			}));
		}
		else if ("/dropgold" == array[0] && array.Length > 1)
		{
			int itemAmountByType = a_player.m_inventory.GetItemAmountByType(254);
			if (0 < itemAmountByType)
			{
				int num = 0;
				try
				{
					num = int.Parse(array[1]);
				}
				catch (Exception ex)
				{
					num = 0;
				}
				if (0 < num)
				{
					num = Mathf.Min(itemAmountByType, num);
					a_player.m_inventory.DeclineItemAmountByType(254, num);
					this.CreateFreeWorldItem(254, num, a_player.GetPosition());
					this.SendMoneyUpdate(a_player);
				}
			}
		}
		else if ("/char" == array[0] && array.Length > 1)
		{
			eCharType eCharType = eCharType.ePlayer;
			try
			{
				eCharType = (eCharType)int.Parse(array[1]);
			}
			catch (Exception ex2)
			{
				eCharType = eCharType.ePlayer;
			}
			if ((a_player.m_isAdmin || eCharType == eCharType.ePlayer || eCharType == eCharType.ePlayerFemale) && a_player.m_charType != eCharType)
			{
				a_player.m_charType = eCharType;
				a_player.m_updateInfoFlag = true;
			}
		}
		if (a_player.m_isAdmin)
		{
			if ("/stats" == array[0])
			{
				NetOutgoingMessage netOutgoingMessage = a_player.m_connection.Peer.CreateMessage();
				netOutgoingMessage.Write(MessageIds.Chat);
				netOutgoingMessage.Write(string.Concat(new object[]
				{
					"[SERVER]: fps_cur: ",
					(int)(1f / Time.smoothDeltaTime),
					" fps_alltime: ",
					(int)((float)Time.frameCount / Time.time),
					" buildings: ",
					this.m_buildingMan.m_buildings.Count,
					" fwitems: ",
					this.m_freeWorldItems.Count,
					" fwicontainers: ",
					this.m_freeWorldContainers.Count
				}));
				a_player.m_connection.SendMessage(netOutgoingMessage, NetDeliveryMethod.ReliableOrdered, 1);
			}
			else if ("/airdrop" == array[0])
			{
				base.Invoke("CreateAirdrop", 3f);
			}
			else if ("/shutdown" == array[0])
			{
				this.m_serverRestartTime = Time.time + 300f;
			}
			else if ("/shutdownnow" == array[0])
			{
				this.m_serverRestartTime = Time.time;
			}
			else if ("/message" == array[0])
			{
				this.SendNotification(a_chatText.Substring(8));
			}
			else if ("/addxp" == array[0] && array.Length > 1)
			{
				int a_xp = 0;
				try
				{
					a_xp = int.Parse(array[1]);
				}
				catch (Exception ex3)
				{
					a_xp = 0;
				}
				a_player.AddXp(a_xp);
			}
			else if ("/addkarma" == array[0] && array.Length > 1)
			{
				int num2 = 0;
				try
				{
					num2 = int.Parse(array[1]);
				}
				catch (Exception ex4)
				{
					num2 = 0;
				}
				a_player.ChangeKarmaBy((float)num2);
			}
			else if ("/addhp" == array[0] && array.Length > 1)
			{
				int num3 = 0;
				try
				{
					num3 = int.Parse(array[1]);
				}
				catch (Exception ex5)
				{
					num3 = 0;
				}
				a_player.ChangeHealthBy((float)num3);
			}
			else if ("/addenergy" == array[0] && array.Length > 1)
			{
				int num4 = 0;
				try
				{
					num4 = int.Parse(array[1]);
				}
				catch (Exception ex6)
				{
					num4 = 0;
				}
				a_player.ChangeEnergyBy((float)num4);
			}
			else if ("/setcondition" == array[0] && array.Length > 1)
			{
				int conditions = 0;
				try
				{
					conditions = int.Parse(array[1]);
				}
				catch (Exception ex7)
				{
					conditions = 0;
				}
				a_player.SetConditions(conditions);
			}
			else if ("/drop" == array[0] && array.Length > 1)
			{
				int num5 = 0;
				int a_amount = 1;
				try
				{
					num5 = int.Parse(array[1]);
					a_amount = ((array.Length <= 2) ? 1 : int.Parse(array[2]));
				}
				catch (Exception ex8)
				{
					num5 = 0;
					a_amount = 1;
				}
				if (num5 != 0 && Items.GetItemDef(num5).ident != null)
				{
					this.CreateFreeWorldItem(num5, a_amount, a_player.GetPosition());
				}
			}
			else if ("/teleport" == array[0] && array.Length > 2)
			{
				int num6 = 0;
				int num7 = 0;
				try
				{
					num6 = int.Parse(array[1]);
					num7 = int.Parse(array[2]);
				}
				catch (Exception ex9)
				{
				}
				if (num6 != 0 && num7 != 0)
				{
					a_player.SetPosition(new Vector3((float)num6, 0f, (float)num7));
				}
			}
		}
	}

	private void CreateAirdrop()
	{
		int a_containerType = 121;
		Vector3 a_pos = new Vector3(UnityEngine.Random.Range(-345f, -335f), 0f, UnityEngine.Random.Range(-240f, -230f));
		int num = UnityEngine.Random.Range(60, 66);
		this.CreateTempContainerItem(num, 1, a_pos, a_containerType);
		int ammoItemType = Items.GetItemDef(num).ammoItemType;
		this.CreateTempContainerItem(ammoItemType, 100, a_pos, a_containerType);
		int a_newItemType = UnityEngine.Random.Range(90, 94);
		this.CreateTempContainerItem(a_newItemType, 1, a_pos, a_containerType);
		int a_newItemType2 = UnityEngine.Random.Range(210, 220);
		this.CreateTempContainerItem(a_newItemType2, 1, a_pos, a_containerType);
		this.CreateTempContainerItem(2, 10, a_pos, a_containerType);
		this.CreateTempContainerItem(3, 10, a_pos, a_containerType);
		this.CreateTempContainerItem(130, 200, a_pos, a_containerType);
		this.CreateTempContainerItem(131, 200, a_pos, a_containerType);
		this.CreateTempContainerItem(132, 200, a_pos, a_containerType);
		this.CreateTempContainerItem(133, 200, a_pos, a_containerType);
	}

	private bool OpenShopContainer(ServerPlayer a_player)
	{
		if (a_player != null && this.m_shopContainers != null)
		{
			Vector3 vector = Vector3.zero;
			for (int i = 0; i < this.m_shopContainers.Length; i++)
			{
				vector = this.m_shopContainers[i].transform.position;
				if (Mathf.Abs(vector.x - a_player.GetPosition().x) < 1.4f && Mathf.Abs(vector.z - a_player.GetPosition().z) < 1.4f)
				{
					a_player.m_shopContainer = this.m_shopContainers[i];
					a_player.m_updateContainersFlag = true;
					this.SendShopInfo(a_player, this.m_shopContainers[i].m_buyPriceMuliplier, this.m_shopContainers[i].m_sellPriceMuliplier);
					return true;
				}
			}
		}
		return false;
	}

	private bool TryEnterVehicle(ServerPlayer a_player)
	{
		bool result = false;
		if (a_player != null && a_player.CanEnterExitVehicle() && this.m_vehicles != null && null == a_player.GetVehicle() && a_player.m_onlineId != -1)
		{
			Vector3 vector = Vector3.zero;
			for (int i = 0; i < this.m_vehicles.Length; i++)
			{
				vector = this.m_vehicles[i].transform.position;
				if (Mathf.Abs(vector.x - a_player.GetPosition().x) < 2.5f && Mathf.Abs(vector.z - a_player.GetPosition().z) < 2.5f && this.m_vehicles[i].AddPassenger(a_player.m_onlineId))
				{
					result = a_player.SetVehicle(this.m_vehicles[i]);
					break;
				}
			}
		}
		return result;
	}

	public void Dig(Vector3 a_pos)
	{
		this.m_sql.RequestHiddenItems(a_pos);
		for (int i = 0; i < this.m_freeWorldItems.Count; i++)
		{
			if (!Items.IsContainer(this.m_freeWorldItems[i].type) && Mathf.Abs(this.m_freeWorldItems[i].x - a_pos.x) < 0.5f && Mathf.Abs(this.m_freeWorldItems[i].y - a_pos.z) < 0.5f)
			{
				DatabaseItem a_item = this.m_freeWorldItems[i];
				a_item.hidden = true;
				a_item.flag = eDbAction.insert;
				this.m_sql.SaveItem(a_item);
				this.DeleteFreeWorldItem(i);
				i--;
			}
		}
	}

	public void CreateFreeWorldItem(int a_newItemType, int a_amount, Vector3 a_pos) // need to mirror this method for entites possible /summon /spawn commands etc.
	{
		if (Items.IsContainer(a_newItemType))
		{
			return;
		}
		bool flag = false;
		if (Items.IsStackable(a_newItemType))
		{
			for (int i = 0; i < m_freeWorldItems.Count; i++)
			{
				DatabaseItem databaseItem = m_freeWorldItems[i];
				if (a_newItemType != databaseItem.type)
				{
					continue;
				}
				DatabaseItem databaseItem2 = m_freeWorldItems[i];
				if (databaseItem2.amount + a_amount > 254)
				{
					continue;
				}
				DatabaseItem databaseItem3 = m_freeWorldItems[i];
				if (Mathf.Abs(databaseItem3.x - a_pos.x) < 0.2f)
				{
					DatabaseItem databaseItem4 = m_freeWorldItems[i];
					if (Mathf.Abs(databaseItem4.y - a_pos.z) < 0.2f)
					{
						DatabaseItem value = m_freeWorldItems[i];
						value.amount += a_amount;
						m_freeWorldItems[i] = value;
						flag = true;
						break;
					}
				}
			}
		}
		if (!flag)
		{
			m_freeWorldItems.Add(new DatabaseItem(a_newItemType, a_pos.x, a_pos.z, a_amount));
		}
	}

	public void CreateFreeWorldItem(int a_newItemType, int a_amount, Vector3 a_pos, int quality)
	{
		if (Items.IsContainer(a_newItemType))
		{
			return;
		}
		bool flag = false;
		if (Items.IsStackable(a_newItemType))
		{
			for (int i = 0; i < m_freeWorldItems.Count; i++)
			{
				DatabaseItem databaseItem = m_freeWorldItems[i];
				if (a_newItemType != databaseItem.type)
				{
					continue;
				}
				DatabaseItem databaseItem2 = m_freeWorldItems[i];
				if (databaseItem2.amount + a_amount > 254)
				{
					continue;
				}
				DatabaseItem databaseItem3 = m_freeWorldItems[i];
				if (Mathf.Abs(databaseItem3.x - a_pos.x) < 0.2f)
				{
					DatabaseItem databaseItem4 = m_freeWorldItems[i];
					if (Mathf.Abs(databaseItem4.y - a_pos.z) < 0.2f)
					{
						DatabaseItem value = m_freeWorldItems[i];
						value.amount += a_amount;
						m_freeWorldItems[i] = value;
						flag = true;
						break;
					}
				}
			}
		}
		if (!flag)
		{
			m_freeWorldItems.Add(new DatabaseItem(a_newItemType, a_pos.x, a_pos.z, quality));
		}
	}

	public void CreateTempContainerItem(int a_newItemType, int a_amount, Vector3 a_pos, int a_containerType = 120)
	{
		a_pos.y = 0f;
		string key = a_pos.ToString();
		ItemContainer itemContainer = (!this.m_freeWorldContainers.Contains(key)) ? null : ((ItemContainer)this.m_freeWorldContainers[key]);
		if (itemContainer == null)
		{
			this.m_freeWorldItems.Add(new DatabaseItem(a_containerType, a_pos.x, a_pos.z, 1, false, 0, 0));
			itemContainer = new ItemContainer(4, 4, 6, 0, null, null);
			itemContainer.m_position = a_pos;
			this.m_freeWorldContainers.Add(key, itemContainer);
		}
		itemContainer.CollectItem(new DatabaseItem(a_newItemType, 0f, 0f, a_amount, false, 0, 0), true, default(Vector3));
	}

	public DatabaseItem PickupItem(ServerPlayer a_player, BrainBase a_npc)
	{
		DatabaseItem databaseItem = new DatabaseItem(0, 0f, 0f, 1, false, 0, 0);
		for (int i = 0; i < this.m_freeWorldItems.Count; i++)
		{
			if (a_player != null)
			{
				if (Mathf.Abs(this.m_freeWorldItems[i].x - a_player.GetPosition().x) < 1.1f && Mathf.Abs(this.m_freeWorldItems[i].y - a_player.GetPosition().z) < 1.1f)
				{
					databaseItem = this.m_freeWorldItems[i];
					if (Items.IsContainer(databaseItem.type))
					{
						string key = this.m_freeWorldItems[i].GetPos().ToString();
						if (this.m_freeWorldContainers.Contains(key))
						{
							a_player.m_freeWorldContainer = (ItemContainer)this.m_freeWorldContainers[key];
						}
					}
					else if (a_player.m_inventory.CollectItem(databaseItem, true, default(Vector3)))
					{
						this.DeleteFreeWorldItem(i);
						if (databaseItem.type == 254)
						{
							this.SendMoneyUpdate(a_player);
						}
					}
					a_player.m_updateContainersFlag = true;
					if (databaseItem.x < 1f)
					{
						this.SendPlayerInfo(a_player);
					}
					break;
				}
			}
			else if (null != a_npc)
			{
				float num = 1.6f;
				if (Mathf.Abs(this.m_freeWorldItems[i].x - a_npc.transform.position.x) < num && Mathf.Abs(this.m_freeWorldItems[i].y - a_npc.transform.position.z) < num)
				{
					databaseItem = this.m_freeWorldItems[i];
					this.DeleteFreeWorldItem(i);
					break;
				}
			}
		}
		return databaseItem;
	}

	public DatabaseItem GetRandomFreeWorldItem()
	{
		if (this.m_freeWorldItems.Count > 0)
		{
			return this.m_freeWorldItems[UnityEngine.Random.Range(0, this.m_freeWorldItems.Count)];
		}
		return new DatabaseItem(0, 0f, 0f, 1, false, 0, 0);
	}

	public int GetPlayerCount()
	{
		return this.m_server.ConnectionsCount;
	}

	public List<DatabaseItem> GetFreeWorldItems()
	{
		return this.m_freeWorldItems;
	}

	public ServerPlayer GetPlayerByTransform(Transform a_t)
	{
		for (int i = 0; i < this.m_players.Length; i++)
		{
			if (this.m_players[i] != null && this.m_players[i].IsSpawned() && a_t == this.m_players[i].GetTransform())
			{
				return this.m_players[i];
			}
		}
		return null;
	}

	public int GetFreeSlots()
	{
		int num = 0;
		for (int i = 0; i < this.m_players.Length; i++)
		{
			if (this.m_players[i] == null)
			{
				num++;
			}
		}
		return num;
	}

	public ServerPlayer GetPlayerByPid(int a_pid)
	{
		for (int i = 0; i < this.m_players.Length; i++)
		{
			if (this.m_players[i] != null && this.m_players[i].IsSpawned() && a_pid == this.m_players[i].m_pid)
			{
				return this.m_players[i];
			}
		}
		return null;
	}
	public ServerPlayer GetPlayerByName(string a_name)
	{
		for (int i = 0; i < m_players.Length; i++)
		{
			if (m_players[i] != null && m_players[i].IsSpawned() && a_name == m_players[i].m_name)
			{
				return m_players[i];
			}
		}
		return null;
	}


	public ServerPlayer GetPlayerByAid(ulong a_aid)
	{
		for (int i = 0; i < this.m_players.Length; i++)
		{
			if (this.m_players[i] != null && this.m_players[i].IsSpawned() && a_aid == this.m_players[i].m_accountId)
			{
				return this.m_players[i];
			}
		}
		return null;
	}

	public ServerPlayer GetPlayerByOnlineid(int a_oid)
	{
		if (-1 < a_oid && a_oid < this.m_players.Length && this.m_players[a_oid] != null && this.m_players[a_oid].IsSpawned())
		{
			return this.m_players[a_oid];
		}
		return null;
	}

	public ServerPlayer GetNearestPlayer(Vector3 a_pos)
	{
		float num = 9999999f;
		ServerPlayer result = null;
		for (int i = 0; i < this.m_players.Length; i++)
		{
			if (this.m_players[i] != null && this.m_players[i].IsSpawned() && !this.m_players[i].IsDead())
			{
				float sqrMagnitude = (a_pos - this.m_players[i].GetPosition()).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = this.m_players[i];
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	public DatabaseItem GetNearestItem(Vector3 a_pos, bool a_petFoodOnly = false)
	{
		float num = 9999999f;
		DatabaseItem result = default(DatabaseItem);
		for (int i = 0; i < this.m_freeWorldItems.Count; i++)
		{
			if (!a_petFoodOnly || Items.IsEatableForPet(this.m_freeWorldItems[i].type))
			{
				float sqrMagnitude = (a_pos - this.m_freeWorldItems[i].GetPos()).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = this.m_freeWorldItems[i];
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	public int GetNearbyItemCount(Vector3 a_pos)
	{
		int num = 0;
		for (int i = 0; i < this.m_freeWorldItems.Count; i++)
		{
			float sqrMagnitude = (a_pos - this.m_freeWorldItems[i].GetPos()).sqrMagnitude;
			if (sqrMagnitude < 1f)
			{
				num++;
			}
		}
		return num;
	}

	public bool PartyContainsPid(int a_partyId, int a_pid)
	{
		if (this.m_partys.Contains(a_partyId))
		{
			List<DatabasePlayer> list = (List<DatabasePlayer>)this.m_partys[a_partyId];
			if (list != null && 0 < list.Count)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (a_pid == list[i].pid)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public bool PartyContainsAid(int a_partyId, ulong a_aid)
	{
		if (this.m_partys.Contains(a_partyId))
		{
			List<DatabasePlayer> list = (List<DatabasePlayer>)this.m_partys[a_partyId];
			if (list != null && 0 < list.Count)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (a_aid == list[i].aid)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public float GetDayLight()
	{
		float num = 0f;
		return Util.GetLightIntensity(this.m_dayNightCycle, out num);
	}

	public SQLThreadManager GetSql()
	{
		return this.m_sql;
	}

	public void BroadcastStaticBuildingChange(ServerBuilding a_building)
	{
		NetOutgoingMessage netOutgoingMessage = this.m_server.CreateMessage();
		netOutgoingMessage.Write(MessageIds.StaticBuildingUpdate);
		this.AddBuildingToMsg(netOutgoingMessage, a_building, false);
		this.m_server.SendToAll(netOutgoingMessage, NetDeliveryMethod.ReliableUnordered);
	}

	public void SendPartyUpdate(ServerPlayer a_player, DatabasePlayer[] a_party)
	{
		NetOutgoingMessage netOutgoingMessage = a_player.m_connection.Peer.CreateMessage();
		netOutgoingMessage.Write(MessageIds.PartyUpdate);
		netOutgoingMessage.Write((byte)((a_party == null) ? 0 : a_party.Length));
		if (a_party != null)
		{
			for (int i = 0; i < a_party.Length; i++)
			{
				netOutgoingMessage.Write(a_party[i].name);
				netOutgoingMessage.Write(a_party[i].aid);
				netOutgoingMessage.Write((byte)a_party[i].partyRank);
			}
		}
		a_player.m_connection.SendMessage(netOutgoingMessage, NetDeliveryMethod.Unreliable, 0);
	}

	public void SendPartyFeedback(ServerPlayer a_player, ePartyFeedback a_type, string a_otherPlayerName)
	{
		if (15 < a_otherPlayerName.Length)
		{
			a_otherPlayerName = a_otherPlayerName.Substring(0, 15) + "...";
		}
		NetOutgoingMessage netOutgoingMessage = a_player.m_connection.Peer.CreateMessage();
		netOutgoingMessage.Write(MessageIds.PartyFeedback);
		netOutgoingMessage.Write((byte)a_type);
		netOutgoingMessage.Write(a_otherPlayerName);
		a_player.m_connection.SendMessage(netOutgoingMessage, NetDeliveryMethod.Unreliable, 0);
	}

	public void SendRankUpdate(ServerPlayer a_player, int a_addedXp)
	{
		NetOutgoingMessage netOutgoingMessage = a_player.m_connection.Peer.CreateMessage();
		netOutgoingMessage.Write(MessageIds.RankUpdate);
		netOutgoingMessage.Write((byte)(a_player.GetRankProgress() * 255f));
		netOutgoingMessage.Write((short)Mathf.Max(a_addedXp, 0));
		a_player.m_connection.SendMessage(netOutgoingMessage, NetDeliveryMethod.Unreliable, 0);
	}

	public void SendConditionUpdate(ServerPlayer a_player)
	{
		NetOutgoingMessage netOutgoingMessage = a_player.m_connection.Peer.CreateMessage();
		netOutgoingMessage.Write(MessageIds.ConditionUpdate);
		netOutgoingMessage.Write(a_player.GetConditions());
		a_player.m_connection.SendMessage(netOutgoingMessage, NetDeliveryMethod.Unreliable, 0);
	}

	public void SendMoneyUpdate(ServerPlayer a_player)
	{
		NetOutgoingMessage netOutgoingMessage = a_player.m_connection.Peer.CreateMessage();
		netOutgoingMessage.Write(MessageIds.MoneyUpdate);
		netOutgoingMessage.Write(a_player.m_gold);
		a_player.m_connection.SendMessage(netOutgoingMessage, NetDeliveryMethod.Unreliable, 0);
	}

	public void SendShopInfo(ServerPlayer a_player, float a_buyMultip, float a_sellMultip)
	{
		NetOutgoingMessage netOutgoingMessage = a_player.m_connection.Peer.CreateMessage();
		netOutgoingMessage.Write(MessageIds.ShopInfo);
		netOutgoingMessage.Write(a_buyMultip);
		netOutgoingMessage.Write(a_sellMultip);
		a_player.m_connection.SendMessage(netOutgoingMessage, NetDeliveryMethod.Unreliable, 0);
	}

	public void SendSpecialEvent(ServerPlayer a_player, eSpecialEvent a_event)
	{
		if (a_event > eSpecialEvent.none)
		{
			NetOutgoingMessage netOutgoingMessage = a_player.m_connection.Peer.CreateMessage();
			netOutgoingMessage.Write(MessageIds.SpecialEvent);
			netOutgoingMessage.Write((byte)a_event);
			a_player.m_connection.SendMessage(netOutgoingMessage, NetDeliveryMethod.Unreliable, 0);
		}
	}

	public void SendMissionPropose(ServerPlayer a_player, Mission a_mission)
	{
		if (a_mission != null)
		{
			NetOutgoingMessage netOutgoingMessage = a_player.m_connection.Peer.CreateMessage();
			netOutgoingMessage.Write(MessageIds.MissionPropose);
			netOutgoingMessage.Write((byte)a_mission.m_type);
			netOutgoingMessage.Write((byte)a_mission.m_objPerson);
			netOutgoingMessage.Write((byte)a_mission.m_objObject);
			netOutgoingMessage.Write((byte)a_mission.m_location);
			netOutgoingMessage.Write((short)a_mission.m_xpReward);
			a_player.m_connection.SendMessage(netOutgoingMessage, NetDeliveryMethod.Unreliable, 0);
		}
	}

	public void SendMissionUpdate(ServerPlayer a_player, List<Mission> a_missions)
	{
		NetOutgoingMessage netOutgoingMessage = a_player.m_connection.Peer.CreateMessage();
		netOutgoingMessage.Write(MessageIds.MissionUpdate);
		if (a_missions != null)
		{
			for (int i = 0; i < a_missions.Count; i++)
			{
				if (a_missions[i] != null)
				{
					netOutgoingMessage.Write((byte)a_missions[i].m_type);
					netOutgoingMessage.Write((byte)a_missions[i].m_objPerson);
					netOutgoingMessage.Write((byte)a_missions[i].m_objObject);
					netOutgoingMessage.Write((byte)a_missions[i].m_location);
					netOutgoingMessage.Write((short)a_missions[i].m_xpReward);
					netOutgoingMessage.Write((short)(a_missions[i].m_dieTime - Time.time));
				}
			}
		}
		a_player.m_connection.SendMessage(netOutgoingMessage, NetDeliveryMethod.Unreliable, 0);
	}

	public void SendNotification(string a_text)
	{
		NetOutgoingMessage netOutgoingMessage = this.m_server.CreateMessage();
		netOutgoingMessage.Write(MessageIds.Notification);
		netOutgoingMessage.Write(a_text);
		this.m_server.SendToAll(netOutgoingMessage, NetDeliveryMethod.Unreliable);
	}

	public SpawnPos[] GetSpawnPoints()
	{
		return this.m_spawnPoints;
	}

	public ServerTutorial GetTutorial()
	{
		return this.m_tutorial;
	}

	public bool IsInSpecialArea(Vector3 a_pos, eAreaType a_area)
	{
		foreach (SpecialArea specialArea in this.m_specialAreas)
		{
			if (null != specialArea && a_area == specialArea.m_type)
			{
				Vector3 vector = specialArea.transform.position - a_pos;
				if (specialArea.m_type == eAreaType.noPvp)
				{
					if (Mathf.Abs(vector.x) < specialArea.m_radius && Mathf.Abs(vector.z) < specialArea.m_radius)
					{
						return true;
					}
				}
				else if (vector.sqrMagnitude < specialArea.m_radius * specialArea.m_radius)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool RepairVehicle(ServerPlayer a_player, Vector3 a_distCheckPos)
	{
		bool result = false;
		float num = 9999999f;
		int num2 = -1;
		Vector3 a = Vector3.zero;
		for (int i = 0; i < this.m_vehicles.Length; i++)
		{
			if (null != this.m_vehicles[i] && !this.m_vehicles[i].IsDead())
			{
				a = this.m_vehicles[i].transform.position;
				float sqrMagnitude = (a - a_distCheckPos).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num2 = i;
					num = sqrMagnitude;
				}
			}
		}
		if (10f > num)
		{
			this.m_vehicles[num2].ChangeHealthBy(5f);
			result = true;
		}
		return result;
	}

	public void DealExplosionDamage(Vector3 a_pos, float a_damage, float a_radius)
	{
		for (int i = 0; i < this.m_players.Length; i++)
		{
			if (this.m_players[i] != null && this.m_players[i].IsSpawned() && !this.m_players[i].IsDead())
			{
				float sqrMagnitude = (a_pos - this.m_players[i].GetPosition()).sqrMagnitude;
				if (sqrMagnitude < a_radius * a_radius)
				{
					float a_delta = -a_damage * (1f - sqrMagnitude / (a_radius * a_radius));
					this.m_players[i].ChangeHealthBy(a_delta);
				}
			}
		}
		for (int j = 0; j < this.m_buildingMan.m_buildings.Count; j++)
		{
			if (null != this.m_buildingMan.m_buildings[j])
			{
				float sqrMagnitude2 = (a_pos - this.m_buildingMan.m_buildings[j].transform.position).sqrMagnitude;
				if (sqrMagnitude2 < a_radius * a_radius)
				{
					float a_dif = -a_damage * (1f - sqrMagnitude2 / (a_radius * a_radius));
					this.m_buildingMan.m_buildings[j].ChangeHealthBy(a_dif);
					this.m_buildingMan.m_buildings[j].SetAggressor(base.transform);
				}
			}
		}
		for (int k = 0; k < this.m_npcs.Length; k++)
		{
			if (null != this.m_npcs[k])
			{
				float sqrMagnitude3 = (a_pos - this.m_npcs[k].transform.position).sqrMagnitude;
				if (sqrMagnitude3 < a_radius * a_radius)
				{
					float a_delta2 = -a_damage * (1f - sqrMagnitude3 / (a_radius * a_radius));
					this.m_npcs[k].ChangeHealthBy(a_delta2);
				}
			}
		}
		if (this.m_vehicles != null)
		{
			for (int l = 0; l < this.m_vehicles.Length; l++)
			{
				float sqrMagnitude4 = (a_pos - this.m_vehicles[l].transform.position).sqrMagnitude;
				if (sqrMagnitude4 < a_radius * a_radius)
				{
					float a_delta3 = -a_damage * (1f - sqrMagnitude4 / (a_radius * a_radius));
					this.m_vehicles[l].ChangeHealthBy(a_delta3);
				}
			}
		}
	}

	private const float c_updateXradius = 22f;

	private const float c_updateZradius = 19f;

	private const float c_playerPickupRadius = 1.1f;

	private const float c_playerVehicleRadius = 2.5f;

	public GameObject m_controlledCharPrefab;

	public float m_updateIntervall = 0.2f;

	public float m_playerDbWriteIntervall = 10.5f;

	public float m_serverRestartTime = 86399f;

	public bool m_shutdownIfEmpty;

	private int m_restartMinutes = 5;

	private int m_maxPartyId = 1;

	private float m_dayNightCycleSpeed = 0.001f;

	private float m_nextPlayerDbWriteTime;

	private float m_nextServerListUpdate = 5f;

	private float m_nextItemUpdate = 5f;

	private NetServer m_server;

	private SQLThreadManager m_sql;

	private BuildingManager m_buildingMan;

	private MissionManager m_missionMan;

	private string m_serverName = string.Empty;

	private bool m_inited;

	public ServerPlayer[] m_players = new ServerPlayer[50];

	public ServerNpc[] m_npcs;

	public ServerVehicle[] m_vehicles;

	public ShopContainer[] m_shopContainers;

	public ServerBuilding[] m_staticBuildings;

	public SpawnPos[] m_spawnPoints;

	public SpecialArea[] m_specialAreas;

	public RepairingNpc[] m_repairNpcs;

	public ServerTutorial m_tutorial;

	private List<DatabaseItem> m_freeWorldItems = new List<DatabaseItem>();

	private Hashtable m_freeWorldContainers = new Hashtable();

	private Hashtable m_partys = new Hashtable();

	private float m_dayNightCycle;
}

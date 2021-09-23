using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
	public MissionManager()
	{
	}

	private void Start()
	{
		this.m_missionNpcs = (MissionNpc[])UnityEngine.Object.FindObjectsOfType(typeof(MissionNpc));
		this.m_missionObjs = (MissionObjective[])UnityEngine.Object.FindObjectsOfType(typeof(MissionObjective));
		this.m_server = (LidServer)UnityEngine.Object.FindObjectOfType(typeof(LidServer));
		this.m_missionXpLocMultip = new float[15];
		for (int i = 0; i < 15; i++)
		{
			this.m_missionXpLocMultip[i] = 1f;
		}
		this.m_missionXpLocMultip[1] = 0.8f;
		this.m_missionXpLocMultip[2] = 1.1f;
		this.m_missionXpLocMultip[3] = 1.5f;
		this.m_missionXpLocMultip[4] = 0.8f;
		this.m_missionXpLocMultip[5] = 1.2f;
		this.m_missionXpLocMultip[6] = 0.5f;
		this.m_missionXpLocMultip[7] = 0.7f;
		this.m_missionXpLocMultip[8] = 1.2f;
		this.m_missionXpLocMultip[9] = 1f;
		this.m_missionXpLocMultip[10] = 2f;
		this.m_missionXpLocMultip[11] = 1.4f;
		this.m_missionXpLocMultip[12] = 1.3f;
		this.m_missionXpLocMultip[13] = 1.5f;
		this.m_missionXpLocMultip[14] = 1.3f;
	}

	private void Update()
	{
		if (Time.time > this.m_nextMissionDeleteUpdate)
		{
			foreach (object obj in this.m_missions)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				List<Mission> list = (List<Mission>)dictionaryEntry.Value;
				for (int i = 0; i < list.Count; i++)
				{
					if (Time.time > list[i].m_dieTime)
					{
						list.RemoveAt(i);
						this.UpdatePlayer(this.m_server.GetPlayerByPid((int)dictionaryEntry.Key));
						break;
					}
				}
				if (list.Count == 0)
				{
					this.m_missions.Remove(dictionaryEntry.Key);
					break;
				}
			}
			this.m_nextMissionDeleteUpdate = Time.time + 1f;
		}
	}

	private MissionNpc GetNearbyNpc(Vector3 a_pos)
	{
		for (int i = 0; i < this.m_missionNpcs.Length; i++)
		{
			Vector3 position = this.m_missionNpcs[i].transform.position;
			Vector3 vector = a_pos;
			if (Mathf.Abs(position.x - vector.x) < 1.4f && Mathf.Abs(position.z - vector.z) < 1.4f)
			{
				return this.m_missionNpcs[i];
			}
		}
		return null;
	}

	public void DeleteMissions(ServerPlayer a_player)
	{
		if (a_player != null && this.m_missions.ContainsKey(a_player.m_pid))
		{
			this.m_missions.Remove(a_player.m_pid);
			this.UpdatePlayer(a_player);
		}
	}

	public void UpdatePlayer(ServerPlayer a_player)
	{
		if (a_player != null)
		{
			this.m_server.SendMissionUpdate(a_player, (!this.m_missions.ContainsKey(a_player.m_pid)) ? null : ((List<Mission>)this.m_missions[a_player.m_pid]));
		}
	}

	public bool RequestMission(ServerPlayer a_player)
	{
		bool flag = this.m_missions.ContainsKey(a_player.m_pid) && 3 <= ((List<Mission>)this.m_missions[a_player.m_pid]).Count;
		MissionNpc nearbyNpc = this.GetNearbyNpc(a_player.GetPosition());
		if (null != nearbyNpc)
		{
			if (flag)
			{
				this.m_server.SendSpecialEvent(a_player, eSpecialEvent.tooManyMissions);
			}
			else
			{
				bool flag2 = false;
				if (this.m_missions.ContainsKey(a_player.m_pid))
				{
					List<Mission> list = (List<Mission>)this.m_missions[a_player.m_pid];
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].IsEqual(nearbyNpc.m_mission))
						{
							flag2 = true;
							this.m_server.SendSpecialEvent(a_player, eSpecialEvent.alreadyGotMission);
							break;
						}
					}
				}
				if (!flag2)
				{
					this.m_missionProposals[a_player.m_pid] = nearbyNpc.m_mission;
					this.m_server.SendMissionPropose(a_player, nearbyNpc.m_mission);
				}
			}
		}
		return !flag && null != nearbyNpc;
	}

	public void AcceptMission(ServerPlayer a_player)
	{
		if (this.m_missionProposals.ContainsKey(a_player.m_pid))
		{
			if (!this.m_missions.ContainsKey(a_player.m_pid))
			{
				this.m_missions.Add(a_player.m_pid, new List<Mission>());
			}
			Mission mission = (Mission)this.m_missionProposals[a_player.m_pid];
			MissionNpc nearbyNpc = this.GetNearbyNpc(a_player.GetPosition());
			if (null != nearbyNpc && nearbyNpc.m_mission.IsEqual(mission))
			{
				nearbyNpc.AcceptMission();
			}
			mission.m_dieTime = Time.time + 3600f;
			((List<Mission>)this.m_missions[a_player.m_pid]).Add(mission);
			this.m_missionProposals.Remove(a_player.m_pid);
			this.UpdatePlayer(a_player);
		}
	}

	public bool SolveMission(ServerPlayer a_player, bool a_interactionOnly = false)
	{
		if (this.m_missions.ContainsKey(a_player.m_pid))
		{
			for (int i = 0; i < this.m_missionObjs.Length; i++)
			{
				Vector3 b = a_player.GetPosition() + a_player.GetForward() * 2f;
				float num = (!a_interactionOnly) ? 144f : 9f;
				if (null != this.m_missionObjs[i] && (this.m_missionObjs[i].gameObject.transform.position - b).sqrMagnitude < num)
				{
					List<Mission> list = (List<Mission>)this.m_missions[a_player.m_pid];
					for (int j = 0; j < list.Count; j++)
					{
						if ((!a_interactionOnly || list[j].m_type == eMissiontype.eRescue) && this.m_missionObjs[i].IsMission(list[j]))
						{
							a_player.AddXp(list[j].m_xpReward);
							list.RemoveAt(j);
							this.UpdatePlayer(a_player);
							this.m_server.SendSpecialEvent(a_player, eSpecialEvent.missionComplete);
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public Mission GetRandomMission(int a_randomSeed, bool a_easyMode = false)
	{
		UnityEngine.Random.seed = a_randomSeed;
		Mission mission = new Mission();
		mission.m_type = (eMissiontype)UnityEngine.Random.Range(1, 4);
		mission.m_objPerson = (eObjectivesPerson)UnityEngine.Random.Range(1, 9);
		mission.m_objObject = (eObjectivesObject)UnityEngine.Random.Range(1, 5);
		mission.m_location = (eLocation)UnityEngine.Random.Range(1, 15);
		if (a_easyMode)
		{
			for (int i = 0; i < 1; i++)
			{
				int num = UnityEngine.Random.Range(1, 15);
				if (this.m_missionXpLocMultip[num] < this.m_missionXpLocMultip[(int)mission.m_location])
				{
					mission.m_location = (eLocation)num;
				}
			}
		}
		mission.m_xpReward = (int)(200.001f * this.m_missionXpLocMultip[(int)mission.m_location]);
		return mission;
	}

	private const float m_missionXp = 200.001f;

	private MissionNpc[] m_missionNpcs;

	private MissionObjective[] m_missionObjs;

	private LidServer m_server;

	private float[] m_missionXpLocMultip;

	private Hashtable m_missions = new Hashtable();

	private Hashtable m_missionProposals = new Hashtable();

	private float m_nextMissionDeleteUpdate;
}

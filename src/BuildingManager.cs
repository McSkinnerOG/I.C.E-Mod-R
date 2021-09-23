using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
	public BuildingManager()
	{
	}

	private void Start()
	{
		this.m_server = (LidServer)UnityEngine.Object.FindObjectOfType(typeof(LidServer));
		this.m_resSpawns = (ResourceSpawner[])UnityEngine.Object.FindObjectsOfType(typeof(ResourceSpawner));
		this.m_nextUpdate = UnityEngine.Random.Range(3f, 12f);
	}

	private void Update()
	{
		if (Time.time > this.m_nextUpdate && this.m_resSpawns != null && this.m_resSpawns.Length > 0)
		{
			this.SpawnResources();
			this.m_nextUpdate = Time.time + 30.8f;
		}
	}

	private void SpawnResources()
	{
		int num = 0;
		for (int i = 0; i < this.m_buildings.Count; i++)
		{
			if (null != this.m_buildings[i])
			{
				if (Buildings.IsResource(this.m_buildings[i].m_type))
				{
					num++;
				}
			}
			else
			{
				this.m_buildings.RemoveAt(i);
				i--;
			}
		}
		if (null != this.m_server && Mathf.Clamp(6 + this.m_server.GetPlayerCount() * 2, 0, 40) > num)
		{
			int num2 = UnityEngine.Random.Range(0, this.m_resSpawns.Length);
			float radius = this.m_resSpawns[num2].m_radius;
			Vector3 a_pos = this.m_resSpawns[num2].transform.position + new Vector3(UnityEngine.Random.Range(-radius, radius), 0f, UnityEngine.Random.Range(-radius, radius));
			this.CreateBuilding(this.m_resSpawns[num2].m_resourceBuildingType, a_pos, 0, 0f, 100, true);
		}
	}

	public bool CreateBuilding(int a_type, Vector3 a_pos, int a_ownerPid = 0, float a_yRot = 0f, int a_health = 100, bool a_isNew = true)
	{
		bool flag = false == a_isNew;
		if (!flag)
		{
			ServerBuilding serverBuilding = this.GetNearestPlayerBuilding(a_pos, -1);
			if (null == serverBuilding || (serverBuilding.transform.position - a_pos).sqrMagnitude > 0.20249999f)
			{
				serverBuilding = ((Buildings.IsCollider(a_type) && a_ownerPid != 0) ? this.GetNearestPlayerBuilding(a_pos, a_ownerPid) : null);
				if ((null == serverBuilding || !Buildings.IsDoor(serverBuilding.m_type) || (serverBuilding.transform.position - a_pos).sqrMagnitude > 25f) && (Buildings.IsDoor(a_type) || !Raycaster.BuildingSphereCast(a_pos)) && 0.8f < Util.GetTerrainHeight(a_pos))
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			GameObject gameObject = (GameObject)Resources.Load("buildings/building_" + a_type);
			if (null != gameObject)
			{
				GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(gameObject, a_pos, Quaternion.Euler(0f, a_yRot, 0f));
				ServerBuilding component = gameObject2.GetComponent<ServerBuilding>();
				NavMeshObstacle componentInChildren = gameObject2.GetComponentInChildren<NavMeshObstacle>();
				if (null != componentInChildren)
				{
					componentInChildren.carving = this.m_carveNavMesh;
				}
				if (null != component)
				{
					component.Init(this.m_server, a_type, a_ownerPid, a_health, a_isNew);
					this.m_buildings.Add(component);
				}
				else
				{
					Debug.Log("BuildingManager.cs: ERROR: Building without ServerBuilding.cs script spawned!");
				}
			}
		}
		return flag;
	}

	public bool RepairBuilding(ServerPlayer a_player, Vector3 a_distCheckPos)
	{
		return this.UseBuilding(a_player, a_distCheckPos, true);
	}

	public bool UseBuilding(ServerPlayer a_player, Vector3 a_distCheckPos, bool a_repair = false)
	{
		bool result = false;
		float num = 9999999f;
		int num2 = -1;
		Vector3 a = Vector3.zero;
		for (int i = 0; i < this.m_buildings.Count; i++)
		{
			if (null != this.m_buildings[i])
			{
				a = this.m_buildings[i].transform.position;
				if (Buildings.IsDoor(this.m_buildings[i].m_type))
				{
					a += this.m_buildings[i].transform.right;
				}
				float sqrMagnitude = (a - a_distCheckPos).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num2 = i;
					num = sqrMagnitude;
				}
			}
		}
		float num3 = (num2 == -1 || this.m_buildings[num2].m_type != 103) ? 2.4f : 1.2f;
		if (num < num3)
		{
			if (a_repair)
			{
				result = this.m_buildings[num2].Repair(a_player);
			}
			else
			{
				result = this.m_buildings[num2].Use(a_player);
			}
		}
		return result;
	}

	public ServerBuilding GetNearestPlayerBuilding(Vector3 a_pos, int a_ignoreOwnerPid = -1)
	{
		float num = 9999999f;
		ServerBuilding result = null;
		for (int i = 0; i < this.m_buildings.Count; i++)
		{
			if (null != this.m_buildings[i] && this.m_buildings[i].GetState() > 0f && this.m_buildings[i].GetOwnerId() != a_ignoreOwnerPid && this.m_buildings[i].GetOwnerId() != 0)
			{
				float sqrMagnitude = (a_pos - this.m_buildings[i].transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = this.m_buildings[i];
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	public Vector3 GetRespawnPos(Vector3 a_pos, int a_pid)
	{
		Vector3 result = Vector3.zero;
		float num = 9999999f;
		for (int i = 0; i < this.m_buildings.Count; i++)
		{
			if (null != this.m_buildings[i] && this.m_buildings[i].m_type == 101 && this.m_buildings[i].GetState() > 0f && this.m_buildings[i].GetOwnerId() == a_pid && this.m_buildings[i].GetOwnerId() != 0)
			{
				float sqrMagnitude = (a_pos - this.m_buildings[i].transform.position).sqrMagnitude;
				if (sqrMagnitude < num && sqrMagnitude > 400f)
				{
					result = this.m_buildings[i].transform.position;
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	public void IgnoreBedCollision(int a_pid, Collider a_playerCollider)
	{
		for (int i = 0; i < this.m_buildings.Count; i++)
		{
			if (null != this.m_buildings[i] && this.m_buildings[i].m_type == 101 && this.m_buildings[i].GetState() > 0f && this.m_buildings[i].GetOwnerId() == a_pid && this.m_buildings[i].GetOwnerId() != 0)
			{
				Physics.IgnoreCollision(this.m_buildings[i].collider, a_playerCollider);
			}
		}
	}

	public bool IsNearCampfire(Vector3 a_pos)
	{
		for (int i = 0; i < this.m_buildings.Count; i++)
		{
			if (null != this.m_buildings[i] && this.m_buildings[i].m_type == 100 && this.m_buildings[i].GetState() < 0.5f && (this.m_buildings[i].transform.position - a_pos).sqrMagnitude < 16f)
			{
				return true;
			}
		}
		return false;
	}

	public Lootbox AddItemToLootContainer(DatabaseItem a_item)
	{
		for (int i = 0; i < this.m_buildings.Count; i++)
		{
			if (null != this.m_buildings[i] && this.m_buildings[i].m_type == 103 && 0f < this.m_buildings[i].GetState())
			{
				Lootbox lootbox = (Lootbox)this.m_buildings[i];
				if (null != lootbox && a_item.cid == lootbox.m_cid && lootbox.m_container != null)
				{
					lootbox.m_container.UpdateOrCreateItem(a_item);
					return lootbox;
				}
			}
		}
		return null;
	}

	public bool m_carveNavMesh = true;

	[HideInInspector]
	public List<ServerBuilding> m_buildings = new List<ServerBuilding>();

	private LidServer m_server;

	private float m_nextUpdate;

	private ResourceSpawner[] m_resSpawns;
}

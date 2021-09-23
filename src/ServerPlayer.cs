using System;
using System.Collections.Generic;
using Lidgren.Network;
using UnityEngine;

public class ServerPlayer
{
	public ServerPlayer(string a_name, ulong a_accountId, int a_onlineId, eCharType a_type, NetConnection a_con, SQLThreadManager a_sql, LidServer a_server, BuildingManager a_buildMan, MissionManager a_missionMan)
	{
		this.m_name = a_name;
		this.m_accountId = a_accountId;
		this.m_onlineId = a_onlineId;
		this.m_charType = a_type;
		this.m_connection = a_con;
		if (this.m_connection != null)
		{
			this.m_connection.Tag = this.m_onlineId;
		}
		this.m_sql = a_sql;
		this.m_server = a_server;
		this.m_buildMan = a_buildMan;
		this.m_missionMan = a_missionMan;
	}

	public void Spawn(GameObject a_prefab, DatabasePlayer a_dbPlayer)
	{
		Vector3 position = Vector3.zero;
		bool flag = a_dbPlayer.x == 0f && 0f == a_dbPlayer.y;
		if (flag)
		{
			a_dbPlayer.karma = 200;
			ServerTutorial tutorial = this.m_server.GetTutorial();
			if (null != tutorial)
			{
				position = tutorial.StartTutorial();
			}
		}
		else
		{
			position.x = a_dbPlayer.x;
			position.z = a_dbPlayer.y;
		}
		position.y = 0f;
		this.m_pid = a_dbPlayer.pid;
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(a_prefab, position, Quaternion.identity);
		this.m_char = gameObject.GetComponent<ControlledChar>();
		this.m_health = (float)a_dbPlayer.health;
		this.m_energy = (float)a_dbPlayer.energy;
		this.m_karma = (float)a_dbPlayer.karma;
		this.m_lastUpdateKarma = (float)a_dbPlayer.karma;
		this.m_xp = a_dbPlayer.xp;
		this.m_condition = a_dbPlayer.condition;
		this.m_gold = a_dbPlayer.gold;
		this.m_partyId = a_dbPlayer.partyId;
		this.m_partyRank = a_dbPlayer.partyRank;
		this.RecalculateRank();
		if (this.m_health == 0f)
		{
			this.m_respawnTime = Time.time;
		}
		this.m_char.Init(this);
		this.m_inventory = new ItemContainer(5, 4, 0, this.m_sql.PidToCid(this.m_pid), this.m_sql, this);
		this.m_buildMan.IgnoreBedCollision(this.m_pid, this.m_char.collider);
	}

	public bool IsAttacking()
	{
		return Time.time < this.m_nextAttackTime;
	}

	public bool IsSpawned()
	{
		return null != this.m_char;
	}

	public bool HasUseChanged()
	{
		return this.IsSpawned() && this.m_char.HasUseChanged();
	}

	public bool HasSpaceChanged()
	{
		return this.IsSpawned() && this.m_char.HasSpaceChanged();
	}

	public void SetRotation(float a_rot)
	{
		this.m_forcedRotation = a_rot;
	}

	public void SetVictim(Transform a_victim)
	{
		this.m_victim = a_victim;
	}

	public Transform GetVictim()
	{
		return this.m_victim;
	}

	public void ResetContainers()
	{
		this.m_freeWorldContainer = null;
		this.m_persistentContainer = null;
		this.m_shopContainer = null;
		this.m_updateContainersFlag = true;
		this.m_server.SendShopInfo(this, -1f, -1f);
	}

	public Transform GetTransform()
	{
		Transform result = null;
		if (null != this.m_vehicle)
		{
			result = this.m_vehicle.transform;
		}
		else if (this.IsSpawned())
		{
			result = this.m_char.transform;
		}
		return result;
	}

	public Vector3 GetPosition()
	{
		Transform transform = this.GetTransform();
		return (!(null == transform)) ? this.GetTransform().position : Vector3.zero;
	}

	public void SetPosition(Vector3 a_pos)
	{
		if (this.IsSpawned())
		{
			a_pos.y = 0f;
			this.m_char.transform.position = a_pos;
		}
	}

	public float GetRotation()
	{
		float result = 0f;
		if (null != this.m_vehicle)
		{
			result = this.m_vehicle.transform.rotation.eulerAngles.y;
		}
		else if (this.IsSpawned())
		{
			result = this.m_char.GetRotation();
		}
		return result;
	}

	public Vector3 GetForward()
	{
		return Quaternion.Euler(0f, this.GetRotation(), 0f) * Vector3.forward;
	}

	public void ConsumeItem(int a_itemType)
	{
		if (Items.IsMedicine(a_itemType))
		{
			switch (a_itemType)
			{
			case 140:
				this.SetCondition(eCondition.bleeding, false);
				break;
			case 141:
				this.SetCondition(eCondition.infection, false);
				break;
			case 142:
				this.SetCondition(eCondition.pain, false);
				break;
			case 143:
				this.m_healEndTime = Time.time + Items.GetItemDef(a_itemType).healing / this.m_healHealthGainPerSec;
				this.SetCondition(eCondition.pain, false);
				this.SetCondition(eCondition.bleeding, false);
				break;
			}
		}
		else if (Items.IsEatable(a_itemType))
		{
			this.ChangeEnergyBy(Items.GetItemDef(a_itemType).healing);
		}
	}

	public void SetCondition(eCondition a_condition, bool a_state)
	{
		this.m_condition = ((!a_state) ? (this.m_condition & ~(1 << (int)a_condition)) : (this.m_condition | 1 << (int)a_condition));
	}

	public int GetConditions()
	{
		return this.m_condition;
	}

	public void SetConditions(int a_conditions)
	{
		this.m_condition = a_conditions;
	}

	public bool HasCondition(eCondition a_condition)
	{
		return 0 < (this.m_condition & 1 << (int)a_condition);
	}

	public void Progress(float a_deltaTime)
	{
		if (!this.IsSpawned())
		{
			return;
		}
		DatabaseItem itemFromPos = this.m_inventory.GetItemFromPos(0f, 0f);
		ItemDef itemDef = Items.GetItemDef(itemFromPos.type);
		bool flag = itemDef.buildingIndex > 0;
		if (!this.IsDead())
		{
			this.UpdateConditions(a_deltaTime);
			this.CalculateEnergyHealthKarma(a_deltaTime);
			if (this.m_attackBtnPressed)
			{
				if (this.m_server.IsInSpecialArea(this.GetPosition(), eAreaType.noPvp) || (flag && !Buildings.IsHarmless(itemDef.buildingIndex) && this.m_server.IsInSpecialArea(this.GetPosition(), eAreaType.noBuilding)))
				{
					if (Time.time > this.m_nextForbiddenEventTime)
					{
						this.m_server.SendSpecialEvent(this, eSpecialEvent.forbidden);
						this.m_nextForbiddenEventTime = Time.time + 1f;
					}
				}
				else if (null == this.m_vehicle)
				{
					this.TryAttack(itemFromPos.type, itemDef);
				}
			}
			this.HandleRotation();
			DatabaseItem itemFromPos2 = this.m_inventory.GetItemFromPos(0f, 3f);
			ItemDef itemDef2 = Items.GetItemDef(itemFromPos2.type);
			float num = (!Items.IsShoes(itemFromPos2.type)) ? 0f : itemDef2.healing;
			num -= ((!this.HasCondition(eCondition.pain)) ? 0f : 0.2f);
			this.m_char.AddSpeed(num);
			if (Items.IsShoes(itemFromPos2.type) && this.m_isMoving && null == this.m_vehicle)
			{
				this.DamageItem(0f, 3f);
			}
		}
		this.m_char.m_isWalking = (this.IsAttacking() || itemDef.buildingIndex > 0);
		if (this.m_respawnTime > 0f && Time.time > this.m_respawnTime)
		{
			this.Respawn();
			this.m_respawnTime = -1f;
		}
		for (int i = 0; i < this.m_killXp.Count; i++)
		{
			bool flag2 = Time.time > this.m_killXp[i].deletetime;
			if ((null != this.m_killXp[i].npc && this.m_killXp[i].npc.GetHealth() == 0f) || (this.m_killXp[i].player != null && this.m_killXp[i].player.IsDead()) || flag2)
			{
				if (!flag2)
				{
					this.AddXp((int)this.m_killXp[i].xp);
				}
				this.m_killXp.RemoveAt(i);
				break;
			}
		}
	}

	public void Remove()
	{
		if (this.m_char != null)
		{
			UnityEngine.Object.Destroy(this.m_char.gameObject);
		}
	}

	public float GetHealth()
	{
		return this.m_health;
	}

	public float GetEnergy()
	{
		return this.m_energy;
	}

	public float GetKarma()
	{
		return this.m_karma;
	}

	public bool IsSaint()
	{
		return 199f <= this.m_karma;
	}

	public int GetRank()
	{
		return this.m_rank;
	}

	public float GetRankProgress()
	{
		return this.m_rankProgress;
	}

	public int GetXp()
	{
		return this.m_xp;
	}

	public void AddXp(int a_xp)
	{
		this.m_xp = Mathf.Max(this.m_xp + a_xp, 0);
		this.RecalculateRank();
		this.m_server.SendRankUpdate(this, a_xp);
	}

	public float ChangeHealthBy(float a_delta)
	{
		a_delta = this.HandleDamage(a_delta);
		this.m_health = Mathf.Clamp(this.m_health + a_delta, 0f, 100.001f);
		if (this.IsDead() && this.m_respawnTime < 0f)
		{
			this.ExitVehicle(false);
			if (this.IsSpawned())
			{
				this.m_char.AssignInput(0f, 0f, false, false);
				this.m_char.collider.enabled = false;
			}
			this.DropLootAsContainer();
			this.ResetContainers();
			if (null != this.m_missionMan)
			{
				this.m_missionMan.DeleteMissions(this);
			}
			this.m_condition = 0;
			this.AddXp((int)((float)this.m_xp * 0.05f) * -1);
			this.m_gold = (int)((float)this.m_gold * 0.95f);
			this.m_server.SendMoneyUpdate(this);
			this.m_cantLogoutTime = -1f;
			this.m_respawnTime = Time.time + 5f;
		}
		return this.m_health;
	}

	public float ChangeEnergyBy(float a_delta)
	{
		this.m_energy = Mathf.Clamp(this.m_energy + a_delta, 0f, 100.001f);
		return this.m_energy;
	}

	public float ChangeKarmaBy(float a_delta)
	{
		this.m_karma = Mathf.Clamp(this.m_karma + a_delta, 0f, 200.001f);
		if ((int)this.m_karma != (int)this.m_lastUpdateKarma)
		{
			this.m_updateInfoFlag = true;
			this.m_lastUpdateKarma = this.m_karma;
		}
		return this.m_karma;
	}

	public bool IsDead()
	{
		return this.m_health < 1f;
	}

	public bool IsVisible()
	{
		return this.IsSpawned() && this.m_char.collider.enabled;
	}

	public void AssignInput(float a_v, float a_h, bool a_use, bool a_space, float a_buildRotation)
	{
		if (null == this.m_vehicle)
		{
			if (this.IsSpawned())
			{
				this.m_char.AssignInput(a_v, a_h, a_use, a_space);
				this.m_attackBtnPressed = a_space;
			}
		}
		else if (this.m_onlineId == this.m_vehicle.m_data.passengerIds[0])
		{
			this.m_vehicle.AssignInput(a_v, a_h, a_space);
			this.m_attackBtnPressed = false;
		}
		this.m_isMoving = (a_v != 0f || 0f != a_h);
		this.m_buildRotation = a_buildRotation;
	}

	public int GetVehicleId()
	{
		return (!(null == this.m_vehicle)) ? this.m_vehicle.m_id : -1;
	}

	public bool ExitVehicle(bool a_force = false)
	{
		if (null != this.m_vehicle && Time.time > this.m_interactVehicleTime)
		{
			Vector3 passengerExitPos = this.m_vehicle.GetPassengerExitPos(this.m_onlineId);
			if (Vector3.zero == passengerExitPos && !this.IsDead() && !a_force)
			{
				this.m_server.SendSpecialEvent(this, eSpecialEvent.carExitsBlocked);
			}
			else if (Vector3.zero != passengerExitPos || this.IsDead() || a_force)
			{
				this.SetPosition((!(Vector3.zero != passengerExitPos)) ? this.m_vehicle.transform.position : passengerExitPos);
				this.m_vehicle.RemovePassenger(this.m_onlineId);
				this.SetVehicle(null);
				this.m_interactVehicleTime = Time.time + 1f;
				return true;
			}
		}
		return false;
	}

	public ServerVehicle GetVehicle()
	{
		return this.m_vehicle;
	}

	public bool CanEnterExitVehicle()
	{
		return Time.time > this.m_interactVehicleTime;
	}

	public bool SetVehicle(ServerVehicle a_vehicle)
	{
		if (Time.time > this.m_interactVehicleTime)
		{
			this.m_vehicle = a_vehicle;
			this.m_interactVehicleTime = Time.time + 1f;
			if (this.m_char != null)
			{
				this.m_char.collider.enabled = (null == this.m_vehicle);
			}
			return true;
		}
		return false;
	}

	private void RecalculateRank()
	{
		int num = Mathf.Max(this.m_xp, 1);
		float num2 = (float)num * 0.001f;
		this.m_rank = 0;
		if (num2 >= 0.25f && num2 < 1f)
		{
			this.m_rank = ((num2 >= 0.5f) ? 2 : 1);
		}
		else if (num2 >= 1f)
		{
			this.m_rank = Mathf.Max(3 + (int)Mathf.Log(num2, 2f), 0);
		}
		int num3 = (0 >= this.m_rank) ? 0 : ((int)(Mathf.Pow(2f, (float)(this.m_rank - 3)) * 1000f));
		int num4 = (int)(Mathf.Pow(2f, (float)(this.m_rank - 2)) * 1000f);
		this.m_rankProgress = (float)(num - num3) / (float)(num4 - num3);
		if (this.m_lastRank != this.m_rank)
		{
			if (this.m_lastRank != -1)
			{
				this.m_updateInfoFlag = true;
			}
			this.m_lastRank = this.m_rank;
		}
	}

	private float HandleDamage(float a_healthDif)
	{
		float num = a_healthDif;
		if (num < -2f)
		{
			this.m_cantLogoutTime = Time.time + 20f;
			DatabaseItem itemFromPos = this.m_inventory.GetItemFromPos(0f, 2f);
			if (Items.IsBody(itemFromPos.type))
			{
				num *= Items.GetItemDef(itemFromPos.type).healing;
				this.DamageItem(0f, 2f);
			}
			if (Mathf.Abs(num) > UnityEngine.Random.Range(0f, 140f))
			{
				switch (UnityEngine.Random.Range(0, 3))
				{
				case 0:
					this.SetCondition(eCondition.bleeding, true);
					break;
				case 1:
					this.SetCondition(eCondition.infection, true);
					break;
				case 2:
					this.SetCondition(eCondition.pain, true);
					break;
				}
			}
		}
		return num;
	}

	private void HandleRotation()
	{
		float num = -1f;
		if (this.IsAttacking())
		{
			if (null != this.m_victim)
			{
				num = Quaternion.LookRotation(this.m_victim.position - this.m_char.transform.position).eulerAngles.y;
			}
			else if (this.m_forcedRotation > -1f)
			{
				num = this.m_forcedRotation;
				this.m_forcedRotation = -1f;
			}
		}
		if (num != -1f)
		{
			this.m_char.SetForceRotation(num);
		}
	}

	private void DropLootAsContainer()
	{
		if (this.m_inventory != null)
		{
			for (int i = 0; i < this.m_inventory.m_items.Count; i++)
			{
				this.m_server.CreateTempContainerItem(this.m_inventory.m_items[i].type, this.m_inventory.m_items[i].amount, this.GetPosition(), 120);
			}
			this.m_inventory.m_items.Clear();
			this.m_sql.ClearInventory(this.m_pid);
			this.m_updateContainersFlag = true;
			this.m_updateInfoFlag = true;
		}
	}

	private Vector3 Respawn()
	{
		Vector3 vector = Vector3.zero;
		if (Time.time > this.m_nextPossibleBedSpawnTime)
		{
			vector = this.m_buildMan.GetRespawnPos(this.GetPosition(), this.m_pid);
			this.m_nextPossibleBedSpawnTime = Time.time + 60f;
		}
		if (Vector3.zero == vector)
		{
			SpawnPos[] spawnPoints = this.m_server.GetSpawnPoints();
			if (spawnPoints.Length > 0)
			{
				vector = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform.position;
				vector.x += UnityEngine.Random.Range(-this.m_rndSpawnRadius, this.m_rndSpawnRadius);
				vector.y = 0f;
				vector.z += UnityEngine.Random.Range(-this.m_rndSpawnRadius, this.m_rndSpawnRadius);
			}
		}
		if (this.IsSpawned())
		{
			this.SetPosition(vector);
			this.ChangeHealthBy(100f);
			this.ChangeEnergyBy(100f);
		}
		this.m_char.collider.enabled = true;
		this.m_buildMan.IgnoreBedCollision(this.m_pid, this.m_char.collider);
		return vector;
	}

	private void TryAttack(int a_itemType, ItemDef a_handItemDef)
	{
		if (a_handItemDef.buildingIndex > 0)
		{
			bool flag = this.m_buildMan.CreateBuilding(a_handItemDef.buildingIndex, this.m_char.transform.position + this.GetForward() * 2f, this.m_pid, this.m_buildRotation, 100, true);
			if (flag)
			{
				this.m_inventory.DeclineHandItem();
				this.AddXp(20);
				if (a_handItemDef.buildingIndex == 102 && this.IsSaint())
				{
					this.ChangeKarmaBy(-2.5f);
				}
				this.m_updateContainersFlag = true;
				this.m_updateInfoFlag = true;
			}
		}
		else
		{
			ItemDef itemDef = a_handItemDef;
			if (itemDef.damage == 0f)
			{
				itemDef = Items.GetItemDef(0);
			}
			if (Time.time > this.m_nextAttackTime)
			{
				if (this.FireWeapon(itemDef))
				{
					if (!this.HandleSpecialItemAbility(a_itemType))
					{
						Vector3 a_targetPos = this.m_char.transform.position + this.GetForward() * 1.2f;
						Transform victim = this.m_victim;
						float a_weaponDamage = Raycaster.Attack(this.m_char.transform, itemDef, a_targetPos, ref victim);
						this.CalculateXpAndKarma(victim, a_weaponDamage);
					}
					this.m_nextAttackTime = Time.time + itemDef.attackdur;
				}
				else if (Time.time > this.m_nextOutOfAmmoMsgTime)
				{
					this.m_server.SendSpecialEvent(this, eSpecialEvent.noAmmo);
					this.m_nextOutOfAmmoMsgTime = Time.time + 1f;
				}
			}
		}
	}

	private bool FireWeapon(ItemDef a_weaponItem)
	{
		bool flag = true;
		if (a_weaponItem.ammoItemType > 0)
		{
			flag = false;
			if (this.m_inventory.m_items.Count > 0)
			{
				for (int i = 0; i < this.m_inventory.m_items.Count; i++)
				{
					if (this.m_inventory.m_items[i].type == a_weaponItem.ammoItemType)
					{
						DatabaseItem value = this.m_inventory.m_items[i];
						value.amount--;
						if (value.amount < 1)
						{
							value.flag = eDbAction.delete;
						}
						else
						{
							value.flag = eDbAction.update;
						}
						this.m_inventory.m_items[i] = value;
						flag = true;
						break;
					}
				}
			}
		}
		if (flag)
		{
			this.DamageItem(0f, 0f);
		}
		return flag;
	}

	private bool HandleSpecialItemAbility(int a_itemType)
	{
		bool flag = false;
		if (a_itemType == 109)
		{
			this.m_server.Dig(this.GetPosition());
		}
		else if (a_itemType == 92)
		{
			Vector3 a_distCheckPos = this.GetPosition() + this.GetForward() * 1.2f;
			flag = this.m_server.RepairVehicle(this, a_distCheckPos);
			if (!flag)
			{
				flag = this.m_buildMan.RepairBuilding(this, a_distCheckPos);
			}
		}
		else if (a_itemType == 110)
		{
			Vector3 a_pos = this.GetPosition() + this.GetForward() * 3.5f;
			if (0.6f > Util.GetTerrainHeight(a_pos))
			{
				if (UnityEngine.Random.Range(0, 20) == 0)
				{
					this.m_server.CreateFreeWorldItem(11, 1, this.GetPosition());
				}
				else
				{
					this.m_server.SendSpecialEvent(this, eSpecialEvent.fishingfail);
				}
			}
		}
		return flag;
	}

	private void DamageItem(float a_x, float a_y)
	{
		DatabaseItem itemFromPos = this.m_inventory.GetItemFromPos(a_x, a_y);
		ItemDef itemDef = Items.GetItemDef(itemFromPos.type);
		if (0f < itemDef.durability && 1f > itemDef.durability && UnityEngine.Random.Range(0f, 1f) > itemDef.durability && this.m_inventory.DeclineItem(a_x, a_y))
		{
			this.m_updateContainersFlag = true;
			if (itemFromPos.amount == 1)
			{
				this.m_updateInfoFlag = true;
				this.m_server.SendSpecialEvent(this, eSpecialEvent.itemBroke);
				if (0 < itemDef.wood)
				{
					int num = UnityEngine.Random.Range(0, itemDef.wood);
					if (0 < num)
					{
						this.m_server.CreateFreeWorldItem(130, num, this.GetPosition());
					}
				}
				if (0 < itemDef.metal)
				{
					int num2 = UnityEngine.Random.Range(0, itemDef.metal);
					if (0 < num2)
					{
						this.m_server.CreateFreeWorldItem(131, num2, this.GetPosition());
					}
				}
				if (0 < itemDef.stone)
				{
					int num3 = UnityEngine.Random.Range(0, itemDef.stone);
					if (0 < num3)
					{
						this.m_server.CreateFreeWorldItem(132, num3, this.GetPosition());
					}
				}
				if (0 < itemDef.cloth)
				{
					int num4 = UnityEngine.Random.Range(0, itemDef.cloth);
					if (0 < num4)
					{
						this.m_server.CreateFreeWorldItem(133, num4, this.GetPosition());
					}
				}
			}
		}
	}

	private void UpdateConditions(float a_deltaTime)
	{
		bool flag = a_deltaTime > UnityEngine.Random.Range(0f, 150f);
		if (this.HasCondition(eCondition.bleeding) && flag)
		{
			this.SetCondition(eCondition.bleeding, false);
			flag = false;
		}
		if (this.HasCondition(eCondition.infection) && flag)
		{
			this.SetCondition(eCondition.infection, false);
			flag = false;
		}
		if (this.HasCondition(eCondition.pain) && flag)
		{
			this.SetCondition(eCondition.pain, false);
		}
		this.SetCondition(eCondition.starvation, this.m_energy < 0.01f);
		if (Time.time > this.m_nextComplexConditionUpdate && this.m_inventory != null)
		{
			Vector3 position = this.GetPosition();
			DatabaseItem itemFromPos = this.m_inventory.GetItemFromPos(0f, 2f);
			bool a_state = 0.1f > this.m_server.GetDayLight() && (!Items.IsBody(itemFromPos.type) && !this.m_server.IsInSpecialArea(position, eAreaType.warm)) && false == this.m_buildMan.IsNearCampfire(position);
			this.SetCondition(eCondition.freezing, a_state);
			this.SetCondition(eCondition.radiation, this.m_server.IsInSpecialArea(position, eAreaType.radiation));
			this.m_nextComplexConditionUpdate = Time.time + 3f;
		}
		if (this.m_lastCondition != this.m_condition)
		{
			this.m_lastCondition = this.m_condition;
			this.m_server.SendConditionUpdate(this);
		}
	}

	private void CalculateEnergyHealthKarma(float a_deltaTime)
	{
		float num = 0f;
		float num2 = this.m_energyLossPerSec;
		if (this.HasCondition(eCondition.starvation))
		{
			num -= 0.2f;
		}
		if (this.HasCondition(eCondition.bleeding))
		{
			num -= 0.3f;
		}
		if (this.HasCondition(eCondition.infection))
		{
			num -= 0.15f;
		}
		if (this.HasCondition(eCondition.radiation))
		{
			num -= 0.1f;
		}
		if (this.m_healEndTime > Time.time)
		{
			num = this.m_healHealthGainPerSec;
		}
		else if (num == 0f)
		{
			num = this.m_healthGainPerSec * (this.m_energy * 0.01f);
		}
		if (this.HasCondition(eCondition.freezing) || this.HasCondition(eCondition.radiation))
		{
			num2 *= 2f;
		}
		this.ChangeHealthBy(a_deltaTime * num);
		this.ChangeEnergyBy(a_deltaTime * -num2);
		this.ChangeKarmaBy(a_deltaTime * this.m_karmaGainPerSec);
	}

	private void CalculateXpAndKarma(Transform a_victim, float a_weaponDamage)
	{
		if (null != a_victim && 0f < a_weaponDamage)
		{
			ServerNpc serverNpc = (a_victim.gameObject.layer != 9) ? null : a_victim.GetComponent<ServerNpc>();
			if (null == serverNpc)
			{
				if (a_victim.gameObject.layer == 13)
				{
					ControlledChar component = a_victim.GetComponent<ControlledChar>();
					ServerPlayer serverPlayer = (!(null != component)) ? null : component.GetServerPlayer();
					if (serverPlayer != null && !serverPlayer.IsSaint())
					{
						if (8f < serverPlayer.m_karma)
						{
							float num = a_weaponDamage * 0.5f * (serverPlayer.m_karma / 200f);
							if (this.IsSaint())
							{
								num = Mathf.Max(2.5f, num);
							}
							this.ChangeKarmaBy(-num);
						}
						else if (this.IsSaint())
						{
							this.ChangeKarmaBy(-2.5f);
						}
					}
				}
				else if (a_victim.gameObject.layer == 19)
				{
					ServerBuilding component2 = a_victim.GetComponent<ServerBuilding>();
					if (null != component2 && this.m_pid != component2.GetOwnerId() && this.IsSaint())
					{
						this.ChangeKarmaBy(-2.5f);
					}
				}
				else if (a_victim.gameObject.layer == 11)
				{
					ServerVehicle component3 = a_victim.GetComponent<ServerVehicle>();
					if (null != component3 && 0 < component3.GetPassengerCount())
					{
						float num2 = a_weaponDamage * 0.25f;
						if (this.IsSaint())
						{
							num2 = Mathf.Max(2.5f, num2);
						}
						this.ChangeKarmaBy(-num2);
					}
				}
			}
			else
			{
				int handItem = serverNpc.GetHandItem();
				float num3 = Mathf.Min(serverNpc.GetLastHealth(), a_weaponDamage);
				float num4 = num3 * 0.2f + Items.GetWeaponXpMultiplier(handItem) * (num3 * 0.01f);
				if (0f < num4)
				{
					bool flag = true;
					for (int i = 0; i < this.m_killXp.Count; i++)
					{
						if (this.m_killXp[i].player == null && serverNpc == this.m_killXp[i].npc)
						{
							KillXp value = this.m_killXp[i];
							value.xp += num4;
							value.deletetime = Time.time + 20f;
							this.m_killXp[i] = value;
							flag = false;
							break;
						}
					}
					if (flag)
					{
						KillXp item = new KillXp(null, serverNpc, num4, Time.time + 20f);
						this.m_killXp.Add(item);
					}
				}
			}
		}
	}

	private const float c_xpForgetTime = 20f;

	private const int c_npcLayer = 9;

	private const int c_playerLayer = 13;

	private const int c_buildingLayer = 19;

	private const int c_vehicleLayer = 11;

	public int m_onlineId = -1;

	public string m_name = string.Empty;

	public bool m_isAdmin;

	public int m_partyId;

	public int m_partyRank;

	public int m_partyInviteId;

	public float m_nextPartyActionTime;

	public eCharType m_charType;

	public NetConnection m_connection;

	public float m_nextUpdate;

	public int m_updateCount;

	public int m_pid = -1;

	public ulong m_accountId;

	public int m_lookIndex;

	public int m_skinIndex;

	public ItemContainer m_freeWorldContainer;

	public ItemContainer m_persistentContainer;

	public ShopContainer m_shopContainer;

	public bool m_updateInfoFlag = true;

	public bool m_updateContainersFlag = true;

	public int m_gold;

	public float m_cantLogoutTime = -1f;

	public float m_disconnectTime = -1f;

	public ItemContainer m_inventory;

	private ControlledChar m_char;

	private ServerVehicle m_vehicle;

	private float m_health = 100f;

	private float m_energy = 100f;

	private float m_karma = 100f;

	private float m_lastUpdateKarma = 100f;

	private int m_xp;

	private int m_lastRank = -1;

	private int m_condition;

	private int m_lastCondition;

	private int m_rank;

	private float m_rankProgress;

	private float m_healthGainPerSec = 0.6f;

	private float m_healHealthGainPerSec = 10f;

	private float m_energyLossPerSec = 0.1f;

	private float m_karmaGainPerSec = 0.0075f;

	private float m_healEndTime;

	private SQLThreadManager m_sql;

	private LidServer m_server;

	private BuildingManager m_buildMan;

	private MissionManager m_missionMan;

	private Transform m_victim;

	private List<KillXp> m_killXp = new List<KillXp>();

	private float m_rndSpawnRadius = 3f;

	private bool m_attackBtnPressed;

	private float m_buildRotation;

	private float m_forcedRotation = -1f;

	private bool m_isMoving;

	private float m_respawnTime = -1f;

	private float m_nextAttackTime;

	private float m_nextForbiddenEventTime;

	private float m_nextComplexConditionUpdate;

	private float m_nextPossibleBedSpawnTime;

	private float m_interactVehicleTime = -1f;

	private float m_nextOutOfAmmoMsgTime;
}

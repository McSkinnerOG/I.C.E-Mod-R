using System;
using UnityEngine;

public class ServerBuilding : MonoBehaviour
{
	public ServerBuilding()
	{
	}

	public int GetOwnerId()
	{
		return this.m_ownerPid;
	}

	public virtual bool Use(ServerPlayer a_player)
	{
		return true;
	}

	public virtual bool Repair(ServerPlayer a_player)
	{
		this.m_decayTime = this.m_def.decayTime;
		if (null != this.m_sql)
		{
			this.m_dbBuilding.health = 100;
			this.m_dbBuilding.flag = eDbAction.update;
			this.m_sql.SaveBuilding(this.m_dbBuilding);
			this.m_server.SendSpecialEvent(a_player, eSpecialEvent.buildingRepaired);
			return true;
		}
		return false;
	}

	public virtual float GetState()
	{
		return (this.m_def.decayTime <= 0.0) ? 1f : Mathf.Clamp01((float)(this.m_decayTime / this.m_def.decayTime));
	}

	protected virtual void Awake()
	{
		short num = (short)(base.transform.position.x * 10f);
		short num2 = (short)(base.transform.position.z * 10f);
		Vector3 position = new Vector3((float)num * 0.1f, 0f, (float)num2 * 0.1f);
		base.transform.position = position;
		this.m_def = Buildings.GetBuildingDef(this.m_type);
		if (!Global.isServer && !this.m_isStatic)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	public virtual void Init(LidServer a_server, int a_type, int a_ownerPid = 0, int a_health = 100, bool a_isNew = true)
	{
		this.m_isStatic = false;
		this.m_server = a_server;
		this.m_type = a_type;
		this.m_ownerPid = a_ownerPid;
		this.m_def = Buildings.GetBuildingDef(this.m_type);
		this.m_decayTime = this.m_def.decayTime * (double)((float)a_health / 100f);
		if (this.m_def.persistent)
		{
			this.m_dbBuilding = new DatabaseBuilding(a_type, base.transform.position.x * 1.00001f, base.transform.position.z * 1.00001f, base.transform.rotation.eulerAngles.y * 1.00001f, a_ownerPid, a_health);
			this.m_sql = this.m_server.GetSql();
			if (null != this.m_sql)
			{
				if (a_isNew)
				{
					this.m_dbBuilding.flag = eDbAction.insert;
					this.m_sql.SaveBuilding(this.m_dbBuilding);
				}
				this.m_nextSqlUpdateTime = UnityEngine.Random.Range(100f, 300f);
			}
		}
	}

	protected virtual void Update()
	{
		if (!this.m_isStatic && this.m_def.decayTime > 0.0)
		{
			if (this.m_gotDamage > 0f && null != this.m_gotAttacker)
			{
				this.m_decayTime -= (double)(this.m_gotDamage * (float)this.m_damageInSeconds);
				this.m_gotAttacker = null;
				this.m_gotDamage = 0f;
			}
			this.m_decayTime -= (double)Time.deltaTime;
			if (this.m_decayTime <= 0.0)
			{
				this.m_buildingIsDead = true;
				this.RemoveFromSQL();
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else if (null != this.m_sql && Time.time > this.m_nextSqlUpdateTime)
			{
				this.m_dbBuilding.health = (int)(this.m_decayTime / this.m_def.decayTime * 100.0);
				this.m_dbBuilding.flag = eDbAction.update;
				this.m_sql.SaveBuilding(this.m_dbBuilding);
				this.m_nextSqlUpdateTime = Time.time + UnityEngine.Random.Range(300f, 500f);
			}
		}
	}

	private void RemoveFromSQL()
	{
		if (null != this.m_sql)
		{
			this.m_dbBuilding.flag = eDbAction.delete;
			this.m_sql.SaveBuilding(this.m_dbBuilding);
		}
	}

	public void ChangeHealthBy(float a_dif)
	{
		this.m_gotDamage = -a_dif;
	}

	public void SetAggressor(Transform a_aggressor)
	{
		this.m_gotAttacker = a_aggressor;
	}

	public int m_type = 1;

	public int m_damageInSeconds = 100;

	protected bool m_isStatic = true;

	protected int m_ownerPid;

	protected float m_gotDamage;

	protected Transform m_gotAttacker;

	protected BuildingDef m_def = default(BuildingDef);

	protected LidServer m_server;

	protected double m_decayTime = 86400.0;

	protected SQLThreadManager m_sql;

	protected bool m_buildingIsDead;

	private DatabaseBuilding m_dbBuilding;

	private float m_nextSqlUpdateTime;
}

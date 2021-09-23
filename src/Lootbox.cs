using System;
using UnityEngine;

public class Lootbox : ServerBuilding
{
	public Lootbox()
	{
	}

	private void CreateCid()
	{
		float num = base.transform.position.x + 10000f;
		float num2 = base.transform.position.z + 10000f;
		this.m_cid = (int)(num * 10.00001f + num2 * 20.00001f);
	}

	public override bool Use(ServerPlayer a_player)
	{
		if (this.m_loadItemsFromDb)
		{
			this.m_sql.RequestContainer(this.m_cid);
			this.m_loadItemsFromDb = false;
		}
		a_player.m_persistentContainer = this.m_container;
		a_player.m_updateContainersFlag = true;
		return base.Use(a_player);
	}

	protected override void Awake()
	{
		this.CreateCid();
		base.Awake();
	}

	public override void Init(LidServer a_server, int a_type, int a_ownerPid = 0, int a_health = 100, bool a_isNew = true)
	{
		this.CreateCid();
		this.m_sql = a_server.GetSql();
		if (null != this.m_sql)
		{
			this.m_container = new ItemContainer(4, 4, 6, this.m_cid, this.m_sql, null);
			this.m_container.m_position = base.transform.position;
			if (!a_isNew)
			{
				this.m_loadItemsFromDb = true;
			}
		}
		base.Init(a_server, a_type, a_ownerPid, a_health, a_isNew);
	}

	private void OnDestroy()
	{
		if (this.m_buildingIsDead && Global.isServer && null != this.m_sql && this.m_container != null)
		{
			Debug.Log("Lootbox.OnDestroy() has been called ... deleting items in box");
			this.m_container.DeleteItems();
		}
	}

	[HideInInspector]
	public ItemContainer m_container;

	[HideInInspector]
	public int m_cid;

	private bool m_loadItemsFromDb;
}

using System;
using UnityEngine;

public class PersistentContainer : MonoBehaviour
{
	public PersistentContainer()
	{
	}

	private void Awake()
	{
		if (Global.isServer)
		{
			this.m_cid = (int)(base.transform.position.x * 10f + base.transform.position.z * 20f);
			this.m_sql = (SQLThreadManager)UnityEngine.Object.FindObjectOfType(typeof(SQLThreadManager));
			this.m_container = new ItemContainer(4, 4, 6, this.m_cid, this.m_sql, null);
			this.m_container.m_position = base.transform.position;
			this.SetNextDropTime();
		}
		else
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Update()
	{
		if (Global.isServer && this.m_container != null && Time.time > this.m_nextDropOnEmptyTime)
		{
			if (this.m_container.Count() == 0 && UnityEngine.Random.Range(0, 5) == 0)
			{
				DatabaseItem a_item = new DatabaseItem(Items.GetRandomType(70f), 0f, 0f, 1, false, 0, 0);
				a_item.amount = ((!Items.HasCondition(a_item.type)) ? 1 : UnityEngine.Random.Range(1, 50));
				this.m_container.CollectItem(a_item, true, default(Vector3));
			}
			this.SetNextDropTime();
		}
	}

	private void SetNextDropTime()
	{
		this.m_nextDropOnEmptyTime = Time.time + UnityEngine.Random.Range(600f, 7200f);
	}

	private void OnDestroy()
	{
		if (Global.isServer && null != this.m_sql)
		{
			this.m_container.DeleteItems();
		}
	}

	public bool m_dropOnEmpty = true;

	[HideInInspector]
	public ItemContainer m_container;

	private float m_nextDropOnEmptyTime;

	private int m_cid;

	private SQLThreadManager m_sql;
}

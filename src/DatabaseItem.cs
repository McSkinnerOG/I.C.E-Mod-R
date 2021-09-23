using System;
using UnityEngine;

public struct DatabaseItem
{
	public DatabaseItem(int a_type, float a_x = 0f, float a_y = 0f, int a_amount = 1, bool a_hidden = false, int a_cid = 0, int a_iid = 0)
	{
		this.type = a_type;
		this.amount = a_amount;
		this.hidden = a_hidden;
		this.iid = a_iid;
		this.x = a_x;
		this.y = a_y;
		this.cid = a_cid;
		this.dropTime = Time.time;
		this.dropPlayerId = 0;
		this.flag = eDbAction.none;
	}

	public Vector3 GetPos()
	{
		return new Vector3(this.x, 0f, this.y);
	}

	public int iid;

	public int cid;

	public bool hidden;

	public int type;

	public int amount;

	public float x;

	public float y;

	public eDbAction flag;

	public float dropTime;

	public int dropPlayerId;
}

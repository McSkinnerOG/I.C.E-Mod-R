using System;
using UnityEngine;

public struct DatabaseBuilding
{
	public DatabaseBuilding(int a_type, float a_x = 0f, float a_y = 0f, float a_rot = 0f, int a_pid = 0, int a_health = 100)
	{
		this.type = a_type;
		this.x = a_x;
		this.y = a_y;
		this.rot = a_rot;
		this.pid = a_pid;
		this.health = a_health;
		this.flag = eDbAction.none;
	}

	public Vector3 GetPos()
	{
		return new Vector3(this.x, 0f, this.y);
	}

	public int pid;

	public int type;

	public int health;

	public float x;

	public float y;

	public float rot;

	public eDbAction flag;
}

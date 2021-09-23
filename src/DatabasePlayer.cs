using System;
using UnityEngine;

public struct DatabasePlayer
{
	public DatabasePlayer(ulong a_aid, string a_name = "", int a_pid = 0, float a_x = 0f, float a_y = 0f, int a_h = 100, int a_e = 100, int a_k = 100, int a_xp = 0, int a_condition = 0, int a_gold = 0, int a_partyId = 0, int a_partyRank = 0)
	{
		this.aid = a_aid;
		this.pid = a_pid;
		this.name = a_name;
		this.x = a_x;
		this.y = a_y;
		this.health = a_h;
		this.energy = a_e;
		this.karma = a_k;
		this.xp = a_xp;
		this.condition = a_condition;
		this.gold = a_gold;
		this.partyId = a_partyId;
		this.partyRank = a_partyRank;
	}

	public Vector3 GetPos()
	{
		return new Vector3(this.x, 0f, this.y);
	}

	public ulong aid;

	public int pid;

	public string name;

	public float x;

	public float y;

	public int health;

	public int energy;

	public int karma;

	public int xp;

	public int condition;

	public int gold;

	public int partyId;

	public int partyRank;
}

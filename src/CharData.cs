using System;

public struct CharData
{
	public CharData(eCharType a_type)
	{
		this.name = string.Empty;
		this.aid = 0UL;
		this.handItem = 0;
		this.look = 0;
		this.skin = 0;
		this.body = 0;
		this.rank = 0;
		this.karma = 100;
		this.type = a_type;
	}

	public string name;

	public ulong aid;

	public int handItem;

	public int look;

	public int body;

	public int skin;

	public int rank;

	public int karma;

	public eCharType type;
}

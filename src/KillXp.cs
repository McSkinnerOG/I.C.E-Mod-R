using System;

public struct KillXp
{
	public KillXp(ServerPlayer a_player, ServerNpc a_npc, float a_xp, float a_time)
	{
		this.player = a_player;
		this.npc = a_npc;
		this.xp = a_xp;
		this.deletetime = a_time;
	}

	public ServerPlayer player;

	public ServerNpc npc;

	public float xp;

	public float deletetime;
}

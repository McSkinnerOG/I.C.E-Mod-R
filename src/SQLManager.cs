using System;

public static class SQLManager
{
	// Note: this type is marked as 'beforefieldinit'.
	static SQLManager()
	{
	}

	public static bool SpawnPlayer(ref ServerPlayer a_player, string a_name)
	{
		SQLWorker.GetOrCreatePlayer(ref a_player, a_name);
		if (a_player != null)
		{
			for (int i = 0; i < SQLManager.m_players.Length; i++)
			{
				if (SQLManager.m_players[i] == null)
				{
					SQLManager.m_players[i] = a_player;
					SQLManager.m_players[i].m_onlineId = i;
					break;
				}
			}
		}
		return true;
	}

	public static ServerPlayer GetPlayer(int a_onlineId)
	{
		if (a_onlineId < 0 && a_onlineId > SQLManager.m_players.Length)
		{
			return null;
		}
		return SQLManager.m_players[a_onlineId];
	}

	public static void DeletePlayer(int a_onlineId)
	{
		if (a_onlineId >= 0 && a_onlineId < SQLManager.m_players.Length)
		{
			SQLManager.m_players[a_onlineId] = null;
		}
	}

	public static ServerPlayer[] m_players = new ServerPlayer[50];
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Mono.Data.SqliteClient;
using UnityEngine;

public class SQLThreadManager : MonoBehaviour
{
	public SQLThreadManager()
	{
	}

	private void Start()
	{
		if (this.m_sqlConnection == null)
		{
			this.m_sqlConnection = new SqliteConnection("URI=file:immune.db");
			this.m_sqlConnection.Close();
			this.m_sqlConnection.Open();
			this.m_sqlCommand = this.m_sqlConnection.CreateCommand();
			string text = "CREATE TABLE IF NOT EXISTS player ( pid INTEGER NOT NULL PRIMARY KEY, aid INTEGER NOT NULL, name TEXT NOT NULL DEFAULT '', x REAL NOT NULL DEFAULT '0.0', y REAL NOT NULL DEFAULT '0.0', health INTEGER NOT NULL DEFAULT '100', energy INTEGER NOT NULL DEFAULT '100', karma INTEGER NOT NULL DEFAULT '100', xp INTEGER NOT NULL DEFAULT '0', condition INTEGER NOT NULL DEFAULT '0', gold INTEGER NOT NULL DEFAULT '0', partyId INTEGER NOT NULL DEFAULT '0', partyRank INTEGER NOT NULL DEFAULT '0' );";
			text += "CREATE TABLE IF NOT EXISTS item ( iid INTEGER NOT NULL PRIMARY KEY, cid INTEGER NOT NULL DEFAULT '0', hidden INTEGER NOT NULL DEFAULT '0', type INTEGER NOT NULL DEFAULT '0', amount INTEGER NOT NULL DEFAULT '0', x REAL NOT NULL DEFAULT '0.0', y REAL NOT NULL DEFAULT '0.0' );";
			text += "CREATE TABLE IF NOT EXISTS building ( bid INTEGER NOT NULL PRIMARY KEY, pid INTEGER NOT NULL DEFAULT '0', type INTEGER NOT NULL DEFAULT '0', health INTEGER NOT NULL DEFAULT '100', x REAL NOT NULL DEFAULT '0.0', y REAL NOT NULL DEFAULT '0.0', rot REAL NOT NULL DEFAULT '0.0' );";
			this.SQLExecute(text);
			this.m_maxPartyId = this.SQLGetInt("SELECT MAX(partyId) FROM player;");
			this.m_thread = new Thread(new ThreadStart(this.ThreadFunc));
			this.m_thread.IsBackground = true;
			this.m_thread.Start();
		}
	}

	private void AddColumnIfNotExists()
	{
		string text = "ALTER TABLE player ADD COLUMN partyId INTEGER NOT NULL DEFAULT '0';ALTER TABLE player ADD COLUMN partyRank INTEGER NOT NULL DEFAULT '0';";
		using (IDbTransaction dbTransaction = this.m_sqlConnection.BeginTransaction())
		{
			this.m_sqlCommand.CommandText = string.Format("PRAGMA table_info({0})", "player");
			using (IDataReader dataReader = this.m_sqlCommand.ExecuteReader())
			{
				while (dataReader.Read())
				{
					if ("partyId" == dataReader.GetString(dataReader.GetOrdinal("Name")))
					{
						text = string.Empty;
						break;
					}
				}
				dataReader.Close();
			}
			dbTransaction.Commit();
		}
		if (string.Empty != text)
		{
			this.SQLExecute(text);
		}
	}

	private void OnApplicationQuit()
	{
		if (this.m_thread != null)
		{
			this.m_thread.Abort();
		}
	}

	public int GetMaxPartyId()
	{
		return this.m_maxPartyId;
	}

	public int PidToCid(int a_pid)
	{
		if (a_pid == 0)
		{
			return 0;
		}
		return (1000000000 >= a_pid) ? (a_pid + 1000000000) : a_pid;
	}

	public int CidToPid(int a_cid)
	{
		if (a_cid == 0)
		{
			return 0;
		}
		return (1000000000 <= a_cid) ? (a_cid - 1000000000) : a_cid;
	}

	public bool IsInventoryContainer(int a_cid)
	{
		return a_cid >= 1000000000;
	}

	public void RequestPlayer(ulong a_id)
	{
		object threadLock = this.m_threadLock;
		lock (threadLock)
		{
			if (!this.m_playerRequests.Contains(a_id))
			{
				this.m_playerRequests.Add(a_id);
			}
		}
	}

	public void RequestParty(int a_id)
	{
		if (0 < a_id)
		{
			object threadLock = this.m_threadLock;
			lock (threadLock)
			{
				if (!this.m_partyRequests.Contains(a_id))
				{
					this.m_partyRequests.Add(a_id);
				}
			}
		}
	}

	public void SavePlayers(ServerPlayer[] a_players, int a_count)
	{
		if (a_players == null || a_players.Length < 1 || a_count < 1)
		{
			return;
		}
		object threadLock = this.m_threadLock;
		lock (threadLock)
		{
			this.m_playersToWrite = new List<DatabasePlayer>(a_count);
			for (int i = 0; i < a_players.Length; i++)
			{
				if (a_players[i] != null && a_players[i].IsSpawned())
				{
					this.m_playersToWrite.Add(new DatabasePlayer(a_players[i].m_accountId, this.saveStr(a_players[i].m_name), a_players[i].m_pid, a_players[i].GetPosition().x, a_players[i].GetPosition().z, (int)(a_players[i].GetHealth() + 0.5f), (int)(a_players[i].GetEnergy() + 0.5f), (int)a_players[i].GetKarma(), a_players[i].GetXp(), a_players[i].GetConditions(), a_players[i].m_gold, a_players[i].m_partyId, a_players[i].m_partyRank));
				}
			}
		}
	}

	public void SavePlayer(ServerPlayer a_player, bool a_justUpdateParty = false)
	{
		if (a_player == null)
		{
			return;
		}
		object threadLock = this.m_threadLock;
		lock (threadLock)
		{
			List<DatabasePlayer> list = (!a_justUpdateParty) ? this.m_playersToWrite : this.m_partyPlayersToWrite;
			list.Add(new DatabasePlayer(a_player.m_accountId, this.saveStr(a_player.m_name), a_player.m_pid, a_player.GetPosition().x, a_player.GetPosition().z, (int)(a_player.GetHealth() + 0.5f), (int)(a_player.GetEnergy() + 0.5f), (int)a_player.GetKarma(), a_player.GetXp(), a_player.GetConditions(), a_player.m_gold, a_player.m_partyId, a_player.m_partyRank));
		}
	}

	public void SavePartyPlayer(DatabasePlayer a_player)
	{
		if (a_player.aid == 0UL)
		{
			return;
		}
		object threadLock = this.m_threadLock;
		lock (threadLock)
		{
			this.m_partyPlayersToWrite.Add(a_player);
		}
	}

	public DatabasePlayer[] PopRequestedPlayers()
	{
		DatabasePlayer[] result = null;
		object threadLock = this.m_threadLock;
		lock (threadLock)
		{
			if (0 < this.m_playerAnswers.Count)
			{
				result = this.m_playerAnswers.ToArray();
				this.m_playerAnswers.Clear();
			}
		}
		return result;
	}

	public DatabasePlayer[] PopRequestedParty()
	{
		DatabasePlayer[] result = null;
		object threadLock = this.m_threadLock;
		lock (threadLock)
		{
			if (0 < this.m_partyAnswers.Count)
			{
				result = this.m_partyAnswers.ToArray();
				this.m_partyAnswers.Clear();
			}
		}
		return result;
	}

	public void RequestContainer(int a_cid)
	{
		object threadLock = this.m_threadLock;
		lock (threadLock)
		{
			this.m_containerRequests.Add(a_cid);
		}
	}

	public void ClearInventory(int a_pid)
	{
		object threadLock = this.m_threadLock;
		lock (threadLock)
		{
			this.m_inventoryClear.Add(a_pid);
		}
	}

	public void RequestHiddenItems(Vector3 a_pos)
	{
		object threadLock = this.m_threadLock;
		lock (threadLock)
		{
			this.m_hiddenItemsRequests.Add(a_pos);
		}
	}

	public void SaveItem(DatabaseItem a_item)
	{
		object threadLock = this.m_threadLock;
		lock (threadLock)
		{
			this.m_itemsToWrite.Add(a_item);
		}
	}

	public void RequestBuildings()
	{
		object threadLock = this.m_threadLock;
		lock (threadLock)
		{
			this.m_requestBuildings = true;
		}
	}

	public void SaveBuilding(DatabaseBuilding a_building)
	{
		object threadLock = this.m_threadLock;
		lock (threadLock)
		{
			this.m_buildingsToWrite.Add(a_building);
		}
	}

	public DatabaseBuilding[] PopRequestedBuildings()
	{
		DatabaseBuilding[] result = null;
		object threadLock = this.m_threadLock;
		lock (threadLock)
		{
			if (0 < this.m_buildingAnswers.Count)
			{
				result = this.m_buildingAnswers.ToArray();
				this.m_buildingAnswers.Clear();
			}
		}
		return result;
	}

	public DatabaseItem[] PopRequestedItems()
	{
		DatabaseItem[] result = null;
		object threadLock = this.m_threadLock;
		lock (threadLock)
		{
			if (0 < this.m_itemAnswers.Count)
			{
				result = this.m_itemAnswers.ToArray();
				this.m_itemAnswers.Clear();
			}
		}
		return result;
	}

	private void ThreadFunc()
	{
		int num = 0;
		for (;;)
		{
			Thread.Sleep(25);
			num++;
			if (num % 2 == 0)
			{
				ulong[] a_ids = null;
				int num2 = 0;
				DatabasePlayer[] a_players = null;
				DatabasePlayer[] a_players2 = null;
				bool flag = false;
				object threadLock = this.m_threadLock;
				lock (threadLock)
				{
					a_ids = this.m_playerRequests.ToArray();
					this.m_playerRequests.Clear();
					if (0 < this.m_partyRequests.Count)
					{
						num2 = this.m_partyRequests[0];
						this.m_partyRequests.RemoveAt(0);
					}
					a_players = this.m_playersToWrite.ToArray();
					this.m_playersToWrite.Clear();
					a_players2 = this.m_partyPlayersToWrite.ToArray();
					this.m_partyPlayersToWrite.Clear();
					flag = this.m_requestBuildings;
					this.m_requestBuildings = false;
				}
				this.SQLGetOrCreatePlayer(a_ids);
				this.SQLUpdatePlayers(a_players);
				this.SQLUpdatePartyPlayers(a_players2);
				if (num2 != 0)
				{
					this.SQLGetParty(num2);
				}
				if (flag)
				{
					this.SQLGetBuildings();
				}
			}
			else
			{
				Vector3[] a_hiPos = null;
				int[] a_cids = null;
				int[] a_inClearRequestees = null;
				DatabaseItem[] a_changedItems = null;
				DatabaseBuilding[] a_changedBuildings = null;
				object threadLock2 = this.m_threadLock;
				lock (threadLock2)
				{
					a_hiPos = this.m_hiddenItemsRequests.ToArray();
					this.m_hiddenItemsRequests.Clear();
					a_cids = this.m_containerRequests.ToArray();
					this.m_containerRequests.Clear();
					a_inClearRequestees = this.m_inventoryClear.ToArray();
					this.m_inventoryClear.Clear();
					a_changedItems = this.m_itemsToWrite.ToArray();
					this.m_itemsToWrite.Clear();
					a_changedBuildings = this.m_buildingsToWrite.ToArray();
					this.m_buildingsToWrite.Clear();
				}
				this.SQLClearInventory(a_inClearRequestees);
				this.SQLRevealItems(a_hiPos);
				this.SQLGetContainerItems(a_cids);
				this.SQLChangeItems(a_changedItems);
				this.SQLChangeBuildings(a_changedBuildings);
			}
		}
	}

	private void SQLGetOrCreatePlayer(ulong[] a_ids)
	{
		foreach (ulong num in a_ids)
		{
			DatabasePlayer item = new DatabasePlayer(num, string.Empty, 0, 0f, 0f, 100, 100, 100, 0, 0, 0, 0, 0);
			using (IDbTransaction dbTransaction = this.m_sqlConnection.BeginTransaction())
			{
				this.m_sqlCommand.CommandText = "SELECT aid, pid, x, y, health, energy, karma, xp, condition, gold, name, partyId, partyRank FROM player WHERE aid='" + num + "' LIMIT 1;";
				using (IDataReader dataReader = this.m_sqlCommand.ExecuteReader())
				{
					if (dataReader.Read())
					{
						item.aid = (ulong)dataReader.GetInt64(0);
						item.pid = dataReader.GetInt32(1);
						item.x = dataReader.GetFloat(2);
						item.y = dataReader.GetFloat(3);
						item.health = dataReader.GetInt32(4);
						item.energy = dataReader.GetInt32(5);
						item.karma = dataReader.GetInt32(6);
						item.xp = dataReader.GetInt32(7);
						item.condition = dataReader.GetInt32(8);
						item.gold = dataReader.GetInt32(9);
						item.name = dataReader.GetString(10);
						item.partyId = dataReader.GetInt32(11);
						item.partyRank = dataReader.GetInt32(12);
					}
					dataReader.Close();
				}
				dbTransaction.Commit();
			}
			if (item.pid == 0)
			{
				item.pid = this.SQLExecuteAndGetId("INSERT INTO player (aid) VALUES('" + num + "');");
			}
			object threadLock = this.m_threadLock;
			lock (threadLock)
			{
				this.m_playerAnswers.Add(item);
			}
		}
	}

	private void SQLGetParty(int a_id)
	{
		if (1 > a_id)
		{
			return;
		}
		DatabasePlayer item = new DatabasePlayer(0UL, string.Empty, 0, 0f, 0f, 100, 100, 100, 0, 0, 0, 0, 0);
		using (IDbTransaction dbTransaction = this.m_sqlConnection.BeginTransaction())
		{
			this.m_sqlCommand.CommandText = "SELECT aid, pid, name, partyId, partyRank FROM player WHERE partyId=" + a_id + ";";
			using (IDataReader dataReader = this.m_sqlCommand.ExecuteReader())
			{
				while (dataReader.Read())
				{
					item.aid = (ulong)dataReader.GetInt64(0);
					item.pid = dataReader.GetInt32(1);
					item.name = dataReader.GetString(2);
					item.partyId = dataReader.GetInt32(3);
					item.partyRank = dataReader.GetInt32(4);
					object threadLock = this.m_threadLock;
					lock (threadLock)
					{
						this.m_partyAnswers.Add(item);
					}
				}
				dataReader.Close();
			}
			dbTransaction.Commit();
		}
	}

	private void SQLUpdatePlayers(DatabasePlayer[] a_players)
	{
		if (a_players != null && a_players.Length > 0)
		{
			string text = string.Empty;
			foreach (DatabasePlayer databasePlayer in a_players)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					"UPDATE player SET name='",
					this.saveStr(databasePlayer.name),
					"', x=",
					databasePlayer.x,
					", y=",
					databasePlayer.y,
					", health=",
					databasePlayer.health,
					", energy=",
					databasePlayer.energy,
					", karma=",
					databasePlayer.karma,
					", xp=",
					databasePlayer.xp,
					", condition=",
					databasePlayer.condition,
					", gold=",
					databasePlayer.gold,
					", partyId=",
					databasePlayer.partyId,
					", partyRank=",
					databasePlayer.partyRank,
					" WHERE pid=",
					databasePlayer.pid,
					";"
				});
			}
			this.SQLExecute(text);
		}
	}

	private void SQLUpdatePartyPlayers(DatabasePlayer[] a_players)
	{
		if (a_players != null && a_players.Length > 0)
		{
			string text = string.Empty;
			foreach (DatabasePlayer databasePlayer in a_players)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					"UPDATE player SET partyId=",
					databasePlayer.partyId,
					", partyRank=",
					databasePlayer.partyRank,
					" WHERE pid=",
					databasePlayer.pid,
					";"
				});
			}
			this.SQLExecute(text);
		}
	}

	private void SQLClearInventory(int[] a_inClearRequestees)
	{
		string text = string.Empty;
		foreach (int a_pid in a_inClearRequestees)
		{
			int num = this.PidToCid(a_pid);
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"DELETE FROM item WHERE cid=",
				num,
				";"
			});
		}
		if (text.Length > 1)
		{
			this.SQLExecute(text);
		}
	}

	private void SQLRevealItems(Vector3[] a_hiPos)
	{
		float num = 0.5f;
		foreach (Vector3 vector in a_hiPos)
		{
			List<DatabaseItem> list = new List<DatabaseItem>();
			using (IDbTransaction dbTransaction = this.m_sqlConnection.BeginTransaction())
			{
				this.m_sqlCommand.CommandText = string.Concat(new object[]
				{
					"SELECT type, x, y, amount, iid FROM item WHERE hidden=1 AND cid=0 AND x>",
					vector.x - num,
					" AND x<",
					vector.x + num,
					" AND y>",
					vector.z - num,
					" AND y<",
					vector.z + num,
					";"
				});
				using (IDataReader dataReader = this.m_sqlCommand.ExecuteReader())
				{
					while (dataReader.Read())
					{
						DatabaseItem item = new DatabaseItem(dataReader.GetInt32(0), dataReader.GetFloat(1), dataReader.GetFloat(2), dataReader.GetInt32(3), false, 0, 0);
						object threadLock = this.m_threadLock;
						lock (threadLock)
						{
							this.m_itemAnswers.Add(item);
						}
						item.flag = eDbAction.delete;
						item.iid = dataReader.GetInt32(4);
						list.Add(item);
					}
					dataReader.Close();
				}
				dbTransaction.Commit();
			}
			if (list.Count > 0)
			{
				this.SQLChangeItems(list.ToArray());
			}
		}
	}

	private void SQLGetContainerItems(int[] a_cids)
	{
		foreach (int num in a_cids)
		{
			using (IDbTransaction dbTransaction = this.m_sqlConnection.BeginTransaction())
			{
				this.m_sqlCommand.CommandText = "SELECT type, x, y, amount, iid FROM item WHERE cid=" + num + ";";
				using (IDataReader dataReader = this.m_sqlCommand.ExecuteReader())
				{
					while (dataReader.Read())
					{
						object threadLock = this.m_threadLock;
						lock (threadLock)
						{
							this.m_itemAnswers.Add(new DatabaseItem(dataReader.GetInt32(0), dataReader.GetFloat(1), dataReader.GetFloat(2), dataReader.GetInt32(3), false, num, dataReader.GetInt32(4)));
						}
					}
					dataReader.Close();
				}
				dbTransaction.Commit();
			}
		}
	}

	private void SQLChangeItems(DatabaseItem[] a_changedItems)
	{
		if (a_changedItems != null && a_changedItems.Length > 0)
		{
			string text = string.Empty;
			foreach (DatabaseItem databaseItem in a_changedItems)
			{
				if (databaseItem.flag == eDbAction.delete)
				{
					string text2 = text;
					text = string.Concat(new object[]
					{
						text2,
						"DELETE FROM item WHERE iid=",
						databaseItem.iid,
						";"
					});
				}
				else if (databaseItem.flag == eDbAction.update)
				{
					string text2 = text;
					text = string.Concat(new object[]
					{
						text2,
						"UPDATE item SET cid=",
						databaseItem.cid,
						", hidden=",
						(!databaseItem.hidden) ? 0 : 1,
						", type=",
						databaseItem.type,
						", amount=",
						databaseItem.amount,
						", x=",
						databaseItem.x,
						", y=",
						databaseItem.y,
						" WHERE iid=",
						databaseItem.iid,
						";"
					});
				}
				else if (databaseItem.flag == eDbAction.insert)
				{
					string text3 = string.Concat(new object[]
					{
						"INSERT INTO item (cid, hidden, type, amount, x, y) VALUES(",
						databaseItem.cid,
						", ",
						(!databaseItem.hidden) ? 0 : 1,
						", ",
						databaseItem.type,
						", ",
						databaseItem.amount,
						", ",
						databaseItem.x,
						", ",
						databaseItem.y,
						");"
					});
					if (databaseItem.cid > 0)
					{
						DatabaseItem item = databaseItem;
						item.iid = this.SQLExecuteAndGetId(text3);
						object threadLock = this.m_threadLock;
						lock (threadLock)
						{
							this.m_itemAnswers.Add(item);
						}
					}
					else
					{
						text += text3;
					}
				}
			}
			if (text.Length > 1)
			{
				this.SQLExecute(text);
			}
		}
	}

	private void SQLGetBuildings()
	{
		using (IDbTransaction dbTransaction = this.m_sqlConnection.BeginTransaction())
		{
			this.m_sqlCommand.CommandText = "SELECT type, x, y, rot, pid, health FROM building;";
			using (IDataReader dataReader = this.m_sqlCommand.ExecuteReader())
			{
				while (dataReader.Read())
				{
					object threadLock = this.m_threadLock;
					lock (threadLock)
					{
						this.m_buildingAnswers.Add(new DatabaseBuilding(dataReader.GetInt32(0), dataReader.GetFloat(1), dataReader.GetFloat(2), dataReader.GetFloat(3), dataReader.GetInt32(4), dataReader.GetInt32(5)));
					}
				}
				dataReader.Close();
			}
			dbTransaction.Commit();
		}
	}

	private void SQLChangeBuildings(DatabaseBuilding[] a_changedBuildings)
	{
		if (a_changedBuildings != null && a_changedBuildings.Length > 0)
		{
			string text = string.Empty;
			foreach (DatabaseBuilding databaseBuilding in a_changedBuildings)
			{
				string text2 = string.Concat(new object[]
				{
					" WHERE x > (",
					databaseBuilding.x,
					"-0.2) AND x < (",
					databaseBuilding.x,
					"+0.2) AND y > (",
					databaseBuilding.y,
					"-0.2) AND y < (",
					databaseBuilding.y,
					"+0.2);"
				});
				if (databaseBuilding.flag == eDbAction.delete)
				{
					text = text + "DELETE FROM building" + text2;
				}
				else if (databaseBuilding.flag == eDbAction.update)
				{
					string text3 = text;
					text = string.Concat(new object[]
					{
						text3,
						"UPDATE building SET pid=",
						databaseBuilding.pid,
						", type=",
						databaseBuilding.type,
						", health=",
						databaseBuilding.health,
						", x=",
						databaseBuilding.x,
						", y=",
						databaseBuilding.y,
						", rot=",
						databaseBuilding.rot,
						text2
					});
				}
				else if (databaseBuilding.flag == eDbAction.insert)
				{
					string text3 = text;
					text = string.Concat(new object[]
					{
						text3,
						"INSERT INTO building (pid, type, health, x, y, rot) VALUES(",
						databaseBuilding.pid,
						", ",
						databaseBuilding.type,
						", ",
						databaseBuilding.health,
						", ",
						databaseBuilding.x,
						", ",
						databaseBuilding.y,
						", ",
						databaseBuilding.rot,
						");"
					});
				}
			}
			if (text.Length > 1)
			{
				this.SQLExecute(text);
			}
		}
	}

	private void SQLExecute(string a_sql)
	{
		using (IDbTransaction dbTransaction = this.m_sqlConnection.BeginTransaction())
		{
			this.m_sqlCommand.CommandText = a_sql;
			this.m_sqlCommand.ExecuteNonQuery();
			dbTransaction.Commit();
		}
	}

	private int SQLExecuteAndGetId(string a_sql)
	{
		int result = -1;
		using (IDbTransaction dbTransaction = this.m_sqlConnection.BeginTransaction())
		{
			this.m_sqlCommand.CommandText = a_sql + "SELECT last_insert_rowid();";
			using (IDataReader dataReader = this.m_sqlCommand.ExecuteReader())
			{
				if (dataReader.Read())
				{
					result = dataReader.GetInt32(0);
				}
				dataReader.Close();
			}
			dbTransaction.Commit();
		}
		return result;
	}

	private int SQLGetInt(string a_sql)
	{
		int result = -1;
		using (IDbTransaction dbTransaction = this.m_sqlConnection.BeginTransaction())
		{
			this.m_sqlCommand.CommandText = a_sql;
			using (IDataReader dataReader = this.m_sqlCommand.ExecuteReader())
			{
				if (dataReader.Read())
				{
					result = dataReader.GetInt32(0);
				}
				dataReader.Close();
			}
			dbTransaction.Commit();
		}
		return result;
	}

	private string saveStr(string a_str)
	{
		return a_str.Replace("'", string.Empty);
	}

	private const int c_pidToCidOffset = 1000000000;

	private int m_maxPartyId = 1;

	private List<DatabasePlayer> m_playersToWrite = new List<DatabasePlayer>();

	private List<ulong> m_playerRequests = new List<ulong>();

	private List<DatabasePlayer> m_playerAnswers = new List<DatabasePlayer>();

	private List<int> m_partyRequests = new List<int>();

	private List<DatabasePlayer> m_partyAnswers = new List<DatabasePlayer>();

	private List<DatabasePlayer> m_partyPlayersToWrite = new List<DatabasePlayer>();

	private bool m_requestBuildings;

	private List<DatabaseBuilding> m_buildingAnswers = new List<DatabaseBuilding>();

	private List<int> m_inventoryClear = new List<int>();

	private List<int> m_containerRequests = new List<int>();

	private List<Vector3> m_hiddenItemsRequests = new List<Vector3>();

	private List<DatabaseItem> m_itemsToWrite = new List<DatabaseItem>();

	private List<DatabaseItem> m_itemAnswers = new List<DatabaseItem>();

	private List<DatabaseBuilding> m_buildingsToWrite = new List<DatabaseBuilding>();

	private object m_threadLock = new object();

	private Thread m_thread;

	private IDbConnection m_sqlConnection;

	private IDbCommand m_sqlCommand;

	private IDataReader m_sqlReader;
}

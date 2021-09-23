using System;
using System.Data;
using Mono.Data.SqliteClient;
using UnityEngine;

public static class SQLWorker
{
	// Note: this type is marked as 'beforefieldinit'.
	static SQLWorker()
	{
	}

	private static void Init()
	{
		if (!SQLWorker.m_inited)
		{
			SQLWorker.m_inited = true;
			SQLWorker.m_sqlConnection = new SqliteConnection("URI=file:deathcarriers.db");
			SQLWorker.m_sqlConnection.Close();
			SQLWorker.m_sqlConnection.Open();
			SQLWorker.m_sqlCommand = SQLWorker.m_sqlConnection.CreateCommand();
			SQLWorker.m_sqlCommand.CommandText = "CREATE TABLE IF NOT EXISTS player ( pid INTEGER NOT NULL PRIMARY KEY, name TEXT NOT NULL, x REAL NOT NULL DEFAULT '0.0', y REAL NOT NULL DEFAULT '0.0' );";
			SQLWorker.m_sqlCommand.ExecuteNonQuery();
		}
	}

	public static void GetOrCreatePlayer(ref ServerPlayer a_player, string a_name)
	{
		if (!SQLWorker.m_inited)
		{
			SQLWorker.Init();
		}
		a_player = null;
		using (IDbTransaction dbTransaction = SQLWorker.m_sqlConnection.BeginTransaction())
		{
			SQLWorker.m_sqlCommand.CommandText = "SELECT pid, x, y FROM player WHERE name='" + a_name + "' LIMIT 1;";
			using (IDataReader dataReader = SQLWorker.m_sqlCommand.ExecuteReader())
			{
				if (dataReader.Read())
				{
				}
				dataReader.Close();
			}
			if (a_player == null)
			{
				SQLWorker.m_sqlCommand.CommandText = "INSERT INTO player (name) VALUES('" + a_name + "');SELECT last_insert_rowid();";
				using (IDataReader dataReader2 = SQLWorker.m_sqlCommand.ExecuteReader())
				{
					if (!dataReader2.Read())
					{
						Debug.Log("SQLWorker.cs: ERROR: Couldn't create new player in database?!");
					}
					dataReader2.Close();
				}
			}
			dbTransaction.Commit();
		}
	}

	private static IDbConnection m_sqlConnection;

	private static IDbCommand m_sqlCommand;

	private static IDataReader m_sqlReader;

	private static bool m_inited;
}

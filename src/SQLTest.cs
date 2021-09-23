using System;
using UnityEngine;

public class SQLTest : MonoBehaviour
{
	public SQLTest()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
		DatabaseItem[] array = this.m_sql.PopRequestedItems();
		if (array != null)
		{
			Debug.Log(string.Concat(new object[]
			{
				"got items: ",
				array.Length,
				" time: ",
				Time.time
			}));
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].cid == 0 && array[i].iid != 0)
				{
					Debug.Log(array[i].type + " " + array[i].amount);
				}
			}
		}
		DatabasePlayer[] array2 = this.m_sql.PopRequestedPlayers();
		if (array2 != null)
		{
			Debug.Log(string.Concat(new object[]
			{
				"got players: ",
				array2[0].name,
				" x ",
				array2[0].x,
				" time: ",
				Time.time
			}));
		}
	}

	private SQLThreadManager m_sql;
}

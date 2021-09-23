using System;
using UnityEngine;

public class ServerTutorial : MonoBehaviour
{
	public ServerTutorial()
	{
	}

	private void Start()
	{
		this.m_server = (LidServer)UnityEngine.Object.FindObjectOfType(typeof(LidServer));
	}

	public Vector3 StartTutorial()
	{
		float num = 3f;
		this.SpawnItems(2, this.m_startItemType, 86, this.m_itemSpawnPos.position, num);
		this.SpawnItems(1, this.m_foodItemType, 3, this.m_foodSpawnPos.position, num);
		Vector3 position = base.transform.position;
		position.x += UnityEngine.Random.Range(-num, num);
		position.z += UnityEngine.Random.Range(-num, num);
		position.y = 0f;
		return position;
	}

	private void SpawnItems(int a_amount, int a_itemId, int a_itemAmountOrState, Vector3 a_pos, float a_rndDist)
	{
		for (int i = 0; i < a_amount; i++)
		{
			Vector3 a_pos2 = a_pos;
			a_pos2.x += UnityEngine.Random.Range(-a_rndDist, a_rndDist);
			a_pos2.z += UnityEngine.Random.Range(-a_rndDist, a_rndDist);
			a_pos2.y = 0f;
			this.m_server.CreateFreeWorldItem(a_itemId, a_itemAmountOrState, a_pos2);
		}
	}

	public int m_startItemType = 108;

	public int m_foodItemType = 5;

	public Transform m_itemSpawnPos;

	public Transform m_foodSpawnPos;

	private LidServer m_server;
}

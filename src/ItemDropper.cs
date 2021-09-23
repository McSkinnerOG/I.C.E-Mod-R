using System;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
	public ItemDropper()
	{
	}

	private void Start()
	{
		this.m_server = UnityEngine.Object.FindObjectOfType<LidServer>();
	}

	private void SetAggressor(Transform a_aggressor)
	{
		if (Global.isServer && null != this.m_server && null != a_aggressor)
		{
			bool flag = false;
			if (Time.time > this.m_nextDropTime)
			{
				int num = -1;
				int a_amount = 1;
				int num2 = UnityEngine.Random.Range(0, 8);
				if (num2 == 0)
				{
					num = Items.GetRandomType(90f);
					a_amount = ((!Items.HasCondition(num)) ? 1 : UnityEngine.Random.Range(1, 20));
				}
				else if (3 > num2)
				{
					num = UnityEngine.Random.Range(130, 134);
				}
				if (num != -1 && Items.IsValid(num))
				{
					this.m_server.CreateFreeWorldItem(num, a_amount, a_aggressor.position);
					flag = true;
				}
				this.m_nextDropTime = Time.time + UnityEngine.Random.Range(this.m_dropInterval * 0.6f, this.m_dropInterval * 1.4f);
			}
			if (!flag)
			{
				ServerPlayer playerByTransform = this.m_server.GetPlayerByTransform(a_aggressor);
				if (playerByTransform != null)
				{
					this.m_server.SendSpecialEvent(playerByTransform, eSpecialEvent.empty);
				}
			}
		}
	}

	private float m_dropInterval = 900f;

	private float m_nextDropTime;

	private LidServer m_server;
}

using System;
using UnityEngine;

public class JobStalker : JobBase
{
	public JobStalker()
	{
	}

	private void Start()
	{
		base.Init();
		this.m_server = (LidServer)UnityEngine.Object.FindObjectOfType(typeof(LidServer));
	}

	public void SetAggressor(Transform a_aggressor)
	{
		this.m_body.Attack(a_aggressor, true);
		this.m_resetAttackTime = Time.time + UnityEngine.Random.Range(3f, 10f);
	}

	public override void Execute(float deltaTime)
	{
		if ((this.m_body.GetState() == eBodyBaseState.none || 600f < Time.time - this.m_nextPickupTime) && null != this.m_server)
		{
			this.PickupThings();
		}
		if (this.m_resetAttackTime > 0f && Time.time > this.m_resetAttackTime)
		{
			this.m_body.Attack(null, false);
		}
	}

	private void PickupThings()
	{
		if (Time.time > this.m_nextPickupTime)
		{
			this.m_nextPickupTime = Time.time + UnityEngine.Random.Range(10f, 30f);
			Vector3 vector = Vector3.zero;
			DatabaseItem randomFreeWorldItem = new DatabaseItem(0, 0f, 0f, 1, false, 0, 0);
			if (this.m_pickupThings)
			{
				this.m_server.PickupItem(null, this.m_brain);
				randomFreeWorldItem = this.m_server.GetRandomFreeWorldItem();
				if (randomFreeWorldItem.type != 0 && 600f < Time.time - randomFreeWorldItem.dropTime)
				{
					vector = new Vector3(randomFreeWorldItem.x, 0f, randomFreeWorldItem.y);
					if (this.m_server.IsInSpecialArea(vector, eAreaType.noPvp))
					{
						vector = Vector3.zero;
					}
				}
			}
			if (Vector3.zero == vector)
			{
				vector = base.transform.position + new Vector3(UnityEngine.Random.Range(-100f, 100f), 0f, UnityEngine.Random.Range(-100f, 100f));
			}
			this.m_body.GoTo(vector);
		}
	}

	public bool m_pickupThings = true;

	private float m_nextPickupTime;

	private LidServer m_server;

	private float m_resetAttackTime;
}

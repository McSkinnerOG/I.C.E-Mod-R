using System;
using System.Collections.Generic;
using UnityEngine;

public class Fireplace : ServerBuilding
{
	public Fireplace()
	{
	}

	public override bool Use(ServerPlayer a_player)
	{
		this.m_isOnFire = !this.m_isOnFire;
		return true;
	}

	public override float GetState()
	{
		return (!this.m_isOnFire) ? 1f : 0.4f;
	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Update()
	{
		if (this.m_isOnFire)
		{
			float time = Time.time;
			this.m_decayTime -= (double)(Time.deltaTime * 9f);
			if (time > this.m_nextCookTime)
			{
				List<DatabaseItem> freeWorldItems = this.m_server.GetFreeWorldItems();
				for (int i = 0; i < freeWorldItems.Count; i++)
				{
					if (Items.IsCookable(freeWorldItems[i].type) && time > freeWorldItems[i].dropTime + 3f)
					{
						float sqrMagnitude = (base.transform.position - freeWorldItems[i].GetPos()).sqrMagnitude;
						if (sqrMagnitude < 3f)
						{
							DatabaseItem value = freeWorldItems[i];
							value.type++;
							freeWorldItems[i] = value;
						}
					}
				}
				this.m_nextCookTime = Time.time + 4f;
			}
		}
		base.Update();
	}

	private bool m_isOnFire;

	private float m_nextCookTime;
}

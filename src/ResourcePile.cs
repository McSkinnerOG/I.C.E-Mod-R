using System;
using UnityEngine;

public class ResourcePile : ServerBuilding
{
	public ResourcePile()
	{
	}

	protected override void Update()
	{
		if (null != this.m_server && this.m_gotDamage > 0f && null != this.m_gotAttacker)
		{
			int num = 1 + (int)(this.m_gotDamage * 0.08f);
			this.m_server.CreateFreeWorldItem(this.m_itemIndex, num, this.m_gotAttacker.position);
			this.m_quantity -= num;
			if (this.m_quantity <= 0)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			this.m_gotAttacker = null;
			this.m_gotDamage = 0f;
		}
		base.Update();
	}

	public int m_itemIndex = 130;

	public int m_quantity = 10;
}

using System;
using UnityEngine;

public class ItemSpawner : ServerBuilding
{
	public ItemSpawner()
	{
	}

	protected override void Awake()
	{
		this.m_nextSpawnTime = Time.time + this.m_spawnDuration;
		if (this.m_containerType == 0)
		{
			this.m_maxItems = 1;
		}
		base.Awake();
	}

	protected override void Update()
	{
		if (Time.time > this.m_nextSpawnTime)
		{
			if (null == this.m_server)
			{
				this.m_server = UnityEngine.Object.FindObjectOfType<LidServer>();
			}
			if (null != this.m_server)
			{
				int nearbyItemCount = this.m_server.GetNearbyItemCount(base.transform.position);
				ServerPlayer serverPlayer = (!this.m_dropWithPlayerAround) ? this.m_server.GetNearestPlayer(base.transform.position) : null;
				if (this.m_maxItems > nearbyItemCount && (serverPlayer == null || (base.transform.position - serverPlayer.GetPosition()).sqrMagnitude > 2500f))
				{
					this.DropLoot();
				}
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			this.m_nextSpawnTime = Time.time + UnityEngine.Random.Range(this.m_spawnDuration * 0.5f, this.m_spawnDuration * 1.5f);
		}
		base.Update();
	}

	private void DropLoot()
	{
		if (null != this.m_server && this.m_itemDrops != null && this.m_itemDrops.Length > 0)
		{
			int num = UnityEngine.Random.Range(0, this.m_itemDrops.Length);
			if (this.m_itemDrops[num] != null && UnityEngine.Random.Range(0, 100) < this.m_itemDrops[num].chance)
			{
				int num2 = (this.m_itemDrops[num].typeFrom != this.m_itemDrops[num].typeTo) ? UnityEngine.Random.Range(this.m_itemDrops[num].typeFrom, this.m_itemDrops[num].typeTo + 1) : this.m_itemDrops[num].typeFrom;
				ItemDef itemDef = Items.GetItemDef(num2);
				if (itemDef.ident != null && itemDef.ident.Length > 0)
				{
					int num3 = (!Items.HasAmountOrCondition(num2)) ? 1 : UnityEngine.Random.Range(this.m_itemDrops[num].min, this.m_itemDrops[num].max + 1);
					num3 = Mathf.Clamp(num3, 1, 254);
					if (this.m_containerType != 0)
					{
						this.m_server.CreateTempContainerItem(num2, num3, base.transform.position, this.m_containerType);
					}
					else
					{
						int num4 = 10;
						Vector3 b = new Vector3((float)UnityEngine.Random.Range(-num4, num4) * 0.1f, 0f, (float)UnityEngine.Random.Range(-num4, num4) * 0.1f);
						this.m_server.CreateFreeWorldItem(num2, num3, base.transform.position + b);
					}
				}
			}
		}
	}

	public bool m_dropWithPlayerAround;

	public int m_maxItems = 1;

	public DropItem[] m_itemDrops;

	public int m_containerType;

	public float m_spawnDuration = 10f;

	private float m_nextSpawnTime;
}

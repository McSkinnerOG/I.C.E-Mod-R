using System;
using UnityEngine;

public class ShopContainer : MonoBehaviour
{
	public ShopContainer()
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static ShopContainer()
	{
	}

	private void Awake()
	{
		if (Global.isServer)
		{
			ShopContainer.m_cid++;
			this.m_container = new ItemContainer(4, 4, 6, 0, null, null);
			this.m_container.m_position = base.transform.position;
		}
		else
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		for (int i = 0; i < this.m_minItemCount; i++)
		{
			this.AddRandomItem();
		}
	}

	private void Update()
	{
		if (Time.time > this.m_nextUpdateTime)
		{
			if (this.m_container.Count() < this.m_minItemCount)
			{
				this.AddRandomItem();
			}
			else
			{
				this.m_container.DeleteItem(UnityEngine.Random.Range(0, this.m_container.Count()));
			}
			this.m_nextUpdateTime = Time.time + UnityEngine.Random.Range(30f, 120f);
		}
	}

	private void AddRandomItem()
	{
		int a_type;
		if (this.m_type == eShopType.eFood)
		{
			a_type = Items.GetRandomFood();
		}
		else if (this.m_type == eShopType.eRareAmmo)
		{
			a_type = UnityEngine.Random.Range(40, 45);
		}
		else if (this.m_type == eShopType.ePharmacy)
		{
			a_type = UnityEngine.Random.Range(140, 143);
		}
		else if (this.m_type == eShopType.eResources)
		{
			a_type = UnityEngine.Random.Range(130, 134);
		}
		else
		{
			a_type = ((UnityEngine.Random.Range(0, 8) != 0) ? Items.GetRandomType(90f) : 92);
		}
		DatabaseItem a_item = new DatabaseItem(a_type, 0f, 0f, 1, false, 0, 0);
		if (Items.HasAmountOrCondition(a_item.type))
		{
			a_item.amount = ((!Items.HasCondition(a_item.type)) ? UnityEngine.Random.Range(1, 10) : UnityEngine.Random.Range(10, 100));
			if (Items.HasCondition(a_item.type))
			{
				a_item.amount = UnityEngine.Random.Range(20, 100);
			}
			else if (Items.IsMedicine(a_item.type))
			{
				a_item.amount = 1;
			}
			else if (Items.IsResource(a_item.type))
			{
				a_item.amount = UnityEngine.Random.Range(10, 50);
			}
			else if (Items.IsEatable(a_item.type))
			{
				a_item.amount = UnityEngine.Random.Range(1, 5);
			}
			else
			{
				a_item.amount = UnityEngine.Random.Range(1, 10);
			}
		}
		this.m_container.CollectItem(a_item, false, default(Vector3));
	}

	public float m_buyPriceMuliplier = 1f;

	public float m_sellPriceMuliplier = 1f;

	public int m_minItemCount = 8;

	public eShopType m_type;

	[HideInInspector]
	public ItemContainer m_container;

	private static int m_cid;

	private float m_nextUpdateTime;
}

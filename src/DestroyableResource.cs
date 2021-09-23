using System;
using UnityEngine;

public class DestroyableResource : ServerBuilding
{
	public DestroyableResource()
	{
	}

	public override bool Use(ServerPlayer a_player)
	{
		return false;
	}

	public override float GetState()
	{
		bool flag = this.m_respawnTime > 0f && Time.time < this.m_respawnTime;
		return (!flag) ? 1f : 0.4f;
	}

	protected override void Awake()
	{
		this.m_curQuantity = this.m_quantity;
		base.Awake();
	}

	protected override void Update()
	{
		if (this.m_gotDamage > 0f && this.m_curQuantity > 0)
		{
			int num = Mathf.Min(1 + (int)(this.m_gotDamage * 0.08f), this.m_curQuantity);
			if (null == this.m_server)
			{
				this.m_server = UnityEngine.Object.FindObjectOfType<LidServer>();
			}
			if (null != this.m_server)
			{
				Vector3 a_pos = (!(null == this.m_gotAttacker)) ? this.m_gotAttacker.position : (base.transform.position + base.transform.forward);
				this.m_server.CreateFreeWorldItem(this.m_itemIndex, num, a_pos);
			}
			this.m_curQuantity -= num;
			if (this.m_curQuantity <= 0)
			{
				this.m_respawnTime = Time.time + this.m_respawnDur;
				if (this.m_isStatic)
				{
					this.SendStateToClients();
				}
			}
			this.m_gotAttacker = null;
			this.m_gotDamage = 0f;
		}
		if (this.m_respawnTime > 0f && Time.time > this.m_respawnTime)
		{
			this.m_curQuantity = this.m_quantity;
			this.m_respawnTime = 0f;
			this.m_gotDamage = 0f;
			if (this.m_isStatic)
			{
				this.SendStateToClients();
			}
		}
		base.Update();
	}

	private void SendStateToClients()
	{
		if (null == this.m_server)
		{
			this.m_server = UnityEngine.Object.FindObjectOfType<LidServer>();
		}
		if (null != this.m_server)
		{
			this.m_server.BroadcastStaticBuildingChange(this);
		}
	}

	private void OnCollisionEnter(Collision a_col)
	{
		float num = Mathf.Clamp(a_col.relativeVelocity.sqrMagnitude - 10f, 0f, 10000f);
		if (num > 1f)
		{
			this.m_gotDamage = num;
		}
	}

	public int m_itemIndex = 130;

	public int m_quantity = 10;

	public float m_respawnDur = 120f;

	private float m_respawnTime;

	private int m_curQuantity;
}

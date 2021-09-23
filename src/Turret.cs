using System;
using UnityEngine;

public class Turret : ServerBuilding
{
	public Turret()
	{
	}

	protected override void Awake()
	{
		this.m_nextCheckTime = UnityEngine.Random.Range(4f, 8f);
		base.Awake();
	}

	protected override void Update()
	{
		if (Time.time > this.m_nextCheckTime)
		{
			float num = this.m_damageIntervall;
			if (null == this.m_server)
			{
				this.m_server = UnityEngine.Object.FindObjectOfType<LidServer>();
			}
			if (null != this.m_server)
			{
				Vector3 position = base.transform.position;
				ServerPlayer nearestPlayer = this.m_server.GetNearestPlayer(position);
				if (nearestPlayer != null && nearestPlayer.m_pid != this.m_ownerPid && !this.m_server.PartyContainsPid(nearestPlayer.m_partyId, this.m_ownerPid))
				{
					float sqrMagnitude = (nearestPlayer.GetPosition() - position).sqrMagnitude;
					if (this.m_attackRadius * this.m_attackRadius > sqrMagnitude)
					{
						nearestPlayer.ChangeHealthBy(-this.m_damage);
					}
					num += ((sqrMagnitude <= 2500f) ? 0f : 4f);
				}
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			this.m_nextCheckTime = Time.time + num;
		}
		base.Update();
	}

	public float m_attackRadius = 5f;

	public float m_damage = 10f;

	public float m_damageIntervall = 2f;

	private float m_nextCheckTime;
}

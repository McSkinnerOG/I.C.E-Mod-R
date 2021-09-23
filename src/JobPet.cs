using System;
using UnityEngine;

public class JobPet : JobBase
{
	public JobPet()
	{
	}

	private void Start()
	{
		base.Init();
		this.m_server = (LidServer)UnityEngine.Object.FindObjectOfType(typeof(LidServer));
	}

	public void SetAggressor(Transform a_aggressor)
	{
		if (this.m_masterPlayer == null || this.m_masterPlayer.GetTransform() != a_aggressor || UnityEngine.Random.Range(0, 10) < 2)
		{
			this.m_body.Attack(a_aggressor, true);
			this.m_resetAttackTime = Time.time + UnityEngine.Random.Range(3f, 10f);
		}
	}

	public override void Execute(float deltaTime)
	{
		if (this.m_body.GetState() == eBodyBaseState.none && null != this.m_server)
		{
			if (this.m_masterPlayer == null)
			{
				this.PickupNearestFood();
			}
			else
			{
				Vector3 position = this.m_masterPlayer.GetPosition();
				float sqrMagnitude = (base.transform.position - position).sqrMagnitude;
				if (Time.time > this.m_resetAttackTime)
				{
					Transform victim = this.m_masterPlayer.GetVictim();
					if (null != victim && (victim.position - position).sqrMagnitude < 200f && !this.m_server.IsInSpecialArea(victim.position, eAreaType.noPvp))
					{
						ControlledChar component = victim.GetComponent<ControlledChar>();
						if (null == component || (component.GetServerPlayer() != null && !component.GetServerPlayer().IsSaint()))
						{
							this.SetAggressor(this.m_masterPlayer.GetVictim());
						}
					}
					else if (sqrMagnitude > 900f)
					{
						this.m_masterPlayer = null;
					}
					else if (sqrMagnitude > 16f && Time.time > this.m_nextFollowUpdate)
					{
						this.m_body.GoTo(position);
						this.m_nextFollowUpdate = Time.time + 0.5f;
					}
				}
				else if (sqrMagnitude > 300f)
				{
					this.ResetAttack();
				}
			}
		}
		if (this.m_resetAttackTime > 0f && Time.time > this.m_resetAttackTime)
		{
			this.ResetAttack();
		}
	}

	private void ResetAttack()
	{
		this.m_resetAttackTime = 0f;
		this.m_body.Attack(null, false);
	}

	private void PickupNearestFood()
	{
		if (Time.time > this.m_nextPickupTime || (this.m_targetItemPos - base.transform.position).sqrMagnitude < 1.4f)
		{
			this.m_nextPickupTime = Time.time + UnityEngine.Random.Range(5f, 15f);
			DatabaseItem databaseItem = this.m_server.PickupItem(null, this.m_brain);
			Vector3 vector = Vector3.zero;
			if (databaseItem.dropPlayerId > 0)
			{
				ServerPlayer playerByPid = this.m_server.GetPlayerByPid(databaseItem.dropPlayerId);
				if (playerByPid != null && (playerByPid.GetPosition() - base.transform.position).sqrMagnitude < 64f)
				{
					this.m_masterPlayer = playerByPid;
					vector = this.m_masterPlayer.GetPosition();
				}
			}
			if (Vector3.zero == vector)
			{
				DatabaseItem nearestItem = this.m_server.GetNearestItem(base.transform.position, true);
				this.m_targetItemPos = new Vector3(nearestItem.x, 0f, nearestItem.y);
				if (nearestItem.type == 0 || (this.m_targetItemPos - base.transform.position).sqrMagnitude > 40000f)
				{
					vector = base.transform.position + new Vector3(UnityEngine.Random.Range(-100f, 100f), 0f, UnityEngine.Random.Range(-100f, 100f));
				}
				else
				{
					vector = this.m_targetItemPos;
				}
			}
			this.m_body.GoTo(vector);
		}
	}

	private float m_nextPickupTime;

	private LidServer m_server;

	private float m_resetAttackTime;

	private float m_nextFollowUpdate;

	private Vector3 m_targetItemPos;

	private ServerPlayer m_masterPlayer;
}

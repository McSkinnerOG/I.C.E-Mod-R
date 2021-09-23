using System;
using UnityEngine;

public class JobAI : JobBase
{
	public JobAI()
	{
	}

	private void Start()
	{
		base.Init();
		this.m_server = (LidServer)UnityEngine.Object.FindObjectOfType(typeof(LidServer));
		this.m_curPos = this.m_body.m_homePos;
	}

	private void Update()
	{
		if (null != this.m_server)
		{
			this.m_threatRadius = ((this.m_server.GetDayLight() != 0f) ? this.m_threatRadiusDay : this.m_threatRadiusNight);
		}
		if (Vector3.zero != this.m_relocationPos && null == this.m_enemy)
		{
			if ((this.m_relocationPos - base.transform.position).sqrMagnitude < 9f)
			{
				this.m_body.m_homePos = this.m_relocationPos;
				this.m_relocationPos = Vector3.zero;
			}
			else
			{
				this.m_body.m_homePos = base.transform.position;
			}
		}
	}

	private void RunAway(Transform a_target)
	{
		if (!this.m_brain.IsHappy())
		{
			return;
		}
		if (null != a_target)
		{
			Vector3 normalized = (base.transform.position - a_target.position).normalized;
			this.m_body.GoTo(base.transform.position + normalized * this.m_threatRadius * 1.5f);
		}
	}

	private bool AttackOrRunAway(Transform a_target, Vector3 a_focusPos)
	{
		bool result = false;
		if (this.m_attackOnSight)
		{
			this.m_enemy = null;
			if (null != a_target)
			{
				float sqrMagnitude = (a_target.position - a_focusPos).sqrMagnitude;
				if (sqrMagnitude < this.m_threatRadius * this.m_threatRadius)
				{
					float sqrMagnitude2 = (a_target.position - this.m_body.m_homePos).sqrMagnitude;
					bool a_approach = sqrMagnitude2 < this.m_maxHuntRadius * this.m_maxHuntRadius;
					result = this.m_body.Attack(a_target, a_approach);
					this.m_enemy = a_target;
				}
				else
				{
					this.m_body.Attack(null, false);
					this.m_enemy = null;
				}
			}
		}
		else
		{
			float sqrMagnitude3 = (a_target.position - base.transform.position).sqrMagnitude;
			if (sqrMagnitude3 < this.m_threatRadius * this.m_threatRadius)
			{
				this.RunAway(a_target);
			}
		}
		return result;
	}

	public void SetAggressor(Transform a_aggressor)
	{
		if (!this.m_brain.IsHappy())
		{
			return;
		}
		this.AttackOrRunAway(a_aggressor, (!(null == a_aggressor)) ? a_aggressor.position : Vector3.zero);
		this.m_nextPlayerDistCheckTime = Time.time + 1f;
	}

	public override void Execute(float deltaTime)
	{
		if (Time.time > this.m_nextBoredActionTime)
		{
			if (this.m_body.GetState() == eBodyBaseState.none && !this.m_body.IsMoving() && null == this.m_enemy)
			{
				if (Vector3.zero != this.m_relocationPos)
				{
					this.m_body.GoTo(this.m_relocationPos);
				}
				else
				{
					this.m_curPos = this.m_body.m_homePos + new Vector3(UnityEngine.Random.Range(-this.m_walkRadius, this.m_walkRadius), 0f, UnityEngine.Random.Range(-this.m_walkRadius, this.m_walkRadius));
					this.m_body.GoTo(this.m_curPos);
				}
			}
			this.m_nextBoredActionTime = Time.time + UnityEngine.Random.Range(10f, 30f);
		}
		if (Time.time > this.m_nextPlayerDistCheckTime && null != this.m_server)
		{
			Vector3 vector = base.transform.position + base.transform.forward * (this.m_threatRadius * 0.5f);
			this.m_nearestPlayer = this.m_server.GetNearestPlayer(vector);
			if (this.m_nearestPlayer != null)
			{
				this.AttackOrRunAway(this.m_nearestPlayer.GetTransform(), vector);
			}
			this.m_nextPlayerDistCheckTime = Time.time + 0.5f + UnityEngine.Random.Range(0f, 0.5f);
		}
	}

	public void RelocateHome(Vector3 a_pos)
	{
		this.m_relocationPos = a_pos;
		if (null == this.m_enemy && Vector3.zero != this.m_relocationPos)
		{
			this.m_body.GoTo(this.m_relocationPos);
		}
	}

	public bool IsRelocating()
	{
		return Vector3.zero != this.m_relocationPos && !this.m_brain.IsDead();
	}

	public Transform GetEnemy()
	{
		return this.m_enemy;
	}

	public ServerPlayer GetNearestPlayer()
	{
		return this.m_nearestPlayer;
	}

	public bool m_attackOnSight = true;

	public float m_walkRadius = 5f;

	public float m_threatRadiusDay = 10f;

	public float m_threatRadiusNight = 10f;

	public float m_maxHuntRadius = 20f;

	private Transform m_enemy;

	private float m_threatRadius = 10f;

	private LidServer m_server;

	private float m_nextPlayerDistCheckTime;

	private ServerPlayer m_nearestPlayer;

	private float m_nextBoredActionTime;

	private Vector3 m_curPos = Vector3.zero;

	private Vector3 m_relocationPos = Vector3.zero;
}

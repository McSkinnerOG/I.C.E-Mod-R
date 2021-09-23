using System;
using UnityEngine;

public class BrainAI : BrainBase
{
	public BrainAI()
	{
	}

	private void Start()
	{
		base.Init();
		this.m_server = (LidServer)UnityEngine.Object.FindObjectOfType(typeof(LidServer));
		base.SetStateTolerance(UnityEngine.Random.Range(this.m_stateTolMin, this.m_stateTolMax));
		base.SetStateDurability(eBrainBaseState.hungry, this.m_hungerMultip);
		base.SetStateDurability(eBrainBaseState.thirsty, this.m_thirstMultip);
		base.SetStateDurability(eBrainBaseState.fatigued, this.m_fatigueMultip);
		base.SetStateDurability(eBrainBaseState.lonely, this.m_lonelyMultip);
		base.SetStateDurability(eBrainBaseState.injured, this.m_injuryMultip);
		base.SetStateDurability(eBrainBaseState.stressed, this.m_stressMultip);
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		base.UpdateState(deltaTime);
		if (base.IsDead() && this.m_respawnTime < 0f)
		{
			base.collider.enabled = false;
			this.m_respawnTime = Time.time + this.m_respawnDuration;
		}
		if (this.m_respawnTime > 0f && Time.time > this.m_respawnTime && null != this.m_server)
		{
			if (this.m_spawnWhenVisible)
			{
				this.Respawn();
			}
			else
			{
				ServerPlayer nearestPlayer = this.m_server.GetNearestPlayer(base.transform.position);
				if (nearestPlayer == null || (base.transform.position - nearestPlayer.GetPosition()).sqrMagnitude > 1600f)
				{
					this.Respawn();
				}
				else
				{
					this.m_respawnTime = Time.time + 10f;
				}
			}
		}
	}

	private void Respawn()
	{
		base.collider.enabled = true;
		base.Reset();
		this.m_body.Respawn();
		this.m_respawnTime = -1f;
	}

	public float m_respawnDuration = 180f;

	public bool m_spawnWhenVisible;

	public float m_hungerMultip;

	public float m_thirstMultip;

	public float m_fatigueMultip;

	public float m_lonelyMultip;

	public float m_injuryMultip;

	public float m_stressMultip;

	public float m_stateTolMin = 0.5f;

	public float m_stateTolMax = 0.9f;

	private float m_respawnTime = -1f;

	private LidServer m_server;
}

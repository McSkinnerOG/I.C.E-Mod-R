using System;
using UnityEngine;

public class ServerNpc : MonoBehaviour
{
	public ServerNpc()
	{
	}

	private void Start()
	{
		this.m_brain = base.GetComponent<BrainBase>();
		this.m_body = base.GetComponent<BodyAISimple>();
		this.m_agent = base.GetComponent<NavMeshAgent>();
	}

	public int GetHandItem()
	{
		return (!(null != this.m_body)) ? 0 : this.m_body.m_handItemType;
	}

	public int GetBodyItem()
	{
		return (!(null != this.m_body)) ? 0 : this.m_body.m_bodyItemType;
	}

	public int GetLookItem()
	{
		return (!(null != this.m_body)) ? 0 : this.m_body.m_lookItemType;
	}

	public eBodyBaseState GetBodyState()
	{
		return this.m_body.GetState();
	}

	public float GetHealth()
	{
		return (!this.m_brain.IsDead()) ? ((1f - this.m_brain.GetState(eBrainBaseState.injured)) * 100f) : 0f;
	}

	public float GetLastHealth()
	{
		return this.m_lastHealth;
	}

	public float ChangeHealthBy(float a_delta)
	{
		this.m_lastHealth = this.GetHealth();
		if (0f > a_delta)
		{
			a_delta *= this.m_damageMultiplier * this.m_body.GetVestMultiplier();
		}
		this.m_brain.ChangeStateBy(eBrainBaseState.injured, a_delta * -0.01f);
		return this.GetHealth();
	}

	private void OnCollisionEnter(Collision a_col)
	{
		float num = 0.5f * Mathf.Clamp(a_col.relativeVelocity.sqrMagnitude - 10f, 0f, 10000f) * this.m_damageMultiplier;
		if (num > 1f)
		{
			this.ChangeHealthBy(-num);
		}
		if (0f < this.GetHealth() && null != this.m_agent)
		{
			Vector3 offset = Vector3.zero;
			offset = base.transform.position - a_col.collider.transform.position;
			offset.y = 0f;
			offset = offset.normalized * (1f + num * 0.02f);
			this.m_agent.Move(offset);
		}
	}

	public eCharType m_npcType = eCharType.eGasmaskGuy;

	public float m_damageMultiplier = 1f;

	private BrainBase m_brain;

	private BodyAISimple m_body;

	private float m_lastHealth = 100f;

	private NavMeshAgent m_agent;
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class BodyAISimple : BodyBase
{
	public BodyAISimple()
	{
	}

	private void Start()
	{
		base.Init();
		this.m_handItemDef = Items.GetItemDef(this.m_handItemType);
		this.m_bodyItemDef = Items.GetItemDef(this.m_bodyItemType);
		try
		{
			this.m_jobAi = (JobAI)this.m_job;
		}
		catch
		{
		}
		this.m_agent = base.GetComponent<NavMeshAgent>();
		this.m_npc = base.GetComponent<ServerNpc>();
		this.m_server = (LidServer)UnityEngine.Object.FindObjectOfType(typeof(LidServer));
		if (null != this.m_npc)
		{
			int a_chance = (int)(100f * this.m_dropMultiplier);
			int num;
			if (this.m_npc.m_npcType == eCharType.eGasmaskGuy)
			{
				this.m_containerType = 120;
				if (this.m_handItemType != 0)
				{
					this.m_itemDrops.Add(new DropItem(this.m_handItemType, this.m_handItemType, (int)(15f * this.m_dropMultiplier), 1, 50));
				}
				if (0 < this.m_handItemDef.ammoItemType)
				{
					this.m_itemDrops.Add(new DropItem(this.m_handItemDef.ammoItemType, this.m_handItemDef.ammoItemType, (int)(80f * this.m_dropMultiplier), 1, 10));
				}
				if (0 < this.m_bodyItemType)
				{
					this.m_itemDrops.Add(new DropItem(this.m_bodyItemType, this.m_bodyItemType, (int)(5f * this.m_dropMultiplier), 1, 60));
				}
				this.m_itemDrops.Add(new DropItem(1, 12, 67, 1, 1));
				this.m_itemDrops.Add(new DropItem(15, 19, 67, 1, 1));
				this.m_itemDrops.Add(new DropItem(254, 254, 20, 1, 5));
				num = 12;
			}
			else if (this.m_npc.m_npcType == eCharType.eChicken || this.m_npc.m_npcType == eCharType.eRaven || this.m_npc.m_npcType == eCharType.eEagle)
			{
				this.m_itemDrops.Add(new DropItem(4, 4, a_chance, 1, 1));
				num = 1;
			}
			else if (this.m_npc.m_npcType == eCharType.eBull || this.m_npc.m_npcType == eCharType.eCow)
			{
				this.m_itemDrops.Add(new DropItem(4, 4, a_chance, 3, 4));
				this.m_itemDrops.Add(new DropItem(133, 133, a_chance, 3, 6));
				num = 2;
			}
			else if (this.m_npc.m_npcType == eCharType.eMutant || this.m_npc.m_npcType == eCharType.eSurvivorMutant)
			{
				this.m_itemDrops.Add(new DropItem(104, 104, 15, 10, 90));
				this.m_itemDrops.Add(new DropItem(254, 254, 15, 1, 3));
				num = ((this.m_npc.m_npcType != eCharType.eMutant) ? 30 : 15);
			}
			else if (this.m_npc.m_npcType == eCharType.eSpider || this.m_npc.m_npcType == eCharType.eSpiderPoison)
			{
				this.m_itemDrops.Add(new DropItem(254, 254, 10, 1, 3));
				num = ((this.m_npc.m_npcType != eCharType.eSpider) ? 15 : 3);
			}
			else
			{
				this.m_itemDrops.Add(new DropItem(4, 4, a_chance, 1, 3));
				this.m_itemDrops.Add(new DropItem(133, 133, a_chance, 1, 3));
				num = 1;
			}
			if (0 < num)
			{
				int randomType = Items.GetRandomType(150f);
				int a_max = (!Items.HasCondition(randomType)) ? 1 : UnityEngine.Random.Range(1, 30);
				this.m_itemDrops.Add(new DropItem(randomType, randomType, num, 1, a_max));
			}
		}
	}

	private void Update()
	{
		ServerPlayer serverPlayer = (!(null != this.m_jobAi)) ? null : this.m_jobAi.GetNearestPlayer();
		bool flag = 0 < this.m_server.GetPlayerCount() && (this.m_activeOffscreen || (serverPlayer != null && (serverPlayer.GetPosition() - base.transform.position).sqrMagnitude < 2500f));
		if (flag)
		{
			float deltaTime = Time.deltaTime;
			bool flag2 = !this.m_isDead && true == this.m_brain.IsDead();
			this.m_isDead = this.m_brain.IsDead();
			this.m_state = eBodyBaseState.none;
			if (null != this.m_enemy && null != this.m_enemy.collider && !this.m_enemy.collider.enabled)
			{
				this.m_enemy = null;
			}
			if (!this.m_isDead)
			{
				bool flag3 = this.m_brain.IsHappy();
				if (flag3 != this.m_isHappy)
				{
					if (!flag3)
					{
						this.m_nextRepositionTime = 0f;
					}
					this.m_isHappy = flag3;
				}
				this.HandleEnemy();
				this.HandleStuckOnPath(deltaTime);
				if (this.m_state != eBodyBaseState.attacking && this.m_agent.enabled && this.m_agent.hasPath)
				{
					this.m_agent.Resume();
					this.m_state = ((!this.IsMoving()) ? eBodyBaseState.none : eBodyBaseState.running);
				}
			}
			else if (flag2)
			{
				this.ResetTargets();
				this.DropLoot();
			}
		}
		if (flag != this.m_agent.enabled)
		{
			this.m_agent.enabled = flag;
		}
	}

	private void HandleEnemy()
	{
		if (null != this.m_enemy)
		{
			if (this.m_isHappy && this.m_handItemDef.damage > 5f)
			{
				float num = 10000f;
				Vector3 vector = (this.m_dontAttackTime >= Time.time) ? Vector3.zero : this.GetAttackDir(ref num);
				if (Vector3.zero != vector)
				{
					base.transform.rotation = Quaternion.LookRotation(vector);
					this.Idle();
					if (Time.time > this.m_nextAttackTime)
					{
						Transform enemy = this.m_enemy;
						Raycaster.Attack(base.transform, this.m_handItemDef, this.m_enemy.transform.position, ref enemy);
						if (this.m_handItemDef.ammoItemType > 0 && null != enemy && enemy != this.m_enemy)
						{
							this.m_dontAttackTime = Time.time + UnityEngine.Random.Range(3f, 6f);
						}
						if (enemy == this.m_enemy && this.m_causeCondition != eCondition.none)
						{
							ControlledChar component = this.m_enemy.GetComponent<ControlledChar>();
							ServerPlayer serverPlayer = (!(null != component)) ? null : component.GetServerPlayer();
							if (serverPlayer != null)
							{
								serverPlayer.SetCondition(this.m_causeCondition, true);
							}
						}
						this.m_nextAttackTime = Time.time + this.m_handItemDef.attackdur * this.attachDurMultip;
					}
					this.m_state = eBodyBaseState.attacking;
				}
				else if (this.m_approach)
				{
					if (Time.time > this.m_nextRepositionTime)
					{
						this.GoTo(this.m_enemy.position);
						this.m_nextRepositionTime = Time.time + 0.05f + Mathf.Clamp01(num / 200f);
					}
				}
				else
				{
					this.m_enemy = null;
					this.GoTo(this.m_homePos);
				}
			}
			else if (Time.time > this.m_nextRepositionTime)
			{
				Vector3 normalized = (base.transform.position - this.m_enemy.position).normalized;
				Vector3 vector2 = base.transform.position + normalized * 10f;
				if (0.8f > Util.GetTerrainHeight(vector2) || UnityEngine.Random.Range(0, 5) == 0)
				{
					float y = (UnityEngine.Random.Range(0, 2) != 0) ? 90f : -90f;
					vector2 = base.transform.position + Quaternion.Euler(0f, y, 0f) * normalized * 10f;
				}
				this.GoTo(vector2);
				this.m_nextRepositionTime = Time.time + 1f;
			}
		}
	}

	private void HandleStuckOnPath(float a_deltaTime)
	{
		if (this.m_agent.enabled && this.m_agent.hasPath && !this.IsMoving())
		{
			this.m_stuckDuration += a_deltaTime;
			if (this.m_stuckDuration > 2f)
			{
				float d = (UnityEngine.Random.Range(0, 2) != 0) ? -1f : 1f;
				Vector3 b = base.transform.right * UnityEngine.Random.Range(4f, 12f) * d + base.transform.forward * UnityEngine.Random.Range(-2f, 0f);
				this.GoTo(base.transform.position + b);
				this.m_stuckDuration = 0f;
				this.m_nextRepositionTime = Time.time + b.magnitude / this.m_agent.speed;
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawSphere(base.transform.position + Vector3.up * 0.5f, 0.5f);
	}

	public float GetVestMultiplier()
	{
		return (!Items.IsBody(this.m_bodyItemType)) ? 1f : this.m_bodyItemDef.healing;
	}

	public override bool GoTo(Vector3 a_target)
	{
		bool result = false;
		if (this.m_agent.enabled && (base.transform.position - a_target).sqrMagnitude > 1f)
		{
			result = this.m_agent.SetDestination(a_target);
		}
		return result;
	}

	public override bool IsMoving()
	{
		return this.m_agent.velocity.sqrMagnitude > 0.01f;
	}

	public override void Idle()
	{
		if (this.m_agent.enabled)
		{
			this.m_agent.Stop();
		}
	}

	public override bool Respawn()
	{
		this.ResetTargets();
		this.m_enemy = null;
		if (this.m_agent.enabled)
		{
			this.m_agent.Warp(this.m_homePos);
		}
		return true;
	}

	public override bool Attack(Transform a_victim, bool a_approach)
	{
		this.m_relaxTime = Time.time + 20f;
		this.m_approach = a_approach;
		if (a_victim != this.m_enemy && this.m_handItemDef.damage > 0f)
		{
			this.m_nextAttackTime = Time.time + 1f;
		}
		this.m_enemy = a_victim;
		return this.m_nextAttackTime > Time.time;
	}

	public override bool FindFood(float status, float deltaTime)
	{
		return this.IsRelaxed();
	}

	public override bool FindDrink(float status, float deltaTime)
	{
		return this.IsRelaxed();
	}

	public override bool FindSleep(float status, float deltaTime)
	{
		return this.IsRelaxed();
	}

	public override bool FindHealing(float status, float deltaTime)
	{
		return false;
	}

	public override bool FindCatharsis(float status, float deltaTime)
	{
		return this.IsRelaxed();
	}

	public override bool FindMates(float status, float deltaTime)
	{
		return this.IsRelaxed();
	}

	private bool IsRelaxed()
	{
		return Time.time > this.m_relaxTime;
	}

	private void DropLoot()
	{
		if (null != this.m_server && this.m_itemDrops != null && this.m_itemDrops.Count > 0)
		{
			for (int i = 0; i < this.m_itemDrops.Count; i++)
			{
				if (this.m_itemDrops[i] != null && UnityEngine.Random.Range(0, 100) < this.m_itemDrops[i].chance)
				{
					int num = (this.m_itemDrops[i].typeFrom != this.m_itemDrops[i].typeTo) ? UnityEngine.Random.Range(this.m_itemDrops[i].typeFrom, this.m_itemDrops[i].typeTo + 1) : this.m_itemDrops[i].typeFrom;
					int num2 = (!Items.HasAmountOrCondition(num)) ? 1 : UnityEngine.Random.Range(this.m_itemDrops[i].min, this.m_itemDrops[i].max + 1);
					if (num2 < 1)
					{
						num2 = 1;
					}
					if (this.m_containerType != 0)
					{
						this.m_server.CreateTempContainerItem(num, num2, base.transform.position, this.m_containerType);
					}
					else
					{
						int num3 = 4;
						Vector3 b = new Vector3((float)UnityEngine.Random.Range(-num3, num3) * 0.1f, 0f, (float)UnityEngine.Random.Range(-num3, num3) * 0.1f);
						this.m_server.CreateFreeWorldItem(num, num2, base.transform.position + b);
					}
				}
			}
		}
	}

	private void ResetTargets()
	{
		if (this.m_agent.enabled && this.m_agent.hasPath)
		{
			this.m_agent.ResetPath();
		}
	}

	private Vector3 GetAttackDir(ref float a_sqrDist)
	{
		Vector3 result = Vector3.zero;
		if (null != this.m_enemy)
		{
			result = this.m_enemy.position - base.transform.position;
			a_sqrDist = result.sqrMagnitude;
			float num = Mathf.Clamp(this.m_handItemDef.range * this.m_handItemDef.range * 0.6f, 2f, 10000f);
			if (a_sqrDist > num)
			{
				result = Vector3.zero;
			}
		}
		return result;
	}

	private const float c_circleDist = 8f;

	public float attachDurMultip = 1.75f;

	public eCondition m_causeCondition = eCondition.none;

	public int m_handItemType = 60;

	public int m_bodyItemType;

	public int m_lookItemType;

	public float m_dropMultiplier = 1f;

	public bool m_activeOffscreen;

	private List<DropItem> m_itemDrops = new List<DropItem>();

	private int m_containerType;

	private NavMeshAgent m_agent;

	private LidServer m_server;

	private ServerNpc m_npc;

	private JobAI m_jobAi;

	private bool m_isHappy;

	private bool m_isDead;

	private ItemDef m_bodyItemDef;

	private ItemDef m_handItemDef;

	private Transform m_enemy;

	private float m_nextAttackTime;

	private float m_relaxTime;

	private float m_nextRepositionTime;

	private float m_stuckDuration;

	private bool m_approach;

	private float m_dontAttackTime;
}

using System;
using UnityEngine;

public class Horde : MonoBehaviour
{
	public Horde()
	{
	}

	private void Start()
	{
		this.rndRadius = (float)this.m_actors.Length;
		for (int i = 0; i < this.m_actors.Length; i++)
		{
			BodyAISimple component = this.m_actors[i].gameObject.GetComponent<BodyAISimple>();
			if (null != component)
			{
				component.m_activeOffscreen = true;
			}
		}
	}

	private void Update()
	{
		if (Time.time > this.m_nextUpdate)
		{
			if (this.DidActorsArrive())
			{
				this.m_curPoint++;
				if (this.m_curPoint == this.m_waypoints.Length)
				{
					this.m_curPoint = 0;
				}
				this.RelocateActors(this.m_waypoints[this.m_curPoint].position);
			}
			this.AttackAsTeam();
			this.m_nextUpdate = Time.time + UnityEngine.Random.Range(1f, 2f);
		}
	}

	private bool DidActorsArrive()
	{
		for (int i = 0; i < this.m_actors.Length; i++)
		{
			if (null != this.m_actors[i] && this.m_actors[i].IsRelocating())
			{
				return false;
			}
		}
		return true;
	}

	private void RelocateActors(Vector3 a_pos)
	{
		for (int i = 0; i < this.m_actors.Length; i++)
		{
			if (null != this.m_actors[i])
			{
				Vector3 a_pos2 = a_pos + new Vector3(UnityEngine.Random.Range(-this.rndRadius, this.rndRadius), 0f, UnityEngine.Random.Range(-this.rndRadius, this.rndRadius));
				this.m_actors[i].RelocateHome(a_pos2);
			}
		}
	}

	private void AttackAsTeam()
	{
		Transform transform = null;
		for (int i = 0; i < this.m_actors.Length; i++)
		{
			if (null != this.m_actors[i] && null != this.m_actors[i].GetEnemy())
			{
				transform = this.m_actors[i].GetEnemy();
				break;
			}
		}
		if (null != transform)
		{
			for (int j = 0; j < this.m_actors.Length; j++)
			{
				if (null != this.m_actors[j] && null == this.m_actors[j].GetEnemy())
				{
					this.m_actors[j].SetAggressor(transform);
				}
			}
		}
	}

	public JobAI[] m_actors;

	public Transform[] m_waypoints;

	private int m_curPoint;

	private float m_nextUpdate;

	private float rndRadius = 1f;
}

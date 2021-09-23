using System;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
	public GoalTrigger()
	{
	}

	private void OnTriggerEnter(Collider a_col)
	{
		if (a_col.transform == this.m_ball)
		{
			if (this.m_isGoalA)
			{
				this.m_manager.m_goalsB++;
			}
			else
			{
				this.m_manager.m_goalsA++;
			}
		}
	}

	public bool m_isGoalA = true;

	public SoccerManager m_manager;

	public Transform m_ball;
}

using System;

public class Mission
{
	public Mission()
	{
	}

	public bool IsEqual(Mission m)
	{
		return m != null && (this.m_type == m.m_type && this.m_objObject == m.m_objObject) && this.m_location == m.m_location;
	}

	public eMissiontype m_type;

	public eObjectivesPerson m_objPerson;

	public eObjectivesObject m_objObject;

	public eLocation m_location;

	public float m_dieTime;

	public int m_xpReward;
}

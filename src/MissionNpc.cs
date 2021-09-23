using System;
using UnityEngine;

public class MissionNpc : MonoBehaviour
{
	public MissionNpc()
	{
	}

	private void Start()
	{
		if (Global.isServer)
		{
			this.m_manager = (MissionManager)UnityEngine.Object.FindObjectOfType(typeof(MissionManager));
		}
		else
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Update()
	{
		if (Time.time > this.m_nextMissionChangeTime && this.m_nextMissionChangeTime != -1f)
		{
			int num = DateTime.Now.Second + DateTime.Now.Minute * 60;
			this.m_mission = this.m_manager.GetRandomMission((int)(10f * base.transform.position.x + 1000f * base.transform.position.z) + num, this.m_easyMode);
			this.m_nextMissionChangeTime = -1f;
		}
	}

	public void AcceptMission()
	{
		this.m_nextMissionChangeTime = Time.time + 30f;
	}

	public bool m_easyMode;

	public Mission m_mission = new Mission();

	private float m_nextMissionChangeTime;

	private MissionManager m_manager;
}

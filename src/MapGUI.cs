using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGUI : MonoBehaviour
{
	public MapGUI()
	{
	}

	private void Start()
	{
		if (Global.isServer)
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
		}
	}

	private void Update()
	{
		if (Time.time > this.m_nextBlipTime)
		{
			if (null != this.m_client && this.m_client.enabled)
			{
				this.m_playerBlip.localPosition = this.WorldToMapPos(this.m_client.GetPos());
			}
			else if (Application.isEditor)
			{
				List<Mission> list = new List<Mission>(3);
				for (int i = 0; i < 3; i++)
				{
					list.Add(new Mission
					{
						m_location = (eLocation)UnityEngine.Random.Range(1, 15)
					});
				}
				this.UpdateMissions(list);
			}
			this.m_nextBlipTime = Time.time + 1f;
		}
	}

	private Vector3 WorldToMapPos(Vector3 a_worldPos)
	{
		a_worldPos.x = a_worldPos.x / this.m_mapRadius * this.m_miniMapRadius;
		a_worldPos.y = a_worldPos.z / this.m_mapRadius * this.m_miniMapRadius;
		a_worldPos.z = -0.1f;
		return a_worldPos;
	}

	private Vector3 GetMissionPos(eLocation a_location)
	{
		Vector3 zero = Vector3.zero;
		switch (a_location)
		{
		case eLocation.eHometown:
			zero = new Vector3(-870f, 0f, 525f);
			break;
		case eLocation.eGastown:
			zero = new Vector3(-1035f, 0f, 334f);
			break;
		case eLocation.eTerminus:
			zero = new Vector3(-945f, 0f, 886f);
			break;
		case eLocation.eVenore:
			zero = new Vector3(-435f, 0f, 628f);
			break;
		case eLocation.eFortBenning:
			zero = new Vector3(-1035f, 0f, 45f);
			break;
		case eLocation.eGarbageStation:
			zero = new Vector3(-635f, 0f, 1091f);
			break;
		case eLocation.eTallahassee:
			zero = new Vector3(55f, 0f, 1095f);
			break;
		case eLocation.eRiverside:
			zero = new Vector3(425f, 0f, 815f);
			break;
		case eLocation.eGasRanch:
			zero = new Vector3(690f, 0f, 1147f);
			break;
		case eLocation.ePowerPlant:
			zero = new Vector3(1130f, 0f, 1170f);
			break;
		case eLocation.eAirport:
			zero = new Vector3(-335f, 0f, 1105f);
			break;
		case eLocation.eAlexandria:
			zero = new Vector3(351f, 0f, 212f);
			break;
		case eLocation.eArea42:
			zero = new Vector3(-1091f, 0f, -315f);
			break;
		case eLocation.eValley:
			zero = new Vector3(-990f, 0f, -1033f);
			break;
		}
		return zero;
	}

	public void UpdateMissions(List<Mission> a_missions)
	{
		int num = (a_missions == null) ? 0 : a_missions.Count;
		for (int i = 0; i < this.m_missionBlip.Length; i++)
		{
			bool flag = i < num;
			this.m_missionBlip[i].renderer.enabled = flag;
			if (flag)
			{
				this.m_missionBlip[i].localPosition = this.WorldToMapPos(this.GetMissionPos(a_missions[i].m_location));
			}
		}
	}

	public Transform m_playerBlip;

	public Transform[] m_missionBlip;

	public float m_mapRadius = 1400f;

	public float m_miniMapRadius = 0.477f;

	private float m_nextBlipTime;

	private LidClient m_client;
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class Hud : MonoBehaviour
{
	public Hud()
	{
	}

	private void Start()
	{
		this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
		this.m_startScale = this.m_barHealth.localScale;
		this.UpdateMissions(null);
	}

	private void Update()
	{
		if (null != this.m_client)
		{
			this.m_barHealth.localScale = new Vector3(this.m_client.GetHealth() * 0.01f * this.m_startScale.x, this.m_startScale.y, this.m_startScale.z);
			this.m_barEnergy.localScale = new Vector3(this.m_client.GetEnergy() * 0.01f * this.m_startScale.x, this.m_startScale.y, this.m_startScale.z);
		}
		bool flag = !this.m_inventory.activeSelf && false == this.m_comGui.IsActive(true);
		if (flag != this.m_active)
		{
			this.m_active = flag;
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				if (null != child)
				{
					child.gameObject.SetActive(this.m_active);
				}
			}
		}
		int num = (!(null != this.m_client)) ? this.m_debugCondition : this.m_client.GetCondition();
		if (this.m_condition != num)
		{
			this.m_condition = num;
			this.UpdateConditions();
		}
	}

	private void UpdateConditions()
	{
		float num = -0.4f;
		float num2 = 0.15f;
		for (int i = 0; i < this.m_conditions.Length; i++)
		{
			bool flag = 0 < (this.m_condition & 1 << i);
			this.m_conditions[i].SetActive(flag);
			if (flag)
			{
				Vector3 localPosition = this.m_conditions[i].transform.localPosition;
				localPosition.y = num;
				this.m_conditions[i].transform.localPosition = localPosition;
				num -= num2;
			}
		}
	}

	public void UpdateMissions(List<Mission> a_missions)
	{
		int num = (a_missions == null) ? 0 : a_missions.Count;
		for (int i = 0; i < this.m_missions.Length; i++)
		{
			bool flag = i < num;
			this.m_missions[i].parent.gameObject.SetActive(flag);
			if (flag)
			{
				this.m_missions[i].renderer.material.mainTextureOffset = new Vector2(0.125f * ((float)a_missions[i].m_type - 1f), 0f);
			}
		}
	}

	public Transform m_barHealth;

	public Transform m_barEnergy;

	public GameObject m_inventory;

	public QmunicatorGUI m_comGui;

	public int m_debugCondition;

	public GameObject[] m_conditions;

	public Transform[] m_missions;

	private Vector3 m_startScale = Vector3.zero;

	private LidClient m_client;

	private bool m_active = true;

	private int m_condition = -1;
}

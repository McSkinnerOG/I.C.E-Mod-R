using System;
using UnityEngine;

public class DynamicChatLabel : MonoBehaviour
{
	public DynamicChatLabel()
	{
	}

	private void Start()
	{
		if (Global.isServer)
		{
			UnityEngine.Object.DestroyImmediate(this);
		}
		else
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_chatLabel, base.transform.position + this.m_addVector, Quaternion.Euler(55f, 0f, 0f));
			gameObject.transform.parent = base.transform;
			this.m_label = gameObject.GetComponent<ChatLabel>();
			this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
		}
	}

	private void Update()
	{
		if (null != this.m_label && null != this.m_client && Time.time > this.m_nextUpdateTime)
		{
			bool flag = (base.transform.position - this.m_client.GetPos()).sqrMagnitude < this.m_displayLabelDist * this.m_displayLabelDist;
			this.m_label.SetText((!flag) ? string.Empty : LNG.Get(this.m_lngKey), true);
			this.m_nextUpdateTime = Time.time + UnityEngine.Random.Range(0.5f, 1.5f);
		}
	}

	public GameObject m_chatLabel;

	public Vector3 m_addVector = Vector3.zero;

	public string m_lngKey = string.Empty;

	public float m_displayLabelDist = 12f;

	private float m_nextUpdateTime;

	private ChatLabel m_label;

	private LidClient m_client;
}

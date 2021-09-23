using System;
using UnityEngine;

public class Minimap : MonoBehaviour
{
	public Minimap()
	{
	}

	private void Start()
	{
		this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
		this.m_markerStartOffset = this.m_mapMarker.localPosition;
	}

	private void Update()
	{
		if (!Global.isServer && null != this.m_client && this.m_client.enabled)
		{
			float value = 1f - Mathf.Clamp01(this.m_client.GetHealth() * 0.01f) * 0.93f;
			float value2 = 1f - Mathf.Clamp01(this.m_client.GetEnergy() * 0.01f) * 0.93f;
			this.m_hpBar.material.SetFloat("_Cutoff", value);
			this.m_energyBar.material.SetFloat("_Cutoff", value2);
			Vector3 pos = this.m_client.GetPos();
			pos.x = pos.x / this.m_mapRadius * 0.25f;
			pos.y = pos.z / this.m_mapRadius * 0.25f;
			pos.z = 0f;
			this.m_mapMarker.localPosition = this.m_markerStartOffset + pos;
		}
	}

	public float m_mapRadius = 500f;

	public Transform m_mapMarker;

	public Renderer m_hpBar;

	public Renderer m_energyBar;

	private Vector3 m_markerStartOffset = Vector3.zero;

	private LidClient m_client;
}

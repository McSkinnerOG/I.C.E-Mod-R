using System;
using UnityEngine;

public class BirdCam : MonoBehaviour
{
	public BirdCam()
	{
	}

	private void Start()
	{
		this.m_dof = base.GetComponent<DepthOfFieldScatter>();
	}

	private void LateUpdate()
	{
		if (null == this.m_target)
		{
			return;
		}
		if (null != this.m_dof)
		{
			this.m_dof.focalTransform = this.m_target;
		}
		float deltaTime = Time.deltaTime;
		if (Time.time > this.m_nextSpeedUpdate)
		{
			this.m_speed = (this.m_lastTargetPos - this.m_target.position).magnitude / this.m_speedUpdateInterval;
			this.m_lastTargetPos = this.m_target.position;
			this.m_nextSpeedUpdate = Time.time + this.m_speedUpdateInterval;
		}
		float num = 0.25f * Mathf.Clamp01(this.m_speed / 15f);
		this.m_zoomAdd += (num - this.m_zoomAdd) * deltaTime;
		this.m_zoom = Mathf.Clamp(this.m_zoom - Input.GetAxis("Mouse ScrollWheel") * deltaTime * 5f, this.m_zoomMin, this.m_zoomMax);
		Vector3 b = this.m_startOffset * (this.m_zoom + this.m_zoomAdd);
		Vector3 a = this.m_target.forward * this.m_lookDirInfluence;
		this.m_targetOffset += (a - this.m_targetOffset) * deltaTime;
		base.transform.position = this.m_target.position + this.m_targetOffset + b;
	}

	public Transform m_target;

	public Vector3 m_startOffset = new Vector3(0f, 16f, 16f);

	public float m_lookDirInfluence = 2f;

	public float m_zoomMin = 0.4f;

	public float m_zoomMax = 1.2f;

	private float m_zoom = 1f;

	private float m_zoomAdd;

	private Vector3 m_targetOffset = Vector3.zero;

	private DepthOfFieldScatter m_dof;

	private float m_speed;

	private Vector3 m_lastTargetPos = Vector3.zero;

	private float m_nextSpeedUpdate;

	private float m_speedUpdateInterval = 0.3f;
}

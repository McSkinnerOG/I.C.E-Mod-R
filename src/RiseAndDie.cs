using System;
using UnityEngine;

public class RiseAndDie : MonoBehaviour
{
	public RiseAndDie()
	{
	}

	private void Start()
	{
		this.m_startPos = base.transform.localPosition;
		if (Vector3.zero == this.m_endPos)
		{
			this.m_endPos = base.transform.localPosition + this.m_riseVector;
		}
		this.m_renderers = base.GetComponentsInChildren<Renderer>();
	}

	public void SetEndByCollision(Vector3 a_collisionStartPos)
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(a_collisionStartPos, this.m_riseVector, out raycastHit))
		{
			this.m_riseVector = this.m_riseVector.normalized * (raycastHit.point - base.transform.localPosition).magnitude;
			this.m_endPos = base.transform.localPosition + this.m_riseVector;
		}
	}

	private void Update()
	{
		this.m_myTime += Time.deltaTime * Time.timeScale;
		this.m_progress = this.m_myTime / this.m_riseTime;
		float t = Mathf.Sin(1.5707964f * this.m_progress);
		base.transform.localPosition = Vector3.Lerp(this.m_startPos, this.m_endPos, t);
		foreach (Renderer renderer in this.m_renderers)
		{
			if (renderer.material.HasProperty("_Color"))
			{
				Color color = renderer.material.color;
				color.a = 1f - (this.m_progress - this.m_alphaFadeOutStart) / (1f - this.m_alphaFadeOutStart);
				renderer.material.color = color;
			}
		}
		if (this.m_progress >= 1f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public Vector3 m_riseVector = new Vector3(0f, 9f, 0f);

	public float m_riseTime = 2f;

	public float m_alphaFadeOutStart = 0.7f;

	private Vector3 m_startPos = Vector3.zero;

	private Vector3 m_endPos = Vector3.zero;

	private float m_myTime;

	private float m_progress;

	private Renderer[] m_renderers;
}

using System;
using UnityEngine;

public class GUI3dButton : MonoBehaviour
{
	public GUI3dButton()
	{
	}

	protected virtual void Start()
	{
		this.m_StartScale = base.transform.localScale;
		this.m_startX = base.transform.localPosition.x;
		this.m_guimaster = (GUI3dMaster)UnityEngine.Object.FindObjectOfType(typeof(GUI3dMaster));
		if (null != this.m_guimaster && this.m_changePosOnRatioChange)
		{
			Vector3 localPosition = base.transform.localPosition;
			localPosition.x = this.m_startX * this.m_guimaster.GetRatioMultiplier();
			base.transform.localPosition = localPosition;
		}
	}

	protected virtual void Update()
	{
		this.ProgressAnimation();
	}

	private void ProgressAnimation()
	{
		this.m_animPlaying = (this.m_animate || (base.transform.localScale - this.m_StartScale).sqrMagnitude > 1E-05f);
		if (this.m_animate)
		{
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, this.m_StartScale * this.m_clickAnimSize, 0.5f);
			if ((this.m_StartScale * this.m_clickAnimSize - base.transform.localScale).sqrMagnitude < 1E-05f)
			{
				this.m_animate = false;
			}
		}
		else if (this.m_animPlaying)
		{
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, this.m_StartScale, 0.5f);
		}
	}

	public void Animate()
	{
		this.m_animate = true;
	}

	public bool IsAnimating()
	{
		return this.m_animPlaying;
	}

	public float m_clickAnimSize = 1.5f;

	public bool m_changePosOnRatioChange = true;

	private float m_startX;

	private Vector3 m_StartScale;

	protected GUI3dMaster m_guimaster;

	protected bool m_animate;

	protected bool m_animPlaying;
}

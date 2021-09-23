using System;
using UnityEngine;

public class ScaleAnimOnHit : MonoBehaviour
{
	public ScaleAnimOnHit()
	{
	}

	private void Start()
	{
		this.m_animTrans = ((!this.m_animateParent || !(null != base.transform.parent)) ? base.transform : base.transform.parent);
		this.m_startScale = this.m_animTrans.localScale;
	}

	private void Update()
	{
		if (-1f < this.m_animProgress)
		{
			this.m_animProgress = Mathf.Clamp01(this.m_animProgress + Time.deltaTime * 10f);
			this.m_animTrans.localScale = this.m_startScale * (1f - 0.1f * Mathf.Sin(3.1415927f * this.m_animProgress));
			if (this.m_animProgress == 1f)
			{
				this.m_animProgress = -1f;
			}
		}
	}

	private void SetAggressor(Transform a_aggressor)
	{
		if (!Global.isServer)
		{
			this.m_animProgress = 0f;
			if (null != base.audio && this.m_audioEffects != null && 0 < this.m_audioEffects.Length)
			{
				base.audio.clip = this.m_audioEffects[UnityEngine.Random.Range(0, this.m_audioEffects.Length)];
				base.audio.Play();
			}
		}
	}

	public bool m_animateParent = true;

	public AudioClip[] m_audioEffects;

	private Vector3 m_startScale = Vector3.zero;

	private Transform m_animTrans;

	private float m_animProgress = -1f;
}

using System;
using UnityEngine;

public class ImageEffects : MonoBehaviour
{
	public ImageEffects()
	{
	}

	private void Start()
	{
		this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
		this.m_vignetteEffect = base.GetComponent<Vignetting>();
		this.m_overlayEffect = base.GetComponent<ScreenOverlay>();
		if (2 > QualitySettings.GetQualityLevel())
		{
			SSAOEffect component = base.GetComponent<SSAOEffect>();
			if (null != component)
			{
				UnityEngine.Object.Destroy(component);
			}
		}
	}

	private void Update()
	{
		if (!Global.isServer && null != this.m_client && this.m_client.enabled)
		{
			float num = 1f - Mathf.Clamp01(this.m_client.GetHealth() * 0.01f);
			float num2 = 1f - Mathf.Clamp01(this.m_client.GetEnergy() * 0.01f);
			float num3 = 0.5f + (FastSin.Get(this.m_sinProgress) + 1f) * 0.25f;
			this.m_sinProgress += Time.deltaTime * this.m_pulseSpeed;
			if (this.m_sinProgress > 6.2831855f)
			{
				this.m_sinProgress -= 6.2831855f;
			}
			this.m_vignetteEffect.intensity = this.m_minVignette + num2 * (this.m_maxVignette - this.m_minVignette);
			this.m_overlayEffect.intensity = num * this.m_maxOverlay * num3;
		}
	}

	public float m_minVignette = 1f;

	public float m_maxVignette = 5f;

	public float m_maxOverlay = 0.5f;

	public float m_pulseSpeed = 2f;

	private Vignetting m_vignetteEffect;

	private ScreenOverlay m_overlayEffect;

	private float m_sinProgress;

	private LidClient m_client;
}

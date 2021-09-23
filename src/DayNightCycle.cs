using System;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
	public DayNightCycle()
	{
	}

	private void Start()
	{
		this.m_maxIntensity = base.light.intensity;
		this.m_maxBgIntensity = this.m_backgroundDirLight.intensity;
	}

	private void FixedUpdate()
	{
		this.m_progress += Time.fixedDeltaTime * this.m_speed;
		if (this.m_progress > 1f)
		{
			this.m_progress -= 1f;
		}
		float num = 1f;
		float lightIntensity = Util.GetLightIntensity(this.m_progress, out num);
		base.light.intensity = lightIntensity * this.m_maxIntensity;
		this.m_backgroundDirLight.intensity = lightIntensity * this.m_maxBgIntensity;
		Vector3 position = base.transform.position;
		position.x = -(this.m_moveDistance * this.m_progress * num - this.m_moveDistance * 0.5f);
		base.transform.position = position;
		base.transform.LookAt(this.m_target);
		base.audio.volume = Mathf.Clamp01(1f - this.GetDayLight() - 0.8f);
		this.HandleHighlight();
	}

	private void HandleHighlight()
	{
		float num = FastSin.Get(Time.time * 3f);
		float num2 = 0.2f;
		this.m_highlight.intensity = base.light.intensity + num2 + num * (base.light.intensity * 0.1f + num2);
		this.m_highlightMouse.intensity = base.light.intensity + 0.2f;
	}

	public void Init(float a_progress, float a_speed)
	{
		this.m_progress = a_progress;
		this.m_speed = a_speed;
	}

	public float GetDayLight()
	{
		return base.light.intensity;
	}

	public float GetProgress()
	{
		return this.m_progress;
	}

	public string GetTime()
	{
		float num = this.m_progress * 24f + 4f;
		if (num >= 24f)
		{
			num -= 24f;
		}
		string text = ((int)((num - (float)((int)num)) * 60f)).ToString();
		if (text.Length == 1)
		{
			text = "0" + text;
		}
		string text2 = ((int)num).ToString();
		if (text2.Length == 1)
		{
			text2 = "0" + text2;
		}
		return text2 + ":" + text;
	}

	public float m_speed = 0.01f;

	public float m_moveDistance = 10f;

	public Transform m_target;

	public Light m_highlight;

	public Light m_highlightMouse;

	public Light m_backgroundDirLight;

	private float m_progress;

	private float m_maxIntensity;

	private float m_maxBgIntensity;
}

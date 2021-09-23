using System;
using UnityEngine;

public class ClientVehicle : MonoBehaviour
{
	public ClientVehicle()
	{
	}

	private void Start()
	{
		this.m_startRotLeft = this.m_leftWheel.localRotation;
		this.m_startRotRight = this.m_rightWheel.localRotation;
		this.m_lastPos = base.transform.position;
		this.m_lastRot = base.transform.rotation.eulerAngles.y;
		this.m_dayNight = (DayNightCycle)UnityEngine.Object.FindObjectOfType(typeof(DayNightCycle));
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		Vector3 rhs = base.transform.position - this.m_lastPos;
		float magnitude = rhs.magnitude;
		float num = magnitude / deltaTime;
		float num2 = base.transform.rotation.eulerAngles.y - this.m_lastRot;
		float num3 = (0f <= Vector3.Dot(rhs.normalized, base.transform.forward)) ? 1f : -1f;
		this.UpdateSound(deltaTime, num);
		if (num2 != 0f)
		{
			float num4 = 0f;
			if (Mathf.Abs(num2) > 10f * deltaTime)
			{
				num4 = num3 * ((num2 <= 0f) ? -1f : 1f);
			}
			this.m_leftWheel.localRotation = Quaternion.Lerp(this.m_leftWheel.localRotation, this.m_startRotLeft * Quaternion.Euler(0f, num4 * 35f, 0f), deltaTime * 4f);
			this.m_rightWheel.localRotation = Quaternion.Lerp(this.m_rightWheel.localRotation, this.m_startRotRight * Quaternion.Euler(0f, num4 * 35f, 0f), deltaTime * 4f);
			float num5 = (Mathf.Abs(num4) <= 0.5f) ? 0f : (Mathf.Abs(num4) * 0.05f);
			float num6 = Mathf.Max(num - 12f, 0f) * 0.25f;
			this.m_soundSqueel.volume = Mathf.Clamp(Mathf.Lerp(this.m_soundSqueel.volume, num6 * num5, deltaTime), 0f, 0.12f);
		}
		else
		{
			this.m_soundSqueel.volume = 0f;
		}
		if (Vector3.zero != rhs)
		{
			float x = num * num3;
			this.m_leftWheel.localRotation *= Quaternion.Euler(x, 0f, 0f);
			this.m_rightWheel.localRotation *= Quaternion.Euler(x, 0f, 0f);
			this.m_rearWheels[0].localRotation *= Quaternion.Euler(x, 0f, 0f);
			this.m_rearWheels[1].localRotation *= Quaternion.Euler(x, 0f, 0f);
			this.m_nextEngineStartSound = Time.time + 3f;
		}
		this.m_lastPos = base.transform.position;
		this.m_lastRot = base.transform.rotation.eulerAngles.y;
		this.m_light.enabled = (this.m_dayNight.GetDayLight() == 0f && this.m_health > 0f);
	}

	private void UpdateSound(float a_dt, float a_speedMs)
	{
		float num = (a_speedMs <= 1f) ? 0f : Mathf.Clamp01(a_speedMs / this.m_maxNoiseSpeed);
		float to = 1f + Mathf.Clamp01(a_speedMs / this.m_maxNoiseSpeed) * 0.3f;
		if (Time.time > this.m_nextEngineStartSound && base.audio.volume == 0f && 0f < num)
		{
			this.m_soundCarStart.Play();
		}
		base.audio.volume = ((num != 0f) ? Mathf.Lerp(base.audio.volume, num, a_dt * 2f) : 0f);
		base.audio.pitch = Mathf.Lerp(base.audio.pitch, to, a_dt * 2f);
	}

	private void SetHealth(float a_health)
	{
		if (a_health < this.m_health && base.audio.volume > 0f)
		{
			this.m_soundCrash.Play();
		}
		this.m_health = a_health;
		this.m_damageFire.SetActive(this.m_health < 20f && 0f < this.m_health);
		if (this.m_carPartsStartRot == null)
		{
			this.m_carPartsStartRot = new Quaternion[this.m_carParts.Length];
			for (int i = 0; i < this.m_carParts.Length; i++)
			{
				this.m_carPartsStartRot[i] = this.m_carParts[i].localRotation;
			}
		}
		for (int j = 0; j < this.m_carParts.Length; j++)
		{
			this.m_carParts[j].localRotation = Quaternion.Lerp(this.m_carPartsStartRot[j], UnityEngine.Random.rotation, (100f - a_health) * 0.01f * this.m_maxCarPartRot);
		}
	}

	public Transform m_leftWheel;

	public Transform m_rightWheel;

	public Transform[] m_rearWheels;

	public float m_maxCarPartRot = 0.05f;

	public Light m_light;

	public Transform[] m_carParts;

	public float m_maxNoiseSpeed = 15f;

	public AudioSource m_soundCarStart;

	public AudioSource m_soundCrash;

	public AudioSource m_soundSqueel;

	public GameObject m_damageFire;

	private Quaternion[] m_carPartsStartRot;

	private Quaternion m_startRotLeft;

	private Quaternion m_startRotRight;

	private Vector3 m_lastPos = Vector3.zero;

	private float m_lastRot;

	private DayNightCycle m_dayNight;

	private float m_health = 100f;

	private float m_nextEngineStartSound;
}

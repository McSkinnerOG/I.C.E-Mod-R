using System;
using UnityEngine;

public class CharSounds : MonoBehaviour
{
	public CharSounds()
	{
	}

	private void Start()
	{
		this.m_lastPos = base.transform.position;
	}

	private void FixedUpdate()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		bool flag = (base.transform.position - this.m_lastPos).sqrMagnitude > 0.001f;
		this.m_lastPos = base.transform.position;
		if (flag)
		{
			this.m_timeToNextStep -= fixedDeltaTime;
			if (this.m_timeToNextStep < 0f && this.m_stepSounds != null && 0 < this.m_stepSounds.Length)
			{
				base.audio.clip = this.m_stepSounds[UnityEngine.Random.Range(0, this.m_stepSounds.Length)];
				base.audio.Play();
				this.m_timeToNextStep = this.m_stepIntervall;
			}
		}
	}

	private void InstantiateSound(AudioClip a_clip, float a_volume = 1f)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_audioPrefab, base.transform.position, Quaternion.identity);
		TimedDestroy component = gameObject.GetComponent<TimedDestroy>();
		component.m_destroyAfter = a_clip.length + 0.1f;
		gameObject.audio.clip = a_clip;
		gameObject.audio.volume = a_volume;
		gameObject.audio.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
		gameObject.audio.Play();
	}

	public void Attack(ItemDef a_weapon)
	{
		if (a_weapon.ammoItemType > 0)
		{
			this.InstantiateSound(this.m_rangedSounds[Items.GetAmmoSoundIndex(a_weapon.ammoItemType)], 0.4f);
		}
		else
		{
			this.InstantiateSound(this.m_meleeSounds[UnityEngine.Random.Range(0, this.m_meleeSounds.Length)], 0.4f);
		}
	}

	public void Suffer(bool a_isDead)
	{
		this.InstantiateSound(this.m_sufferSounds[UnityEngine.Random.Range(0, this.m_sufferSounds.Length)], 0.4f);
	}

	public float m_stepIntervall = 0.5f;

	public GameObject m_audioPrefab;

	public AudioClip[] m_stepSounds;

	public AudioClip[] m_sufferSounds;

	public AudioClip[] m_rangedSounds;

	public AudioClip[] m_meleeSounds;

	private Vector3 m_lastPos = Vector3.zero;

	private float m_timeToNextStep;
}

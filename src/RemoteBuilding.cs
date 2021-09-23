using System;
using UnityEngine;

public class RemoteBuilding : MonoBehaviour
{
	public RemoteBuilding()
	{
	}

	public void Refresh(float a_yRot, float a_health)
	{
		if (this.m_explosionTimer == -1f)
		{
			return;
		}
		if (!this.m_visible)
		{
			this.SwitchVisibility();
			if (base.transform.rotation.eulerAngles.y != a_yRot)
			{
				base.transform.rotation = Quaternion.Euler(0f, a_yRot, 0f);
			}
		}
		Quaternion targetRot = this.m_targetRot;
		this.m_targetRot = Quaternion.Euler(0f, a_yRot, 0f);
		if (base.transform.rotation != this.m_targetRot && base.transform.rotation == targetRot && null != base.audio)
		{
			base.audio.Stop();
			base.audio.clip = this.m_squeelSound;
			base.audio.Play();
		}
		this.m_health = a_health;
		if (null != this.m_animation)
		{
			bool flag = this.m_health < 50f;
			this.m_animation.gameObject.SetActive(flag);
			if (null != this.m_collider)
			{
				this.m_collider.enabled = !flag;
			}
		}
		this.m_lastUpdate = Time.time;
	}

	public void Init(Vector3 a_pos, int a_type, bool a_isMine, bool a_isStatic)
	{
		base.transform.position = a_pos;
		this.m_type = a_type;
		this.m_isMine = a_isMine;
		this.m_isStatic = a_isStatic;
		if (!a_isStatic)
		{
			GameObject gameObject = (GameObject)Resources.Load("buildings/building_" + a_type);
			if (null != gameObject)
			{
				GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(gameObject, base.transform.position, Quaternion.identity);
				gameObject2.transform.parent = base.transform;
				this.m_animation = gameObject2.transform.Find("Animation");
				ServerBuilding componentInChildren = base.GetComponentInChildren<ServerBuilding>();
				if (null != componentInChildren)
				{
					UnityEngine.Object.Destroy(componentInChildren);
				}
			}
			else
			{
				GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				gameObject2.transform.position = base.transform.position;
				gameObject2.transform.localScale = Vector3.one * 0.5f;
				gameObject2.transform.parent = base.transform;
			}
			this.m_lastUpdate = Time.time;
		}
		else
		{
			this.m_animation = base.transform.Find("Animation");
		}
		this.m_collider = base.collider;
		if (null == this.m_collider)
		{
			this.m_collider = base.GetComponentInChildren<Collider>();
		}
		if (null != base.audio && Time.timeSinceLevelLoad > 5f)
		{
			base.audio.clip = this.m_buildSound;
			base.audio.Play();
		}
		if (this.m_type == 102)
		{
			this.m_explosionTimer = Time.time + ((float)Buildings.GetBuildingDef(102).decayTime - 0.3f);
		}
	}

	private void FixedUpdate()
	{
		if (this.m_visible)
		{
			float t = Time.fixedDeltaTime * 5f;
			if (this.m_targetRot != base.transform.rotation)
			{
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.m_targetRot, t);
			}
			if (!this.m_isStatic && this.m_lastUpdate + this.m_disappearTime < Time.time)
			{
				this.SwitchVisibility();
			}
		}
		else if (!this.m_isStatic && this.m_lastUpdate + this.m_dieTime < Time.time)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		if (Time.time > this.m_explosionTimer && this.m_explosionTimer > 0f)
		{
			Vector3 position = base.transform.position + Vector3.up * 0.5f + (Camera.main.transform.position - base.transform.position) * 0.25f;
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_explosion, position, Quaternion.identity);
			gameObject.transform.parent = Camera.main.transform;
			this.SwitchVisibility();
			this.m_explosionTimer = -1f;
		}
	}

	public bool IsVisible()
	{
		return this.m_visible;
	}

	public bool IsExploding()
	{
		return Mathf.Abs(Time.time - this.m_explosionTimer) < 0.5f;
	}

	public void SwitchVisibility()
	{
		this.m_visible = !this.m_visible;
		if (null != this.m_collider)
		{
			this.m_collider.enabled = this.m_visible;
		}
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.enabled = this.m_visible;
		}
	}

	private const int c_tntBuildingType = 102;

	[HideInInspector]
	public int m_type;

	[HideInInspector]
	public bool m_isMine;

	[HideInInspector]
	public bool m_isStatic;

	[HideInInspector]
	public float m_health = 100f;

	public AudioClip m_squeelSound;

	public AudioClip m_buildSound;

	public GameObject m_explosion;

	private float m_explosionTimer;

	private Quaternion m_targetRot = Quaternion.identity;

	private bool m_visible;

	private float m_lastUpdate;

	private float m_disappearTime = 0.5f;

	private float m_dieTime = 10f;

	private Transform m_animation;

	private Collider m_collider;
}

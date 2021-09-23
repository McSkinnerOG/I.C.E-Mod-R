using System;
using UnityEngine;

public class ControlledChar : MonoBehaviour
{
	public ControlledChar()
	{
	}

	public bool IsMoving()
	{
		return this.m_isMoving;
	}

	public bool HasUseChanged()
	{
		return this.m_useChanged;
	}

	public bool HasSpaceChanged()
	{
		return this.m_spaceChanged;
	}

	public void AssignInput(float a_v, float a_h, bool a_use, bool a_space)
	{
		this.m_useChanged = (a_use != this.m_use);
		this.m_spaceChanged = (a_space != this.m_space);
		this.m_axis_v = a_v;
		this.m_axis_h = a_h;
		this.m_use = a_use;
		this.m_space = a_space;
	}

	public void Init(ServerPlayer a_player)
	{
		this.m_serverPlayer = a_player;
	}

	public ServerPlayer GetServerPlayer()
	{
		return this.m_serverPlayer;
	}

	public void AddSpeed(float a_percent)
	{
		this.m_curSpeed = this.m_speed * (1f + a_percent);
	}

	public void SetForceRotation(float a_rot)
	{
		if (a_rot != -1f)
		{
			this.m_rot = a_rot;
			this.m_ignoreMoveRotTime = Time.time + 0.5f;
		}
	}

	public float GetRotation()
	{
		return this.m_rot;
	}

	private void Start()
	{
		this.m_controller = base.GetComponent<CharacterController>();
		this.m_curSpeed = this.m_speed;
		this.m_testMode &= Application.isEditor;
	}

	private void Update()
	{
		if (null != this.m_aggressor && this.m_changeHealth != 0f && this.m_serverPlayer != null)
		{
			if (this.m_aggressor.gameObject.layer != 13 || !this.m_serverPlayer.IsSaint())
			{
				this.m_serverPlayer.ChangeHealthBy(this.m_changeHealth);
			}
			this.m_aggressor = null;
			this.m_changeHealth = 0f;
		}
		if (this.m_testMode)
		{
			float axis = Input.GetAxis("Horizontal");
			float axis2 = Input.GetAxis("Vertical");
			this.AssignInput(axis2, axis, false, false);
		}
		this.Move(Time.deltaTime);
	}

	private void Move(float a_dt)
	{
		this.m_isMoving = false;
		if (this.m_controller.enabled && (this.m_serverPlayer == null || !this.m_serverPlayer.IsDead()))
		{
			float d = (!this.m_isWalking) ? this.m_curSpeed : (this.m_curSpeed * 0.6f);
			Vector3 vector = Vector3.forward * this.m_axis_v + Vector3.right * this.m_axis_h;
			if (Vector3.zero != vector)
			{
				if (Time.time > this.m_ignoreMoveRotTime)
				{
					float num = (!this.m_isWalking) ? this.m_rotationSpeed2 : (this.m_rotationSpeed2 * 0.5f);
					this.m_rot = Quaternion.Lerp(Quaternion.Euler(0f, this.m_rot, 0f), Quaternion.LookRotation(vector), a_dt * num).eulerAngles.y;
				}
				Vector3 vector2 = vector.normalized * d * a_dt;
				if (this.m_testMode || 0.8f < Util.GetTerrainHeight(base.transform.position + vector2))
				{
					this.m_controller.Move(vector2);
				}
				this.m_isMoving = true;
			}
		}
	}

	private void LateUpdate()
	{
		Vector3 position = base.transform.position;
		position.y = 0f;
		base.transform.position = position;
	}

	private void OnCollisionEnter(Collision a_col)
	{
		float num = 0.5f * Mathf.Clamp(a_col.relativeVelocity.sqrMagnitude - 10f, 0f, 10000f);
		if (num > 1f)
		{
			this.ChangeHealthBy(-num);
			this.m_aggressor = a_col.transform;
			a_col.gameObject.SendMessage("CausedDamage", num, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void ChangeHealthBy(float a_delta)
	{
		this.m_changeHealth = a_delta;
	}

	private void SetAggressor(Transform a_aggressor)
	{
		this.m_aggressor = a_aggressor;
	}

	public bool m_testMode;

	[HideInInspector]
	public bool m_isWalking;

	public float m_speed = 8f;

	public float m_rotationSpeed2 = 6f;

	private CharacterController m_controller;

	private ServerPlayer m_serverPlayer;

	private float m_ignoreMoveRotTime = -1f;

	private float m_rot = 180f;

	private bool m_isMoving;

	private float m_curSpeed;

	private float m_changeHealth;

	private Transform m_aggressor;

	private float m_axis_v;

	private float m_axis_h;

	private bool m_use;

	private bool m_space;

	private bool m_useChanged;

	private bool m_spaceChanged;
}

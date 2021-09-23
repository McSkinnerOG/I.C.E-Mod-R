using System;
using UnityEngine;

public class WheelCar : MonoBehaviour
{
	public WheelCar()
	{
	}

	public void AssignInput(bool a_w, bool a_a, bool a_s, bool a_d, bool a_space)
	{
		if (a_w)
		{
			this.m_axis_v = 1f;
		}
		else if (a_s)
		{
			this.m_axis_v = -1f;
		}
		else
		{
			this.m_axis_v = 0f;
		}
		if (a_d)
		{
			this.m_axis_h_target = 1f;
		}
		else if (a_a)
		{
			this.m_axis_h_target = -1f;
		}
		else
		{
			this.m_axis_h_target = 0f;
		}
		this.m_space = a_space;
	}

	private void Start()
	{
		this.m_wheels = base.gameObject.GetComponentsInChildren<WheelCollider>();
		base.rigidbody.centerOfMass = this.m_centerOfMass;
	}

	private void FixedUpdate()
	{
		this.m_speed = base.rigidbody.velocity.magnitude;
		if (!this.m_isControlledByPlayer)
		{
			return;
		}
		float num = (this.m_maxControlSpeed - Mathf.Min(this.m_speed, this.m_maxControlSpeed)) / this.m_maxControlSpeed;
		float num2 = 0.1f * (this.m_torque * this.m_frontFriction) + 0.8f * num * (this.m_torque * this.m_frontFriction);
		this.m_maxRearSlip = 0.5f * (this.m_torque * this.m_rearFriction) + num * (this.m_torque * this.m_rearFriction);
		bool flag = Vector3.Dot(base.rigidbody.velocity.normalized, base.transform.forward) > 0f;
		bool handbrake = this.GetHandbrake();
		if (handbrake)
		{
			float num3 = this.m_maxRearSlip * 0.4f;
			float num4 = this.m_maxRearSlip * 0.6f;
			this.m_rearSlipTorque = ((!flag) ? num4 : num3);
			this.m_frontSlipTorque = ((!flag) ? num3 : num4);
		}
		else
		{
			if (this.m_rearSlipTorque < this.m_maxRearSlip * 0.9f)
			{
				this.m_rearSlipTorque += this.m_maxRearSlip * 0.01f;
			}
			else
			{
				this.m_rearSlipTorque = this.m_maxRearSlip;
			}
			if (this.m_frontSlipTorque < num2 * 0.9f)
			{
				this.m_frontSlipTorque += num2 * 0.01f;
			}
			else
			{
				this.m_frontSlipTorque = num2;
			}
		}
		this.HandleWheels(handbrake, num2, this.m_maxRearSlip, flag);
		if (this.m_testText)
		{
			this.m_testText.text = ((int)(this.m_speed * 3.6f)).ToString() + " kmh" + ((int)this.m_distanceDriven).ToString() + " m";
		}
	}

	private void Update()
	{
		if (Mathf.Abs(this.m_axis_h_target - this.m_axis_h) > 0.05f)
		{
			this.m_axis_h += Mathf.Clamp(Time.deltaTime * this.m_steerSpeed * ((this.m_axis_h_target >= this.m_axis_h) ? 1f : -1f), -1f, 1f);
		}
		else
		{
			this.m_axis_h = this.m_axis_h_target;
		}
		if (this.m_testMode && Application.isEditor)
		{
			this.AssignInput(Input.GetKey(KeyCode.W), Input.GetKey(KeyCode.A), Input.GetKey(KeyCode.S), Input.GetKey(KeyCode.D), Input.GetKey(KeyCode.Space));
		}
	}

	private void OnCollisionEnter(Collision col)
	{
		if (col.relativeVelocity.sqrMagnitude > 10f)
		{
			float num = Mathf.Clamp01(1.5f - Mathf.Clamp01((col.relativeVelocity.sqrMagnitude - 10f) * 0.001f));
			this.m_rearSlipTorque *= num;
			this.m_frontSlipTorque *= num;
		}
	}

	private void HandleWheels(bool isBraking, float maxFrontSlip, float maxRearSlip, bool isDrivingForward)
	{
		float steering = this.GetSteering();
		this.m_groundedWheelCount = 0;
		foreach (WheelCollider wheelCollider in this.m_wheels)
		{
			this.m_groundedWheelCount += ((!wheelCollider.isGrounded) ? 0 : 1);
			bool flag = "tire_fl" == wheelCollider.gameObject.name || "tire_fr" == wheelCollider.gameObject.name;
			if (isBraking)
			{
				wheelCollider.brakeTorque = ((!flag) ? ((!isDrivingForward) ? (this.m_brakeTorque * 0.4f) : (this.m_brakeTorque * 0.6f)) : 0f);
				wheelCollider.motorTorque = 0f;
			}
			else
			{
				float num = this.GetAcceleration() * this.m_torque;
				this.m_usesOrdinaryBrake = ((num > 0f && !isDrivingForward) || (num < 0f && isDrivingForward));
				if (!this.m_usesOrdinaryBrake)
				{
					wheelCollider.brakeTorque = 0f;
					if (isDrivingForward)
					{
						float num2 = num * 0.5f + Mathf.Clamp((steering + 0.5f) * num, 0f, num);
						float num3 = num * 0.5f + Mathf.Clamp((steering * -1f + 0.5f) * num, 0f, num);
						if (this.m_allWheelDrive)
						{
							if ("tire_rr" == wheelCollider.gameObject.name || "tire_fr" == wheelCollider.gameObject.name)
							{
								wheelCollider.motorTorque = num3 * 0.5f;
							}
							else
							{
								wheelCollider.motorTorque = num2 * 0.5f;
							}
						}
						else if ("tire_rr" == wheelCollider.gameObject.name)
						{
							wheelCollider.motorTorque = num3;
						}
						else if ("tire_rl" == wheelCollider.gameObject.name)
						{
							wheelCollider.motorTorque = num2;
						}
						else
						{
							wheelCollider.motorTorque = 0f;
						}
					}
					else
					{
						wheelCollider.motorTorque = ((!flag) ? num : 0f);
					}
				}
				else
				{
					num = Mathf.Abs((this.GetAcceleration() + ((!isDrivingForward) ? 0.1f : -0.1f)) * this.m_brakeTorque);
					wheelCollider.brakeTorque = ((!flag) ? (num * 0.6f) : (num * 0.4f));
					wheelCollider.motorTorque = 0f;
					this.m_rearSlipTorque = maxRearSlip * 0.66f;
				}
			}
			WheelFrictionCurve sidewaysFriction = wheelCollider.sidewaysFriction;
			if (flag)
			{
				wheelCollider.steerAngle = steering * this.m_steerAngle;
				sidewaysFriction.extremumValue = this.m_frontSlipTorque;
				sidewaysFriction.asymptoteValue = this.m_frontSlipTorque * 0.5f;
			}
			else
			{
				sidewaysFriction.extremumValue = this.m_rearSlipTorque;
				sidewaysFriction.asymptoteValue = this.m_rearSlipTorque * 0.5f;
			}
			wheelCollider.sidewaysFriction = sidewaysFriction;
		}
	}

	private float GetSteering()
	{
		float value = Mathf.Clamp(this.m_axis_h * 1.5f, -1f, 1f);
		return Mathf.Clamp(value, -1f, 1f);
	}

	private float GetAcceleration()
	{
		float value = (Mathf.Abs(this.m_speed) >= this.m_maxSpeed) ? 0f : (this.m_axis_v * 1.5f);
		return Mathf.Clamp(value, -1f, 1f);
	}

	private bool GetHandbrake()
	{
		return this.m_space;
	}

	public float GetDistanceDriven()
	{
		return this.m_distanceDriven;
	}

	public bool m_testMode;

	public GUIText m_testText;

	public float m_torque = 600f;

	public float m_brakeTorque = 1800f;

	public bool m_allWheelDrive;

	public float m_steerAngle = 20f;

	public float m_steerSpeed = 4f;

	public float m_maxSpeed = 15f;

	public float m_maxControlSpeed = 15f;

	public Vector3 m_centerOfMass = new Vector3(0f, 0.3f, 0.5f);

	public float m_frontFriction = 1f;

	public float m_rearFriction = 1f;

	private WheelCollider[] m_wheels;

	private float m_speed;

	private float m_frontSlipTorque;

	private float m_rearSlipTorque;

	private bool m_isControlledByPlayer = true;

	private int m_groundedWheelCount;

	private float m_distanceDriven;

	private bool m_usesOrdinaryBrake;

	private float m_maxRearSlip;

	private float m_axis_v;

	private float m_axis_h;

	private float m_axis_h_target;

	private bool m_space;
}

using System;
using UnityEngine;

public class AntiRoll : MonoBehaviour
{
	public AntiRoll()
	{
	}

	private void FixedUpdate()
	{
		WheelHit wheelHit = default(WheelHit);
		float num = 1f;
		float num2 = 1f;
		float num3 = 1f;
		float num4 = 1f;
		bool groundHit = this.m_wheelFL.GetGroundHit(out wheelHit);
		if (groundHit)
		{
			num = (-this.m_wheelFL.transform.InverseTransformPoint(wheelHit.point).y - this.m_wheelFL.radius) / this.m_wheelFL.suspensionDistance;
		}
		bool groundHit2 = this.m_wheelFR.GetGroundHit(out wheelHit);
		if (groundHit2)
		{
			num2 = (-this.m_wheelFR.transform.InverseTransformPoint(wheelHit.point).y - this.m_wheelFR.radius) / this.m_wheelFR.suspensionDistance;
		}
		bool groundHit3 = this.m_wheelRL.GetGroundHit(out wheelHit);
		if (groundHit3)
		{
			num3 = (-this.m_wheelRL.transform.InverseTransformPoint(wheelHit.point).y - this.m_wheelRL.radius) / this.m_wheelRL.suspensionDistance;
		}
		bool groundHit4 = this.m_wheelRR.GetGroundHit(out wheelHit);
		if (groundHit4)
		{
			num4 = (-this.m_wheelRR.transform.InverseTransformPoint(wheelHit.point).y - this.m_wheelRR.radius) / this.m_wheelRR.suspensionDistance;
		}
		float num5 = (num - num2) * this.m_wheelFL.suspensionSpring.spring * this.m_antiRoll;
		float num6 = (num3 - num4) * this.m_wheelRL.suspensionSpring.spring * this.m_antiRoll;
		if (groundHit)
		{
			base.rigidbody.AddForceAtPosition(this.m_wheelFL.transform.up * -num5, this.m_wheelFL.transform.position);
		}
		if (groundHit2)
		{
			base.rigidbody.AddForceAtPosition(this.m_wheelFR.transform.up * num5, this.m_wheelFR.transform.position);
		}
		if (groundHit3)
		{
			base.rigidbody.AddForceAtPosition(this.m_wheelRL.transform.up * -num6, this.m_wheelRL.transform.position);
		}
		if (groundHit4)
		{
			base.rigidbody.AddForceAtPosition(this.m_wheelRR.transform.up * num6, this.m_wheelRR.transform.position);
		}
	}

	public WheelCollider m_wheelFL;

	public WheelCollider m_wheelFR;

	public WheelCollider m_wheelRL;

	public WheelCollider m_wheelRR;

	public float m_antiRoll = 1f;
}

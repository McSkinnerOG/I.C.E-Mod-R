using System;
using UnityEngine;

public class BallPhysics : MonoBehaviour
{
	public BallPhysics()
	{
	}

	private void Start()
	{
		this.m_startPos = base.transform.position;
	}

	private void FixedUpdate()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		if (base.transform.position.y < 1.1f)
		{
			base.transform.position = this.m_startPos;
			base.rigidbody.velocity = Vector3.zero;
		}
		Vector3 velocity = base.rigidbody.velocity;
		if (velocity.magnitude > this.m_maxMagnitude)
		{
			velocity = velocity.normalized * this.m_maxMagnitude;
		}
		velocity.y += this.m_gravity * fixedDeltaTime;
		base.rigidbody.velocity = velocity;
	}

	public float m_maxMagnitude = 10f;

	public float m_gravity = -20f;

	private Vector3 m_startPos;
}

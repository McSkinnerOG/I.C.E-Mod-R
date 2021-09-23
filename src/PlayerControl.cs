using System;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
	public PlayerControl()
	{
	}

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		this.m_isGrounded = (base.transform.position.y < 1.6f);
		if (this.m_isGrounded)
		{
			this.m_lastGroundedTime = Time.time;
		}
		Vector3 velocity = base.rigidbody.velocity;
		velocity.y += this.m_gravity * fixedDeltaTime;
		if (Input.GetKey(KeyCode.W))
		{
			if (this.m_isGrounded)
			{
				velocity.y = this.m_jumpForce;
			}
			else if (Time.time - this.m_lastGroundedTime < 0.3f)
			{
				velocity.y += this.m_jumpForce * fixedDeltaTime;
			}
		}
		velocity.x = Input.GetAxis("Horizontal") * this.m_moveForce;
		base.rigidbody.velocity = velocity;
	}

	private void Update()
	{
	}

	public float m_jumpForce = 100f;

	public float m_moveForce = 100f;

	public float m_gravity = -20f;

	private bool m_isGrounded = true;

	private float m_lastGroundedTime;
}

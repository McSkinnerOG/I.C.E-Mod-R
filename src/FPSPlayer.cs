using System;
using UnityEngine;

public class FPSPlayer : MonoBehaviour
{
	public FPSPlayer()
	{
	}

	private void Start()
	{
		this.m_char = base.GetComponent<CharacterController>();
	}

	private void FixedUpdate()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		float y = base.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * this.m_mouseSensitivity;
		base.transform.localEulerAngles = new Vector3(0f, y, 0f);
		float axis = Input.GetAxis("Horizontal");
		float axis2 = Input.GetAxis("Vertical");
		Vector3 vector = base.transform.forward * axis2 + base.transform.right * axis;
		this.m_char.Move((vector.normalized + Vector3.up * -5f) * fixedDeltaTime * this.m_speed);
	}

	public float m_mouseSensitivity = 5f;

	public float m_speed = 5f;

	private CharacterController m_char;
}

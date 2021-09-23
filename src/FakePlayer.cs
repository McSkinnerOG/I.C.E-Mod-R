using System;
using UnityEngine;

public class FakePlayer : MonoBehaviour
{
	public FakePlayer()
	{
	}

	private void Start()
	{
		this.charCon = (CharacterController)base.GetComponent(typeof(CharacterController));
	}

	private void FixedUpdate()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		Vector3 vector = Vector3.zero;
		if (this.m_inputdir == 1 || this.m_inputdir == 2 || this.m_inputdir == 8)
		{
			vector = base.transform.forward * this.m_speed;
		}
		else if (this.m_inputdir == 5 || this.m_inputdir == 4 || this.m_inputdir == 6)
		{
			vector = base.transform.forward * (this.m_speed * -0.5f);
		}
		if (this.m_inputdir > 1 && this.m_inputdir < 5)
		{
			base.transform.Rotate(Vector3.up, this.m_rotateSpeed * fixedDeltaTime);
		}
		else if (this.m_inputdir > 5 && this.m_inputdir < 9)
		{
			base.transform.Rotate(Vector3.up, this.m_rotateSpeed * -fixedDeltaTime);
		}
		if (Vector3.zero != vector)
		{
			this.charCon.Move(vector * fixedDeltaTime);
		}
	}

	public void SetInput(int a_inputdir)
	{
		this.m_inputdir = a_inputdir;
	}

	public int m_id;

	public float m_speed = 5f;

	public float m_rotateSpeed = 270f;

	private int m_inputdir;

	private CharacterController charCon;
}

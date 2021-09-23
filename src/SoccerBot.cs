using System;
using UnityEngine;

public class SoccerBot : MonoBehaviour
{
	public SoccerBot()
	{
	}

	private void Start()
	{
		this.m_neuralNet = new NeuralNet();
		this.m_neuralNet.Init(3, 3);
	}

	private void FixedUpdate()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		float y = base.transform.rotation.eulerAngles.y;
		float y2 = Quaternion.LookRotation((this.m_ball.position - base.transform.position).normalized).eulerAngles.y;
		this.m_inputs[0] = y;
		this.m_inputs[1] = y2;
		this.m_inputs[2] = this.m_ball.position.y;
		this.m_outputs = this.m_neuralNet.DoNetIO(this.m_inputs);
		this.Move(fixedDeltaTime);
	}

	private void Move(float a_dt)
	{
		float d = (this.m_outputs[0] + this.m_outputs[1]) / 2f;
		float d2 = Mathf.Clamp(this.m_outputs[2], -this.m_speed, this.m_speed);
		base.rigidbody.MovePosition(base.transform.position + base.transform.forward * a_dt * d2);
		base.rigidbody.AddTorque(Vector3.up * d * a_dt * this.m_rotSpeed);
	}

	private const int c_netLayers = 3;

	private const int c_ioCount = 3;

	public bool m_inTeamA = true;

	public Transform m_ball;

	public float m_speed = 5f;

	public float m_rotSpeed = 0.5f;

	[HideInInspector]
	public NeuralNet m_neuralNet;

	private float[] m_inputs = new float[3];

	private float[] m_outputs = new float[3];
}

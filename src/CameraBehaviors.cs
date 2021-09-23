using System;
using UnityEngine;

public class CameraBehaviors : MonoBehaviour
{
	public CameraBehaviors()
	{
	}

	private void UpdateInput()
	{
		this.dest_speedX = Input.GetAxis("Horizontal");
		this.dest_speedZ = Input.GetAxis("Vertical");
		this.speedX = Mathf.Lerp(this.speedX, this.dest_speedX, this.breakSpeed);
		this.speedZ = Mathf.Lerp(this.speedZ, this.dest_speedZ, this.breakSpeed);
		Mathf.Clamp(this.speedX, -this.maxSpeed, this.maxSpeed);
		Mathf.Clamp(this.speedZ, -this.maxSpeed, this.maxSpeed);
	}

	private void UpdatePosition()
	{
		Vector3 position = base.transform.position;
		if (this.InvertX)
		{
			position.x -= this.speedX;
			if (position.x > this.BoundLeft)
			{
				position.x = this.BoundLeft;
			}
			if (position.x < this.BoundRight)
			{
				position.x = this.BoundRight;
			}
		}
		else
		{
			position.x += this.speedX;
			if (position.x < this.BoundLeft)
			{
				position.x = this.BoundLeft;
			}
			if (position.x > this.BoundRight)
			{
				position.x = this.BoundRight;
			}
		}
		if (this.InvertZ)
		{
			position.z -= this.speedZ;
			if (position.z > this.BoundTop)
			{
				position.z = this.BoundTop;
			}
			if (position.z < this.BoundBottom)
			{
				position.z = this.BoundBottom;
			}
		}
		else
		{
			position.z += this.speedZ;
			if (position.z < this.BoundTop)
			{
				position.z = this.BoundTop;
			}
			if (position.z > this.BoundBottom)
			{
				position.z = this.BoundBottom;
			}
		}
		base.transform.position = position;
	}

	private void Update()
	{
		this.UpdateInput();
		this.UpdatePosition();
	}

	public float maxSpeed = 1f;

	public float breakSpeed = 0.1f;

	public float BoundTop;

	public float BoundBottom;

	public float BoundLeft;

	public float BoundRight;

	public bool InvertX;

	public bool InvertZ;

	private float dest_speedX;

	private float dest_speedZ;

	private float speedX;

	private float speedZ;
}

using System;
using UnityEngine;

public class Swarm : MonoBehaviour
{
	public Swarm()
	{
	}

	private void Start()
	{
		this.angle = UnityEngine.Random.Range(0f, 360f);
		this.lastPosition = this.GetNewPos();
		float max = this.swarmRadius / this.birdsDistance;
		for (int i = 0; i < this.birdsCount; i++)
		{
			Vector3 vector = new Vector3(UnityEngine.Random.Range(0f, max) * this.birdsDistance, UnityEngine.Random.Range(0f, max) * this.birdsDistance, UnityEngine.Random.Range(0f, max) * this.birdsDistance);
			vector += base.transform.position;
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.bird, vector, base.transform.rotation);
			gameObject.transform.parent = base.transform;
		}
	}

	private void FixedUpdate()
	{
		Vector3 newPos = this.GetNewPos();
		base.transform.position += newPos - this.lastPosition;
		this.lastPosition = newPos;
		this.angle = Mathf.MoveTowardsAngle(this.angle, this.angle + this.speed * Time.deltaTime, this.speed * Time.deltaTime);
	}

	private Vector3 GetNewPos()
	{
		Vector3 result;
		result.x = 0f;
		result.y = Mathf.Sin(this.angle * 0.017453292f) * this.amplitude;
		result.z = 0f;
		return result;
	}

	public GameObject bird;

	public int birdsCount;

	public float swarmRadius;

	public float birdsDistance;

	public float amplitude;

	public float speed;

	private float angle;

	private Vector3 lastPosition;
}

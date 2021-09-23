using System;
using UnityEngine;

public class Bird : MonoBehaviour
{
	public Bird()
	{
	}

	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
		this.angleX = (float)UnityEngine.Random.Range(0, 360);
		this.angleY = (float)UnityEngine.Random.Range(0, 360);
		this.angleZ = (float)UnityEngine.Random.Range(0, 360);
		this.lastPosition = this.GetNewPos();
	}

	private void OnAnimatorMove()
	{
		if (this.anim.GetCurrentAnimatorStateInfo(0).IsTag("NewAnim"))
		{
			if (this.canChangeAnim)
			{
				this.anim.SetInteger("AnimNum", UnityEngine.Random.Range(0, this.animCount + 1));
				this.canChangeAnim = false;
				Debug.Log("Bird anim: " + this.anim.GetInteger("AnimNum"));
			}
		}
		else
		{
			this.canChangeAnim = true;
		}
		Vector3 newPos = this.GetNewPos();
		base.transform.position += newPos - this.lastPosition;
		this.lastPosition = newPos;
		this.angleX = Mathf.MoveTowardsAngle(this.angleX, this.angleX + this.speedX * Time.deltaTime, this.speedX * Time.deltaTime);
		this.angleY = Mathf.MoveTowardsAngle(this.angleY, this.angleY + this.speedY * Time.deltaTime, this.speedY * Time.deltaTime);
		this.angleZ = Mathf.MoveTowardsAngle(this.angleZ, this.angleZ + this.speedZ * Time.deltaTime, this.speedZ * Time.deltaTime);
	}

	private Vector3 GetNewPos()
	{
		Vector3 result;
		result.x = Mathf.Sin(this.angleX * 0.017453292f) * this.amplitudeX;
		result.y = Mathf.Sin(this.angleY * 0.017453292f) * this.amplitudeY;
		result.z = Mathf.Sin(this.angleZ * 0.017453292f) * this.amplitudeZ;
		return result;
	}

	public int animCount = 2;

	public float speedX;

	public float speedY;

	public float speedZ;

	public float amplitudeX;

	public float amplitudeY;

	public float amplitudeZ;

	private Animator anim;

	private bool canChangeAnim;

	private float angleX;

	private float angleY;

	private float angleZ;

	private Vector3 lastPosition;
}

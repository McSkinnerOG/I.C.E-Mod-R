using System;
using UnityEngine;

public class EmitAndDie : MonoBehaviour
{
	public EmitAndDie()
	{
	}

	private void Start()
	{
		if (null != base.transform.particleEmitter)
		{
			base.transform.particleEmitter.Emit(this.emitCount);
		}
		this.dieTime = Time.time + this.timeToLife;
	}

	private void Update()
	{
		if (Time.time > this.dieTime)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public int emitCount = 1;

	public float timeToLife = 1f;

	private float dieTime;
}

using System;
using UnityEngine;

public class CFX_Demo_RandomDir : MonoBehaviour
{
	public CFX_Demo_RandomDir()
	{
	}

	private void Awake()
	{
		base.transform.eulerAngles = new Vector3(UnityEngine.Random.Range(this.min.x, this.max.x), UnityEngine.Random.Range(this.min.y, this.max.y), UnityEngine.Random.Range(this.min.z, this.max.z));
	}

	public Vector3 min = new Vector3(0f, 0f, 0f);

	public Vector3 max = new Vector3(0f, 360f, 0f);
}

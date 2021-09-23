using System;
using UnityEngine;

public class AntFood : MonoBehaviour
{
	public AntFood()
	{
	}

	public void Consume()
	{
		base.transform.position = new Vector3(UnityEngine.Random.Range(-10f, 10f), 0f, UnityEngine.Random.Range(-10f, 10f));
	}
}

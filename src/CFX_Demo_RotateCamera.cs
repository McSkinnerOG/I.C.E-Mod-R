using System;
using UnityEngine;

public class CFX_Demo_RotateCamera : MonoBehaviour
{
	public CFX_Demo_RotateCamera()
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static CFX_Demo_RotateCamera()
	{
	}

	private void Update()
	{
		if (CFX_Demo_RotateCamera.rotating)
		{
			base.transform.RotateAround(this.rotationCenter.position, Vector3.up, this.speed * Time.deltaTime);
		}
	}

	public static bool rotating = true;

	public float speed = 30f;

	public Transform rotationCenter;
}

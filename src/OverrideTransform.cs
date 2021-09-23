using System;
using UnityEngine;

public class OverrideTransform : MonoBehaviour
{
	public OverrideTransform()
	{
	}

	private void Update()
	{
		base.transform.rotation = Quaternion.identity;
	}
}

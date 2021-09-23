using System;
using UnityEngine;

public class DestroyIf : MonoBehaviour
{
	public DestroyIf()
	{
	}

	private void Awake()
	{
		if ((this.ifServer && Global.isServer) || (this.ifClient && !Global.isServer))
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
	}

	public bool ifServer;

	public bool ifClient;
}

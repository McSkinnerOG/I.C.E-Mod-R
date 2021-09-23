using System;
using UnityEngine;

public class RunAway : MonoBehaviour
{
	public RunAway()
	{
	}

	private void Start()
	{
		NavMeshAgent component = base.GetComponent<NavMeshAgent>();
		if (null != component)
		{
			component.SetDestination(Vector3.zero);
		}
	}
}

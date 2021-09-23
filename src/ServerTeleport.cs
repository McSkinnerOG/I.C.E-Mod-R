using System;
using UnityEngine;

public class ServerTeleport : MonoBehaviour
{
	public ServerTeleport()
	{
	}

	private void OnTriggerEnter(Collider a_collider)
	{
		if (null != this.m_target && null != a_collider && null != a_collider.transform && this.m_teleportLayer == a_collider.gameObject.layer)
		{
			Vector3 position = this.m_target.position;
			position.y = 0f;
			a_collider.transform.position = position;
		}
	}

	public Transform m_target;

	public int m_teleportLayer;
}

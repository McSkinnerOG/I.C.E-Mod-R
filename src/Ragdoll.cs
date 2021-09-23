using System;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
	public Ragdoll()
	{
	}

	private void Start()
	{
		this.m_freezeTime = Time.time + this.m_timeTillFreeze;
		this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
		if (null != this.m_client)
		{
			Vector3 nearbyExplosion = this.m_client.GetNearbyExplosion(base.transform.position);
			if (Vector3.zero != nearbyExplosion)
			{
				Rigidbody[] componentsInChildren = base.GetComponentsInChildren<Rigidbody>();
				foreach (Rigidbody rigidbody in componentsInChildren)
				{
					rigidbody.AddExplosionForce(this.m_explosionForce, nearbyExplosion - Vector3.up, 10f);
				}
			}
		}
	}

	private void Update()
	{
		if (Time.time > this.m_freezeTime)
		{
			Rigidbody[] componentsInChildren = base.GetComponentsInChildren<Rigidbody>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Transform transform = componentsInChildren[i].transform;
				CharacterJoint component = transform.GetComponent<CharacterJoint>();
				if (component)
				{
					UnityEngine.Object.Destroy(component);
				}
				if (transform.collider)
				{
					UnityEngine.Object.Destroy(transform.collider);
				}
				UnityEngine.Object.Destroy(componentsInChildren[i]);
			}
			UnityEngine.Object.Destroy(this);
		}
	}

	public float m_timeTillFreeze = 3f;

	public float m_explosionForce = 2000f;

	private float m_freezeTime;

	private LidClient m_client;
}

using System;
using UnityEngine;

public class ExplodePhysics : MonoBehaviour
{
	public ExplodePhysics()
	{
	}

	private void Start()
	{
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			Transform transform = renderer.transform;
			transform.parent = null;
			transform.gameObject.AddComponent<TimedDestroy>();
			transform.gameObject.AddComponent<BoxCollider>();
			transform.gameObject.AddComponent<Rigidbody>();
		}
		Vector3 position = base.transform.position + Vector3.up * 0.5f + (Camera.main.transform.position - base.transform.position) * 0.25f;
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_explosion, position, Quaternion.identity);
		gameObject.transform.parent = Camera.main.transform;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public GameObject m_explosion;
}

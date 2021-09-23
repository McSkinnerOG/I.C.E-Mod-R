using System;
using UnityEngine;

public class DrawGizmo : MonoBehaviour
{
	public DrawGizmo()
	{
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = this.m_color;
		if (this.m_isSphere)
		{
			Gizmos.DrawSphere(base.transform.position + Vector3.up * 0.5f, this.m_radius);
		}
		else
		{
			Gizmos.DrawCube(base.transform.position, new Vector3(this.m_radius * 2f, 1f, this.m_radius * 2f));
		}
	}

	public float m_radius = 0.5f;

	public Color m_color = Color.blue;

	public bool m_isSphere = true;
}

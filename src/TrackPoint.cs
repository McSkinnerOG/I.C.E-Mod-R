using System;
using System.Collections;
using UnityEngine;

public class TrackPoint : MonoBehaviour
{
	public TrackPoint()
	{
	}

	public void UpdateTrack()
	{
		this.m_subPoints.Clear();
		Vector3 vector = Vector3.zero;
		if (this.m_prevPoint && this.m_prevPoint.m_subPoints.Count > 0)
		{
			Vector3 b = (Vector3)this.m_prevPoint.m_subPoints[this.m_prevPoint.m_subPoints.Count - 1];
			vector = (base.transform.position - b).normalized;
		}
		if (this.m_nextPoint)
		{
			Vector3 dirToNextPoint = this.m_nextPoint.transform.position - base.transform.position;
			if (Vector3.zero == vector)
			{
				vector = dirToNextPoint.normalized;
			}
			this.CreateSubPoints(vector, dirToNextPoint);
			if (this.m_nextPoint.gameObject.name != "1")
			{
				this.m_nextPoint.UpdateTrack();
			}
		}
	}

	private void OnDrawGizmos()
	{
		if ((this.m_lastPos - base.transform.position).sqrMagnitude > 0f)
		{
			TrackPoint trackPoint = this;
			while (trackPoint.gameObject.name != "1")
			{
				trackPoint = trackPoint.m_prevPoint;
			}
			trackPoint.UpdateTrack();
			this.m_lastPos = base.transform.position;
		}
		if (this.m_nextPoint)
		{
			Vector3 from = base.transform.position;
			foreach (object obj in this.m_subPoints)
			{
				Vector3 vector = (Vector3)obj;
				Gizmos.DrawLine(from, vector);
				from = vector;
			}
			Gizmos.DrawLine(from, this.m_nextPoint.transform.position);
		}
	}

	private void CreateSubPoints(Vector3 startLookDir, Vector3 dirToNextPoint)
	{
		int num = (int)((base.transform.position - this.m_nextPoint.transform.position).magnitude / this.m_chunkLength);
		Vector3 vector = base.transform.position;
		float y = Quaternion.LookRotation(startLookDir).eulerAngles.y;
		for (int i = 1; i < num + 1; i++)
		{
			float num2 = Mathf.Clamp((float)i / (float)num, 0f, 1f);
			float num3 = Quaternion.LookRotation(this.m_nextPoint.transform.position - vector).eulerAngles.y - y;
			if (num3 > 180f)
			{
				num3 -= 360f;
			}
			else if (num3 < -180f)
			{
				num3 += 360f;
			}
			vector += Quaternion.Euler(0f, num3 * num2, 0f) * startLookDir * this.m_chunkLength;
			this.m_subPoints.Add(vector);
		}
	}

	public int m_branch;

	public float m_chunkLength = 10f;

	public TrackPoint m_nextPoint;

	public TrackPoint m_prevPoint;

	private ArrayList m_subPoints = new ArrayList();

	private Vector3 m_lastPos = new Vector3(0f, -1123f, 0f);
}

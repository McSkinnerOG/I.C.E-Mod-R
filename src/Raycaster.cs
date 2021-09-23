using System;
using UnityEngine;

public static class Raycaster
{
	public static float Attack(Transform a_aggressor, ItemDef a_weapon, Vector3 a_targetPos, ref Transform a_target)
	{
		if (a_weapon.damage < 1f)
		{
			Debug.Log(a_aggressor.name + " is firing an invalid weapon at " + Time.time);
			return 0f;
		}
		Transform transform = null;
		Vector3 a = (!(null != a_target)) ? a_targetPos : a_target.position;
		a.y = 0f;
		bool flag = 0 == a_weapon.ammoItemType;
		Vector3 b = Vector3.up * ((!flag) ? 1.5f : 0.8f);
		Vector3 vector = a_aggressor.position;
		vector.y = 0f;
		vector += b;
		Vector3 a2 = a + b;
		Vector3 vector2 = a2 - vector;
		float num = a_weapon.damage;
		if (vector2.sqrMagnitude > 3.5f || null == a_target)
		{
			int layerMask = -5;
			RaycastHit raycastHit;
			if (flag)
			{
				Collider[] array = Physics.OverlapSphere(a_targetPos + b, 0.4f, layerMask);
				if (array != null && 0 < array.Length)
				{
					transform = array[0].transform;
				}
			}
			else if (Physics.Raycast(vector, vector2.normalized, out raycastHit, a_weapon.range, layerMask))
			{
				transform = raycastHit.transform;
				float num2 = vector2.sqrMagnitude / (a_weapon.range * a_weapon.range) * 0.5f + 0.5f;
				num *= Mathf.Sin(num2 * 3.1415927f);
				if (num < 1f)
				{
					num = 1f;
				}
			}
		}
		else
		{
			transform = a_target;
		}
		if (null != transform)
		{
			Util.Attack(transform, a_aggressor, num);
		}
		else
		{
			num = 0f;
		}
		a_target = transform;
		return num;
	}

	public static bool BuildingSphereCast(Vector3 a_pos)
	{
		Vector3 a_pos2 = a_pos;
		a_pos2.y = 2f;
		return Raycaster.CheckSphere(a_pos2, 0.8f);
	}

	public static bool CheckSphere(Vector3 a_pos, float a_radius)
	{
		Collider[] array = Physics.OverlapSphere(a_pos, a_radius);
		bool flag = array != null && 0 < array.Length;
		if (flag && Application.isEditor)
		{
			string str = "CheckSphere colliders: ";
			for (int i = 0; i < array.Length; i++)
			{
				str = str + array[i].name + ", ";
			}
		}
		return flag;
	}
}

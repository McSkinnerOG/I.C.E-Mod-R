using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class Util : MonoBehaviour
{
	public Util()
	{
	}

	public static string Md5(string strToEncrypt)
	{
		UTF8Encoding utf8Encoding = new UTF8Encoding();
		byte[] bytes = utf8Encoding.GetBytes(strToEncrypt);
		MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] array = md5CryptoServiceProvider.ComputeHash(bytes);
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			text += Convert.ToString(array[i], 16).PadLeft(2, '0');
		}
		return text.PadLeft(32, '0');
	}

	public static void SetLayerRecursively(Transform a_transform, int a_layer)
	{
		a_transform.gameObject.layer = a_layer;
		foreach (object obj in a_transform)
		{
			Transform transform = (Transform)obj;
			transform.gameObject.layer = a_layer;
			if (0 < transform.childCount)
			{
				Util.SetLayerRecursively(transform, a_layer);
			}
		}
	}

	public static float GetLightIntensity(float a_progress, out float a_pip)
	{
		a_pip = 1.33f;
		return Mathf.Clamp01(Mathf.Sin(a_progress * 3.1415927f * a_pip));
	}

	public static void Attack(Transform a_victim, Transform a_aggressor, float a_damage)
	{
		if (null != a_victim)
		{
			a_victim.SendMessage("ChangeHealthBy", -a_damage, SendMessageOptions.DontRequireReceiver);
			a_victim.SendMessage("SetAggressor", a_aggressor, SendMessageOptions.DontRequireReceiver);
		}
	}

	public static string GetItemDefHash(int a_defId, ulong a_steamId)
	{
		return Util.Md5(a_defId.ToString() + a_steamId.ToString() + "Version_0_4_8_B");
	}

	public static float GetTerrainHeight(Vector3 a_pos)
	{
		foreach (Terrain terrain in Terrain.activeTerrains)
		{
			if (!(a_pos.x < 0f ^ terrain.transform.position.x < 0f) && !(a_pos.z < 0f ^ terrain.transform.position.z < 0f))
			{
				return terrain.SampleHeight(a_pos);
			}
		}
		return 0f;
	}
}

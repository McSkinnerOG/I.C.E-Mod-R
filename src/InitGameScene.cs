using System;
using UnityEngine;

public class InitGameScene : MonoBehaviour
{
	public InitGameScene()
	{
	}

	private void Start()
	{
		Renderer[] array = UnityEngine.Object.FindObjectsOfType<Renderer>();
		for (int i = 0; i < array.Length; i++)
		{
			if (this.m_dummyTex == array[i].material.mainTexture)
			{
				UnityEngine.Object.Destroy(array[i]);
			}
		}
		if (!Global.isServer)
		{
			BodyHeadAnim[] array2 = UnityEngine.Object.FindObjectsOfType<BodyHeadAnim>();
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j].gameObject.layer == 9)
				{
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_exlMarkPrefab, array2[j].transform.position + Vector3.up * 4.4f, Quaternion.Euler(270f, 0f, 0f));
					gameObject.transform.parent = array2[j].transform;
				}
			}
		}
	}

	public GameObject m_exlMarkPrefab;

	public Texture m_dummyTex;
}

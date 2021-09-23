using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
	public WorldGenerator()
	{
	}

	private void Start()
	{
		this.Generate();
	}

	private void Generate()
	{
		List<GameObject> list = new List<GameObject>(this.m_edgeParts);
		List<GameObject> list2 = new List<GameObject>(this.m_cornerParts);
		List<GameObject> list3 = new List<GameObject>(this.m_middleParts);
		for (int i = 0; i < 9; i++)
		{
			Vector3 position = new Vector3((float)(i % 3 - 1) * 1000f, 0f, (float)(1 - i / 3) * 1000f);
			GameObject gameObject;
			if (i == 4)
			{
				int index = UnityEngine.Random.Range(0, list3.Count);
				gameObject = list3[index];
				list3.RemoveAt(index);
			}
			else if (i % 2 == 0)
			{
				int index2 = UnityEngine.Random.Range(0, list2.Count);
				gameObject = list2[index2];
				list2.RemoveAt(index2);
			}
			else
			{
				int index3 = UnityEngine.Random.Range(0, list.Count);
				gameObject = list[index3];
				list.RemoveAt(index3);
			}
			if (null != gameObject)
			{
				gameObject.transform.position = position;
				gameObject.SetActive(true);
			}
		}
	}

	public GameObject[] m_edgeParts;

	public GameObject[] m_cornerParts;

	public GameObject[] m_middleParts;
}

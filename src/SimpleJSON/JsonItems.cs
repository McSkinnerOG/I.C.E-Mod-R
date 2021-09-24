using System;
using SimpleJSON;
using UnityEngine;

public class JsonItems
{
	public JsonItems()
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static JsonItems()
	{
	}

	private static void Init()
	{
		TextAsset textAsset = (TextAsset)Resources.Load("inventory_steam");
		if (null != textAsset)
		{
			JsonItems.m_items = JSONNode.Parse(textAsset.text);
		}
	}

	public static JSONNode GetItem(int a_id)
	{
		if (null == JsonItems.m_items)
		{
			JsonItems.Init();
		}
		if (null != JsonItems.m_items)
		{
			for (int i = 0; i < JsonItems.m_items["items"].Count; i++)
			{
				if (JsonItems.m_items["items"][i]["itemdefid"].AsInt == a_id)
				{
					return JsonItems.m_items["items"][i];
				}
			}
		}
		return null;
	}

	private static JSONNode m_items;
}

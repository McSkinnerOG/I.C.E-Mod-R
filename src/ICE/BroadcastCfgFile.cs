using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class BroadcastCfgFile : ConfigFile
{
	private static Hashtable m_broadcastCfgVars = new Hashtable();

	private static bool m_loadedBroadcastCfg = false;

	private static void LoadConfig()
	{
		if (!m_loadedBroadcastCfg)
		{
			m_loadedBroadcastCfg = true;
			string path = "Plugin_data\\broadcasts.ice";
			try
			{
				if (File.Exists(path))
				{
					StreamReader streamReader = File.OpenText(path);
					string text = streamReader.ReadToEnd();
					text = text.Replace("\r", string.Empty).Replace("　", string.Empty);
					string[] array = text.Split('\n');
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] != null && array[i].Contains("="))
						{
							string[] array2 = array[i].Split('=');
							if (array2 != null && array2.Length == 2 && 0 < array2[0].Length && 0 < array2[1].Length)
							{
								m_broadcastCfgVars.Add(array2[0], array2[1]);
							}
						}
					}
				}
			}
			catch (Exception arg)
			{
				Debug.Log("CfgFile.cs: caught exception " + arg);
			}
		}
	}

	public static string GetVar(string a_key, string a_emptyReturn = "")
	{
		LoadConfig();
		return (!m_broadcastCfgVars.Contains(a_key)) ? a_emptyReturn : ((string)m_broadcastCfgVars[a_key]);
	}
}

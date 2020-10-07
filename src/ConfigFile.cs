using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class ConfigFile : MonoBehaviour
{
	public static Hashtable m_cfgVars = new Hashtable();
	public static bool m_loadedServerCfg = false;
	public static void LoadConfig()
	{
		if (!m_loadedServerCfg)
		{
			m_loadedServerCfg = true;
			string path = "Plugin_Data\\config.ice";
			try
			{
				if (File.Exists(path))
				{
					StreamReader streamReader = File.OpenText(path);
					string text = streamReader.ReadToEnd();
					text = text.Replace("\r", string.Empty).Replace(" ", string.Empty);
					string[] array = text.Split('\n');
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] != null && array[i].Contains("="))
						{
							string[] array2 = array[i].Split('=');
							if (array2 != null && array2.Length == 2 && 0 < array2[0].Length && 0 < array2[1].Length)
							{
								m_cfgVars.Add(array2[0], array2[1]);
							}
						}
					}
				}
			}
			catch (Exception arg)
			{
				Debug.Log("ConfigFile.cs: caught exception " + arg);
			}
		}
	}

	public static string GetVar(string a_key, string a_emptyReturn = "")
	{
		LoadConfig();
		return (!m_cfgVars.Contains(a_key)) ? a_emptyReturn : ((string)m_cfgVars[a_key]);
	}
}

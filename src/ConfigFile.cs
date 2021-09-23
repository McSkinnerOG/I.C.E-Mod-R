using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class ConfigFile : MonoBehaviour
{
	public ConfigFile()
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static ConfigFile()
	{
	}

	private static void LoadConfig()
	{
		if (!ConfigFile.m_loadedCfg)
		{
			ConfigFile.m_loadedCfg = true;
			string path = "config.txt";
			try
			{
				if (File.Exists(path))
				{
					StreamReader streamReader = File.OpenText(path);
					string text = streamReader.ReadToEnd();
					text = text.Replace("\r", string.Empty).Replace(" ", string.Empty);
					string[] array = text.Split(new char[]
					{
						'\n'
					});
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] != null && array[i].Contains("="))
						{
							string[] array2 = array[i].Split(new char[]
							{
								'='
							});
							if (array2 != null && array2.Length == 2 && 0 < array2[0].Length && 0 < array2[1].Length)
							{
								ConfigFile.m_cfgVars.Add(array2[0], array2[1]);
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
		ConfigFile.LoadConfig();
		return (!ConfigFile.m_cfgVars.Contains(a_key)) ? a_emptyReturn : ((string)ConfigFile.m_cfgVars[a_key]);
	}

	private static Hashtable m_cfgVars = new Hashtable();

	private static bool m_loadedCfg = false;
}

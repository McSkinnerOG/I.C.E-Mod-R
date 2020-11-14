using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;

namespace Windows
{
	/// <summary>
	/// Creates a console window that actually works in Unity
	/// You should add a script that redirects output using Console.Write to write to it.
	/// </summary>
	public class ConsoleSystem
	{
		public static void Run(string command)
		{
			string text = command.ToLower();

			if (text.StartsWith("hello"))
			{
				Debug.Log("reeeeeee");
			}
		}
	}
}
using System;
using UnityEngine;

public class TestGamepad : MonoBehaviour
{
	public TestGamepad()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
		for (KeyCode keyCode = KeyCode.JoystickButton0; keyCode < KeyCode.JoystickButton19; keyCode++)
		{
			if (Input.GetKeyDown(keyCode))
			{
				Debug.Log("joykey0 pressed: " + (keyCode - KeyCode.JoystickButton0));
			}
		}
		for (KeyCode keyCode2 = KeyCode.Joystick1Button0; keyCode2 < KeyCode.Joystick1Button19; keyCode2++)
		{
			if (Input.GetKeyDown(keyCode2))
			{
				Debug.Log("joykey1 pressed: " + (keyCode2 - KeyCode.Joystick1Button0));
			}
		}
		for (KeyCode keyCode3 = KeyCode.Joystick2Button0; keyCode3 < KeyCode.Joystick2Button19; keyCode3++)
		{
			if (Input.GetKeyDown(keyCode3))
			{
				Debug.Log("joykey2 pressed: " + (keyCode3 - KeyCode.Joystick2Button0));
			}
		}
		for (KeyCode keyCode4 = KeyCode.Joystick3Button0; keyCode4 < KeyCode.Joystick3Button19; keyCode4++)
		{
			if (Input.GetKeyDown(keyCode4))
			{
				Debug.Log("joykey3 pressed: " + (keyCode4 - KeyCode.Joystick3Button0));
			}
		}
	}
}

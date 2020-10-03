using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ICE
{
	public class CCE_TP : MonoBehaviour
	{
		internal static void HandleChatCommand(string text, ServerPlayer player, NetIncomingMessage msg)
		{
			var server = (LidServer)UnityEngine.Object.FindObjectOfType(typeof(LidServer));
			var p_pos = player.GetPosition();
			string[] commands = text.Split(' ');

			switch (commands[0])
			{ 
				case "/tp-p":
					var re_name = server.GetPlayerByName(commands[1].ToString()).m_name.ToString();
					var re_pos = server.GetPlayerByName(commands[1].ToString()).GetPosition();
					if (player.m_isAdmin == true && re_name == commands[1])
					{
						player.SetPosition(re_pos);
						server.SendMessageToPlayerLocal("Teleported to player: <color='#ffa500ff'>" + re_name.ToString() + "</color>.", player, msg);
						Debug.Log(player.m_name + " Teleported to: " + commands[1].ToString() + "!");
					}
					else if (re_name == null)
						server.SendMessageToPlayerLocal("Please Enter the name of the player/nto teleport to after '/tp-p'/n EG: '/tp=p UserName'", player, msg);
					break;
				case "/tp":
					if ("HomeTown" == commands[1] && player.m_isAdmin == true)
					{
						var loc_ht = new Vector3(-909, 0, 612);
						player.SetPosition(loc_ht);
					}
					else if ("Terminus" == commands[1] && player.m_isAdmin == true)
					{
						var loc = new Vector3(-925, 0, 869);
						player.SetPosition(loc);
					}
					else if ("Garbage" == commands[1] && player.m_isAdmin == true)
					{
						var loc = new Vector3(-646, 0, 1054);
						player.SetPosition(loc);
					}
					else if ("Castle" == commands[1] && player.m_isAdmin == true)
					{
						var loc = new Vector3(-267, 0, 178);
						player.SetPosition(loc);
					}
					else if ("Area42" == commands[1] && player.m_isAdmin == true)
					{
						var loc = new Vector3(-1030, 0, -274);
						player.SetPosition(loc);
					}
					else if ("Fort-B" == commands[1] && player.m_isAdmin == true)
					{
						var loc = new Vector3(-1043, 0, 14);
						player.SetPosition(loc);
					}
					else if ("GasTown" == commands[1] && player.m_isAdmin == true)
					{
						var loc = new Vector3(-1054, 0, 310);
						player.SetPosition(loc);
					}
					else if ("West-Port" == commands[1] && player.m_isAdmin == true)
					{
						var loc = new Vector3(1113, 0, 109);
						player.SetPosition(loc);
					}
					else if ("Madison" == commands[1] && player.m_isAdmin == true)
					{
						var loc = new Vector3(-643, 0, -1033);
						player.SetPosition(loc);
					}
					else if ("Venmore" == commands[1] && player.m_isAdmin == true)
					{
						var loc = new Vector3(-407, 0, 647);
						player.SetPosition(loc);
					}
					else if ("Valley" == commands[1] && player.m_isAdmin == true)
					{
						var loc = new Vector3(-1004, 0, -1044);
						player.SetPosition(loc);
					}
					else if ("Alexandria" == commands[1] && player.m_isAdmin == true)
					{
						var loc = new Vector3(341, 0, 161);
						player.SetPosition(loc);
					}
					else if ("Riverside" == commands[1] && player.m_isAdmin == true)
					{
						var loc = new Vector3(430, 0, 735);
						player.SetPosition(loc);
					}
					else if ("Tallahassee" == commands[1] && player.m_isAdmin == true)
					{
						var loc = new Vector3(94, 0, 1014);
						player.SetPosition(loc);
					}
					else if ("Airport" == commands[1] && player.m_isAdmin == true)
					{
						var loc = new Vector3(-355, 0, 1053);
						player.SetPosition(loc);
					}
					else if ("PowePlant" == commands[1] && player.m_isAdmin == true)
					{
						var loc = new Vector3(1192, 0, 1184);
						player.SetPosition(loc);
					}
					else if ("GasRanch" == commands[1] && player.m_isAdmin == true)
					{
						var loc = new Vector3(681, 0, 1158);
						player.SetPosition(loc);
					}
					else if ("Riverside-SZ" == commands[1] && player.m_isAdmin == true)
					{
						var loc = new Vector3(628, 0, 631);
						player.SetPosition(loc);

					}
					else if ("/tp" == commands[0] && commands.Length > 2 && player.m_isAdmin == true)
					{
						int num9 = 0;
						int num10 = 0;
						string final_dest = "X:" + num9.ToString() + " Z:" + num10.ToString();
						try
						{
							num9 = int.Parse(commands[1]);
							num10 = int.Parse(commands[2]);
						}
						catch (Exception)
						{
							if (num9 != 1 - 12000 && num10 != 1 - 12000)
							{
								server.SendMessageToPlayerLocal("Please use correctly EG: /tp 123 123", player, msg);
							}
						}
						if (num9 != 1 - 12000 && num10 != 1 - 12000)
						{
							server.SendMessageToPlayerLocal("Please use correctly EG: /tp 123 123", player, msg);
						}
						if (num9 != 0 && num10 != 0)
						{
							player.SetPosition(new Vector3(num9, 0f, num10));
							num9.ToString();
							num10.ToString();
							server.SendMessageToPlayerLocal(LNG.Get("TO_POS").Replace("[dest]", num9.ToString() + " " + num10.ToString()), player, msg);
						}
					}
					break;
				case "/help-skin":
					server.SendMessageToPlayerLocal("Usage for /buff:/n/buff status 'Freezing, Bleeding, Clear' Note: /buff can only applied to YOUR char rn ", player, msg);
					break;

				default:
					break;
			}
			switch (commands[1])
			{
				case "prefill":
					server.SendMessageToPlayerLocal("prefill2", player, msg);
					break;
				default:
					break;
			}
		}
	}
}

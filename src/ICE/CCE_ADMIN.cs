using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ICE
{
    public class CCE_ADMIN
    {
        internal static void HandleChatCommand(string text, ServerPlayer player, NetIncomingMessage msg)
        {
            var server = (LidServer)UnityEngine.Object.FindObjectOfType(typeof(LidServer));
            var remchar = (RemoteCharacter)UnityEngine.Object.FindObjectOfType(typeof(RemoteCharacter));
            var p_pos = player.GetPosition();

            string[] commands = text.Split(' ');
            switch (commands[0])
            {

                case "/logout":
                    if (player.m_isAdmin == true)
                    {
                        player.m_isAdmin = false;
                        server.SendMessageToPlayerLocal("<b><color='#fd0505ff'>Admin commands revoked.</color></b>", player, msg);
                        Debug.Log(player.m_name + " (Steam ID: " + player.m_accountId + ") just logged out of admin!");
                    }
                    break;

                case "/f-logout":
                    var p2_name = server.GetPlayerByName(commands[1]);
                    if (player.m_isAdmin == true)
                    {
                        p2_name.m_isAdmin = false;
                        server.SendMessageToPlayerLocal("<b><color='#fd0505ff'>NUKED user: </color></b>" + p2_name.m_name.ToString(), player, msg);
                        Debug.Log("ADMIN: " + player.m_name + " (Steam ID: " + p2_name.m_accountId + ")" + " Forced " + p2_name.m_name + " (Steam ID: " + p2_name.m_accountId + ") out of admin!");
                    }
                    break;

                case "/slay":
                    var p2_name1 = server.GetPlayerByName(commands[1]);
                    if (player.m_isAdmin == true)
                    {
                        p2_name1.ChangeHealthBy(-100f);
                        server.SendMessageToPlayerLocal("Slayed user" + p2_name1.ToString(), player, msg);
                    }
                    else
                    {
                        server.SendMessageToPlayerLocal("Please login to admin for this command.", player, msg);
                    }
                    break;

                case "/heal-p":
                    var p2_name3 = server.GetPlayerByName(commands[1]);
                    if (player.m_isAdmin == true)
                    {
                        p2_name3.ChangeHealthBy(+100f);
                        server.SendMessageToPlayerLocal("Slayed user" + p2_name3.ToString(), player, msg);
                    }
                    else
                    {
                        server.SendMessageToPlayerLocal("Please login to admin for this command.", player, msg);
                    }
                    break;

                case "/check-gold":
                    var p_gold = server.GetPlayerByName(commands[1]).m_gold;
                    if (player.m_isAdmin == true)
                    {
                        server.SendMessageToPlayerLocal(p_gold.ToString(), player, msg);
                    }
                    else 
                    {
                        server.SendMessageToPlayerLocal("Please login to admin for this command.", player, msg);
                    }
                    break;

                case "/pos-p":
                    var p2_pos = server.GetPlayerByName(commands[2]).GetPosition();
                    if (player.m_isAdmin == true)
                    {
                        server.SendMessageToPlayerLocal(p2_pos.ToString(), player, msg);
                    }
                    else
                    {
                        server.SendMessageToPlayerLocal("Please login to admin for this command.", player, msg);
                    }
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

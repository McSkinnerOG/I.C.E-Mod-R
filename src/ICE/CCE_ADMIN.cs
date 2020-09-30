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
        private List<CharData> m_charData = new List<CharData>();
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

                case "/nuke":
                    var p2_name = server.GetPlayerByName(commands[1]);
                    if (player.m_isAdmin == true)
                    {
                        p2_name.m_isAdmin = false;
                        server.SendMessageToPlayerLocal("<b><color='#fd0505ff'>Admin commands revoked from user: .</color></b>" + p2_name.m_name.ToString(), player, msg);
                        Debug.Log(player.m_name + " (Steam ID: " + player.m_accountId + ") just logged out of admin!");
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

                case "/nuke_p":
                    var p2_name4 = server.GetPlayerByName(commands[1]);
                    if (player.m_isAdmin == true && commands[2].ToString() == "true")
                    {
                        p2_name4.Remove();
                        server.SendMessageToPlayerLocal("NUKED Player " + p2_name4.ToString(), player, msg);
                        server.SendNotification("NUKED Player " + p2_name4.ToString());
                    }
                    else
                    {
                        server.SendMessageToPlayerLocal("Please login to admin for this command.", player, msg);
                    }
                    break;

                case "/heal_p":
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

                case "/check_gold":
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

                case "/?":
                    server.SendMessageToPlayerLocal("I.C.E-Mod: <color=purple>Made by</color> <color=green>Va1idUser: Github.com/McSkinnerOG/I.C.E-Mod</color> and <color=red>Donaut: Github.com/Donaut/ImmuneCommandMod</color>.", player, msg);
                    break;
             
                default:
                    break;
            }
            switch (commands[1])
            {
                case "kit-guard3":
                    server.SendMessageToPlayerLocal("<color=purple>Guardian-kit 3 costs 5000gold and recieves: </color> <color=yellow>Guardian-Vest x1, Sneakers x1, </color> <color=white>Giant-Sword x1, Knife x1, Crowbar x1, AK47 x1, </color> <color=brown>Canned-Food x6, Energy-Bar x1, Soda x1, </color> <color=cyan>Water x2,</color> <color=red>Medpack x1.</color>", player, msg);
                    break;
                default:
                    break;
            }
            switch (commands[2])
            {
                case "kit-guard33":
                    server.SendMessageToPlayerLocal("<color=purple>Guardian-kit 3 costs 5000gold and recieves: </color> <color=yellow>Guardian-Vest x1, Sneakers x1, </color> <color=white>Giant-Sword x1, Knife x1, Crowbar x1, AK47 x1, </color> <color=brown>Canned-Food x6, Energy-Bar x1, Soda x1, </color> <color=cyan>Water x2,</color> <color=red>Medpack x1.</color>", player, msg);
                    break;
                default:
                    break;
            }
        }
    }
}

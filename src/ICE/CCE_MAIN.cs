using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ICE
{
    public class CCE_MAIN // THIS FILE WILL BE RE-PURPOSED AS THE MOD/COMMAND HOOK/MANAGER. "2.0?"
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
                case "/id":
                    var p_id = server.GetPlayerByName(commands[1]).m_pid;
                    server.SendMessageToPlayerLocal(p_id.ToString(), player, msg);
                    break;

                case "/oid":
                    var p_oid = server.GetPlayerByName(commands[1]).m_onlineId;
                    server.SendMessageToPlayerLocal(p_oid.ToString(), player, msg);
                    break;

                case "/admin?":
                    var p_admin = server.GetPlayerByName(commands[1]).m_isAdmin;
                    var msg_admin_y = " !!!YES!!! This player is an admin!";
                    var msg_admin_n = "This player is NOT admin!";
                    if (p_admin == true)
                    {
                        server.SendMessageToPlayerLocal(msg_admin_y, player, msg);
                    }
                    else
                    {
                        server.SendMessageToPlayerLocal(msg_admin_n, player, msg);
                    }
                    break;

                case "/pos":
                    server.SendMessageToPlayerLocal(p_pos.ToString(), player, msg);
                    break;

                case "/online":
                    server.SendMessageToPlayerLocal(LNG.Get("CMD_ONLINE_PLAYERS").Replace("{p_online}", server.GetPlayerCount().ToString()), player, msg);
                    break;

                case "/about":
                    server.SendMessageToPlayerLocal("I.C.E is a project to help expand the Immune-Dedicated software capabilities. For more info please use /commands and /help commandname.",
                        player, msg);
                    break;
                case "/shout":
                    server.SendNotification(text.Remove(0,6)); 
                    break;
                case "/commands":
                    server.SendMessageToPlayerLocal("<color=red>COMMANDS</color> <color=green>ARE</color> <color=purple>COLOR</color> <color=purple>CODED</color>! Each command is <color=red>color coded</color> within /help-commandname. Colors represent permission level needed to use them. <color=red>RED COMMANDS ARE ADMIN ONLY!!!</color> <color=yellow>YELLOW COMMANDS REQUIRE GOLD TO EXECUTE!</color> <color=green>GREEN COMMANDS ARE ALL LEVEL ACCESS!</color>", player, msg);
                    break;

                case "/help":
                    server.SendMessageToPlayerLocal("Avalible commands are: <color=green>/kit-doc</color>, <color=purple>/weapon</color>, <color=purple>/food</color>, <color=purple>/medicine</color> and <color=red>/about</color>", player, msg);
                    break;

                case "/help-kit":
                    server.SendMessageToPlayerLocal("Usage for /kit-xxx: To purchase a kit you must have the required amount of gold and or permissions. Example: Enter */kit-doc* without the ** to buy a doctors kit for 500gold.", player, msg);
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
        }
    }
}

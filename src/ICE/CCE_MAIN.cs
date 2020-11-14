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
        private float m_nextServerListUpdate = 5f;
        private float m_nextServerBroadcastTime = 5f;
        private float m_nextServerBroadcastMsg = 1;
        
        private static void Update()
        {
            var server = (LidServer)UnityEngine.Object.FindObjectOfType(typeof(LidServer));
            

        }
        
        internal static void HandleChatCommand(string text, ServerPlayer player, NetIncomingMessage msg)
        {
            var server = (LidServer)UnityEngine.Object.FindObjectOfType(typeof(LidServer));
            var remchar = (RemoteCharacter)UnityEngine.Object.FindObjectOfType(typeof(RemoteCharacter));
            var p_pos = player.GetPosition();
            string[] commands = text.Split(' ');
            switch (commands[0])
            {
                case "/id": // RETURN PLAYERS ID
                    var p_id = server.GetPlayerByName(commands[1]).m_pid;
                    server.SendMessageToPlayerLocal(p_id.ToString(), player, msg);
                    break;

                case "/oid": // RETURN PLAYERS ONLINE-ID
                    var p_oid = server.GetPlayerByName(commands[1]).m_onlineId;
                    server.SendMessageToPlayerLocal(p_oid.ToString(), player, msg);
                    break;

                case "/admin?": // CHECK A PLAYERS ADMIN STATUS
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

                case "/pos": // RETURNS PLAYERS CURRENT POSITION
                    server.SendMessageToPlayerLocal(p_pos.ToString(), player, msg);
                    break;

                case "/online": // RETURNS CURRENT AMOUNT OF PLAYERS IN ONLINE STATE
                    server.SendMessageToPlayerLocal(LNG.Get("CMD_ONLINE_PLAYERS").Replace("{p_online}", server.GetPlayerCount().ToString()), player, msg);
                    break;

                case "/about":
                    server.SendMessageToPlayerLocal("I.C.E is a project to help expand the Immune-Dedicated software capabilities. For more info please use /commands and /help commandname.",
                        player, msg);
                    break;
                case "/shout": // SEND MESSAGE TO ALL PLAYERS VIA BROADCAST
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
                case "prefill":
                    server.SendMessageToPlayerLocal("prefill2", player, msg);
                    break;
                default:
                    break;
            }
        }
    }
}

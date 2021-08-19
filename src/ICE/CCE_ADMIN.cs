using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
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

            string[] commands = text.Split(' ');
            if (commands[0] != null)
            {
                var RP_REF = server.GetPlayerByName(commands[1]);
                var RP_AID = server.GetPlayerByName(commands[1]).m_accountId;
                var RP_NAME = server.GetPlayerByName(commands[1]).m_name;
                var RP_POS = server.GetPlayerByName(commands[1]).GetPosition();
                var RP_GOLD = server.GetPlayerByName(commands[1]).m_gold;
                var RP_ADMIN = server.GetPlayerByName(commands[1]).m_isAdmin;

                var LP_AID = server.GetPlayerByAid(player.m_accountId).m_accountId;
                var LP_NAME = server.GetPlayerByAid(player.m_accountId).m_name;
                var LP_POS = server.GetPlayerByAid(player.m_accountId).GetPosition();
                var LP_GOLD = server.GetPlayerByAid(player.m_accountId).m_gold;
                var LP_ADMIN = server.GetPlayerByAid(player.m_accountId).m_isAdmin;
                switch (commands[0])
                {
                    case "/logout":
                        if (LP_ADMIN == true)
                        {
                            LP_ADMIN = false;
                            server.SendMessageToPlayerLocal("<b><color='#fd0505ff'>Admin commands revoked.</color></b>", player, msg);
                            Debug.Log(LP_NAME + " (Steam ID: " + LP_AID + ") just logged out of admin!");
                        }
                        break;

                    case "/f-logout":
                        
                        if (LP_ADMIN == true)
                        {
                            RP_ADMIN = false;
                            server.SendMessageToPlayerLocal("<b><color='#fd0505ff'>NUKED user: </color></b>" + RP_NAME.ToString(), player, msg);
                            Debug.Log("ADMIN: " + LP_NAME + " (Steam ID: " + RP_AID + ")" + " Forced " + RP_NAME + " (Steam ID: " + RP_AID + ") out of admin!");
                        }
                        break;

                    case "/slay":
                        if (LP_ADMIN == true)
                        {
                            RP_REF.ChangeHealthBy(-100f);
                            server.SendMessageToPlayerLocal("Slayed user" + RP_NAME.ToString(), player, msg);
                        }
                        else
                        {
                            server.SendMessageToPlayerLocal("Please login to admin for this command.", player, msg);
                        }
                        break;

                    case "/heal-p":
                        if (LP_ADMIN == true)
                        {
                            RP_REF.ChangeHealthBy(+100f);
                            server.SendMessageToPlayerLocal("Slayed user" + RP_NAME.ToString(), player, msg);
                        }
                        else
                        {
                            server.SendMessageToPlayerLocal("Please login to admin for this command.", player, msg);
                        }
                        break;

                    case "/check-gold":
                        if (LP_ADMIN == true)
                        {
                            server.SendMessageToPlayerLocal(RP_GOLD.ToString(), player, msg);
                        }
                        else
                        {
                            server.SendMessageToPlayerLocal("Please login to admin for this command.", player, msg);
                        }
                        break;

                    case "/pos-p":
                        if (LP_ADMIN == true)
                        {
                            server.SendMessageToPlayerLocal(RP_POS.ToString(), player, msg);
                        }
                        else
                        {
                            server.SendMessageToPlayerLocal("Please login to admin for this command.", player, msg);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

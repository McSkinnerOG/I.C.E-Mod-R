using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ICE
{
    public class CCE_P_SKIN : MonoBehaviour
    {

       

         internal static void HandleChatCommand(string text, ServerPlayer player, NetIncomingMessage msg)
        {
            var server = (LidServer)UnityEngine.Object.FindObjectOfType(typeof(LidServer));
            var p_pos = player.GetPosition();
            string[] commands = text.Split(' ');

            switch (commands[0])
            {
                case "/skin":
					if (player.m_isAdmin == true)
					{
						var skinResponse = "Changed Skin to <b><color='#ffa500ff'>" + commands[1].ToString() + "</color></b>.";
						var skin = player.m_skinIndex;
						if (commands[1] == "0" || commands[1] == "Player" || commands[1] == "Default")
						{
							player.m_skinIndex = 0;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
							Debug.Log(player.m_name + " Changed Skin to: " + commands[1].ToString() + "!");
						}
						else if (commands[1] == "Chad" || commands[1] == "chad" || commands[1] == "CHAD" || commands[1] == "Yakuza" || commands[1] == "yakuza" || commands[1] == "YAKUZA")
						{
							player.m_skinIndex = 1;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
							Debug.Log(player.m_name + " Changed Skin to: " + commands[1].ToString() + "!");
						}
						else if (commands[1] == "Chuck" || commands[1] == "Chad".ToUpper() || commands[1] == "Chad".ToLower() || commands[1] == "LumberJack" || commands[1] == "LumberJack".ToUpper() || commands[1] == "LumberJack".ToLower())
						{
							player.m_skinIndex = 2;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
							Debug.Log(player.m_name + " Changed Skin to: " + commands[1].ToString() + "!");
						}
						else if (commands[1] == "Rollins" || commands[1] == "Business" || commands[1] == "Rollins".ToUpper() || commands[1] == "Business".ToUpper() || commands[1] == "Rollins".ToLower() || commands[1] == "Business".ToLower())
						{
							player.m_skinIndex = 3;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
							Debug.Log(player.m_name + " Changed Skin to: " + commands[1].ToString() + "!");
						}
						else if (commands[1] == "Vince" || commands[1] == "Latino" || commands[1] == "Vince".ToLower() || commands[1] == "Latino".ToLower() || commands[1] == "Vince".ToUpper() || commands[1] == "Latino".ToUpper())
						{
							player.m_skinIndex = 4;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
						}
						else if (commands[1] == "Jenkins" || commands[1] == "Elder" || commands[1] == "Jenkins".ToLower() || commands[1] == "Elder".ToLower() || commands[1] == "Jenkins".ToUpper() || commands[1] == "Elder".ToUpper())
						{
							player.m_skinIndex = 5;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
						}
						else if (commands[1] == "Andrew" || commands[1] == "Soldier" || commands[1] == "Andrew".ToLower() || commands[1] == "Soldier".ToLower() || commands[1] == "Andrew".ToUpper() || commands[1] == "Soldier".ToUpper())
						{
							player.m_skinIndex = 6;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
						}
						else if (commands[1] == "Boxer" || commands[1] == "Boxer".ToLower() || commands[1] == "Boxer".ToUpper())
						{
							player.m_skinIndex = 7;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
						}
						else if (commands[1] == "Pirate" || commands[1] == "Viking" || commands[1] == "Pirate".ToLower() || commands[1] == "Viking".ToLower() || commands[1] == "Pirate".ToUpper() || commands[1] == "Viking".ToUpper())
						{
							player.m_skinIndex = 8;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
						}
						else if (commands[1] == "Thief" || commands[1] == "Warrior" || commands[1] == "Thief".ToLower() || commands[1] == "Warrior".ToLower() || commands[1] == "Thief".ToUpper() || commands[1] == "Warrior".ToUpper())
						{
							player.m_skinIndex = 9;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
						}
						else if (commands[1] == "Psyco" || commands[1] == "Occult" || commands[1] == "Psyco".ToLower() || commands[1] == "Occult".ToLower() || commands[1] == "Psyco".ToUpper() || commands[1] == "Occult".ToUpper())
						{
							player.m_skinIndex = 10;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
						}
					}
					break;
				case "/skin-p":
					if (player.m_isAdmin == true)
					{
						var skinResponse = "Changed Skin to <b><color='#ffa500ff'>" + commands[1].ToString() + "</color></b>.";
						var skin = player.m_skinIndex;
						var p2_name4 = server.GetPlayerByName(commands[2]);
						if (commands[1] == "0" || commands[1] == "Player" || commands[1] == "Default")
						{
							p2_name4.m_skinIndex = 0;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
							Debug.Log(player.m_name + " Changed Skin to: " + commands[1].ToString() + "!");
						}
						else if (commands[1] == "Chad" || commands[1] == "chad" || commands[1] == "CHAD" || commands[1] == "Yakuza" || commands[1] == "yakuza" || commands[1] == "YAKUZA")
						{
							p2_name4.m_skinIndex = 1;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
							Debug.Log(player.m_name + " Changed Skin to: " + commands[1].ToString() + "!");
						}
						else if (commands[1] == "Chuck" || commands[1] == "Chad".ToUpper() || commands[1] == "Chad".ToLower() || commands[1] == "LumberJack" || commands[1] == "LumberJack".ToUpper() || commands[1] == "LumberJack".ToLower())
						{
							p2_name4.m_skinIndex = 2;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
							Debug.Log(player.m_name + " Changed Skin to: " + commands[1].ToString() + "!");
						}
						else if (commands[1] == "Rollins" || commands[1] == "Business" || commands[1] == "Rollins".ToUpper() || commands[1] == "Business".ToUpper() || commands[1] == "Rollins".ToLower() || commands[1] == "Business".ToLower())
						{
							p2_name4.m_skinIndex = 3;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
							Debug.Log(player.m_name + " Changed Skin to: " + commands[1].ToString() + "!");
						}
						else if (commands[1] == "Vince" || commands[1] == "Latino" || commands[1] == "Vince".ToLower() || commands[1] == "Latino".ToLower() || commands[1] == "Vince".ToUpper() || commands[1] == "Latino".ToUpper())
						{
							p2_name4.m_skinIndex = 4;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
						}
						else if (commands[1] == "Jenkins" || commands[1] == "Elder" || commands[1] == "Jenkins".ToLower() || commands[1] == "Elder".ToLower() || commands[1] == "Jenkins".ToUpper() || commands[1] == "Elder".ToUpper())
						{
							p2_name4.m_skinIndex = 5;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
						}
						else if (commands[1] == "Andrew" || commands[1] == "Soldier" || commands[1] == "Andrew".ToLower() || commands[1] == "Soldier".ToLower() || commands[1] == "Andrew".ToUpper() || commands[1] == "Soldier".ToUpper())
						{
							p2_name4.m_skinIndex = 6;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
						}
						else if (commands[1] == "Boxer" || commands[1] == "Boxer".ToLower() || commands[1] == "Boxer".ToUpper())
						{
							p2_name4.m_skinIndex = 7;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
						}
						else if (commands[1] == "Pirate" || commands[1] == "Viking" || commands[1] == "Pirate".ToLower() || commands[1] == "Viking".ToLower() || commands[1] == "Pirate".ToUpper() || commands[1] == "Viking".ToUpper())
						{
							p2_name4.m_skinIndex = 8;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
						}
						else if (commands[1] == "Thief" || commands[1] == "Warrior" || commands[1] == "Thief".ToLower() || commands[1] == "Warrior".ToLower() || commands[1] == "Thief".ToUpper() || commands[1] == "Warrior".ToUpper())
						{
							p2_name4.m_skinIndex = 9;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
						}
						else if (commands[1] == "Psyco" || commands[1] == "Occult" || commands[1] == "Psyco".ToLower() || commands[1] == "Occult".ToLower() || commands[1] == "Psyco".ToUpper() || commands[1] == "Occult".ToUpper())
						{
							p2_name4.m_skinIndex = 10;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(skinResponse, player, msg);
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
                case "skin":
                    server.SendMessageToPlayerLocal("<color=white>To use a skin please use its</color>/n<color=yellow>'ID' or 'Name'</color> after /skin/n'/skin Chad'", player, msg);
                    break;
                default:
                    break;
            }

        }
    }
}

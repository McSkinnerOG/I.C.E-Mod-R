using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ICE
{
    public class CCE_P_COSMETIC : MonoBehaviour
    {
         internal static void HandleChatCommand(string text, ServerPlayer player, NetIncomingMessage msg)
        {
            var server = (LidServer)UnityEngine.Object.FindObjectOfType(typeof(LidServer));
            var p_pos = player.GetPosition();
            string[] commands = text.Split(' ');

            switch (commands[0])
            {
                case "/hat":
					if (player.m_isAdmin == true)
					{
						var textResponse = "Changed Hat to <b><color='#ffa500ff'>" + commands[1].ToString() + "</color></b>.";

						if (commands[1] == "0" || commands[1] == "ArmyCap" || commands[1] == "ArmyCap".ToLower() || commands[1] == "ArmyCap".ToUpper())
						{
							player.m_lookIndex = 0;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "1" || commands[1] == "ArmyCap")
						{
							player.m_lookIndex = 1;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "2" || commands[1] == "WoolHat")
						{
							player.m_lookIndex = 2;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "3" || commands[1] == "Pirate")
						{
							player.m_lookIndex = 3;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "4" || commands[1] == "BunnyEars")
						{
							player.m_lookIndex = 4;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "5" || commands[1] == "BaseBall")
						{
							player.m_lookIndex = 5;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "6" || commands[1] == "Santa")
						{
							player.m_lookIndex = 6;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "7" || commands[1] == "Cowboy")
						{
							player.m_lookIndex = 7;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "8" || commands[1] == "Ushanka")
						{
							player.m_lookIndex = 8;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "9" || commands[1] == "TopHat")
						{
							player.m_lookIndex = 9;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "10" || commands[1] == "Rasta")
						{
							player.m_lookIndex = 10;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "11" || commands[1] == "Cone" || commands[1] == "RoadCone")
						{
							player.m_lookIndex = 11;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "12" || commands[1] == "Bucket")
						{
							player.m_lookIndex = 12;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "13" || commands[1] == "Biker")
						{
							player.m_lookIndex = 13;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "14" || commands[1] == "Horse")
						{
							player.m_lookIndex = 14;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "15" || commands[1] == "Veteran")
						{
							player.m_lookIndex = 15;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "16" || commands[1] == "Bandana")
						{
							player.m_lookIndex = 16;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "17" || commands[1] == "Beanie-Blue")
						{
							player.m_lookIndex = 17;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "18" || commands[1] == "Irish")
						{
							player.m_lookIndex = 18;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "19" || commands[1] == "ACap")
						{
							player.m_lookIndex = 19;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "20" || commands[1] == "Pot" || commands[1] == "Pan")
						{
							player.m_lookIndex = 20;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "21" || commands[1] == "ArmyBoonie")
						{
							player.m_lookIndex = 21;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "22" || commands[1] == "Glasses" || commands[1] == "Sunnies")
						{
							player.m_lookIndex = 22;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "23" || commands[1] == "Wizard")
						{
							player.m_lookIndex = 23;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "24" || commands[1] == "Football")
						{
							player.m_lookIndex = 24;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "25" || commands[1] == "Bandana-Skull")
						{
							player.m_lookIndex = 25;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "26" || commands[1] == "Party")
						{
							player.m_lookIndex = 26;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "27" || commands[1] == "Derby")
						{
							player.m_lookIndex = 27;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "28" || commands[1] == "Miner")
						{
							player.m_lookIndex = 28;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "29" || commands[1] == "Headphones")
						{
							player.m_lookIndex = 29;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "30" || commands[1] == "Sherif")
						{
							player.m_lookIndex = 30;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "31" || commands[1] == "Police")
						{
							player.m_lookIndex = 31;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "32" || commands[1] == "Green-Beret")
						{
							player.m_lookIndex = 32;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "33" || commands[1] == "Halo")
						{
							player.m_lookIndex = 33;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "34" || commands[1] == "Crown")
						{
							player.m_lookIndex = 34;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "35" || commands[1] == "Skull")
						{
							player.m_lookIndex = 35;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "36" || commands[1] == "Fedora")
						{
							player.m_lookIndex = 36;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "37" || commands[1] == "SkiMask")
						{
							player.m_lookIndex = 37;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "38" || commands[1] == "Russia")
						{
							player.m_lookIndex = 38;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "39" || commands[1] == "USA")
						{
							player.m_lookIndex = 39;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "40" || commands[1] == "Brazil")
						{
							player.m_lookIndex = 40;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "41" || commands[1] == "Germany")
						{
							player.m_lookIndex = 41;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "42" || commands[1] == "Turkey")
						{
							player.m_lookIndex = 42;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "43" || commands[1] == "Poland")
						{
							player.m_lookIndex = 43;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "44" || commands[1] == "Canada")
						{
							player.m_lookIndex = 44;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "45" || commands[1] == "France")
						{
							player.m_lookIndex = 45;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "46" || commands[1] == "UK")
						{
							player.m_lookIndex = 46;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "47" || commands[1] == "Thailand")
						{
							player.m_lookIndex = 47;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "48" || commands[1] == "Spain")
						{
							player.m_lookIndex = 48;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "49" || commands[1] == "Australia")
						{
							player.m_lookIndex = 49;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "50" || commands[1] == "Italy")
						{
							player.m_lookIndex = 50;
							player.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);

						}
					}
					break;
				case "/hat-p":
					var p2_name4 = server.GetPlayerByName(commands[2]);
					if (player.m_isAdmin == true)
					{
						var textResponse = "Changed Hat to <b><color='#ffa500ff'>" + commands[1].ToString() + "</color></b>.";

						if (commands[1] == "0" || commands[1] == "ArmyCap" || commands[1] == "ArmyCap".ToLower() || commands[1] == "ArmyCap".ToUpper())
						{
							p2_name4.m_lookIndex = 0;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "1" || commands[1] == "ArmyCap")
						{
							p2_name4.m_lookIndex = 1;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "2" || commands[1] == "WoolHat")
						{
							p2_name4.m_lookIndex = 2;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "3" || commands[1] == "Pirate")
						{
							p2_name4.m_lookIndex = 3;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "4" || commands[1] == "BunnyEars")
						{
							p2_name4.m_lookIndex = 4;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "5" || commands[1] == "BaseBall")
						{
							p2_name4.m_lookIndex = 5;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "6" || commands[1] == "Santa")
						{
							p2_name4.m_lookIndex = 6;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "7" || commands[1] == "Cowboy")
						{
							p2_name4.m_lookIndex = 7;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "8" || commands[1] == "Ushanka")
						{
							p2_name4.m_lookIndex = 8;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "9" || commands[1] == "TopHat")
						{
							p2_name4.m_lookIndex = 9;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "10" || commands[1] == "Rasta")
						{
							p2_name4.m_lookIndex = 10;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "11" || commands[1] == "Cone" || commands[1] == "RoadCone")
						{
							p2_name4.m_lookIndex = 11;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "12" || commands[1] == "Bucket")
						{
							p2_name4.m_lookIndex = 12;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "13" || commands[1] == "Biker")
						{
							p2_name4.m_lookIndex = 13;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "14" || commands[1] == "Horse")
						{
							p2_name4.m_lookIndex = 14;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "15" || commands[1] == "Veteran")
						{
							p2_name4.m_lookIndex = 15;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "16" || commands[1] == "Bandana")
						{
							p2_name4.m_lookIndex = 16;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "17" || commands[1] == "Beanie-Blue")
						{
							p2_name4.m_lookIndex = 17;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "18" || commands[1] == "Irish")
						{
							p2_name4.m_lookIndex = 18;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "19" || commands[1] == "ACap")
						{
							p2_name4.m_lookIndex = 19;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "20" || commands[1] == "Pot" || commands[1] == "Pan")
						{
							p2_name4.m_lookIndex = 20;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "21" || commands[1] == "ArmyBoonie")
						{
							p2_name4.m_lookIndex = 21;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "22" || commands[1] == "Glasses" || commands[1] == "Sunnies")
						{
							p2_name4.m_lookIndex = 22;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "23" || commands[1] == "Wizard")
						{
							p2_name4.m_lookIndex = 23;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "24" || commands[1] == "Football")
						{
							p2_name4.m_lookIndex = 24;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "25" || commands[1] == "Bandana-Skull")
						{
							p2_name4.m_lookIndex = 25;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "26" || commands[1] == "Party")
						{
							p2_name4.m_lookIndex = 26;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "27" || commands[1] == "Derby")
						{
							p2_name4.m_lookIndex = 27;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "28" || commands[1] == "Miner")
						{
							p2_name4.m_lookIndex = 28;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "29" || commands[1] == "Headphones")
						{
							p2_name4.m_lookIndex = 29;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "30" || commands[1] == "Sherif")
						{
							p2_name4.m_lookIndex = 30;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "31" || commands[1] == "Police")
						{
							p2_name4.m_lookIndex = 31;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "32" || commands[1] == "Green-Beret")
						{
							p2_name4.m_lookIndex = 32;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "33" || commands[1] == "Halo")
						{
							p2_name4.m_lookIndex = 33;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "34" || commands[1] == "Crown")
						{
							p2_name4.m_lookIndex = 34;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "35" || commands[1] == "Skull")
						{
							p2_name4.m_lookIndex = 35;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "36" || commands[1] == "Fedora")
						{
							p2_name4.m_lookIndex = 36;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "37" || commands[1] == "SkiMask")
						{
							p2_name4.m_lookIndex = 37;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "38" || commands[1] == "Russia")
						{
							p2_name4.m_lookIndex = 38;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "39" || commands[1] == "USA")
						{
							p2_name4.m_lookIndex = 39;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "40" || commands[1] == "Brazil")
						{
							p2_name4.m_lookIndex = 40;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "41" || commands[1] == "Germany")
						{
							p2_name4.m_lookIndex = 41;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "42" || commands[1] == "Turkey")
						{
							p2_name4.m_lookIndex = 42;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "43" || commands[1] == "Poland")
						{
							p2_name4.m_lookIndex = 43;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "44" || commands[1] == "Canada")
						{
							p2_name4.m_lookIndex = 44;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "45" || commands[1] == "France")
						{
							p2_name4.m_lookIndex = 45;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "46" || commands[1] == "UK")
						{
							p2_name4.m_lookIndex = 46;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "47" || commands[1] == "Thailand")
						{
							p2_name4.m_lookIndex = 47;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "48" || commands[1] == "Spain")
						{
							p2_name4.m_lookIndex = 48;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "49" || commands[1] == "Australia")
						{
							p2_name4.m_lookIndex = 49;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);
						}
						else if (commands[1] == "50" || commands[1] == "Italy")
						{
							p2_name4.m_lookIndex = 50;
							p2_name4.m_updateInfoFlag = true;
							server.SendMessageToPlayerLocal(textResponse, player, msg);

						}
					}
					break;

				case "/help-buff":
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

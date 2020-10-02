using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ICE
{
    public class CCE_MODEL : MonoBehaviour
    {

       

         internal static void HandleChatCommand(string text, ServerPlayer player, NetIncomingMessage msg)
        {
            var server = (LidServer)UnityEngine.Object.FindObjectOfType(typeof(LidServer));
            var p_pos = player.GetPosition();
            string[] commands = text.Split(' ');

            switch (commands[0])
            {
				case "/model":
					if (player.m_isAdmin == true)
					{
						var mdlTextResponse = "Changed Model to <b><color='#ffa500ff'>" + commands[1].ToString() + "</color></b>.";
						eCharType eCharType = eCharType.ePlayer;
						var eMutant = eCharType.eMutant;

						if (commands[1] == "Car")
						{
							eCharType = (eCharType)(1);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "GasMask-Guy")
						{
							eCharType = (eCharType)(2);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Zombie")
						{
							eCharType = (eCharType)(3);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Chicken")
						{
							eCharType = (eCharType)(4);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Raven" || commands[1] == "Crow")
						{
							eCharType = (eCharType)(5);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Deer")
						{
							eCharType = (eCharType)(6);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Bull")
						{
							eCharType = (eCharType)(7);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Pig")
						{
							eCharType = (eCharType)(8);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Wolf")
						{
							eCharType = (eCharType)(9);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Survivor")
						{
							eCharType = (eCharType)(10);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Survivor-F")
						{
							eCharType = (eCharType)(11);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Player-F")
						{
							eCharType = (eCharType)(12);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Fox")
						{
							eCharType = (eCharType)(13);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Sheep")
						{
							eCharType = (eCharType)(14);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Eagle")
						{
							eCharType = (eCharType)(15);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Cow")
						{
							eCharType = (eCharType)(16);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "SurvivorMutant")
						{
							eCharType = (eCharType)(17);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Spider")
						{
							eCharType = (eCharType)(18);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Poison-Spider")
						{
							eCharType = (eCharType)(19);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}

						if ((player.m_isAdmin || eCharType == eCharType.ePlayer || eCharType == eCharType.ePlayerFemale) && player.m_charType != eCharType)
						{
							player.m_charType = eCharType;
							player.m_updateInfoFlag = true;
						}

					}
					break;

				case "/model-p":
					var p2_name = server.GetPlayerByName(commands[2]);
					if (player.m_isAdmin == true)
					{
						var mdlTextResponse = "Changed Model to <b><color='#ffa500ff'>" + commands[1].ToString() + "</color></b>.";
						eCharType eCharType = eCharType.ePlayer;
						var eMutant = eCharType.eMutant;

						if (commands[1] == "Car")
						{
							eCharType = (eCharType)(1);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "GasMask-Guy")
						{
							eCharType = (eCharType)(2);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Zombie")
						{
							eCharType = (eCharType)(3);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Chicken")
						{
							eCharType = (eCharType)(4);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Raven" || commands[1] == "Crow")
						{
							eCharType = (eCharType)(5);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Deer")
						{
							eCharType = (eCharType)(6);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Bull")
						{
							eCharType = (eCharType)(7);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Pig")
						{
							eCharType = (eCharType)(8);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Wolf")
						{
							eCharType = (eCharType)(9);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Survivor")
						{
							eCharType = (eCharType)(10);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Survivor-F")
						{
							eCharType = (eCharType)(11);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Player-F")
						{
							eCharType = (eCharType)(12);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Fox")
						{
							eCharType = (eCharType)(13);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Sheep")
						{
							eCharType = (eCharType)(14);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Eagle")
						{
							eCharType = (eCharType)(15);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Cow")
						{
							eCharType = (eCharType)(16);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "SurvivorMutant")
						{
							eCharType = (eCharType)(17);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Spider")
						{
							eCharType = (eCharType)(18);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}
						else if (commands[1] == "Poison-Spider")
						{
							eCharType = (eCharType)(19);
							server.SendMessageToPlayerLocal(mdlTextResponse, player, msg);
						}

						if ((eCharType == eCharType.ePlayer || eCharType == eCharType.ePlayerFemale) && p2_name.m_charType != eCharType)
						{
							p2_name.m_charType = eCharType;
							p2_name.m_updateInfoFlag = true;
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
			switch (commands[2])
			{
				case "prefill2":
					server.SendMessageToPlayerLocal("prefill2", player, msg);
					break;
				default:
					break;
			}

		}
    }
}

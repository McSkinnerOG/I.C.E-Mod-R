using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ICE
{
    public class CCE_BUFF : MonoBehaviour
    {
         internal static void HandleChatCommand(string text, ServerPlayer player, NetIncomingMessage msg)
        {
            var server = (LidServer)UnityEngine.Object.FindObjectOfType(typeof(LidServer));
            var p_pos = player.GetPosition();
            string[] commands = text.Split(' ');
            switch (commands[0])
            {
                case "/buff":
                    if (commands[1] == "Clear" || commands[1] == "None" || commands[1] == "Reset" && player.m_isAdmin == true)
                    {
                        player.SetCondition(eCondition.none, true);
                        player.SetCondition(eCondition.pain, false);
                        player.SetCondition(eCondition.radiation, false);
                        player.SetCondition(eCondition.freezing, false);
                        player.SetCondition(eCondition.bleeding, false);
                        player.SetCondition(eCondition.infection, false);
                        server.SendMessageToPlayerLocal("Cleared All effects on your player!", player, msg);
                    }
                    else if (commands[1] == "infect" && player.m_isAdmin == true)
                    {
                        player.SetCondition(eCondition.infection, true);
                    }
                    else if (commands[1] == "bleed" && player.m_isAdmin == true)
                    {
                        player.SetCondition(eCondition.bleeding, true);
                    }
                    else if (commands[1] == "freeze" && player.m_isAdmin == true)
                    {
                        player.SetCondition(eCondition.freezing, true);
                    }
                    else if (commands[1] == "rads" && player.m_isAdmin == true)
                    {
                        player.SetCondition(eCondition.radiation, true);
                    }
                    else if (commands[1] == "pain" && player.m_isAdmin == true)
                    {
                        player.SetCondition(eCondition.pain, true);
                    }
                    else if (commands[1] == null)
                    {
                        server.SendMessageToPlayerLocal("Please enter a buff/effect to apply: /nClear, Infected, Bleeding,/nFreezing, Radiation,/nPain", player, msg);
                    }
                    break;

                case "/buff_p":
                    var p2_name4 = server.GetPlayerByName(commands[2]);
                    if (commands[1] == "Clear" || commands[1] == "None" || commands[1] == "Reset" && player.m_isAdmin == true)
                    {
                        p2_name4.SetCondition(eCondition.none, true);
                        p2_name4.SetCondition(eCondition.pain, false);
                        p2_name4.SetCondition(eCondition.radiation, false);
                        p2_name4.SetCondition(eCondition.freezing, false);
                        p2_name4.SetCondition(eCondition.bleeding, false);
                        p2_name4.SetCondition(eCondition.infection, false);
                        server.SendMessageToPlayerLocal("Cleared All effects on the player!", player, msg);
                    }
                    else if (commands[1] == "Infected" && player.m_isAdmin == true)
                    {
                        p2_name4.SetCondition(eCondition.infection, true);
                    }
                    else if (commands[1] == "Bleeding" && player.m_isAdmin == true)
                    {
                        p2_name4.SetCondition(eCondition.bleeding, true);
                    }
                    else if (commands[1] == "Freezing" && player.m_isAdmin == true)
                    {
                        p2_name4.SetCondition(eCondition.freezing, true);
                    }
                    else if (commands[1] == "Radiation" && player.m_isAdmin == true)
                    {
                        p2_name4.SetCondition(eCondition.radiation, true);
                    }
                    else if (commands[1] == "Pain" && player.m_isAdmin == true)
                    {
                        p2_name4.SetCondition(eCondition.pain, true);
                    }
                    else if (commands[1] == null)
                    {
                        server.SendMessageToPlayerLocal("Please enter a buff/effect to apply: /nClear, Infected, Bleeding,/nFreezing, Radiation,/nPain", player, msg);
                    }
                    else if (commands[2] == null)
                    {
                        server.SendMessageToPlayerLocal("Please enter a players name to apple an effect/buff to.", player, msg);
                    }
                    break;

                case "/help-buff":
                    server.SendMessageToPlayerLocal("Usage for /buff:/n/buff status 'Freezing, Bleeding, Clear'", player, msg);
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

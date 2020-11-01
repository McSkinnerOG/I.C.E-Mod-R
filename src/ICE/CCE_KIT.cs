using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ICE
{
    public class CCE_KIT : MonoBehaviour
    {
         internal static void HandleChatCommand(string text, ServerPlayer player, NetIncomingMessage msg)
        {
            var server = (LidServer)UnityEngine.Object.FindObjectOfType(typeof(LidServer));
            var p_pos = player.GetPosition();
            var IA = player.m_inventory.GetItemAmountByType(254);
            string[] commands = text.Split(' ');

            switch (commands[0])
            {
                case "/kit-doc1":
                    // Start currency Check
                    if (IA <= 349) // Value to check against.
                    {
                        server.SendMessageToPlayerLocal("Not Enough Gold!", player, msg); // Return error message if plater des not have enough currency!
                    }
                    else if (IA >= 350) // If players currency is higher then the value given here
                    {
                        int num = 350; // Kit Price
                        num = Math.Min(IA, num); // Price math logic
                        player.m_inventory.DeclineItemAmountByType(254, num); // Define item used as currency
                        server.CreateFreeWorldItem(153, 1, p_pos, 100);   // Leather-Vest
                        server.CreateFreeWorldItem(171, 1, p_pos, 100);   // Shoes
                        // Weapons
                        server.CreateFreeWorldItem(93, 1, p_pos, 100);    // Knife
                        server.CreateFreeWorldItem(107, 1, p_pos, 100);   // Torch
                        // Food
                        server.CreateFreeWorldItem(3, 6, p_pos);     // Coocked Potatoes
                        server.CreateFreeWorldItem(8, 1, p_pos);     // Energy Bar
                        server.CreateFreeWorldItem(17, 2, p_pos);    // Water
                        // ITEMS
                        server.CreateFreeWorldItem(140, 3, p_pos);   // Bandages
                        server.CreateFreeWorldItem(141, 1, p_pos);   // Antibiotics
                        server.CreateFreeWorldItem(142, 2, p_pos);   // Painkillers
                        server.CreateFreeWorldItem(143, 1, p_pos);   // Medpack
                        LidServer.SendMoneyUpdate(player); // Request money update from server.
                        server.SendMessageToPlayerLocal("BOUGHT DOCTOR KIT!!!", player, msg); // Return success message!!!
                    }
                    break;

                case "/kit-scav1":
                    if (IA <= 349)
                    {
                        server.SendMessageToPlayerLocal("Not Enough Gold!", player, msg);
                    }
                    else if (IA >= 350)
                    {
                        int num = 350;
                        num = Math.Min(IA, num);
                        player.m_inventory.DeclineItemAmountByType(254, num);
                        // Clothing
                        // Weapons
                        server.CreateFreeWorldItem(111, 1, p_pos, 100);    // Machete
                        server.CreateFreeWorldItem(93, 1, p_pos, 100);   // Knife
                        server.CreateFreeWorldItem(61, 1, p_pos, 100);   // Revolver
                        // AMMO
                        server.CreateFreeWorldItem(40, 20, p_pos, 100);   // 45mm
                        // Food
                        server.CreateFreeWorldItem(4, 2, p_pos);     // Raw Meat
                        server.CreateFreeWorldItem(8, 1, p_pos);     // Energy Bar
                        server.CreateFreeWorldItem(10, 2, p_pos);    // Canned Food
                        server.CreateFreeWorldItem(18, 2, p_pos);    // Beer
                        // ITEMS
                        server.CreateFreeWorldItem(140, 3, p_pos);   // Bandages
                        LidServer.SendMoneyUpdate(player);
                        server.SendMessageToPlayerLocal("BOUGHT SCAV KIT!!!", player, msg);
                    }
                    break;

                case "/kit-scav2":
                    if (IA <= 349)
                    {
                        server.SendMessageToPlayerLocal("Not Enough Gold!", player, msg);
                    }
                    else if (IA >= 350)
                    {
                        int num = 350;
                        num = Math.Min(IA, num);
                        player.m_inventory.DeclineItemAmountByType(254, num);
                        // Clothing
                        server.CreateFreeWorldItem(151, 1, p_pos, 100);   // Scrap-Vest
                        server.CreateFreeWorldItem(171, 1, p_pos, 100);   // C_Shoes
                        // Weapons
                        server.CreateFreeWorldItem(104, 1, p_pos, 100);    // Mutant Claw
                        server.CreateFreeWorldItem(106, 1, p_pos, 100);   // C_Knife
                        server.CreateFreeWorldItem(79, 1, p_pos, 100);   // Bow
                        // AMMO
                        server.CreateFreeWorldItem(50, 35, p_pos, 100);   // Arrows
                        // Food
                        server.CreateFreeWorldItem(4, 2, p_pos);     // Raw Meat
                        server.CreateFreeWorldItem(15, 1, p_pos);    // Rum-Bottle
                        // ITEMS
                        server.CreateFreeWorldItem(140, 2, p_pos);   // Bandages
                        server.CreateFreeWorldItem(140, 1, p_pos);   // Painkillers
                        LidServer.SendMoneyUpdate(player);
                        server.SendMessageToPlayerLocal("BOUGHT SCAV-2 KIT!!!", player, msg);
                    }
                    break;

                case "/kit-scav3":
                    if (IA <= 349)
                    {
                        server.SendMessageToPlayerLocal("Not Enough Gold!", player, msg);
                    }
                    else if (IA >= 350)
                    {
                        int num = 350;
                        num = Math.Min(IA, num);
                        player.m_inventory.DeclineItemAmountByType(254, num);
                        // Clothing
                        server.CreateFreeWorldItem(151, 1, p_pos, 100);   // Scrap-Vest
                        server.CreateFreeWorldItem(171, 1, p_pos, 100);   // C_Shoes
                        // Weapons
                        server.CreateFreeWorldItem(79, 1, p_pos, 100);   // Bow
                        // AMMO
                        server.CreateFreeWorldItem(50, 35, p_pos, 100);   // Arrows
                        // Food
                        server.CreateFreeWorldItem(4, 2, p_pos);     // Raw Meat
                        server.CreateFreeWorldItem(9, 4, p_pos);     // Mushrooms
                        server.CreateFreeWorldItem(10, 1, p_pos);    // Canned Food
                        server.CreateFreeWorldItem(15, 1, p_pos);    // Rum-Bottle
                        // ITEMS
                        server.CreateFreeWorldItem(140, 2, p_pos);   // Bandages
                        server.CreateFreeWorldItem(140, 1, p_pos);   // Painkillers
                        LidServer.SendMoneyUpdate(player);
                        server.SendMessageToPlayerLocal("BOUGHT SCAV-3 KIT!!!", player, msg);
                    }
                    break;

                case "/kit-bandit1":
                    if (IA <= 349)
                    {
                        server.SendMessageToPlayerLocal("Not Enough Gold!", player, msg);
                    }
                    else if (IA >= 350)
                    {
                        int num = 350;
                        num = Math.Min(IA, num);
                        player.m_inventory.DeclineItemAmountByType(254, num);
                        // Clothing
                        server.CreateFreeWorldItem(152, 1, p_pos, 100);   // Metal-Vest
                        server.CreateFreeWorldItem(170, 1, p_pos, 100);   // Shoes
                        // Weapons
                        server.CreateFreeWorldItem(95, 1, p_pos, 100);    // Mutant Claw
                        server.CreateFreeWorldItem(93, 1, p_pos, 100);   // Knife
                        server.CreateFreeWorldItem(62, 1, p_pos, 100);   // SMG
                        // AMMO
                        server.CreateFreeWorldItem(42, 35, p_pos, 100);   // 556
                        // Food
                        server.CreateFreeWorldItem(12, 2, p_pos);     // Cooked_fish
                        server.CreateFreeWorldItem(8, 4, p_pos);     // Energy-Bar
                        server.CreateFreeWorldItem(16, 1, p_pos);    // Wine
                        server.CreateFreeWorldItem(15, 1, p_pos);    // Rum-Bottle
                        // ITEMS
                        server.CreateFreeWorldItem(140, 2, p_pos);   // Bandages
                        server.CreateFreeWorldItem(142, 2, p_pos);   // Painkillers
                        LidServer.SendMoneyUpdate(player);
                        server.SendMessageToPlayerLocal("BOUGHT BANDIT KIT!!!", player, msg);
                    }
                    break;

                case "/kit-bandit2":
                    if (IA <= 349)
                    {
                        server.SendMessageToPlayerLocal("Not Enough Gold!", player, msg);
                    }
                    else if (IA >= 350)
                    {
                        int num = 350;
                        num = Math.Min(IA, num);
                        player.m_inventory.DeclineItemAmountByType(254, num);
                        // Clothing
                        server.CreateFreeWorldItem(170, 1, p_pos, 100);   // Shoes
                        // Weapons
                        server.CreateFreeWorldItem(93, 1, p_pos, 100);   // Knife
                        server.CreateFreeWorldItem(68, 1, p_pos, 100);   // Tommy-Gun
                        // AMMO
                        server.CreateFreeWorldItem(40, 35, p_pos, 100);   // 45mm
                        // Food
                        server.CreateFreeWorldItem(12, 2, p_pos);     // Cooked_fish
                        server.CreateFreeWorldItem(8, 4, p_pos);     // Energy-Bar
                        server.CreateFreeWorldItem(16, 1, p_pos);    // Wine
                        server.CreateFreeWorldItem(15, 1, p_pos);    // Rum-Bottle
                        // ITEMS
                        server.CreateFreeWorldItem(140, 2, p_pos);   // Bandages
                        server.CreateFreeWorldItem(142, 2, p_pos);   // Painkillers
                        LidServer.SendMoneyUpdate(player);
                        server.SendMessageToPlayerLocal("BOUGHT BANDIT KIT!!!", player, msg);
                    }
                    break;

                case "/kit-bandit3":
                    if (IA <= 349)
                    {
                        server.SendMessageToPlayerLocal("Not Enough Gold!", player, msg);
                    }
                    else if (IA >= 350)
                    {
                        int num = 350;
                        num = Math.Min(IA, num);
                        player.m_inventory.DeclineItemAmountByType(254, num);
                        // Clothing
                        server.CreateFreeWorldItem(152, 1, p_pos, 100);   // Metal-Vest
                        server.CreateFreeWorldItem(170, 1, p_pos, 100);   // Shoes
                        // Weapons
                        server.CreateFreeWorldItem(95, 1, p_pos, 100);    // Mutant Claw
                        server.CreateFreeWorldItem(93, 1, p_pos, 100);   // Knife
                        // AMMO
                        // Food
                        server.CreateFreeWorldItem(12, 2, p_pos);     // Cooked_fish
                        server.CreateFreeWorldItem(8, 4, p_pos);     // Energy-Bar
                        server.CreateFreeWorldItem(16, 1, p_pos);    // Wine
                        server.CreateFreeWorldItem(15, 1, p_pos);    // Rum-Bottle
                        // ITEMS
                        server.CreateFreeWorldItem(140, 2, p_pos);   // Bandages
                        server.CreateFreeWorldItem(142, 2, p_pos);   // Painkillers
                        LidServer.SendMoneyUpdate(player);
                        server.SendMessageToPlayerLocal("BOUGHT BANDIT KIT!!!", player, msg);
                    }
                    break;
                case "/kit-guard1":
                    if (IA <= 499)
                    {
                        server.SendMessageToPlayerLocal("Not Enough Gold!", player, msg);
                    }
                    else if (IA >= 500)
                    {
                        int num = 500;
                        num = Math.Min(IA, num);
                        player.m_inventory.DeclineItemAmountByType(254, num);
                        // Clothing
                        server.CreateFreeWorldItem(154, 1, p_pos, 100);   // Guardian-Vest
                        server.CreateFreeWorldItem(170, 1, p_pos, 100);   // Shoes
                        // Weapons
                        server.CreateFreeWorldItem(99, 1, p_pos, 100);    // Giant Sword
                        server.CreateFreeWorldItem(93, 1, p_pos, 100);   // Knife
                        server.CreateFreeWorldItem(65, 1, p_pos, 100);   // AK47
                        // AMMO
                        server.CreateFreeWorldItem(43, 35, p_pos, 100);   // 762
                        // Food
                        server.CreateFreeWorldItem(12, 2, p_pos);     // Cooked_fish
                        server.CreateFreeWorldItem(8, 4, p_pos);     // Energy-Bar
                        server.CreateFreeWorldItem(16, 1, p_pos);    // Wine
                        // ITEMS
                        server.CreateFreeWorldItem(140, 2, p_pos);   // Bandages
                        server.CreateFreeWorldItem(142, 2, p_pos);   // Painkillers
                        server.CreateFreeWorldItem(143, 1, p_pos);   // Medkit
                        LidServer.SendMoneyUpdate(player);
                        server.SendMessageToPlayerLocal("BOUGHT GUARD-1 KIT!!!", player, msg);
                    }
                    break;
                case "/kit-guard2":
                    if (IA <= 499)
                    {
                        server.SendMessageToPlayerLocal("Not Enough Gold!", player, msg);
                    }
                    else if (IA >= 500)
                    {
                        int num = 500;
                        num = Math.Min(IA, num);
                        player.m_inventory.DeclineItemAmountByType(254, num);
                        // Clothing
                        server.CreateFreeWorldItem(154, 1, p_pos, 100);   // Guardian-Vest
                        server.CreateFreeWorldItem(170, 1, p_pos, 100);   // Shoes
                        // Weapons
                        server.CreateFreeWorldItem(99, 1, p_pos, 100);    // Giant Sword
                        server.CreateFreeWorldItem(93, 1, p_pos, 100);   // Knife
                        server.CreateFreeWorldItem(64, 1, p_pos, 100);   // Sniper
                        // AMMO
                        server.CreateFreeWorldItem(43, 35, p_pos, 100);   // 762
                        // Food
                        server.CreateFreeWorldItem(12, 2, p_pos);     // Cooked_fish
                        server.CreateFreeWorldItem(8, 4, p_pos);     // Energy-Bar
                        server.CreateFreeWorldItem(16, 1, p_pos);    // Wine
                        // ITEMS
                        server.CreateFreeWorldItem(140, 2, p_pos);   // Bandages
                        server.CreateFreeWorldItem(142, 2, p_pos);   // Painkillers
                        server.CreateFreeWorldItem(143, 1, p_pos);   // Medkit
                        LidServer.SendMoneyUpdate(player);
                        server.SendMessageToPlayerLocal("BOUGHT GUARD-2 KIT!!!", player, msg);
                    }
                    break;
                case "/kit-guard3":
                    if (IA <= 499)
                    {
                        server.SendMessageToPlayerLocal("Not Enough Gold!", player, msg);
                    }
                    else if (IA >= 500)
                    {
                        int num = 500;
                        num = Math.Min(IA, num);
                        player.m_inventory.DeclineItemAmountByType(254, num);
                        // Clothing
                        server.CreateFreeWorldItem(154, 1, p_pos, 100);   // Guardian-Vest
                        server.CreateFreeWorldItem(170, 1, p_pos, 100);   // Shoes
                        // Weapons
                        server.CreateFreeWorldItem(99, 1, p_pos, 100);    // Giant Sword
                        server.CreateFreeWorldItem(93, 1, p_pos, 100);   // Knife
                        server.CreateFreeWorldItem(67, 1, p_pos, 100);   // Auto-SHotgun
                        // AMMO
                        server.CreateFreeWorldItem(44, 35, p_pos, 100);   // Shells
                        // Food
                        server.CreateFreeWorldItem(12, 2, p_pos);     // Cooked_fish
                        server.CreateFreeWorldItem(8, 4, p_pos);     // Energy-Bar
                        server.CreateFreeWorldItem(16, 1, p_pos);    // Wine
                        // ITEMS
                        server.CreateFreeWorldItem(140, 2, p_pos);   // Bandages
                        server.CreateFreeWorldItem(142, 2, p_pos);   // Painkillers
                        server.CreateFreeWorldItem(143, 1, p_pos);   // Medkit
                        LidServer.SendMoneyUpdate(player);
                        server.SendMessageToPlayerLocal("BOUGHT GUARD-3 KIT!!!", player, msg);
                    }
                    break;

                case "/help-kit":
                    server.SendMessageToPlayerLocal("Usage for /kit-xxx: To purchase a kit you must have the required amount of gold and or permissions. Example: Enter */kit-doc* without the ** to buy a doctors kit for 500gold.", player, msg);
                    break;
             
                default:
                    break;
            }
            switch (commands[1])
            {
                case "kit-doc": 
                    server.SendMessageToPlayerLocal("<color=purple>Doctor-kit costs 500gold and recieves: </color> <color=yellow>Leather-Vest x1, Shoes x1, </color> <color=white>Torch x1, Knife x1,</color> <color=brown>Cooked-Potatoes x6, Energy-Bar x1,</color> <color=cyan>Water x2,</color> <color=red>Bandages x3, Anti-Biotics x1 , Painkillers x2, Medpack x1</color>", player, msg);
                    break;
                case "kit-scav1":
                    server.SendMessageToPlayerLocal("<color=purple>Scavenger-kit 1 costs 500gold and recieves: </color> <color=yellow>Scrap-Vest x1, Shoes x1, </color> <color=white>Machete x1, Knife x1, Revolver x1, 45mm Ammo x20</color> <color=brown> Raw-Meat x 1, Canned-Food x6, Energy-Bar x1,</color> <color=cyan>Beer x2,</color> <color=red>Bandages x3</color>", player, msg);
                    break;
                case "kit-scav2":
                    server.SendMessageToPlayerLocal("<color=purple>Scavenger-kit 2 costs 500gold and recieves: </color> <color=yellow>Scrap-Vest x1, Sneakers x1, </color> <color=white>Mutant-Claw x1, Crafted-Knife x1, Bow x1, Arrows x32 </color> <color=brown>Cooked-Potatoes x6, Mushrooms x3,</color> <color=cyan>Rum-Bottle x2,</color> <color=red>Bandages x3, Anti-Biotics x1 , Painkillers x2, Medpack x1</color>", player, msg);
                    break;
                case "kit-bandit":
                    server.SendMessageToPlayerLocal("<color=purple>Bandit-kit costs 500gold and recieves: </color> <color=yellow>Metal-Vest x1, Sneakers x1, </color> <color=white>Katana x1, Knife x1, SMG x1, 9mm Ammo x32 </color> <color=brown>Cooked-Fish x3, Energy-Bar x1,</color> <color=cyan>Rum-Bottle x2, Wine x1, </color> <color=red>Bandages x3, Medpack x1</color>", player, msg);
                    break;
                case "kit-guard1":
                    server.SendMessageToPlayerLocal("<color=purple>Guardian-kit 1 costs 5000gold and recieves: </color> <color=yellow>Guardian-Vest x1, Sneakers x1, </color> <color=white>Torch x1, Knife x1,</color> <color=brown>Cooked-Potatoes x6, Energy-Bar x1,</color> <color=cyan>Water x2,</color> <color=red>Bandages x3, Anti-Biotics x1 , Painkillers x2, Medpack x1</color>", player, msg);
                    break;
                case "kit-guard2":
                    server.SendMessageToPlayerLocal("<color=purple>Guardian-kit 2 costs 5000gold and recieves: </color> <color=yellow>Guardian-Vest x1, Sneakers x1, </color> <color=white>Torch x1, Knife x1,</color> <color=brown>Cooked-Potatoes x6, Energy-Bar x1,</color> <color=cyan>Water x2,</color> <color=red>Bandages x3, Anti-Biotics x1 , Painkillers x2, Medpack x1</color>", player, msg);
                    break;
                case "kit-guard3":
                    server.SendMessageToPlayerLocal("<color=purple>Guardian-kit 3 costs 5000gold and recieves: </color> <color=yellow>Guardian-Vest x1, Sneakers x1, </color> <color=white>Giant-Sword x1, Knife x1, Crowbar x1, AK47 x1, </color> <color=brown>Canned-Food x6, Energy-Bar x1, Soda x1, </color> <color=cyan>Water x2,</color> <color=red>Medpack x1.</color>", player, msg);
                    break;
                default:
                    break;
            }

        }
    }
}

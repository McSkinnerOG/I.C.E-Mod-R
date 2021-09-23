using System;
using UnityEngine;

public static class Items
{
	// Note: this type is marked as 'beforefieldinit'.
	static Items()
	{
	}

	private static void Init()
	{
		Items.m_itemDefs = new ItemDef[255];
		Items.m_itemDefs[0] = new ItemDef("FISTS", 0f, 5f, 0.7f, 1.4f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[1] = new ItemDef("BERRIES", 20f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[2] = new ItemDef("POTATOES_RAW", 15f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[3] = new ItemDef("POTATOES_COOKED", 25f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[4] = new ItemDef("MEAT_RAW", -20f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[5] = new ItemDef("MEAT_COOKED", 30f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[6] = new ItemDef("EGGS_RAW", -20f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[7] = new ItemDef("EGGS_COOKED", 40f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[8] = new ItemDef("ENERGY_BAR", 35f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[9] = new ItemDef("MUSHROOMS", 10f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[10] = new ItemDef("CANNED_FOOD", 45f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[11] = new ItemDef("FISH_RAW", -15f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[12] = new ItemDef("FISH_COOKED", 33f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[15] = new ItemDef("RUMBOTTLE", 5f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[16] = new ItemDef("WINE", 10f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[17] = new ItemDef("WATER", 15f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[18] = new ItemDef("BEER", 25f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[19] = new ItemDef("SODA", 30f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[20] = new ItemDef("C_WOODENDOOR", 0f, 0f, 0f, 0f, 0f, 0, 80, 0, 0, 0, 0, 20);
		Items.m_itemDefs[21] = new ItemDef("C_METALDOOR", 0f, 0f, 0f, 0f, 0f, 0, 0, 180, 0, 0, 3, 21);
		Items.m_itemDefs[22] = new ItemDef("C_WOODWALL", 0f, 0f, 0f, 0f, 0f, 0, 60, 0, 0, 0, 0, 40);
		Items.m_itemDefs[23] = new ItemDef("C_STONEWALL", 0f, 0f, 0f, 0f, 0f, 0, 0, 0, 120, 0, 1, 41);
		Items.m_itemDefs[24] = new ItemDef("C_CAMPFIRE", 0f, 0f, 0f, 0f, 0f, 0, 10, 0, 0, 0, 0, 100);
		Items.m_itemDefs[26] = new ItemDef("C_BED", 0f, 0f, 0f, 0f, 0f, 0, 0, 0, 0, 50, 2, 101);
		Items.m_itemDefs[27] = new ItemDef("C_LOOTBOX", 0f, 0f, 0f, 0f, 0f, 0, 80, 0, 0, 0, 3, 103);
		Items.m_itemDefs[28] = new ItemDef("C_TESLACOIL", 0f, 0f, 0f, 0f, 0f, 0, 0, 600, 40, 0, 4, 104);
		Items.m_itemDefs[30] = new ItemDef("TNT", 0f, 0f, 0f, 0f, 0f, 0, 0, 0, 0, 0, 0, 102);
		Items.m_itemDefs[40] = new ItemDef("45", 0f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[41] = new ItemDef("9", 0f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[42] = new ItemDef("556", 0f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[43] = new ItemDef("762", 0f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[44] = new ItemDef("SHELL", 0f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[50] = new ItemDef("C_ARROW", 0f, 0f, 0f, 0f, 0f, 0, 1, 0, 1, 0, 1, 0);
		Items.m_itemDefs[51] = new ItemDef("C_STONE", 0f, 0f, 0f, 0f, 0f, 0, 0, 0, 1, 0, 0, 0);
		Items.m_itemDefs[60] = new ItemDef("PISTOL", 0f, 22f, 0.6f, 11f, 0.8f, 41, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[61] = new ItemDef("REVOLVER", 0f, 28f, 0.7f, 11f, 0.9f, 40, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[62] = new ItemDef("SMG", 0f, 24f, 0.4f, 12f, 0.7f, 42, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[63] = new ItemDef("SHOTGUN", 0f, 45f, 0.9f, 8f, 0.9f, 44, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[64] = new ItemDef("SNIPERRIFLE", 0f, 40f, 1f, 16f, 0.9f, 43, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[65] = new ItemDef("AK47", 0f, 30f, 0.5f, 12f, 0.8f, 43, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[66] = new ItemDef("UZI", 0f, 20f, 0.35f, 11f, 0.9f, 41, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[67] = new ItemDef("AUTOSHOTGUN", 0f, 36f, 0.6f, 8f, 0.8f, 44, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[68] = new ItemDef("TOMMYGUN", 0f, 25f, 0.5f, 12f, 0.6f, 40, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[77] = new ItemDef("C_SHOTGUN", 0f, 32f, 1f, 8f, 0.1f, 44, 0, 50, 0, 0, 2, 0);
		Items.m_itemDefs[78] = new ItemDef("C_SLINGSHOT", 0f, 20f, 0.9f, 8f, 0.25f, 51, 10, 0, 0, 5, 0, 0);
		Items.m_itemDefs[79] = new ItemDef("C_BOW", 0f, 25f, 1f, 9f, 0.4f, 50, 20, 0, 0, 10, 1, 0);
		Items.m_itemDefs[90] = new ItemDef("FIREAXE", 0f, 32f, 1.1f, 1.4f, 0.85f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[91] = new ItemDef("MACHETE", 0f, 30f, 1f, 1.4f, 0.7f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[92] = new ItemDef("HAMMER", 0f, 24f, 1f, 1.4f, 0.9f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[93] = new ItemDef("KNIFE", 0f, 20f, 0.85f, 1.4f, 0.75f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[94] = new ItemDef("PLUNGER", 0f, 6f, 0.9f, 1.4f, 0.1f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[95] = new ItemDef("KATANA", 0f, 35f, 1f, 1.4f, 0.7f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[96] = new ItemDef("CROWBAR", 0f, 26f, 1f, 1.4f, 0.9f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[97] = new ItemDef("WRENCH", 0f, 25f, 1.1f, 1.4f, 0.9f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[98] = new ItemDef("CLEAVER", 0f, 28f, 1f, 1.4f, 0.8f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[99] = new ItemDef("GIANTSWORD", 0f, 50f, 1.5f, 1.4f, 0.7f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[104] = new ItemDef("MUTANT_CLAW", 0f, 10f, 1f, 1.4f, 0.2f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[105] = new ItemDef("C_STONEHATCHET", 0f, 22f, 0.95f, 1.4f, 0.3f, 0, 10, 0, 5, 0, 2, 0);
		Items.m_itemDefs[106] = new ItemDef("C_KNIFE", 0f, 18f, 0.8f, 1.4f, 0.4f, 0, 0, 5, 0, 0, 1, 0);
		Items.m_itemDefs[107] = new ItemDef("C_TORCH", 0f, 20f, 1.1f, 1.4f, 0.01f, 0, 10, 0, 0, 3, 0, 0);
		Items.m_itemDefs[108] = new ItemDef("C_WOODCLUB", 0f, 14f, 1f, 1.4f, 0.2f, 0, 10, 0, 0, 0, 0, 0);
		Items.m_itemDefs[109] = new ItemDef("C_SPADE", 0f, 10f, 1.1f, 1.4f, 0.6f, 0, 10, 5, 0, 0, 1, 0);
		Items.m_itemDefs[110] = new ItemDef("C_FISHINGROD", 0f, 10f, 2f, 1.4f, 0.5f, 0, 10, 1, 0, 1, 2, 0);
		Items.m_itemDefs[111] = new ItemDef("C_MACHETE", 0f, 28f, 1.05f, 1.4f, 0.5f, 0, 0, 15, 0, 0, 3, 0);
		Items.m_itemDefs[120] = new ItemDef("BACKPACK", 0f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[121] = new ItemDef("CLOTHBOX", 0f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[130] = new ItemDef("WOOD", 0f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[131] = new ItemDef("METAL", 0f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[132] = new ItemDef("STONE", 0f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[133] = new ItemDef("CLOTH", 0f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[140] = new ItemDef("C_BANDAGES", 0f, 0f, 0f, 0f, 0f, 0, 0, 0, 0, 3, 0, 0);
		Items.m_itemDefs[141] = new ItemDef("ANTIBIOTICS", 0f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[142] = new ItemDef("PAINKILLERS", 0f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[143] = new ItemDef("MEDPACK", 30f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[150] = new ItemDef("KEVLARVEST", 0.6f, 0f, 0f, 0f, 0.2f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[151] = new ItemDef("C_SCRAPVEST", 0.8f, 0f, 0f, 0f, 0.05f, 0, 40, 0, 0, 5, 2, 0);
		Items.m_itemDefs[152] = new ItemDef("C_METALVEST", 0.7f, 0f, 0f, 0f, 0.1f, 0, 0, 60, 0, 10, 3, 0);
		Items.m_itemDefs[153] = new ItemDef("C_LEATHERVEST", 0.9f, 0f, 0f, 0f, 0.01f, 0, 0, 0, 0, 20, 1, 0);
		Items.m_itemDefs[154] = new ItemDef("GUARDIANVEST", 0.5f, 0f, 0f, 0f, 0.2f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[170] = new ItemDef("SNEAKERS", 0.1f, 0f, 0f, 0f, 0.997f, 0, 0, 0, 0, 0, 0, 0);
		Items.m_itemDefs[171] = new ItemDef("C_SHOES", 0.05f, 0f, 0f, 0f, 0.994f, 0, 0, 0, 0, 40, 4, 0);
		Items.m_itemDefs[254] = new ItemDef("GOLD", 0f, 5f, 1f, 1.3f, 0f, 0, 0, 0, 0, 0, 0, 0);
	}

	public static ItemDef GetItemDef(int a_type)
	{
		if (Items.m_itemDefs == null)
		{
			Items.Init();
		}
		if (a_type < 0 || a_type >= Items.m_itemDefs.Length)
		{
			return default(ItemDef);
		}
		return Items.m_itemDefs[a_type];
	}

	public static string GetStatsText(int a_type, int a_amount, bool a_displayValue = false)
	{
		string text = string.Empty;
		ItemDef itemDef = Items.GetItemDef(a_type);
		if (itemDef.ident != null && itemDef.ident.Length > 0)
		{
			if (a_amount > -1 && Items.HasAmountOrCondition(a_type))
			{
				text = string.Concat(new object[]
				{
					(!Items.IsStackable(a_type)) ? LNG.Get("CONDITION") : LNG.Get("AMOUNT"),
					": ",
					a_amount,
					(!Items.IsStackable(a_type)) ? "%" : string.Empty,
					"\n"
				});
			}
			if (a_displayValue)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					LNG.Get("VALUE"),
					": ",
					(int)(Items.GetValue(a_type, a_amount) + 0.5f),
					" ",
					LNG.Get("CURRENCY"),
					"\n"
				});
			}
			if (itemDef.damage > 5f)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					LNG.Get("DAMAGE"),
					": ",
					itemDef.damage,
					"\n"
				});
				if (itemDef.ammoItemType > 0)
				{
					text = text + LNG.Get(Items.GetItemDef(itemDef.ammoItemType).ident) + "\n";
				}
			}
			else if (itemDef.healing > 0f)
			{
				if (Items.IsShoes(a_type))
				{
					string text2 = text;
					text = string.Concat(new object[]
					{
						text2,
						LNG.Get("SPEED"),
						": +",
						(int)(itemDef.healing * 100.0001f),
						"%\n"
					});
				}
				else if (Items.IsBody(a_type))
				{
					string text2 = text;
					text = string.Concat(new object[]
					{
						text2,
						LNG.Get("ARMOR"),
						": ",
						(int)((1f - itemDef.healing) * 100.0001f),
						"%\n"
					});
				}
				else
				{
					string text2 = text;
					text = string.Concat(new object[]
					{
						text2,
						LNG.Get("ENERGY"),
						": ",
						itemDef.healing,
						"\n"
					});
				}
			}
		}
		return text;
	}

	public static bool HasAmountOrCondition(int a_type)
	{
		ItemDef itemDef = Items.GetItemDef(a_type);
		return (itemDef.durability > 0f && itemDef.durability < 1f) || Items.IsStackable(a_type);
	}

	public static bool HasCondition(int a_type)
	{
		ItemDef itemDef = Items.GetItemDef(a_type);
		return itemDef.durability > 0f && itemDef.durability < 1f;
	}

	public static int GetAmmoSoundIndex(int a_type)
	{
		if (Items.IsRareAmmo(a_type))
		{
			return a_type - 39;
		}
		return 0;
	}

	public static float GetWeaponXpMultiplier(int a_type)
	{
		ItemDef itemDef = Items.GetItemDef(a_type);
		return itemDef.damage / itemDef.attackdur + itemDef.range * 2f;
	}

	public static float GetNewValue(int a_type)
	{
		float result = 1f;
		ItemDef itemDef = Items.GetItemDef(a_type);
		if (Items.IsEatable(a_type))
		{
			result = 4f + Mathf.Abs(itemDef.healing) * 0.2f;
		}
		else if (Items.IsResource(a_type))
		{
			result = 0.4f;
		}
		else if (Items.IsCraftable(a_type))
		{
			result = (float)(itemDef.wood + itemDef.metal + itemDef.stone + itemDef.cloth) * 0.4f * (1f + (float)itemDef.rankReq * 0.5f);
		}
		else if (5f < itemDef.damage && 0f < itemDef.attackdur)
		{
			result = (itemDef.damage - 5f) / itemDef.attackdur * 3f + itemDef.range * 5f;
		}
		else if (Items.IsRareAmmo(a_type))
		{
			result = 5f;
		}
		else if (Items.IsMedicine(a_type))
		{
			result = 10f;
			if (a_type == 143)
			{
				result = 70f;
			}
			else if (a_type == 140)
			{
				result = 5f;
			}
		}
		else if (Items.IsBody(a_type))
		{
			result = (1f - itemDef.healing) * 500f;
		}
		else if (Items.IsShoes(a_type))
		{
			result = itemDef.healing * 2000f;
		}
		else if (a_type == 30)
		{
			result = 200f;
		}
		return result;
	}

	public static float GetValue(int a_type, int a_amountOrCondition = 1)
	{
		float newValue = Items.GetNewValue(a_type);
		float num = (!Items.HasCondition(a_type)) ? ((float)a_amountOrCondition) : (0.3f + (float)a_amountOrCondition * 0.007f);
		return newValue * num;
	}

	public static int GetRandomType(float a_maxNewValue = 9999999f)
	{
		int num;
		do
		{
			num = UnityEngine.Random.Range(1, 254);
		}
		while (!Items.IsValid(num) || Items.IsContainer(num) || (a_maxNewValue != 9999999f && a_maxNewValue <= Items.GetNewValue(num)));
		return num;
	}

	public static bool IsValid(int a_type)
	{
		return null != Items.GetItemDef(a_type).ident;
	}

	public static int GetRandomFood()
	{
		int num;
		do
		{
			num = UnityEngine.Random.Range(1, 20);
		}
		while (!Items.IsValid(num));
		return num;
	}

	public static bool IsStackable(int a_type)
	{
		return (a_type > 0 && a_type < 20) || (a_type > 39 && a_type < 60) || (a_type > 129 && a_type < 150) || 254 == a_type;
	}

	public static bool IsBeverage(int a_type)
	{
		return a_type > 14 && a_type < 20;
	}

	public static bool IsEatable(int a_type)
	{
		return a_type > 0 && a_type < 20;
	}

	public static bool IsRareAmmo(int a_type)
	{
		return a_type > 39 && a_type < 45;
	}

	public static bool IsMedicine(int a_type)
	{
		return a_type > 139 && a_type < 150;
	}

	public static bool IsEatableForPet(int a_type)
	{
		return a_type == 4 || 5 == a_type;
	}

	public static bool IsCookable(int a_type)
	{
		return a_type == 2 || a_type == 4 || a_type == 6 || 11 == a_type;
	}

	public static bool IsBody(int a_type)
	{
		return a_type > 149 && a_type < 160;
	}

	public static bool IsShoes(int a_type)
	{
		return a_type > 169 && a_type < 180;
	}

	public static bool IsContainer(int a_type)
	{
		return a_type > 119 && a_type < 130;
	}

	public static bool IsResource(int a_type)
	{
		return a_type > 129 && a_type < 140;
	}

	public static bool IsCraftable(int a_type)
	{
		return (a_type > 19 && a_type < 30) || (a_type > 49 && a_type < 60) || (a_type > 74 && a_type < 90) || (a_type > 104 && a_type < 120) || (a_type > 150 && a_type < 154) || a_type == 140 || 171 == a_type;
	}

	public const int c_rawFishType = 11;

	public const int c_mutantclaw = 104;

	public const int c_hammerType = 92;

	public const int c_shovelType = 109;

	public const int c_fishingRodType = 110;

	public const int c_currencyType = 254;

	private static ItemDef[] m_itemDefs;
}

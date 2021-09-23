﻿using System;

public static class Buildings
{
	// Note: this type is marked as 'beforefieldinit'.
	static Buildings()
	{
	}

	private static void Init()
	{
		Buildings.m_buildingDefs = new BuildingDef[255];
		Buildings.m_buildingDefs[0] = new BuildingDef("INVALID", false, 0.0);
		Buildings.m_buildingDefs[1] = new BuildingDef("RES_WOOD", false, 0.0);
		Buildings.m_buildingDefs[2] = new BuildingDef("RES_METAL", false, 0.0);
		Buildings.m_buildingDefs[3] = new BuildingDef("RES_STONE", false, 0.0);
		Buildings.m_buildingDefs[10] = new BuildingDef("RESDES_TREE", false, 0.0);
		Buildings.m_buildingDefs[11] = new BuildingDef("RESDES_TREE2", false, 0.0);
		Buildings.m_buildingDefs[20] = new BuildingDef("DOOR_WOODEN", true, 345600.0);
		Buildings.m_buildingDefs[21] = new BuildingDef("DOOR_METAL", true, 1036800.0);
		Buildings.m_buildingDefs[40] = new BuildingDef("WOODWALL", true, 345600.0);
		Buildings.m_buildingDefs[41] = new BuildingDef("STONEWALL", true, 1036800.0);
		Buildings.m_buildingDefs[60] = new BuildingDef("PLANT_POTATO", false, 1800.0);
		Buildings.m_buildingDefs[61] = new BuildingDef("PLANT_BERRY", false, 1800.0);
		Buildings.m_buildingDefs[62] = new BuildingDef("PLANT_MUSHROOM", false, 0.0);
		Buildings.m_buildingDefs[100] = new BuildingDef("FIREPLACE", false, 28800.0);
		Buildings.m_buildingDefs[101] = new BuildingDef("BED", true, 1036800.0);
		Buildings.m_buildingDefs[102] = new BuildingDef("TNT", false, 5.0);
		Buildings.m_buildingDefs[103] = new BuildingDef("LOOTBOX", true, 1036800.0);
		Buildings.m_buildingDefs[104] = new BuildingDef("TESLACOIL", true, 1036800.0);
	}

	public static BuildingDef GetBuildingDef(int a_type)
	{
		if (Buildings.m_buildingDefs == null)
		{
			Buildings.Init();
		}
		if (a_type < 0 || a_type >= Buildings.m_buildingDefs.Length)
		{
			return default(BuildingDef);
		}
		return Buildings.m_buildingDefs[a_type];
	}

	public static bool IsResource(int a_type)
	{
		return a_type >= 0 && a_type < 20;
	}

	public static bool IsDoor(int a_type)
	{
		return a_type >= 20 && a_type < 40;
	}

	public static bool IsCollider(int a_type)
	{
		return (a_type >= 0 && a_type < 60) || a_type == 101 || a_type == 103 || 104 == a_type;
	}

	public static bool IsHarmless(int a_type)
	{
		return a_type > 59 && a_type < 101;
	}

	private const double c_hourInSec = 3600.0;

	private static BuildingDef[] m_buildingDefs;
}

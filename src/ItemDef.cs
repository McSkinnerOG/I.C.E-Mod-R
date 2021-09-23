using System;

public struct ItemDef
{
	public ItemDef(string a_ident, float a_healing = 0f, float a_damage = 5f, float a_attackdur = 1f, float a_range = 1.3f, float a_durability = 0f, int a_ammoItemIndex = 0, int a_wood = 0, int a_metal = 0, int a_stone = 0, int a_cloth = 0, int a_rankReq = 0, int a_buildingIndex = 0)
	{
		this.ident = a_ident;
		this.healing = a_healing;
		this.damage = a_damage;
		this.attackdur = a_attackdur;
		this.range = a_range;
		this.durability = a_durability;
		this.ammoItemType = a_ammoItemIndex;
		this.wood = a_wood;
		this.metal = a_metal;
		this.stone = a_stone;
		this.cloth = a_cloth;
		this.rankReq = a_rankReq;
		this.buildingIndex = a_buildingIndex;
	}

	public string ident;

	public float healing;

	public float damage;

	public float attackdur;

	public float range;

	public float durability;

	public int ammoItemType;

	public int wood;

	public int metal;

	public int stone;

	public int cloth;

	public int rankReq;

	public int buildingIndex;
}

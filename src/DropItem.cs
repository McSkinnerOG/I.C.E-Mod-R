using System;

[Serializable]
public class DropItem
{
	public DropItem(int a_typeFrom, int a_typeTo, int a_chance = 100, int a_min = 1, int a_max = 1)
	{
		this.typeFrom = a_typeFrom;
		this.typeTo = a_typeTo;
		this.chance = a_chance;
		this.min = a_min;
		this.max = a_max;
	}

	public int typeFrom;

	public int typeTo;

	public int chance;

	public int min;

	public int max;
}

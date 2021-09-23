using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ItemContainer
{
	public ItemContainer(int a_maxX, int a_maxY, int a_xOffset, int a_cid = 0, SQLThreadManager a_sql = null, ServerPlayer a_player = null)
	{
		this.m_maxX = a_maxX;
		this.m_maxY = a_maxY;
		this.m_xOffset = a_xOffset;
		this.m_cid = a_cid;
		this.m_sql = a_sql;
		this.m_player = a_player;
	}

	public bool IsValidPos(Vector3 a_pos)
	{
		return a_pos.x >= (float)this.m_xOffset && a_pos.x < (float)(this.m_maxX + this.m_xOffset) && a_pos.z >= 0f && a_pos.z < (float)this.m_maxY;
	}

	public bool CollectItem(DatabaseItem a_item, bool a_stackIfPossible, [Optional] Vector3 a_pos)
	{
		bool flag = false;
		int itemIndexFromPos = GetItemIndexFromPos(a_pos.x, a_pos.z);
		if (IsPlayerMoney(a_item.type) && m_player != null)
		{
			flag = true;
			m_player.m_gold += a_item.amount;
		}
		else
		{
			if (a_stackIfPossible && Items.IsStackable(a_item.type))
			{
				if (Vector3.zero == a_pos)
				{
					for (int i = 0; i < m_items.Count; i++)
					{
						if (CollectAndStackItem(a_item, i))
						{
							flag = true;
							break;
						}
					}
				}
				else if (CollectAndStackItem(a_item, itemIndexFromPos))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				if (IsValidPos(a_pos) && itemIndexFromPos == -1)
				{
					a_item.x = a_pos.x;
					a_item.y = a_pos.z;
					flag = true;
				}
				else
				{
					flag = FindFreeInventorySlot(ref a_item.x, ref a_item.y);
				}
				if (flag)
				{
					a_item.cid = m_cid;
					m_items.Add(a_item);
					if (null != m_sql)
					{
						SQLChange(a_item, eDbAction.insert);
					}
				}
			}
		}
		return flag;
	}

	public void UpdateOrCreateItem(DatabaseItem a_item)
	{
		int itemIndexFromPos = this.GetItemIndexFromPos(a_item.x, a_item.y);
		if (itemIndexFromPos == -1)
		{
			this.m_items.Add(a_item);
		}
		else
		{
			this.m_items[itemIndexFromPos] = a_item;
		}
	}

	public void SplitItem(Vector3 a_itemPos)
	{
		int itemIndexFromPos = this.GetItemIndexFromPos(a_itemPos.x, a_itemPos.z);
		if (itemIndexFromPos > -1 && Items.IsStackable(this.m_items[itemIndexFromPos].type) && 1 < this.m_items[itemIndexFromPos].amount && this.HasFreeSlots())
		{
			DatabaseItem databaseItem = this.m_items[itemIndexFromPos];
			DatabaseItem a_item = this.m_items[itemIndexFromPos];
			databaseItem.amount = this.m_items[itemIndexFromPos].amount / 2;
			this.m_items[itemIndexFromPos] = databaseItem;
			this.SQLChange(databaseItem, eDbAction.update);
			a_item.amount = a_item.amount / 2 + a_item.amount % 2;
			this.CollectItem(a_item, false, default(Vector3));
		}
	}

	public bool EatItem(Vector3 a_itemPos, ServerPlayer a_player)
	{
		bool result = true;
		int itemIndexFromPos = this.GetItemIndexFromPos(a_itemPos.x, a_itemPos.z);
		if (itemIndexFromPos > -1 && (Items.IsEatable(this.m_items[itemIndexFromPos].type) || Items.IsMedicine(this.m_items[itemIndexFromPos].type)))
		{
			a_player.ConsumeItem(this.m_items[itemIndexFromPos].type);
			result = (0 == this.DeclineItemAmount(itemIndexFromPos, 1));
		}
		return result;
	}

	public DatabaseItem DragDrop(Vector3 a_dragPos, Vector3 a_dropPos, ItemContainer a_otherContainer, Vector3 a_freeWorldDropPos)
	{
		DatabaseItem result = new DatabaseItem(0, 0f, 0f, 1, false, 0, 0);
		int itemIndexFromPos = this.GetItemIndexFromPos(a_dragPos.x, a_dragPos.z);
		if (itemIndexFromPos > -1 && itemIndexFromPos < this.m_items.Count)
		{
			if (!this.IsValidPos(a_dropPos))
			{
				bool flag = true;
				if (a_otherContainer != null && a_otherContainer.IsValidPos(a_dropPos))
				{
					flag = a_otherContainer.CollectItem(this.m_items[itemIndexFromPos], true, a_dropPos);
				}
				else
				{
					result = this.m_items[itemIndexFromPos];
					result.cid = 0;
					result.x = a_freeWorldDropPos.x;
					result.y = a_freeWorldDropPos.z;
					result.dropTime = Time.time;
					int num = (!(null == this.m_sql)) ? this.m_sql.CidToPid(this.m_cid) : 0;
					result.dropPlayerId = ((this.m_cid == num) ? 0 : num);
				}
				if (flag)
				{
					this.SQLChange(this.m_items[itemIndexFromPos], eDbAction.delete);
					this.m_items.RemoveAt(itemIndexFromPos);
				}
			}
			else
			{
				int itemIndexFromPos2 = this.GetItemIndexFromPos(a_dropPos.x, a_dropPos.z);
				if (itemIndexFromPos2 == -1)
				{
					DatabaseItem databaseItem = this.m_items[itemIndexFromPos];
					databaseItem.x = a_dropPos.x;
					databaseItem.y = a_dropPos.z;
					this.m_items[itemIndexFromPos] = databaseItem;
					this.SQLChange(databaseItem, eDbAction.update);
				}
				else if (Items.IsStackable(this.m_items[itemIndexFromPos].type) && this.m_items[itemIndexFromPos].type == this.m_items[itemIndexFromPos2].type)
				{
					int num2 = this.m_items[itemIndexFromPos].amount + this.m_items[itemIndexFromPos2].amount;
					if (num2 <= 254)
					{
						DatabaseItem a_item = this.m_items[itemIndexFromPos];
						this.SQLChange(a_item, eDbAction.delete);
						DatabaseItem databaseItem2 = this.m_items[itemIndexFromPos2];
						databaseItem2.amount = num2;
						this.m_items[itemIndexFromPos2] = databaseItem2;
						this.SQLChange(databaseItem2, eDbAction.update);
						this.m_items.RemoveAt(itemIndexFromPos);
					}
					else
					{
						DatabaseItem databaseItem3 = this.m_items[itemIndexFromPos];
						databaseItem3.amount = num2 - 254;
						this.m_items[itemIndexFromPos] = databaseItem3;
						this.SQLChange(databaseItem3, eDbAction.update);
						DatabaseItem databaseItem4 = this.m_items[itemIndexFromPos2];
						databaseItem4.amount = 254;
						this.m_items[itemIndexFromPos2] = databaseItem4;
						this.SQLChange(databaseItem4, eDbAction.update);
					}
				}
				else
				{
					DatabaseItem databaseItem5 = this.m_items[itemIndexFromPos];
					DatabaseItem databaseItem6 = this.m_items[itemIndexFromPos];
					databaseItem6.x = this.m_items[itemIndexFromPos2].x;
					databaseItem6.y = this.m_items[itemIndexFromPos2].y;
					this.m_items[itemIndexFromPos] = databaseItem6;
					this.SQLChange(databaseItem6, eDbAction.update);
					DatabaseItem databaseItem7 = this.m_items[itemIndexFromPos2];
					databaseItem7.x = databaseItem5.x;
					databaseItem7.y = databaseItem5.y;
					this.m_items[itemIndexFromPos2] = databaseItem7;
					this.SQLChange(databaseItem7, eDbAction.update);
				}
			}
		}
		return result;
	}

	public bool DeleteItem(float a_x, float a_y)
	{
		return this.DeleteItem(this.GetItemIndexFromPos(a_x, a_y));
	}

	public bool DeleteItem(int a_index)
	{
		if (a_index > -1 && a_index < this.m_items.Count)
		{
			this.SQLChange(this.m_items[a_index], eDbAction.delete);
			this.m_items.RemoveAt(a_index);
			return true;
		}
		return false;
	}

	public bool DeleteItems()
	{
		bool result = false;
		for (int i = 0; i < this.m_items.Count; i++)
		{
			this.SQLChange(this.m_items[i], eDbAction.delete);
			this.m_items.RemoveAt(i);
			result = true;
		}
		return result;
	}

	public bool HasFreeSlots()
	{
		return this.m_items == null || this.m_items.Count < this.m_maxX * this.m_maxY;
	}

	public int Count()
	{
		return (this.m_items == null) ? 0 : this.m_items.Count;
	}

	public int GetCid()
	{
		return this.m_cid;
	}

	public DatabaseItem GetItemFromPos(float a_x, float a_y)
	{
		int itemIndexFromPos = this.GetItemIndexFromPos(a_x, a_y);
		if (itemIndexFromPos != -1)
		{
			return this.m_items[itemIndexFromPos];
		}
		return new DatabaseItem(0, 0f, 0f, 1, false, 0, 0);
	}

	public int CraftItem(int a_type, int a_amount)
	{
		if (!Items.IsCraftable(a_type))
		{
			return 0;
		}
		int result = 0;
		ItemDef itemDef = Items.GetItemDef(a_type);
		bool flag = itemDef.durability > 0f && itemDef.durability < 1f;
		if ((itemDef.wood == 0 || this.GetItemAmountByType(130) >= itemDef.wood * a_amount) && (itemDef.metal == 0 || this.GetItemAmountByType(131) >= itemDef.metal * a_amount) && (itemDef.stone == 0 || this.GetItemAmountByType(132) >= itemDef.stone * a_amount) && (itemDef.cloth == 0 || this.GetItemAmountByType(133) >= itemDef.cloth * a_amount))
		{
			result = (itemDef.wood + itemDef.metal + itemDef.stone + itemDef.cloth) * a_amount;
			this.DeclineItemAmountByType(130, itemDef.wood * a_amount);
			this.DeclineItemAmountByType(131, itemDef.metal * a_amount);
			this.DeclineItemAmountByType(132, itemDef.stone * a_amount);
			this.DeclineItemAmountByType(133, itemDef.cloth * a_amount);
			DatabaseItem a_item = new DatabaseItem(a_type, 0f, 0f, 1, false, 0, 0);
			if (a_amount == 1 || Items.IsStackable(a_type))
			{
				a_item.amount = ((!flag) ? a_amount : 100);
				if (!this.CollectItem(a_item, true, default(Vector3)))
				{
				}
			}
			else
			{
				a_item.amount = ((!flag) ? 1 : 100);
				for (int i = 0; i < a_amount; i++)
				{
					this.CollectItem(a_item, true, default(Vector3));
				}
			}
		}
		return result;
	}

	public int GetItemAmountByType(int a_type)
	{
		int num = 0;
		if (this.IsPlayerMoney(a_type))
		{
			num = this.m_player.m_gold;
		}
		else
		{
			for (int i = 0; i < this.m_items.Count; i++)
			{
				if (a_type == this.m_items[i].type)
				{
					num += this.m_items[i].amount;
				}
			}
		}
		return num;
	}

	public bool DeclineHandItem()
	{
		return this.DeclineItem(0f, 0f);
	}

	public bool DeclineVestItem()
	{
		return this.DeclineItem(0f, 2f);
	}

	public bool DeclineItem(float a_x, float a_y)
	{
		int itemIndexFromPos = this.GetItemIndexFromPos(a_x, a_y);
		if (itemIndexFromPos > -1)
		{
			this.DeclineItemAmount(itemIndexFromPos, 1);
			return true;
		}
		return false;
	}

	public bool RepairHandItem()
	{
		return this.RepairItem(0f, 0f);
	}

	public bool RepairItem(float a_x, float a_y)
	{
		int itemIndexFromPos = this.GetItemIndexFromPos(a_x, a_y);
		return itemIndexFromPos > -1 && this.RepairItem(itemIndexFromPos);
	}

	public int DeclineItemAmountByType(int a_type, int a_amount)
	{
		if (1 > a_amount)
		{
			return 0;
		}
		int num = a_amount;
		if (this.IsPlayerMoney(a_type))
		{
			num = Mathf.Max(a_amount - this.m_player.m_gold, 0);
			this.m_player.m_gold = Mathf.Max(this.m_player.m_gold - a_amount, 0);
		}
		else
		{
			for (int i = 0; i < this.m_items.Count; i++)
			{
				if (a_type == this.m_items[i].type)
				{
					num = this.DeclineItemAmount(i, num);
					if (0 >= num)
					{
						break;
					}
					i--;
				}
			}
		}
		return num;
	}

	private bool IsPlayerMoney(int a_type)
	{
		return this.m_player != null && 254 == a_type;
	}

	private bool CollectAndStackItem(DatabaseItem a_itemToCollect, int a_invIndex)
	{
		if (a_invIndex >= 0 && a_invIndex < this.m_items.Count && a_itemToCollect.type == this.m_items[a_invIndex].type && a_itemToCollect.amount + this.m_items[a_invIndex].amount <= 254)
		{
			DatabaseItem value = this.m_items[a_invIndex];
			value.amount += a_itemToCollect.amount;
			this.m_items[a_invIndex] = value;
			if (null != this.m_sql)
			{
				this.SQLChange(this.m_items[a_invIndex], eDbAction.update);
			}
			return true;
		}
		return false;
	}

	private int DeclineItemAmount(int a_index, int a_amount)
	{
		int result = 0;
		DatabaseItem databaseItem = this.m_items[a_index];
		if (a_amount < databaseItem.amount)
		{
			databaseItem.amount -= a_amount;
			this.m_items[a_index] = databaseItem;
			this.SQLChange(databaseItem, eDbAction.update);
		}
		else
		{
			result = a_amount - databaseItem.amount;
			this.SQLChange(databaseItem, eDbAction.delete);
			this.m_items.RemoveAt(a_index);
		}
		return result;
	}

	private bool RepairItem(int a_index)
	{
		DatabaseItem databaseItem = this.m_items[a_index];
		if (Items.HasCondition(databaseItem.type))
		{
			databaseItem.amount = 100;
			this.m_items[a_index] = databaseItem;
			this.SQLChange(databaseItem, eDbAction.update);
			return true;
		}
		return false;
	}

	private int GetItemIndexFromPos(float a_x, float a_y)
	{
		a_x = Mathf.Round(a_x);
		a_y = Mathf.Round(a_y);
		for (int i = 0; i < this.m_items.Count; i++)
		{
			if (a_x == Mathf.Round(this.m_items[i].x) && a_y == Mathf.Round(this.m_items[i].y))
			{
				return i;
			}
		}
		return -1;
	}

	private void SQLChange(DatabaseItem a_item, eDbAction a_action)
	{
		if (null != this.m_sql && 0 < this.m_cid)
		{
			a_item.flag = a_action;
			this.m_sql.SaveItem(a_item);
		}
	}

	private bool FindFreeInventorySlot(ref float a_x, ref float a_y)
	{
		if (this.HasFreeSlots())
		{
			for (int i = 0; i < this.m_maxY; i++)
			{
				for (int j = this.m_xOffset; j < this.m_maxX + this.m_xOffset; j++)
				{
					bool flag = false;
					for (int k = 0; k < this.m_items.Count; k++)
					{
						if (j == (int)(this.m_items[k].x + 0.5f) && i == (int)(this.m_items[k].y + 0.5f))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						a_x = (float)j;
						a_y = (float)i;
						return true;
					}
				}
			}
		}
		return false;
	}

	public List<DatabaseItem> m_items = new List<DatabaseItem>();

	public Vector3 m_position = Vector3.zero;

	private int m_cid;

	private int m_maxX = 1;

	private int m_maxY = 1;

	private int m_xOffset;

	private SQLThreadManager m_sql;

	private ServerPlayer m_player;
}

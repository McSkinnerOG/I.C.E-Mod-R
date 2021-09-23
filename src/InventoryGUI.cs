using System;
using System.Collections;
using UnityEngine;

public class InventoryGUI : MonoBehaviour
{
	public InventoryGUI()
	{
	}

	private void ClearInventory(bool a_onlyContainer = false)
	{
		if (this.m_items != null)
		{
			for (int i = 0; i < this.m_items.Length; i++)
			{
				if (null != this.m_items[i] && (!a_onlyContainer || !this.m_items[i].m_isInventoryItem))
				{
					UnityEngine.Object.Destroy(this.m_items[i].gameObject);
				}
			}
		}
		if (!a_onlyContainer)
		{
			this.m_items = null;
		}
	}

	private void Start()
	{
		this.m_guimaster = (GUI3dMaster)UnityEngine.Object.FindObjectOfType(typeof(GUI3dMaster));
		this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
		this.m_quitGameGui = (QuitGameGUI)UnityEngine.Object.FindObjectOfType(typeof(QuitGameGUI));
	}

	private void Update()
	{
		if (Input.GetButtonDown("Exit") || Input.GetButtonDown("Communicator") || Input.GetButtonDown("Map") || Input.GetButtonDown("Help") || Input.GetButtonDown("Global Chat") || Input.GetButtonDown("Crafting"))
		{
			this.m_guiSteamInventory.SetActive(false);
			this.m_guiBig.SetActive(false);
			this.m_guiItemInfo.SetActive(false);
		}
		else if (Input.GetButtonDown("Inventory"))
		{
			this.m_guiSteamInventory.SetActive(false);
			this.m_guiBig.SetActive(!this.m_guiBig.activeSelf);
			this.m_guiItemInfo.SetActive(false);
		}
		if (this.m_hideInfoTime > 0f && Time.time > this.m_hideInfoTime)
		{
			this.m_guiItemInfo.SetActive(false);
			this.m_hideInfoTime = 0f;
		}
		if (this.IsShopActive() != this.m_guiShopInfo.activeSelf)
		{
			this.m_guiShopInfo.SetActive(this.IsShopActive());
		}
		bool flag = !(null != this.m_client) || this.m_client.IsTutorialActive();
		if (this.m_guiHintBuilding.activeSelf != flag)
		{
			this.m_guiHintBuilding.SetActive(flag);
		}
		if (null != this.m_client)
		{
			this.m_guiGold.text = this.m_client.GetGoldCount().ToString();
		}
		this.m_quitGameGui.m_openWithEsc = (!this.m_communicator.IsActive(true) && false == this.m_guiBig.activeSelf);
	}

	private void LateUpdate()
	{
		if (null != this.m_guimaster)
		{
			string clickedButtonName = this.m_guimaster.GetClickedButtonName();
			if (string.Empty != clickedButtonName)
			{
				if (null != this.m_btnOpen && this.m_btnOpen.name == clickedButtonName)
				{
					this.m_guiBig.SetActive(true);
				}
				else if ((null != this.m_btnClose && this.m_btnClose.name == clickedButtonName) || (null != this.m_btnCraft && this.m_btnCraft.name == clickedButtonName))
				{
					if (null != this.m_btnCraft && this.m_btnCraft.name == clickedButtonName)
					{
						this.m_communicator.OpenCrafting();
						this.m_guiBig.SetActive(false);
						this.m_guiSteamInventory.SetActive(false);
					}
					else if (this.m_guiSteamInventory.activeSelf)
					{
						this.m_guiSteamInventory.SetActive(false);
					}
					else
					{
						this.m_guiBig.SetActive(false);
					}
				}
				else if (null != this.m_btnSteamInventory && this.m_btnSteamInventory.name == clickedButtonName)
				{
					this.m_guiSteamInventory.SetActive(!this.m_guiSteamInventory.activeSelf);
				}
			}
		}
	}

	private void DisableShadows()
	{
		Renderer[] componentsInChildren = base.transform.GetComponentsInChildren<Renderer>(true);
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.castShadows = false;
			renderer.receiveShadows = false;
		}
	}

	private void UpdateResourceCounts()
	{
		this.m_resourceCounts.Clear();
		for (int i = 0; i < this.m_items.Length; i++)
		{
			if (null != this.m_items[i] && this.m_items[i].m_isInventoryItem && Items.IsResource(this.m_items[i].m_type))
			{
				if (this.m_resourceCounts.Contains(this.m_items[i].m_type))
				{
					this.m_resourceCounts[this.m_items[i].m_type] = (int)this.m_resourceCounts[this.m_items[i].m_type] + this.m_items[i].m_amountOrCond;
				}
				else
				{
					this.m_resourceCounts.Add(this.m_items[i].m_type, this.m_items[i].m_amountOrCond);
				}
			}
		}
	}

	private Vector3 ToInventoryPos(Transform a_t, Vector3 a_pos)
	{
		a_t.parent = this.m_guiBig.transform;
		float num = 0f;
		float num2 = (a_pos.x <= 0f) ? 0f : 0.15f;
		a_pos *= 0.3f;
		a_pos.x += num2;
		a_pos.y = a_pos.z * -1f + num;
		a_pos.z = 0f;
		return a_pos;
	}

	public void UpdateInventory(RemoteItem[] a_items)
	{
		if (a_items != null)
		{
			this.ClearInventory(false);
			if (a_items.Length != 1 || !(null == a_items[0]) || a_items[0].m_type != 0 || !this.m_items[0].m_isInventoryItem)
			{
				this.m_items = a_items;
				bool flag = false;
				for (int i = 0; i < this.m_items.Length; i++)
				{
					if (null != this.m_items[i])
					{
						if (this.m_items[i].m_isInventoryOrContainerItem)
						{
							flag |= (false == this.m_items[i].m_isInventoryItem);
							if (this.m_items[i].m_type == 0)
							{
								UnityEngine.Object.Destroy(this.m_items[i].gameObject);
								this.m_items[i] = null;
							}
							else if (!this.m_items[i].IsVisible())
							{
								if (this.IsShopActive() && this.m_items[i].m_type != 254)
								{
									bool flag2 = this.m_items[i].transform.localPosition.x < 6f;
									float num = (!flag2) ? this.GetShopBuyMultiplier() : this.GetShopSellMultiplier();
									int num2 = (int)(Items.GetValue(this.m_items[i].m_type, this.m_items[i].m_amountOrCond) * num + 0.5f);
									this.m_items[i].CreateLabel(this.m_items[i].m_labelPricePrefab, this.m_items[i].m_priceLabelOffset, num2.ToString() + "G");
								}
								if (this.m_items[i].transform.position.sqrMagnitude < 0.1f)
								{
									this.m_handItem = this.m_items[i];
								}
								this.m_items[i].transform.localPosition = this.ToInventoryPos(this.m_items[i].transform, this.m_items[i].transform.position);
								this.m_items[i].transform.localRotation = Quaternion.identity;
								this.m_items[i].SwitchVisibility();
							}
						}
						else
						{
							UnityEngine.Object.Destroy(this.m_items[i].gameObject);
							this.m_items[i] = null;
						}
					}
				}
				if (flag != this.m_guiContainer.activeSelf)
				{
					if (!this.m_guiContainer.activeSelf && flag)
					{
						this.m_guiBig.SetActive(true);
					}
					this.m_guiContainer.SetActive(flag);
				}
			}
			else
			{
				UnityEngine.Object.Destroy(a_items[0].gameObject);
			}
			this.DisableShadows();
			this.UpdateResourceCounts();
		}
	}

	public RemoteItem GetHandItem()
	{
		return this.m_handItem;
	}

	public RemoteItem GetItemFromPos(float a_x, float a_y)
	{
		if (this.m_items != null)
		{
			a_x = Mathf.Round(a_x);
			a_y = Mathf.Round(a_y);
			Vector3 vector = Vector3.zero;
			for (int i = 0; i < this.m_items.Length; i++)
			{
				if (null != this.m_items[i])
				{
					vector = this.ToWorldPos(this.m_items[i].transform.localPosition);
					if (a_x == Mathf.Round(vector.x) && a_y == Mathf.Round(vector.z))
					{
						return this.m_items[i];
					}
				}
			}
		}
		return null;
	}

	public void SetShop(float a_buy, float a_sell)
	{
		this.m_shopBuyMultiplier = a_buy;
		this.m_shopSellMultiplier = a_sell;
	}

	public bool IsShopActive()
	{
		return this.m_shopBuyMultiplier != -1f || -1f != this.m_shopSellMultiplier;
	}

	public float GetShopBuyMultiplier()
	{
		return this.m_shopBuyMultiplier;
	}

	public float GetShopSellMultiplier()
	{
		return this.m_shopSellMultiplier;
	}

	public Vector3 ToWorldPos(Vector3 a_pos)
	{
		if (a_pos.x > 0f)
		{
			a_pos.x -= 0.15f;
		}
		a_pos *= 3.3333333f;
		a_pos.x = Mathf.Round(a_pos.x);
		a_pos.z = Mathf.Round(a_pos.y * -1f);
		a_pos.y = 0f;
		return a_pos;
	}

	public int GetInventoryItemCount()
	{
		int num = 0;
		if (this.m_items != null)
		{
			for (int i = 0; i < this.m_items.Length; i++)
			{
				if (null != this.m_items[i] && this.m_items[i].m_isInventoryItem)
				{
					num++;
				}
			}
		}
		return num;
	}

	public bool DragDrop(ref Vector3 a_startPos, ref Vector3 a_endPos)
	{
		if (this.m_items == null)
		{
			return false;
		}
		a_startPos = this.ToWorldPos(a_startPos);
		a_endPos = this.ToWorldPos(a_endPos);
		return true;
	}

	public void ShowInfo(Vector3 a_pos)
	{
		if (Vector3.zero == a_pos)
		{
			return;
		}
		this.m_hideInfoTime = Time.time + 8f;
		int num = -1;
		int a_amount = 0;
		if (this.m_items != null)
		{
			for (int i = 0; i < this.m_items.Length; i++)
			{
				if (null != this.m_items[i] && a_pos == this.m_items[i].transform.position)
				{
					num = this.m_items[i].m_type;
					a_amount = this.m_items[i].m_amountOrCond;
					break;
				}
			}
		}
		if (num != -1)
		{
			ItemDef itemDef = Items.GetItemDef(num);
			this.m_guiInfoName.text = LNG.Get(itemDef.ident);
			this.m_guiInfoDesc.text = LNG.Get(itemDef.ident + "_DESC");
			this.m_guiInfoStats.text = Items.GetStatsText(num, a_amount, true);
			this.m_guiItemInfo.SetActive(true);
		}
		else
		{
			this.m_guiItemInfo.SetActive(false);
		}
	}

	public int GetResourceCount(int a_resType)
	{
		return (!this.m_resourceCounts.Contains(a_resType)) ? 0 : ((int)this.m_resourceCounts[a_resType]);
	}

	public int GetFreeSlots()
	{
		int num = 20;
		return num - this.GetInventoryItemCount();
	}

	public bool HasItemType(int a_type)
	{
		if (this.m_items != null)
		{
			for (int i = 0; i < this.m_items.Length; i++)
			{
				if (null != this.m_items[i] && a_type == this.m_items[i].m_type)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool HasFood()
	{
		if (this.m_items != null)
		{
			for (int i = 0; i < this.m_items.Length; i++)
			{
				if (null != this.m_items[i] && this.m_items[i].m_isInventoryItem && Items.IsEatable(this.m_items[i].m_type) && Items.GetItemDef(this.m_items[i].m_type).healing > 0f)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool HasBuilding()
	{
		if (this.m_items != null)
		{
			for (int i = 0; i < this.m_items.Length; i++)
			{
				if (null != this.m_items[i] && this.m_items[i].m_isInventoryItem && Items.GetItemDef(this.m_items[i].m_type).buildingIndex > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsVisible()
	{
		return this.m_guiBig.activeSelf;
	}

	public void OpenSteamInventory()
	{
		this.m_guiBig.SetActive(true);
		this.m_guiSteamInventory.SetActive(true);
	}

	public GameObject m_btnOpen;

	public GameObject m_btnClose;

	public GameObject m_btnCraft;

	public GameObject m_btnSteamInventory;

	public GameObject m_guiSteamInventory;

	public GameObject m_guiBig;

	public GameObject m_guiContainer;

	public QmunicatorGUI m_communicator;

	public GameObject m_guiShopInfo;

	public GameObject m_guiItemInfo;

	public GameObject m_guiHintBuilding;

	public TextMesh m_guiInfoName;

	public TextMesh m_guiInfoDesc;

	public TextMesh m_guiInfoStats;

	public TextMesh m_guiGold;

	private float m_shopBuyMultiplier = -1f;

	private float m_shopSellMultiplier = -1f;

	private LidClient m_client;

	private GUI3dMaster m_guimaster;

	private RemoteItem[] m_items;

	private QuitGameGUI m_quitGameGui;

	private RemoteItem m_handItem;

	private float m_hideInfoTime;

	private Hashtable m_resourceCounts = new Hashtable();
}

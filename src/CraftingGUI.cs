using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingGUI : MonoBehaviour
{
	public CraftingGUI()
	{
	}

	private void Start()
	{
		this.m_guimaster = (GUI3dMaster)UnityEngine.Object.FindObjectOfType(typeof(GUI3dMaster));
		this.m_inventory = (InventoryGUI)UnityEngine.Object.FindObjectOfType(typeof(InventoryGUI));
		this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
		this.m_craftableItems = new List<ItemDef>[this.m_btnPages.Length];
		this.m_craftableItemTypes = new List<int>[this.m_btnPages.Length];
		this.m_itemList = new string[this.m_btnPages.Length];
		for (int i = 0; i < this.m_btnPages.Length; i++)
		{
			this.m_craftableItems[i] = new List<ItemDef>();
			this.m_craftableItemTypes[i] = new List<int>();
			this.m_itemList[i] = string.Empty;
		}
		this.m_txtItems.text = string.Empty;
		for (int j = 0; j < 254; j++)
		{
			ItemDef itemDef = Items.GetItemDef(j);
			if (Items.IsCraftable(j) && itemDef.ident != null)
			{
				this.m_craftableItems[itemDef.rankReq].Add(itemDef);
				this.m_craftableItemTypes[itemDef.rankReq].Add(j);
				string[] itemList = this.m_itemList;
				int rankReq = itemDef.rankReq;
				itemList[rankReq] = itemList[rankReq] + LNG.Get(itemDef.ident) + "\n";
			}
		}
		this.ActivatePage(0);
	}

	private void LateUpdate()
	{
		if (null != this.m_guimaster)
		{
			string clickedButtonName = this.m_guimaster.GetClickedButtonName();
			if (string.Empty != clickedButtonName)
			{
				if (null != this.m_btnCraft && this.m_btnCraft.name == clickedButtonName)
				{
					int num = this.m_craftableItemTypes[this.m_activePage][this.m_selectedItem];
					int num2 = (!Items.IsStackable(num)) ? this.m_amount : 1;
					bool flag = this.UpdateNeedHave();
					if (flag)
					{
						if (this.m_inventory.GetFreeSlots() >= num2)
						{
							this.m_client.SendCraftRequest(num, this.m_amount);
							base.audio.clip = this.m_buildClip;
							base.audio.Play();
						}
						else
						{
							Debug.Log(string.Concat(new object[]
							{
								"too less space in inventory ",
								this.m_inventory.GetFreeSlots(),
								" < ",
								num2
							}));
							base.audio.clip = this.m_failClip;
							base.audio.Play();
						}
					}
					else
					{
						base.audio.clip = this.m_failClip;
						base.audio.Play();
					}
				}
				else if (null != this.m_btnMore && this.m_btnMore.name == clickedButtonName)
				{
					this.SetCraftAmount(this.m_amount + 1);
				}
				else if (null != this.m_btnLess && this.m_btnLess.name == clickedButtonName)
				{
					this.SetCraftAmount(this.m_amount - 1);
				}
				else if (null != this.m_btnItems && this.m_btnItems.name == clickedButtonName)
				{
					float num3 = (Input.mousePosition.y / (float)Screen.height - this.m_minClickHeight) / (this.m_maxClickHeight - this.m_minClickHeight);
					int a_index = (int)((1f - num3) * 8f);
					this.ChooseItem(a_index);
				}
				else
				{
					for (int i = 0; i < this.m_btnPages.Length; i++)
					{
						if (null != this.m_btnPages[i] && this.m_btnPages[i].name == clickedButtonName)
						{
							this.ActivatePage(i);
						}
					}
				}
			}
		}
	}

	private void ActivatePage(int a_index)
	{
		this.m_activePage = a_index;
		this.m_txtItems.text = this.m_itemList[this.m_activePage];
		this.ChooseItem(0);
	}

	private void FixedUpdate()
	{
		this.UpdateNeedHave();
	}

	private void ChooseItem(int a_index)
	{
		if (a_index < 0 || this.m_craftableItems[this.m_activePage].Count <= a_index)
		{
			return;
		}
		this.m_selectedItem = a_index;
		int num = this.m_craftableItemTypes[this.m_activePage][this.m_selectedItem];
		this.m_txtItem.text = LNG.Get(this.m_craftableItems[this.m_activePage][this.m_selectedItem].ident);
		this.SetCraftAmount(1);
		if (null != this.m_itemForDisplay)
		{
			UnityEngine.Object.Destroy(this.m_itemForDisplay);
		}
		GameObject gameObject = (GameObject)Resources.Load("items/item_" + num);
		if (null != gameObject)
		{
			this.m_itemForDisplay = (GameObject)UnityEngine.Object.Instantiate(gameObject, this.m_itemDisplayParent.position, Quaternion.identity);
			this.m_itemForDisplay.transform.parent = this.m_itemDisplayParent;
			this.m_itemForDisplay.transform.localScale = Vector3.one * 2f;
			this.m_itemForDisplay.transform.localRotation = Quaternion.identity;
			Renderer[] componentsInChildren = this.m_itemForDisplay.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				renderer.gameObject.layer = 17;
			}
			Transform transform = this.m_itemForDisplay.transform.FindChild("Particles");
			if (null != transform)
			{
				transform.gameObject.SetActive(false);
			}
			Transform transform2 = this.m_itemForDisplay.transform.FindChild("Point light");
			if (null != transform2)
			{
				transform2.gameObject.SetActive(false);
			}
			this.m_txtInfoStats.text = Items.GetStatsText(num, -1, false);
			ItemDef itemDef = Items.GetItemDef(num);
			if (itemDef.ident != null && itemDef.ident.Length > 0 && string.Empty == this.m_txtInfoStats.text)
			{
				this.m_txtInfoStats.text = LNG.Get(itemDef.ident + "_DESC");
			}
		}
		this.UpdateNeedHave();
	}

	private bool UpdateNeedHave()
	{
		if (this.m_selectedItem < 0 || this.m_craftableItems[this.m_activePage].Count <= this.m_selectedItem)
		{
			return false;
		}
		this.m_txtRes.text = string.Empty;
		this.m_txtResNeed.text = string.Empty;
		this.m_txtResHaveEnough.text = string.Empty;
		this.m_txtResHaveTooLess.text = string.Empty;
		if (0 < this.m_craftableItems[this.m_activePage][this.m_selectedItem].wood)
		{
			TextMesh txtRes = this.m_txtRes;
			txtRes.text = txtRes.text + LNG.Get(Items.GetItemDef(130).ident) + "\n";
			int resourceCount = this.m_inventory.GetResourceCount(130);
			int num = this.m_craftableItems[this.m_activePage][this.m_selectedItem].wood * this.m_amount;
			TextMesh txtResNeed = this.m_txtResNeed;
			txtResNeed.text = txtResNeed.text + num + "\n";
			if (num > resourceCount)
			{
				TextMesh txtResHaveTooLess = this.m_txtResHaveTooLess;
				txtResHaveTooLess.text += resourceCount;
			}
			else
			{
				TextMesh txtResHaveEnough = this.m_txtResHaveEnough;
				txtResHaveEnough.text += resourceCount;
			}
			TextMesh txtResHaveTooLess2 = this.m_txtResHaveTooLess;
			txtResHaveTooLess2.text += "\n";
			TextMesh txtResHaveEnough2 = this.m_txtResHaveEnough;
			txtResHaveEnough2.text += "\n";
		}
		if (0 < this.m_craftableItems[this.m_activePage][this.m_selectedItem].metal)
		{
			TextMesh txtRes2 = this.m_txtRes;
			txtRes2.text = txtRes2.text + LNG.Get(Items.GetItemDef(131).ident) + "\n";
			int resourceCount = this.m_inventory.GetResourceCount(131);
			int num = this.m_craftableItems[this.m_activePage][this.m_selectedItem].metal * this.m_amount;
			TextMesh txtResNeed2 = this.m_txtResNeed;
			txtResNeed2.text = txtResNeed2.text + num + "\n";
			if (num > resourceCount)
			{
				TextMesh txtResHaveTooLess3 = this.m_txtResHaveTooLess;
				txtResHaveTooLess3.text += resourceCount;
			}
			else
			{
				TextMesh txtResHaveEnough3 = this.m_txtResHaveEnough;
				txtResHaveEnough3.text += resourceCount;
			}
			TextMesh txtResHaveTooLess4 = this.m_txtResHaveTooLess;
			txtResHaveTooLess4.text += "\n";
			TextMesh txtResHaveEnough4 = this.m_txtResHaveEnough;
			txtResHaveEnough4.text += "\n";
		}
		if (0 < this.m_craftableItems[this.m_activePage][this.m_selectedItem].stone)
		{
			TextMesh txtRes3 = this.m_txtRes;
			txtRes3.text = txtRes3.text + LNG.Get(Items.GetItemDef(132).ident) + "\n";
			int resourceCount = this.m_inventory.GetResourceCount(132);
			int num = this.m_craftableItems[this.m_activePage][this.m_selectedItem].stone * this.m_amount;
			TextMesh txtResNeed3 = this.m_txtResNeed;
			txtResNeed3.text = txtResNeed3.text + num + "\n";
			if (num > resourceCount)
			{
				TextMesh txtResHaveTooLess5 = this.m_txtResHaveTooLess;
				txtResHaveTooLess5.text += resourceCount;
			}
			else
			{
				TextMesh txtResHaveEnough5 = this.m_txtResHaveEnough;
				txtResHaveEnough5.text += resourceCount;
			}
			TextMesh txtResHaveTooLess6 = this.m_txtResHaveTooLess;
			txtResHaveTooLess6.text += "\n";
			TextMesh txtResHaveEnough6 = this.m_txtResHaveEnough;
			txtResHaveEnough6.text += "\n";
		}
		if (0 < this.m_craftableItems[this.m_activePage][this.m_selectedItem].cloth)
		{
			TextMesh txtRes4 = this.m_txtRes;
			txtRes4.text = txtRes4.text + LNG.Get(Items.GetItemDef(133).ident) + "\n";
			int resourceCount = this.m_inventory.GetResourceCount(133);
			int num = this.m_craftableItems[this.m_activePage][this.m_selectedItem].cloth * this.m_amount;
			TextMesh txtResNeed4 = this.m_txtResNeed;
			txtResNeed4.text = txtResNeed4.text + num + "\n";
			if (num > resourceCount)
			{
				TextMesh txtResHaveTooLess7 = this.m_txtResHaveTooLess;
				txtResHaveTooLess7.text += resourceCount;
			}
			else
			{
				TextMesh txtResHaveEnough7 = this.m_txtResHaveEnough;
				txtResHaveEnough7.text += resourceCount;
			}
			TextMesh txtResHaveTooLess8 = this.m_txtResHaveTooLess;
			txtResHaveTooLess8.text += "\n";
			TextMesh txtResHaveEnough8 = this.m_txtResHaveEnough;
			txtResHaveEnough8.text += "\n";
		}
		bool flag = this.m_inventory.GetResourceCount(130) >= this.m_craftableItems[this.m_activePage][this.m_selectedItem].wood * this.m_amount && this.m_inventory.GetResourceCount(131) >= this.m_craftableItems[this.m_activePage][this.m_selectedItem].metal * this.m_amount && this.m_inventory.GetResourceCount(132) >= this.m_craftableItems[this.m_activePage][this.m_selectedItem].stone * this.m_amount && this.m_inventory.GetResourceCount(133) >= this.m_craftableItems[this.m_activePage][this.m_selectedItem].cloth * this.m_amount;
		if (null != this.m_client && this.m_client.GetRank() < this.m_craftableItems[this.m_activePage][this.m_selectedItem].rankReq)
		{
			this.m_txtCraft.text = LNG.Get("TOO_LOW_RANK");
			flag = false;
		}
		else
		{
			this.m_txtCraft.text = ((!flag) ? LNG.Get("TOO_LESS_RES") : LNG.Get("CRAFT"));
		}
		this.m_txtCraft.characterSize = ((!flag) ? 0.012f : 0.018f);
		return flag;
	}

	private void SetCraftAmount(int a_amount)
	{
		this.m_amount = Mathf.Clamp(a_amount, 1, 99);
		this.m_txtAmount.text = this.m_amount.ToString();
		if (this.m_txtAmount.text.Length == 1)
		{
			this.m_txtAmount.text = "0" + this.m_txtAmount.text;
		}
		this.UpdateNeedHave();
	}

	public TextMesh m_txtItems;

	public TextMesh m_txtItem;

	public TextMesh m_txtRes;

	public TextMesh m_txtResNeed;

	public TextMesh m_txtResHaveEnough;

	public TextMesh m_txtResHaveTooLess;

	public TextMesh m_txtAmount;

	public TextMesh m_txtTimeLeft;

	public TextMesh m_txtCraft;

	public TextMesh m_txtInfoStats;

	public GameObject m_btnItems;

	public GameObject m_btnCraft;

	public GameObject m_btnMore;

	public GameObject m_btnLess;

	public GameObject[] m_btnPages;

	public Transform m_itemDisplayParent;

	public float m_minClickHeight = 0.21f;

	public float m_maxClickHeight = 0.75f;

	public AudioClip m_buildClip;

	public AudioClip m_failClip;

	private GameObject m_itemForDisplay;

	private GUI3dMaster m_guimaster;

	private InventoryGUI m_inventory;

	private LidClient m_client;

	private int m_activePage;

	private List<ItemDef>[] m_craftableItems;

	private List<int>[] m_craftableItemTypes;

	private string[] m_itemList;

	private int m_amount = 1;

	private int m_selectedItem;
}

using System;
using System.Collections.Generic;
using SimpleJSON;
using Steamworks;
using UnityEngine;

public class SteamInventoryGUI : MonoBehaviour
{
	public SteamInventoryGUI()
	{
	}

	private void Start()
	{
		this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
		this.m_guimaster = (GUI3dMaster)UnityEngine.Object.FindObjectOfType(typeof(GUI3dMaster));
		this.m_caseOpenGui = (CaseOpeningGUI)UnityEngine.Object.FindObjectOfType(typeof(CaseOpeningGUI));
		this.m_steamInventoryHandler = (SteamInventoryHandler)UnityEngine.Object.FindObjectOfType(typeof(SteamInventoryHandler));
		if (Global.isSteamActive)
		{
			this.m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(new Callback<GameOverlayActivated_t>.DispatchDelegate(this.OnGameOverlayActivated));
		}
		this.RequestSteamInventory();
	}

	private void OnEnable()
	{
		this.m_contextMenu.SetActive(false);
		this.RequestSteamInventory();
	}

	private void OnGameOverlayActivated(GameOverlayActivated_t a_callback)
	{
		if (a_callback.m_bActive == 0)
		{
			this.RequestSteamInventory();
		}
	}

	private void RequestSteamInventory()
	{
		if (Global.isSteamActive && Time.time > this.m_nextPossibleRequestTime)
		{
			this.m_waitForResult = SteamInventory.GetAllItems(out this.m_resultHandle);
			this.m_completeRefresh = true;
			this.m_nextPossibleRequestTime = Time.time + 10f;
		}
	}

	private void Update()
	{
		this.UpdateInventory();
		if (!this.m_waitForResult && !this.m_caseOpenGui.InProgress() && this.m_caseOpenFlag)
		{
			this.UpdateInventoryDisplay();
			this.m_caseOpenFlag = false;
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			if (Global.isSteamActive)
			{
				if ("vidiludi" == this.m_client.GetPlayerName() || "Ethan" == this.m_client.GetPlayerName() || "Editor" == this.m_client.GetPlayerName())
				{
					SteamItemDef_t[] array = new SteamItemDef_t[4];
					uint[] array2 = new uint[4];
					array[0].m_SteamItemDef = 2004;
					array[1].m_SteamItemDef = 2004;
					array[2].m_SteamItemDef = 3000;
					array[3].m_SteamItemDef = 3000;
					array2[0] = 1U;
					array2[1] = 1U;
					array2[2] = 1U;
					array2[3] = 1U;
					this.m_waitForResult = SteamInventory.GenerateItems(out this.m_resultHandle, array, null, 4U);
					this.m_completeRefresh = false;
				}
			}
			else
			{
				SteamItemDetails_t item = default(SteamItemDetails_t);
				item.m_iDefinition.m_SteamItemDef = 2004;
				item.m_unQuantity = 1;
				this.m_itemDetails.Add(item);
				for (int i = 0; i < 10; i++)
				{
					item.m_iDefinition.m_SteamItemDef = 20000 + i;
					item.m_unQuantity = 1;
					this.m_itemDetails.Add(item);
				}
				item.m_iDefinition.m_SteamItemDef = 2004;
				item.m_unQuantity = 1;
				this.m_itemDetails.Add(item);
				this.UpdateInventoryDisplay();
			}
		}
	}

	private void UpdateInventory()
	{
		if (Global.isSteamActive && this.m_waitForResult)
		{
			EResult resultStatus = SteamInventory.GetResultStatus(this.m_resultHandle);
			if (resultStatus != EResult.k_EResultPending)
			{
				if (resultStatus == EResult.k_EResultOK)
				{
					uint num = 0U;
					if (SteamInventory.GetResultItems(this.m_resultHandle, null, ref num))
					{
						SteamItemDetails_t[] array = new SteamItemDetails_t[num];
						int num2 = 0;
						if (this.m_completeRefresh)
						{
							this.m_itemDetails.Clear();
						}
						if (num > 0U)
						{
							SteamInventory.GetResultItems(this.m_resultHandle, array, ref num);
							if (this.m_completeRefresh)
							{
								this.m_itemDetails.AddRange(array);
							}
							else
							{
								for (int i = 0; i < array.Length; i++)
								{
									for (int j = 0; j < this.m_itemDetails.Count; j++)
									{
										if (array[i].m_itemId.m_SteamItemInstanceID == this.m_itemDetails[j].m_itemId.m_SteamItemInstanceID && 0 < (array[i].m_unFlags & 256))
										{
											this.m_itemDetails.RemoveAt(j);
											break;
										}
									}
								}
								for (int k = 0; k < array.Length; k++)
								{
									if ((array[k].m_unFlags & 256) == 0)
									{
										this.m_itemDetails.Add(array[k]);
										num2 = array[k].m_iDefinition.m_SteamItemDef;
									}
								}
							}
							int lookItemDef = this.m_steamInventoryHandler.GetLookItemDef();
							int skinItemDef = this.m_steamInventoryHandler.GetSkinItemDef();
							bool flag = true;
							bool flag2 = true;
							int num3 = 0;
							for (int l = 0; l < this.m_itemDetails.Count; l++)
							{
								if (this.m_itemDetails[l].m_iDefinition.m_SteamItemDef == lookItemDef)
								{
									flag = false;
								}
								else if (this.m_itemDetails[l].m_iDefinition.m_SteamItemDef == skinItemDef)
								{
									flag2 = false;
								}
								if (3000 > this.m_itemDetails[l].m_iDefinition.m_SteamItemDef)
								{
									num3++;
								}
							}
							if (flag2 || flag)
							{
								this.m_steamInventoryHandler.SetLook((!flag) ? lookItemDef : 0, (!flag2) ? skinItemDef : 0);
							}
							this.m_txtCaseCount.transform.parent.gameObject.SetActive(0 < num3);
							this.m_txtCaseCount.text = num3.ToString();
						}
						if (this.m_caseOpenFlag && num2 != 0)
						{
							this.m_caseOpenGui.Showtime(num2, this.m_generatorDefId);
						}
						else
						{
							this.UpdateInventoryDisplay();
						}
					}
				}
				else
				{
					Debug.Log("SteamInventoryGUI.cs: Couldn't get inventory: " + resultStatus.ToString());
				}
				SteamInventory.DestroyResult(this.m_resultHandle);
				this.m_waitForResult = false;
			}
		}
	}

	private void UpdateInventoryDisplay()
	{
		if (this.m_items != null)
		{
			for (int i = 0; i < this.m_items.Length; i++)
			{
				UnityEngine.Object.Destroy(this.m_items[i]);
			}
			this.m_items = null;
		}
		int num = this.m_curPage * 16;
		if (this.m_itemDetails != null && num < this.m_itemDetails.Count)
		{
			int num2 = num + 15;
			this.m_items = new GameObject[Mathf.Min(this.m_itemDetails.Count - num, 16)];
			for (int j = 0; j < this.m_itemDetails.Count; j++)
			{
				if (j >= num && j <= num2)
				{
					int num3 = j - num;
					JSONNode item = JsonItems.GetItem(this.m_itemDetails[j].m_iDefinition.m_SteamItemDef);
					if (null != item)
					{
						this.m_items[num3] = (GameObject)UnityEngine.Object.Instantiate(this.m_itemPrefab);
						this.m_items[num3].transform.parent = base.transform;
						this.m_items[num3].transform.localPosition = new Vector3(-0.01f + (float)(num3 % 4) * 0.278f, (float)(num3 / 4) * -0.278f, -0.01f);
						this.m_items[num3].transform.localRotation = Quaternion.identity;
						TextMesh componentInChildren = this.m_items[num3].GetComponentInChildren<TextMesh>();
						componentInChildren.text = string.Concat(new string[]
						{
							"<color=#",
							item["name_color"],
							">",
							item["market_name"],
							"</color>"
						});
						MeshCollider componentInChildren2 = this.m_items[num3].GetComponentInChildren<MeshCollider>();
						componentInChildren2.renderer.material.mainTexture = Resources.Load<Texture>("inventory_steam/inventory_s_" + this.m_itemDetails[j].m_iDefinition.m_SteamItemDef);
						componentInChildren2.transform.name = "sii-" + j.ToString();
					}
				}
			}
		}
		this.m_txtPage.text = string.Empty;
		int num4 = (this.m_itemDetails.Count - 1) / 16 + 1;
		if (1 < num4)
		{
			for (int k = 1; k < num4 + 1; k++)
			{
				string str = k.ToString() + " ";
				if (this.m_curPage + 1 == k)
				{
					str = "<color=\"#ffffff\">" + str + "</color>";
				}
				if (k < 10)
				{
					str = " " + str;
				}
				TextMesh txtPage = this.m_txtPage;
				txtPage.text += str;
			}
		}
	}

	private void LateUpdate()
	{
		if (null != this.m_guimaster)
		{
			string text = this.m_guimaster.GetClickedButtonName();
			if (string.Empty == text)
			{
				text = this.m_guimaster.GetRightClickedButtonName();
			}
			if (string.Empty != text)
			{
				this.m_contextMenu.SetActive(false);
				if (null != this.m_btnEquipOpen && text == this.m_btnEquipOpen.name)
				{
					if (this.EquipOrOpenItem(this.m_contextMenuItemIndex))
					{
						this.m_contextMenuItemIndex = -1;
					}
				}
				else if (null != this.m_btnSell && text == this.m_btnSell.name)
				{
					if (this.SellItem(this.m_contextMenuItemIndex))
					{
						this.m_contextMenuItemIndex = -1;
					}
				}
				else if (null != this.m_btnOpenCM && text == this.m_btnOpenCM.name)
				{
					this.OpenCommunityMarket();
				}
				else if (null != this.m_btnBuyKey && text == this.m_btnBuyKey.name)
				{
					this.BuyKey(1);
				}
				else if (null != this.m_btnBuyKeys && text == this.m_btnBuyKeys.name)
				{
					this.BuyKey(5);
				}
				else if (text.StartsWith("btn_page"))
				{
					for (int i = 0; i < 14; i++)
					{
						if ("btn_page" + (i + 1) == text)
						{
							this.m_curPage = i;
							this.UpdateInventoryDisplay();
							break;
						}
					}
				}
				else
				{
					this.m_contextMenuItemIndex = this.GetClickedItemIndex(text);
				}
			}
		}
	}

	private bool EquipOrOpenItem(int a_index)
	{
		bool result = false;
		if (this.m_itemDetails != null && a_index > -1 && a_index < this.m_itemDetails.Count)
		{
			int steamItemDef = this.m_itemDetails[a_index].m_iDefinition.m_SteamItemDef;
			ulong steamItemInstanceID = this.m_itemDetails[a_index].m_itemId.m_SteamItemInstanceID;
			if (10000 > steamItemDef)
			{
				if (Global.isSteamActive)
				{
					SteamItemDetails_t steamItemDetails_t = (steamItemDef != 3000) ? this.GetRandomItemFromInventory(3000, 3000) : this.GetRandomItemFromInventory(2000, 2999);
					ulong steamItemInstanceID2 = steamItemDetails_t.m_itemId.m_SteamItemInstanceID;
					if (steamItemInstanceID2 != 0UL)
					{
						this.m_generatorDefId = ((steamItemDef != 3000) ? steamItemDef : steamItemDetails_t.m_iDefinition.m_SteamItemDef) - 1000;
						SteamItemInstanceID_t[] array = new SteamItemInstanceID_t[2];
						uint[] array2 = new uint[2];
						array[0].m_SteamItemInstanceID = steamItemInstanceID;
						array[1].m_SteamItemInstanceID = steamItemInstanceID2;
						array2[0] = 1U;
						array2[1] = 1U;
						SteamItemDef_t[] array3 = new SteamItemDef_t[1];
						uint[] array4 = new uint[1];
						array3[0].m_SteamItemDef = this.m_generatorDefId;
						array4[0] = 1U;
						this.m_waitForResult = SteamInventory.ExchangeItems(out this.m_resultHandle, array3, array4, 1U, array, array2, 2U);
						this.m_completeRefresh = false;
						this.m_caseOpenFlag = this.m_waitForResult;
					}
					else if (steamItemDef == 3000)
					{
						this.OpenCommunityMarket();
					}
					else
					{
						this.BuyKey(1);
					}
				}
				else
				{
					this.m_caseOpenGui.Showtime(steamItemDef, ((steamItemDef != 3000) ? steamItemDef : 2000) - 1000);
				}
			}
			else if (null != this.m_client)
			{
				int num = this.m_steamInventoryHandler.GetLookItemDef();
				int num2 = this.m_steamInventoryHandler.GetSkinItemDef();
				if (20000 > steamItemDef)
				{
					if (num == steamItemDef)
					{
						num = 0;
					}
					else
					{
						num = steamItemDef;
					}
				}
				else if (num2 == steamItemDef)
				{
					num2 = 0;
				}
				else
				{
					num2 = steamItemDef;
				}
				this.m_steamInventoryHandler.SetLook(num, num2);
			}
			result = true;
		}
		return result;
	}

	private void BuyKey(int a_count)
	{
		this.OpenUrlInSteam(string.Concat(new object[]
		{
			"https://store.steampowered.com/buyitem/",
			348670,
			"/",
			3000,
			"/",
			a_count,
			"/"
		}));
	}

	private void OpenCommunityMarket()
	{
		this.OpenUrlInSteam("http://steamcommunity.com/market/search?appid=" + 348670);
	}

	private void OpenUrlInSteam(string a_url)
	{
		if (Global.isSteamActive)
		{
			SteamFriends.ActivateGameOverlayToWebPage(a_url);
		}
		else
		{
			Debug.Log("DEBUG: " + a_url);
		}
	}

	private SteamItemDetails_t GetRandomItemFromInventory(int a_defIdFrom, int a_defIdTo)
	{
		if (this.m_itemDetails != null)
		{
			for (int i = 0; i < this.m_itemDetails.Count; i++)
			{
				if (this.m_itemDetails[i].m_iDefinition.m_SteamItemDef <= a_defIdTo && this.m_itemDetails[i].m_iDefinition.m_SteamItemDef >= a_defIdFrom)
				{
					return this.m_itemDetails[i];
				}
			}
		}
		SteamItemDetails_t result = default(SteamItemDetails_t);
		result.m_itemId.m_SteamItemInstanceID = 0UL;
		return result;
	}

	private bool SellItem(int a_index)
	{
		bool result = false;
		if (this.m_itemDetails != null && a_index > -1 && a_index < this.m_itemDetails.Count)
		{
			ulong num = (!(null != this.m_client)) ? 12345678UL : this.m_client.GetSteamId();
			string text = string.Concat(new object[]
			{
				"http://steamcommunity.com/profiles/",
				num,
				"/inventory#",
				348670,
				"_2_",
				this.m_itemDetails[a_index].m_itemId.m_SteamItemInstanceID
			});
			if (Global.isSteamActive)
			{
				SteamFriends.ActivateGameOverlayToWebPage(text);
			}
			else
			{
				Debug.Log("DEBUG: " + text);
			}
			result = true;
		}
		return result;
	}

	private int GetClickedItemIndex(string a_clickedBtnName)
	{
		int result = -1;
		string[] array = a_clickedBtnName.Split(new char[]
		{
			'-'
		});
		if (array != null && 1 < array.Length && "sii" == array[0])
		{
			int num = -1;
			try
			{
				num = int.Parse(array[1]);
			}
			catch (Exception ex)
			{
				Debug.LogWarning("SteamInventoryGUI.cs: " + ex.ToString());
			}
			if (num > -1 && this.m_itemDetails != null && num < this.m_itemDetails.Count)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				this.m_contextMenu.transform.position = ray.GetPoint(3.5f);
				this.m_contextMenu.transform.localPosition += new Vector3(0.2f, -0.1f, 0f);
				int steamItemDef = this.m_itemDetails[num].m_iDefinition.m_SteamItemDef;
				string sKey = string.Empty;
				if (10000 > steamItemDef)
				{
					sKey = "STEAM_INV_OPEN_CASE";
				}
				else
				{
					int lookItemDef = this.m_steamInventoryHandler.GetLookItemDef();
					int skinItemDef = this.m_steamInventoryHandler.GetSkinItemDef();
					bool flag = lookItemDef == steamItemDef || skinItemDef == steamItemDef;
					if (flag)
					{
						sKey = "STEAM_INV_UNEQUIP";
					}
					else
					{
						sKey = "STEAM_INV_EQUIP";
					}
				}
				this.m_txtEquipOpen.text = LNG.Get(sKey);
				result = num;
				this.m_contextMenu.SetActive(true);
			}
		}
		return result;
	}

	public GameObject m_itemPrefab;

	public GameObject m_contextMenu;

	public TextMesh m_txtEquipOpen;

	public GameObject m_btnEquipOpen;

	public GameObject m_btnSell;

	public GameObject m_btnOpenCM;

	public GameObject m_btnBuyKey;

	public GameObject m_btnBuyKeys;

	public TextMesh m_txtPage;

	public TextMesh m_txtCaseCount;

	private GameObject[] m_items;

	private int m_curPage;

	private SteamInventoryResult_t m_resultHandle;

	private bool m_waitForResult;

	private bool m_completeRefresh;

	private bool m_caseOpenFlag;

	private int m_generatorDefId;

	private GUI3dMaster m_guimaster;

	private LidClient m_client;

	private CaseOpeningGUI m_caseOpenGui;

	private SteamInventoryHandler m_steamInventoryHandler;

	private List<SteamItemDetails_t> m_itemDetails = new List<SteamItemDetails_t>();

	private int m_contextMenuItemIndex = -1;

	private float m_nextPossibleRequestTime;

	private Callback<GameOverlayActivated_t> m_GameOverlayActivated;
}

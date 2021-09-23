using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamInventoryHandler : MonoBehaviour
{
	public SteamInventoryHandler()
	{
	}

	private void Start()
	{
		this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
		if (Global.isSteamActive)
		{
			this.m_waitForResult = SteamInventory.GetAllItems(out this.m_resultHandle);
		}
	}

	private void Update()
	{
		if (Global.isSteamActive && this.m_waitForResult)
		{
			EResult resultStatus = SteamInventory.GetResultStatus(this.m_resultHandle);
			if (resultStatus != EResult.k_EResultPending)
			{
				if (resultStatus == EResult.k_EResultOK)
				{
					uint num = 0U;
					if (SteamInventory.GetResultItems(this.m_resultHandle, null, ref num) && num > 0U)
					{
						SteamItemDetails_t[] array = new SteamItemDetails_t[num];
						SteamInventory.GetResultItems(this.m_resultHandle, array, ref num);
						this.m_itemDetails.AddRange(array);
					}
					this.EquipSteamInventoryItems();
				}
				else
				{
					Debug.Log("SteamInventoryHandler.cs: Couldn't get inventory: " + resultStatus.ToString());
				}
				SteamInventory.DestroyResult(this.m_resultHandle);
				this.m_waitForResult = false;
			}
		}
	}

	private bool HasItemDef(int a_itemDef)
	{
		for (int i = 0; i < this.m_itemDetails.Count; i++)
		{
			if (this.m_itemDetails[i].m_iDefinition.m_SteamItemDef == a_itemDef)
			{
				return true;
			}
		}
		return false;
	}

	private void EquipSteamInventoryItems()
	{
		int num = PlayerPrefs.GetInt("prefLook", 0);
		int num2 = PlayerPrefs.GetInt("prefSkin", 0);
		if (num != 0 && !this.HasItemDef(num))
		{
			num = 0;
		}
		if (num2 != 0 && !this.HasItemDef(num2))
		{
			num2 = 0;
		}
		this.SetLook(num, num2);
	}

	public void SetLook(int a_hatItemId, int a_skinItemId)
	{
		int num = (a_hatItemId != 0) ? (a_hatItemId + 1 - 10000) : 0;
		int num2 = (a_skinItemId != 0) ? (a_skinItemId + 1 - 20000) : 0;
		string itemDefHash = Util.GetItemDefHash(num, this.m_client.GetSteamId());
		PlayerPrefs.SetInt("prefLook", a_hatItemId);
		PlayerPrefs.SetString("prefLookHash", itemDefHash);
		string itemDefHash2 = Util.GetItemDefHash(num2, this.m_client.GetSteamId());
		PlayerPrefs.SetInt("prefSkin", a_skinItemId);
		PlayerPrefs.SetString("prefSkinHash", itemDefHash2);
		this.m_client.SendSetLook(num, itemDefHash, num2, itemDefHash2);
	}

	public int GetLookItemDef()
	{
		return PlayerPrefs.GetInt("prefLook", 0);
	}

	public int GetSkinItemDef()
	{
		return PlayerPrefs.GetInt("prefSkin", 0);
	}

	private List<SteamItemDetails_t> m_itemDetails = new List<SteamItemDetails_t>();

	private SteamInventoryResult_t m_resultHandle;

	private bool m_waitForResult;

	private LidClient m_client;
}

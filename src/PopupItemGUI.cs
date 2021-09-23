using System;
using SimpleJSON;
using Steamworks;
using UnityEngine;

public class PopupItemGUI : MonoBehaviour
{
	public PopupItemGUI()
	{
	}

	private void Start()
	{
		this.m_guimaster = (GUI3dMaster)UnityEngine.Object.FindObjectOfType(typeof(GUI3dMaster));
		this.m_inventory = (InventoryGUI)UnityEngine.Object.FindObjectOfType(typeof(InventoryGUI));
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_steamItemPrefab);
		this.m_itemText = gameObject.GetComponentInChildren<TextMesh>();
		this.m_itemRenderer = gameObject.GetComponentInChildren<MeshCollider>();
		gameObject.transform.parent = this.m_guiParent.transform;
		gameObject.transform.localPosition = new Vector3(-0.29f, 0.245f, -0.04f);
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one * 0.8f;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			this.ShowGui(false, 0);
		}
		if (Application.isEditor && Input.GetKeyDown(KeyCode.K))
		{
			this.ShowGui(true, 2000);
		}
	}

	private void LateUpdate()
	{
		if (null != this.m_guimaster && this.m_guiParent.activeSelf)
		{
			string clickedButtonName = this.m_guimaster.GetClickedButtonName();
			if (string.Empty != clickedButtonName)
			{
				if ("btn_writereview" == clickedButtonName)
				{
					string text = "http://store.steampowered.com/recommended/recommendgame/348670";
					if (Global.isSteamActive)
					{
						SteamFriends.ActivateGameOverlayToWebPage(text);
					}
					else
					{
						Debug.Log("DEBUG: " + text);
					}
				}
				else
				{
					if ("btn_open_inv" == clickedButtonName)
					{
						this.m_inventory.OpenSteamInventory();
					}
					this.ShowGui(false, 0);
				}
			}
		}
	}

	public void ShowGui(bool a_show, int a_itemDefId = 0)
	{
		if (a_show)
		{
			JSONNode item = JsonItems.GetItem(a_itemDefId);
			if (null != item && null != this.m_itemText && null != this.m_itemRenderer)
			{
				this.m_itemText.text = string.Concat(new string[]
				{
					"<color=#",
					item["name_color"],
					">",
					item["market_name"],
					"</color>"
				});
				this.m_itemRenderer.renderer.material.mainTexture = Resources.Load<Texture>("inventory_steam/inventory_s_" + a_itemDefId);
			}
			base.audio.Play();
		}
		this.m_guiParent.SetActive(a_show);
		int @int = PlayerPrefs.GetInt("prefSteamDropCount", 0);
		PlayerPrefs.SetInt("prefSteamDropCount", @int + 1);
		this.m_reviewText.text = LNG.Get((@int % 2 != 1) ? "STEAM_BLUE_ICON" : "STEAM_PLEASE_REVIEW");
	}

	public bool IsActive()
	{
		return this.m_guiParent.activeSelf;
	}

	public GameObject m_guiParent;

	public GameObject m_steamItemPrefab;

	public TextMesh m_reviewText;

	private GUI3dMaster m_guimaster;

	private InventoryGUI m_inventory;

	private TextMesh m_itemText;

	private MeshCollider m_itemRenderer;
}

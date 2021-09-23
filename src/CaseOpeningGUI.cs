using System;
using SimpleJSON;
using UnityEngine;

public class CaseOpeningGUI : MonoBehaviour
{
	public CaseOpeningGUI()
	{
	}

	private void Start()
	{
		this.m_guimaster = (GUI3dMaster)UnityEngine.Object.FindObjectOfType(typeof(GUI3dMaster));
		this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
		this.m_displayTexts = new TextMesh[this.m_displayItems.Length];
		this.m_displayRenderers = new MeshCollider[this.m_displayItems.Length];
		this.m_displayDefIds = new int[this.m_displayItems.Length];
		this.m_gui.SetActive(true);
		for (int i = 0; i < this.m_displayItems.Length; i++)
		{
			this.m_displayTexts[i] = this.m_displayItems[i].GetComponentInChildren<TextMesh>();
			this.m_displayRenderers[i] = this.m_displayItems[i].GetComponentInChildren<MeshCollider>();
		}
		this.m_gui.SetActive(false);
	}

	public void Showtime(int a_itemDefToWin, int a_generatorDef)
	{
		this.m_gui.SetActive(true);
		this.m_btnClose.SetActive(false);
		this.m_itemDefToWin = a_itemDefToWin;
		this.m_timeToSlowdown = Time.time + UnityEngine.Random.Range(0f, 1f);
		this.m_curSpeed = this.m_speed;
		this.m_newItemTxt.text = LNG.Get("STEAM_INV_NEW_ITEM") + "\n ";
		this.m_setWinningItemFlag = false;
		this.GetItemDefsFromGenerator(a_generatorDef);
		for (int i = 0; i < this.m_displayItems.Length; i++)
		{
			this.ChangeItem(i, 0);
		}
	}

	public bool InProgress()
	{
		return this.m_gui.activeSelf && false == this.m_btnClose.activeSelf;
	}

	private void GetItemDefsFromGenerator(int a_generatorDef)
	{
		JSONNode item = JsonItems.GetItem(a_generatorDef);
		if (null != item)
		{
			string text = item["bundle"];
			string[] array = text.Split(new char[]
			{
				';'
			});
			this.m_generatorDefIds = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					'x'
				});
				try
				{
					this.m_generatorDefIds[i] = int.Parse(array2[0]);
				}
				catch (Exception)
				{
				}
			}
		}
	}

	private void Update()
	{
		if (this.m_gui.activeSelf)
		{
			float deltaTime = Time.deltaTime;
			if (this.m_curSpeed > 0.01f)
			{
				for (int i = 0; i < this.m_displayItems.Length; i++)
				{
					Vector3 localPosition = this.m_displayItems[i].transform.localPosition;
					this.m_displayItems[i].transform.localPosition += Vector3.right * deltaTime * this.m_curSpeed;
					if (this.m_displayItems[i].transform.localPosition.x > 0.085f)
					{
						this.m_displayItems[i].transform.localPosition -= Vector3.right * 0.6f;
						int a_newDefId = 0;
						if (this.m_curSpeed < 0.2f && !this.m_setWinningItemFlag)
						{
							a_newDefId = this.m_itemDefToWin;
							this.m_setWinningItemFlag = true;
						}
						this.ChangeItem(i, a_newDefId);
					}
					if (localPosition.x < 0.015f && 0.015f < this.m_displayItems[i].transform.localPosition.x && null != base.audio)
					{
						base.audio.Play();
					}
				}
				if (Time.time > this.m_timeToSlowdown)
				{
					this.m_curSpeed *= 1f - deltaTime * this.m_slowDownRate;
				}
			}
			else if (!this.m_btnClose.activeSelf)
			{
				JSONNode item = JsonItems.GetItem(this.m_itemDefToWin);
				if (null != item)
				{
					this.m_newItemTxt.text = LNG.Get("STEAM_INV_NEW_ITEM") + "\n " + item["market_name"];
					if (null != this.m_client)
					{
						this.m_client.SendChatMsg(":#~" + item["market_name"], false);
					}
					else
					{
						ComChatGUI comChatGUI = UnityEngine.Object.FindObjectOfType<ComChatGUI>();
						comChatGUI.AddString("Ethan The just opened a case and received: \n<color=\"red\">" + item["market_name"] + "</color>");
					}
				}
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_audioEffect);
				gameObject.audio.clip = this.m_successSound;
				gameObject.audio.volume = 0.4f;
				gameObject.audio.Play();
				this.m_btnClose.SetActive(true);
			}
		}
		if (Application.isEditor && Input.GetKeyDown(KeyCode.L))
		{
			this.Showtime(20009, 1004);
		}
	}

	private void ChangeItem(int a_index, int a_newDefId = 0)
	{
		int num = a_newDefId;
		int num2 = 0;
		UnityEngine.Random.seed = (int)(Time.time * 1000f);
		while (num == 0 && this.m_generatorDefIds != null && 0 < this.m_generatorDefIds.Length)
		{
			num = this.m_generatorDefIds[UnityEngine.Random.Range(0, this.m_generatorDefIds.Length)];
			for (int i = 0; i < this.m_displayDefIds.Length; i++)
			{
				if (num == this.m_displayDefIds[i] && 100 > num2)
				{
					num = 0;
					num2++;
					break;
				}
			}
		}
		JSONNode item = JsonItems.GetItem(num);
		if (null != item && a_index < this.m_displayItems.Length && num != this.m_displayDefIds[a_index])
		{
			this.m_displayDefIds[a_index] = num;
			this.m_displayTexts[a_index].text = string.Concat(new string[]
			{
				"<color=#",
				item["name_color"],
				">",
				item["market_name"],
				"</color>"
			});
			this.m_displayRenderers[a_index].renderer.material.mainTexture = Resources.Load<Texture>("inventory_steam/inventory_s_" + num);
		}
	}

	private void LateUpdate()
	{
		if (null != this.m_guimaster)
		{
			string clickedButtonName = this.m_guimaster.GetClickedButtonName();
			if (string.Empty != clickedButtonName && this.m_btnClose.name == clickedButtonName)
			{
				this.m_gui.SetActive(false);
			}
		}
	}

	public float m_speed = 3f;

	public float m_showResultDur = 5f;

	public GameObject[] m_displayItems;

	public TextMesh m_newItemTxt;

	public GameObject m_btnClose;

	public GameObject m_gui;

	public GameObject m_audioEffect;

	public AudioClip m_successSound;

	private TextMesh[] m_displayTexts;

	private MeshCollider[] m_displayRenderers;

	private int[] m_displayDefIds;

	private int[] m_generatorDefIds;

	private float m_slowDownRate = 0.5f;

	private float m_timeToSlowdown;

	private float m_curSpeed;

	private int m_itemDefToWin;

	private bool m_setWinningItemFlag;

	private GUI3dMaster m_guimaster;

	private LidClient m_client;
}

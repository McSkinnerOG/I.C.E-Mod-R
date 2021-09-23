using System;
using Steamworks;
using UnityEngine;

public class RemoteCharacter : MonoBehaviour
{
	public RemoteCharacter()
	{
	}

	public void Refresh(Vector3 a_pos, float a_rotation, CharAnim2.ePose a_anim = CharAnim2.ePose.eStand, float a_health = 100f, float a_energy = 100f)
	{
		if (this.RefreshStatus(a_health, a_energy))
		{
			if (!this.m_visible)
			{
				this.SwitchVisibility();
				base.transform.position = a_pos;
				base.transform.rotation = Quaternion.Euler(0f, a_rotation, 0f);
			}
			if (null != this.m_animControl)
			{
				this.m_animControl.m_isTakingAction = (CharAnim2.ePose.eAttack == a_anim);
				this.m_animControl.m_isSitting = (CharAnim2.ePose.eSit == a_anim);
				this.m_sound.enabled = !this.m_animControl.m_isSitting;
			}
			else if (null != this.m_animControl2)
			{
				this.m_animControl2.PlayAnimation(a_anim);
			}
			this.m_targetPos = a_pos;
			this.m_targetRot = Quaternion.Euler(0f, a_rotation, 0f);
			float num = Time.time - this.m_lastUpdate;
			if (num > 0f && num < 1f)
			{
				this.m_interpSpeed = 1f / num * this.m_interpPercent;
			}
			this.m_lastUpdate = Time.time;
		}
	}

	public bool RefreshStatus(float a_health, float a_energy)
	{
		if (this.m_health == 0f && a_health == 0f)
		{
			return false;
		}
		if (9999999f > a_health)
		{
			if (this.m_visible && this.m_health != a_health)
			{
				float num = a_health - this.m_health;
				bool flag = num < 0f;
				if (flag || 1f < num)
				{
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_damageHealIndicatorPrefab, base.transform.position + Vector3.up * 3f, Quaternion.identity);
					TextMesh component = gameObject.GetComponent<TextMesh>();
					if (null != component)
					{
						string text = num.ToString();
						if (!flag)
						{
							text = "+" + text;
						}
						component.text = string.Concat(new string[]
						{
							"<color=\"",
							(!flag) ? "green" : "red",
							"\">",
							text,
							"</color>"
						});
					}
				}
				if (flag)
				{
					if (null != this.m_sound)
					{
						this.m_sound.Suffer(0f == a_health);
					}
					if (this.m_isOwnPlayer && 0f < a_health && null != this.m_quitGameGui)
					{
						this.m_quitGameGui.m_cantLogoutTime = Time.time + 20f;
					}
					GameObject original = (this.m_type != eCharType.eCar) ? this.m_bloodEffectPrefab : this.m_sparksEffectPrefab;
					UnityEngine.Object.Instantiate(original, base.transform.position + Vector3.up * 2f, Quaternion.LookRotation(Vector3.up));
				}
			}
			if (null != this.m_avatar && this.m_health != a_health)
			{
				this.m_avatar.SendMessage("SetHealth", a_health, SendMessageOptions.DontRequireReceiver);
			}
			this.m_health = a_health;
			if (this.m_health == 0f)
			{
				this.CreateCorpse();
				if (null != this.m_animControl)
				{
					this.m_animControl.ResetAnim();
				}
				if (this.m_visible)
				{
					this.SwitchVisibility();
				}
			}
		}
		if (9999999f > a_energy)
		{
			this.m_energy = a_energy;
		}
		return 0f != this.m_health;
	}

	public bool IsVisible()
	{
		return this.m_visible;
	}

	public void Remove()
	{
		if (null != this.m_label)
		{
			UnityEngine.Object.Destroy(this.m_label.gameObject);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void Spawn(int a_onlineId, eCharType a_type, bool a_isOwnPlayer)
	{
		this.m_id = a_onlineId;
		this.m_isOwnPlayer = a_isOwnPlayer;
		this.m_type = a_type;
		GameObject prefab = this.GetPrefab(false);
		if (null != prefab)
		{
			if (null != this.m_avatar)
			{
				UnityEngine.Object.Destroy(this.m_avatar);
			}
			this.m_avatar = (GameObject)UnityEngine.Object.Instantiate(prefab, base.transform.position, base.transform.rotation);
			this.m_avatar.transform.parent = base.transform;
			this.m_animControl = this.m_avatar.GetComponent<BodyHeadAnim>();
			this.m_animControl2 = this.m_avatar.GetComponent<CharAnim2>();
			this.m_sound = this.m_avatar.GetComponent<CharSounds>();
			if (this.m_isOwnPlayer)
			{
				this.m_itemPopupGui = (PopupItemGUI)UnityEngine.Object.FindObjectOfType(typeof(PopupItemGUI));
				this.m_quitGameGui = (QuitGameGUI)UnityEngine.Object.FindObjectOfType(typeof(QuitGameGUI));
			}
			else
			{
				base.gameObject.layer = this.m_avatar.layer;
			}
			if (null != this.m_animControl)
			{
				this.m_animControl.Init(this.m_isOwnPlayer);
			}
		}
	}

	private void SetName(int a_rank, int a_karma, string a_name)
	{
		this.m_isSaint = (a_karma >= 199);
		if (a_name != null && a_name.Length > 0)
		{
			if (null == this.m_label && null != this.m_labelPrefab)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_labelPrefab, base.transform.position + this.m_labelOffset, Quaternion.identity);
				this.m_label = gameObject.GetComponent<TextMesh>();
				this.m_label.transform.parent = base.transform;
			}
			if (null != this.m_label)
			{
				string text = string.Empty;
				string text2 = string.Empty;
				string text3 = string.Empty;
				this.m_label.text = string.Empty;
				if (0 < a_rank)
				{
					switch (a_rank)
					{
					case 1:
						text2 = "ffff00";
						text3 = "I";
						break;
					case 2:
						text2 = "ffee00";
						text3 = "II";
						break;
					case 3:
						text2 = "ffdd00";
						text3 = "III";
						break;
					case 4:
						text2 = "ffcc00";
						text3 = "IV";
						break;
					case 5:
						text2 = "ffbb00";
						text3 = "V";
						break;
					case 6:
						text2 = "ffaa00";
						text3 = "VI";
						break;
					case 7:
						text2 = "ff9900";
						text3 = "VII";
						break;
					case 8:
						text2 = "ff8800";
						text3 = "VIII";
						break;
					case 9:
						text2 = "ff7700";
						text3 = "IX";
						break;
					default:
						text2 = "ff6600";
						text3 = "X";
						break;
					}
					text3 = string.Concat(new string[]
					{
						"<color=\"#",
						text2,
						"\">",
						text3,
						"</color> "
					});
				}
				if (this.m_isSaint)
				{
					text2 = "FFD800";
					text = "<color=\"#" + text2 + "\">★</color> ";
				}
				else
				{
					text2 = ((int)(75f + Mathf.Clamp01(1f - (float)(a_karma - 100) * 0.01f) * 180f)).ToString("X2") + ((int)(75f + Mathf.Clamp01((float)a_karma * 0.01f) * 180f)).ToString("X2") + ((int)(75f + Mathf.Clamp01(1f - Mathf.Abs((float)(a_karma - 100) * 0.01f)) * 180f)).ToString("X2");
					if (8f >= (float)a_karma)
					{
						text = "<color=\"#ff0000\">☠</color> ";
					}
				}
				this.m_label.text = string.Concat(new string[]
				{
					text,
					text3,
					"<color=\"#",
					text2,
					"\">",
					a_name,
					"</color>"
				});
			}
		}
	}

	public void SetChatText(string a_text)
	{
		if (a_text != null && a_text.Length > 0)
		{
			if (null == this.m_labelChat && null != this.m_labelChatPrefab)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_labelChatPrefab, base.transform.position + this.m_labelChatOffset, Quaternion.identity);
				gameObject.transform.parent = base.transform;
				this.m_labelChat = gameObject.GetComponent<ChatLabel>();
			}
			if (null != this.m_labelChat)
			{
				this.m_labelChat.SetText(a_text, false);
			}
		}
	}

	public void SetInfo(CharData a_data)
	{
		if (a_data.type != this.m_type)
		{
			this.m_type = a_data.type;
			this.Spawn(this.m_id, this.m_type, this.m_isOwnPlayer);
			this.m_handItem = -1;
			this.m_look = -1;
			this.m_skin = -1;
			this.m_body = -1;
		}
		if (null != this.m_animControl)
		{
			if (this.m_handItem != a_data.handItem)
			{
				this.m_animControl.ChangeHandItem(a_data.handItem);
				this.m_handItem = a_data.handItem;
			}
			if (this.m_look != a_data.look)
			{
				this.m_animControl.ChangeHeadItem(a_data.look);
				this.m_look = a_data.look;
			}
			if (this.m_skin != a_data.skin)
			{
				this.m_animControl.ChangeSkin(a_data.skin);
				this.m_skin = a_data.skin;
			}
			if (this.m_body != a_data.body)
			{
				this.m_animControl.ChangeBodyItem(a_data.body);
				this.m_body = a_data.body;
			}
		}
		if (this.m_lastRank != a_data.rank)
		{
			if (this.m_lastRank == -1)
			{
				if (this.m_isOwnPlayer && Global.isSteamActive)
				{
					bool flag = false;
					bool flag2 = true;
					int num = Mathf.Min(10, a_data.rank);
					for (int i = 1; i <= num; i++)
					{
						if (SteamUserStats.GetAchievement("ACH_IMM_RANK_" + i, out flag2) && !flag2)
						{
							SteamUserStats.SetAchievement("ACH_IMM_RANK_" + i);
							flag = true;
						}
					}
					if (flag)
					{
						SteamUserStats.StoreStats();
					}
				}
			}
			else if (a_data.rank > this.m_lastRank && a_data.rank < 11)
			{
				UnityEngine.Object.Instantiate(this.m_lvlUpEffect, base.transform.position + Vector3.up * 3.5f, Quaternion.identity);
				if (this.m_isOwnPlayer && Global.isSteamActive)
				{
					SteamUserStats.SetAchievement("ACH_IMM_RANK_" + Mathf.Clamp(a_data.rank, 1, 10));
					SteamUserStats.StoreStats();
					PlayerPrefs.SetInt("prefHints", 0);
				}
				Debug.Log(string.Concat(new object[]
				{
					base.name,
					" m_lastRank ",
					this.m_lastRank,
					" a_data.rank ",
					a_data.rank,
					" level up"
				}));
			}
			this.m_lastRank = a_data.rank;
		}
		this.SetName(a_data.rank, a_data.karma, a_data.name);
	}

	public void AddXp(int a_xp)
	{
		if (a_xp > 0)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_xpEffect, base.transform.position + Vector3.up * 3f, Quaternion.identity);
			TextMesh component = gameObject.GetComponent<TextMesh>();
			if (null != component)
			{
				component.text = a_xp.ToString() + " XP";
			}
		}
	}

	public void OnSpecialEvent(eSpecialEvent a_event)
	{
		string text = string.Empty;
		Vector3 position = Vector3.zero;
		switch (a_event)
		{
		case eSpecialEvent.itemBroke:
			UnityEngine.Object.Instantiate(this.m_weaponBreakEffect, base.transform.position + Vector3.up * 1.5f + base.transform.forward, Quaternion.identity);
			break;
		case eSpecialEvent.empty:
		case eSpecialEvent.forbidden:
		case eSpecialEvent.fishingfail:
		case eSpecialEvent.tooManyMissions:
		case eSpecialEvent.alreadyGotMission:
		case eSpecialEvent.noAmmo:
		case eSpecialEvent.carExitsBlocked:
		case eSpecialEvent.cantHurtSaints:
		case eSpecialEvent.buildingRepaired:
			position = base.transform.position + Vector3.up * 3f;
			text = "EVENT_" + (int)a_event;
			break;
		case eSpecialEvent.missionComplete:
			UnityEngine.Object.Instantiate(this.m_missionCompleteEffect, base.transform.position + Vector3.up * 4f, Quaternion.identity);
			if (this.m_isOwnPlayer && Global.isSteamActive)
			{
				this.m_waitForResult = SteamInventory.TriggerItemDrop(out this.m_itemDropHandle, (SteamItemDef_t)100);
			}
			break;
		}
		if (string.Empty != text)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_textEffect, position, Quaternion.identity);
			TextLNG component = gameObject.GetComponent<TextLNG>();
			component.m_lngKey = text;
		}
	}

	public bool IsNpc()
	{
		return this.m_type != eCharType.ePlayer && this.m_type != eCharType.ePlayerFemale && eCharType.eCar != this.m_type;
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		float num = Time.time - this.m_lastUpdate;
		if (this.m_visible)
		{
			float num2 = deltaTime * this.m_interpSpeed;
			float num3 = num2;
			if (this.m_isOwnPlayer && this.m_doOwnPlayerPrediciton)
			{
				Vector3 vector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
				if (Vector3.zero != vector)
				{
					this.m_targetRot = Quaternion.LookRotation(vector);
					num3 *= 0.25f;
				}
			}
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.m_targetRot, num3);
			Vector3 vector2 = Vector3.Lerp(base.transform.position, this.m_targetPos, num2);
			if ((null != this.m_animControl && !this.m_animControl.m_isSitting) || null != this.m_animControl2)
			{
				vector2.y = Util.GetTerrainHeight(vector2) - 1.5f;
				if (vector2.y < -1f)
				{
					vector2.y = -4f;
				}
			}
			base.transform.position = vector2;
			if (num > this.m_disappearTime)
			{
				this.SwitchVisibility();
			}
		}
		else if (num > this.m_dieTime)
		{
			this.Remove();
		}
		if (this.m_isOwnPlayer && Global.isSteamActive)
		{
			this.HandleDropResult();
			SteamInventory.SendItemDropHeartbeat();
		}
	}

	private void HandleDropResult()
	{
		if (this.m_waitForResult && Global.isSteamActive)
		{
			bool flag = false;
			EResult resultStatus = SteamInventory.GetResultStatus(this.m_itemDropHandle);
			if (resultStatus != EResult.k_EResultPending)
			{
				if (resultStatus == EResult.k_EResultOK)
				{
					uint num = 0U;
					if (SteamInventory.GetResultItems(this.m_itemDropHandle, null, ref num))
					{
						SteamItemDetails_t[] array = new SteamItemDetails_t[num];
						if (num > 0U)
						{
							SteamInventory.GetResultItems(this.m_itemDropHandle, array, ref num);
							for (int i = 0; i < array.Length; i++)
							{
								if ((array[i].m_unFlags & 256) == 0)
								{
									this.m_itemPopupGui.ShowGui(true, array[i].m_iDefinition.m_SteamItemDef);
									break;
								}
							}
						}
						else
						{
							flag = true;
						}
					}
				}
				else
				{
					Debug.Log("RemoteCharacter.cs: Couldn't get item drop: " + resultStatus.ToString());
				}
				SteamInventory.DestroyResult(this.m_itemDropHandle);
				this.m_waitForResult = false;
				if (flag)
				{
					this.GetPromoItem();
				}
			}
		}
	}

	private void GetPromoItem()
	{
		if (Global.isSteamActive && PlayerPrefs.GetInt("prefGotPromoItemVetHat", 0) == 0)
		{
			this.m_waitForResult = SteamInventory.GrantPromoItems(out this.m_itemDropHandle);
			PlayerPrefs.SetInt("prefGotPromoItemVetHat", 1);
		}
	}

	private void LateUpdate()
	{
		if (this.m_visible && null != this.m_label)
		{
			Vector3 b = this.m_labelOffset;
			if (null != this.m_animControl && this.m_animControl.m_isSitting)
			{
				b = Vector3.up * 1000f;
			}
			this.m_label.transform.position = base.transform.position + b;
			this.m_label.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
		}
		if (null != this.m_label && this.m_label.renderer.enabled != this.m_visible)
		{
			this.m_label.renderer.enabled = this.m_visible;
		}
	}

	private void CreateCorpse()
	{
		GameObject prefab = this.GetPrefab(true);
		if (null != prefab)
		{
			Vector3 b = Vector3.up * ((!(null == this.m_animControl) && !this.m_animControl.m_isSitting) ? 1f : 0.1f);
			UnityEngine.Object.Instantiate(prefab, base.transform.position + b, base.transform.rotation);
		}
	}

	private void SwitchVisibility()
	{
		this.m_visible = !this.m_visible;
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.enabled = this.m_visible;
		}
		if (null != base.collider)
		{
			base.collider.enabled = this.m_visible;
		}
		if (null != this.m_labelChat)
		{
			this.m_labelChat.SetText(string.Empty, false);
		}
	}

	private GameObject GetPrefab(bool a_isDead = false)
	{
		GameObject[] array = (!a_isDead) ? this.m_prefabs : this.m_deadPrefabs;
		int type = (int)this.m_type;
		if (type < 0 || type >= array.Length)
		{
			return null;
		}
		return array[type];
	}

	public GameObject[] m_prefabs;

	public GameObject[] m_deadPrefabs;

	public GameObject m_xpEffect;

	public GameObject m_lvlUpEffect;

	public GameObject m_missionCompleteEffect;

	public GameObject m_bloodEffectPrefab;

	public GameObject m_sparksEffectPrefab;

	public GameObject m_damageHealIndicatorPrefab;

	public GameObject m_weaponBreakEffect;

	public GameObject m_textEffect;

	public bool m_doOwnPlayerPrediciton;

	public float m_interpPercent = 0.9f;

	public GameObject m_labelPrefab;

	public GameObject m_labelChatPrefab;

	public Vector3 m_labelOffset = new Vector3(0f, 0.1f, -0.8f);

	public Vector3 m_labelChatOffset = new Vector3(0f, 0.1f, -1f);

	public float m_corpseDisappearTime = 60f;

	[HideInInspector]
	public bool m_isOwnPlayer;

	[HideInInspector]
	public eCharType m_type;

	[HideInInspector]
	public int m_id = -1;

	[HideInInspector]
	public float m_health = 100f;

	[HideInInspector]
	public float m_energy = 100f;

	[HideInInspector]
	public bool m_isSaint;

	private int m_handItem = -1;

	private int m_look = -1;

	private int m_skin = -1;

	private int m_body = -1;

	private Vector3 m_targetPos = Vector3.zero;

	private Quaternion m_targetRot = Quaternion.identity;

	private float m_interpSpeed = 4f;

	private GameObject m_avatar;

	private TextMesh m_label;

	private ChatLabel m_labelChat;

	private BodyHeadAnim m_animControl;

	private CharAnim2 m_animControl2;

	private CharSounds m_sound;

	private PopupItemGUI m_itemPopupGui;

	private QuitGameGUI m_quitGameGui;

	private bool m_visible;

	private float m_lastUpdate;

	private float m_disappearTime = 1f;

	private float m_dieTime = 10f;

	private int m_lastRank = -1;

	private SteamInventoryResult_t m_itemDropHandle;

	private bool m_waitForResult;
}

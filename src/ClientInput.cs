using System;
using Steamworks;
using UnityEngine;

public class ClientInput : MonoBehaviour
{
	public ClientInput()
	{
	}

	private void Start()
	{
		this.m_path = new NavMeshPath();
		this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
		this.m_inventory = (InventoryGUI)UnityEngine.Object.FindObjectOfType(typeof(InventoryGUI));
		this.m_communicator = (QmunicatorGUI)UnityEngine.Object.FindObjectOfType(typeof(QmunicatorGUI));
		this.m_itemGui = (ItemGUI)UnityEngine.Object.FindObjectOfType(typeof(ItemGUI));
		this.m_popupGui = (PopupGUI)UnityEngine.Object.FindObjectOfType(typeof(PopupGUI));
		this.m_repairNpcs = (RepairingNpc[])UnityEngine.Object.FindObjectsOfType(typeof(RepairingNpc));
		this.m_tooltipText = this.m_tooltip.GetComponentInChildren<TextMesh>();
		this.m_tooltipHudRText = this.m_tooltipHudR.GetComponentInChildren<TextMesh>();
		this.m_tooltipHudLText = this.m_tooltipHudL.GetComponentInChildren<TextMesh>();
		this.ResetTarget();
		this.m_buySellPos = this.m_invalidPos;
	}

	private void Update()
	{
		this.GetMouseInput(Time.deltaTime);
		if (null != this.m_client && this.m_client.enabled)
		{
			this.CalculateAxis();
			this.HandlePopup();
			this.HandleHotKeys();
			if (this.m_client.GetHealth() == 0f)
			{
				this.ResetPath();
			}
			if (Input.GetButtonDown("FindTarget") || (this.IsAttacking() && null == this.m_currentTarget))
			{
				this.m_currentTarget = this.FindTarget(this.m_currentTarget);
			}
			bool flag = null != this.m_currentTarget && this.m_currentTarget.IsVisible();
			bool flag2 = flag && (this.m_currentTarget.m_type == eCharType.ePlayer || eCharType.ePlayerFemale == this.m_currentTarget.m_type);
			int num = Mathf.Clamp((int)(this.m_rotTowardsMousePos / 360f * 255f), 0, 255);
			int a_targetIdOrAtkRot = (!flag) ? num : this.m_currentTarget.m_id;
			int num2 = 0;
			num2 |= ((!this.IsInteracting()) ? 0 : 1);
			num2 |= ((!this.IsAttacking()) ? 0 : 2);
			num2 |= ((!flag) ? 0 : 4);
			num2 |= ((!flag2) ? 0 : 8);
			num2 |= ((!this.IsAttackingWithMouse()) ? 0 : 16);
			num2 |= (int)((byte)((this.m_vertAxis + 1f) * 100f)) << 8;
			num2 |= (int)((byte)((this.m_horiAxis + 1f) * 100f)) << 16;
			if ((num2 != this.m_input || this.m_sendDragPos != this.m_sendDropPos || this.m_nextInputTime + 1f < Time.time) && this.m_nextInputTime < Time.time)
			{
				this.m_input = num2;
				this.m_client.SendInput(this.m_input, a_targetIdOrAtkRot, this.m_buildRot, this.m_sendDragPos, this.m_sendDropPos);
				this.m_sendDragPos = Vector3.zero;
				this.m_sendDropPos = Vector3.zero;
				this.m_nextInputTime = Time.time + this.m_minSendIntervall;
			}
		}
		if (null != this.m_currentTarget)
		{
			if (null != this.m_bullsEye)
			{
				float x = 0.25f * (float)(3 - (int)(this.m_currentTarget.m_health * 0.033f));
				this.m_bullsEye.renderer.material.mainTextureOffset = new Vector2(x, 0f);
				this.m_bullsEye.position = this.m_currentTarget.transform.position + Vector3.up * 0.1f;
			}
			if (!this.m_currentTarget.IsVisible())
			{
				this.ResetTarget();
			}
		}
		else if (!this.IsAttacking())
		{
			this.ResetTarget();
		}
		this.ScreenshotInput();
		if (this.m_burpFlag && !base.audio.isPlaying)
		{
			base.audio.clip = this.m_soundBurp;
			base.audio.Play();
			this.m_burpFlag = false;
		}
		if (null != this.m_currentTarget && this.m_currentTarget.m_isSaint && this.IsAttacking() && Time.time > this.m_attackNoticeTime)
		{
			this.m_client.GetPlayer().OnSpecialEvent(eSpecialEvent.cantHurtSaints);
			this.m_attackNoticeTime = Time.time + 1f;
		}
	}

	private void HandleHotKeys()
	{
		int num = -1;
		if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
		{
			num = 1;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
		{
			num = 2;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
		{
			num = 3;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
		{
			num = 4;
		}
		if (0 < num)
		{
			RemoteItem itemFromPos = this.m_inventory.GetItemFromPos((float)num, 0f);
			if (null != itemFromPos)
			{
				if (Items.IsEatable(itemFromPos.m_type) || Items.IsMedicine(itemFromPos.m_type))
				{
					this.ConsumeItem(itemFromPos);
				}
				else
				{
					this.m_sendDragPos = new Vector3((float)num, 0f);
					this.m_sendDropPos = Vector3.zero;
				}
			}
		}
	}

	private void HandlePopup()
	{
		if (this.m_buySellPopupSessionId == this.m_popupGui.GetSessionId())
		{
			if (this.m_invalidPos != this.m_buySellPos && this.m_popupGui.m_saidYesFlag)
			{
				this.m_sendDragPos = this.m_buySellPos;
				this.m_sendDropPos = Vector3.one * 252f;
				this.m_buySellPos = this.m_invalidPos;
				this.m_popupGui.m_saidYesFlag = false;
			}
			if (this.m_popupGui.IsActive() && (!this.m_inventory.IsVisible() || !this.m_inventory.IsShopActive()))
			{
				this.m_popupGui.ShowGui(false, string.Empty);
			}
		}
		else if (this.m_repairPopupSessionId == this.m_popupGui.GetSessionId())
		{
			if (this.m_popupGui.m_saidYesFlag)
			{
				this.m_client.SendSpecialRequest(eSpecialRequest.repairItem);
				this.m_popupGui.m_saidYesFlag = false;
			}
		}
		else if (this.m_missionPopupSessionId == this.m_popupGui.GetSessionId() && this.m_popupGui.m_saidYesFlag)
		{
			this.m_client.SendSpecialRequest(eSpecialRequest.acceptMission);
			this.m_popupGui.m_saidYesFlag = false;
		}
		if (this.m_popupGui.IsActive() && this.m_isMoving)
		{
			this.m_popupGui.ShowGui(false, string.Empty);
		}
	}

	private void CalculateAxis()
	{
		this.m_vertAxis = Input.GetAxis("Vertical");
		this.m_horiAxis = Input.GetAxis("Horizontal");
		if (this.m_pathCorners != null)
		{
			if (this.m_vertAxis == 0f && this.m_horiAxis == 0f && this.m_nextPathPoint < this.m_pathCorners.Length)
			{
				Vector3 vector = this.m_pathCorners[this.m_nextPathPoint] - this.m_client.GetPos();
				if (2f > vector.sqrMagnitude)
				{
					this.m_nextPathPoint++;
					if (this.m_nextPathPoint == this.m_pathCorners.Length && this.m_interactionAtPathEnd)
					{
						this.m_interactionAtPathEnd = false;
						this.m_forceInteractionTime = Time.time + 0.3f;
					}
				}
				vector = vector.normalized;
				this.m_vertAxis = vector.z;
				this.m_horiAxis = vector.x;
			}
			else if (0 < this.m_pathCorners.Length)
			{
				this.ResetPath();
			}
		}
		this.m_isMoving = (this.m_vertAxis != 0f || 0f != this.m_horiAxis);
	}

	private void ResetPath()
	{
		if (this.m_pathCorners != null && 0 < this.m_pathCorners.Length)
		{
			this.m_path.ClearCorners();
			this.m_pathCorners = null;
			this.m_nextPathPoint = 0;
			this.m_interactionAtPathEnd = false;
		}
	}

	private RemoteCharacter FindTarget(RemoteCharacter a_curTarget)
	{
		RemoteCharacter result = null;
		if (null != this.m_client)
		{
			RemoteCharacter player = this.m_client.GetPlayer();
			if (null != player)
			{
				float num = Mathf.Max(Items.GetItemDef(this.m_client.GetHandItem()).range, 5f);
				float d = num * 0.25f;
				float num2 = num * 0.75f;
				Vector3 vector = player.transform.position + player.transform.forward * d;
				RemoteCharacter remoteCharacter = this.m_client.GetNearestNpc(vector);
				if (null != remoteCharacter && (remoteCharacter.transform.position - vector).magnitude > num2)
				{
					remoteCharacter = null;
				}
				RemoteCharacter remoteCharacter2 = null;
				if (null != remoteCharacter2 && (remoteCharacter2.transform.position - vector).magnitude > num2)
				{
					remoteCharacter2 = null;
				}
				if ((null == remoteCharacter || remoteCharacter == a_curTarget) && player != remoteCharacter2 && null != remoteCharacter2)
				{
					result = remoteCharacter2;
				}
				else if (null != remoteCharacter)
				{
					result = remoteCharacter;
				}
			}
		}
		return result;
	}

	private void ScreenshotInput()
	{
		if (Input.GetKey(KeyCode.F11))
		{
			Application.CaptureScreenshot("Immune_screenshot_" + (int)Time.time + ".png");
		}
		else if (Input.GetKey(KeyCode.F12))
		{
			Application.CaptureScreenshot("Immune_screenshot_2x_" + (int)Time.time + ".png", 2);
		}
	}

	public float GetBuildRot()
	{
		return this.m_buildRot;
	}

	public bool IsAttacking()
	{
		return !this.m_inventory.IsVisible() && !this.m_communicator.IsActive(true) && (this.m_stopAttackingTime > Time.time || Input.GetButton("Attack"));
	}

	private bool IsInteracting()
	{
		bool flag = false;
		if (Input.GetButtonDown("Interact") && !this.m_isMoving)
		{
			for (int i = 0; i < this.m_repairNpcs.Length; i++)
			{
				Vector3 position = this.m_repairNpcs[i].transform.position;
				Vector3 pos = this.m_client.GetPos();
				if (Mathf.Abs(position.x - pos.x) < 1.4f && Mathf.Abs(position.z - pos.z) < 1.4f)
				{
					RemoteItem handItem = this.m_inventory.GetHandItem();
					if (null != handItem)
					{
						if (Items.HasCondition(handItem.m_type) && handItem.m_amountOrCond < 100)
						{
							ItemDef itemDef = Items.GetItemDef(handItem.m_type);
							int num = (int)(1f + Items.GetValue(handItem.m_type, 100) * 0.01f * (float)(100 - handItem.m_amountOrCond));
							num = (int)((float)num * this.m_repairNpcs[i].m_priceMultip + 0.5f);
							if (this.m_client.GetGoldCount() >= num)
							{
								string a_caption = string.Concat(new object[]
								{
									LNG.Get("REPAIR"),
									"\n",
									LNG.Get(itemDef.ident),
									"\n",
									handItem.m_amountOrCond,
									"%\nfor ",
									num,
									" ",
									LNG.Get("CURRENCY"),
									"?"
								});
								this.m_repairPopupSessionId = this.m_popupGui.ShowGui(true, a_caption);
							}
							else
							{
								string a_caption2 = string.Concat(new object[]
								{
									LNG.Get("ITEMSHOP_TOO_LESS_GOLD"),
									"\n",
									num,
									" ",
									LNG.Get("CURRENCY")
								});
								this.m_popupGui.ShowGui(true, a_caption2);
							}
						}
						else
						{
							this.m_popupGui.ShowGui(true, LNG.Get("REPAIR_NPC_NO_NEED"));
						}
					}
					else
					{
						this.m_popupGui.ShowGui(true, LNG.Get("REPAIR_NPC_HOWTO"));
					}
					flag = true;
					break;
				}
			}
			if (!flag)
			{
			}
		}
		return (Input.GetButton("Interact") && !flag) || this.m_forceInteractionTime > Time.time;
	}

	public string GetMissionText(Mission a_mission)
	{
		string text = (a_mission.m_type != eMissiontype.eDestroy) ? LNG.Get("MISSION_PERSON_" + a_mission.m_objPerson.ToString("d")) : LNG.Get("MISSION_OBJECT_" + a_mission.m_objObject.ToString("d"));
		return string.Concat(new string[]
		{
			LNG.Get("MISSION"),
			":\n",
			LNG.Get("MISSION_TYPE_" + a_mission.m_type.ToString("d")),
			" ",
			text,
			"\n\n",
			LNG.Get("LOCATION"),
			": ",
			LNG.Get("MISSION_LOCATION_" + a_mission.m_location.ToString("d")),
			"\n",
			LNG.Get("REWARD"),
			": ",
			a_mission.m_xpReward.ToString("d"),
			" XP\n"
		});
	}

	public void ShowMissionPopup(Mission a_mission)
	{
		this.m_missionPopupSessionId = this.m_popupGui.ShowGui(true, this.GetMissionText(a_mission));
	}

	public bool IsAttackingWithMouse()
	{
		return !this.m_inventory.IsVisible() && !this.m_communicator.IsActive(true) && this.m_stopAttackingTime > Time.time;
	}

	public RemoteCharacter GetTarget()
	{
		return this.m_currentTarget;
	}

	public void SplitItem(RemoteItem a_item)
	{
		if (null != a_item && 1 < a_item.m_amountOrCond)
		{
			this.m_sendDragPos = this.m_inventory.ToWorldPos(a_item.transform.localPosition);
			this.m_sendDropPos = Vector3.one * 254f;
		}
	}

	public void ConsumeItem(RemoteItem a_item)
	{
		if (null != a_item && (Items.IsEatable(a_item.m_type) || Items.IsMedicine(a_item.m_type)))
		{
			this.m_sendDragPos = this.m_inventory.ToWorldPos(a_item.transform.localPosition);
			this.m_sendDropPos = Vector3.one * 253f;
			if (!Items.IsMedicine(a_item.m_type))
			{
				base.audio.clip = ((!Items.IsBeverage(a_item.m_type)) ? this.m_soundFood : this.m_soundBeverage);
				base.audio.Play();
				if (UnityEngine.Random.Range(0, 20) == 0)
				{
					this.m_burpFlag = true;
				}
			}
		}
	}

	private void GetMouseInput(float a_deltaTime)
	{
		if (Time.timeSinceLevelLoad < 0.5f)
		{
			return;
		}
		bool flag = !this.m_inventory.IsVisible() && !this.m_communicator.IsActive(true) && false == this.m_popupGui.IsActive();
		Vector3 mousePosition = Input.mousePosition;
		bool flag2 = (this.m_lastMousePos - mousePosition).sqrMagnitude > 4f;
		this.m_lastMousePos = mousePosition;
		Ray ray = Camera.main.ScreenPointToRay(mousePosition);
		if (flag2)
		{
			this.m_hideCursorTime = Time.time + 1f;
		}
		bool flag3 = Time.time < this.m_hideCursorTime;
		if (flag3 != Screen.showCursor)
		{
			Screen.showCursor = flag3;
		}
		if (Input.GetMouseButton(1) && flag)
		{
			this.m_buildRot += a_deltaTime * 90f;
			while (this.m_buildRot > 360f)
			{
				this.m_buildRot -= 360f;
			}
		}
		if (Input.GetMouseButtonDown(1))
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, 100f, 665600))
			{
				int layer = raycastHit.transform.gameObject.layer;
				if (layer != 13)
				{
					if (layer == 17)
					{
						if (!this.m_inventory.IsShopActive())
						{
							RemoteItem component = raycastHit.transform.GetComponent<RemoteItem>();
							if (null != component && this.m_inventory.IsVisible())
							{
								this.m_itemGui.Show(component, ray.GetPoint(4.5f));
							}
						}
					}
				}
				else if (flag)
				{
					RemoteCharacter component2 = raycastHit.transform.GetComponent<RemoteCharacter>();
					if (null != component2 && null != this.m_client)
					{
						ulong steamId = this.m_client.GetSteamId(component2.m_id);
						if (Global.isSteamActive && 0UL < steamId)
						{
							SteamFriends.ActivateGameOverlayToUser("steamid", new CSteamID(steamId));
						}
					}
				}
			}
		}
		else if (Input.GetMouseButtonDown(0))
		{
			if (flag)
			{
				this.ResetTarget();
			}
			bool flag4 = false;
			RaycastHit raycastHit2;
			if (Physics.Raycast(ray, out raycastHit2, 100f, 6995488))
			{
				switch (raycastHit2.transform.gameObject.layer)
				{
				case 9:
				case 13:
					if (flag)
					{
						RemoteCharacter component3 = raycastHit2.transform.GetComponent<RemoteCharacter>();
						if (null != component3)
						{
							if (!component3.m_isOwnPlayer)
							{
								this.m_currentTarget = component3;
								flag4 = true;
							}
						}
						else if (flag && Input.GetAxis("Vertical") == 0f && Input.GetAxis("Horizontal") == 0f)
						{
							this.CalculatePath(raycastHit2.point, 12 != raycastHit2.transform.gameObject.layer);
						}
					}
					break;
				case 10:
				case 12:
				case 21:
					if (flag && Input.GetAxis("Vertical") == 0f && Input.GetAxis("Horizontal") == 0f && null != this.m_client && !this.m_client.IsInVehicle())
					{
						Vector3 point = raycastHit2.point;
						this.m_walkIndicator.position = new Vector3(raycastHit2.point.x, 0.1f, raycastHit2.point.z);
						this.CalculatePath(point, 12 != raycastHit2.transform.gameObject.layer);
					}
					break;
				case 11:
				case 15:
				case 19:
				case 22:
					flag4 = flag;
					break;
				case 17:
					if (this.m_inventory.IsShopActive())
					{
						RemoteItem component4 = raycastHit2.transform.GetComponent<RemoteItem>();
						if (null != component4 && component4.m_type != 254 && this.m_inventory.IsVisible())
						{
							this.m_buySellPos = this.m_inventory.ToWorldPos(component4.transform.localPosition);
							bool flag5 = this.m_buySellPos.x < 6f;
							float num = (!flag5) ? this.m_inventory.GetShopBuyMultiplier() : this.m_inventory.GetShopSellMultiplier();
							string text = (!flag5) ? LNG.Get("BUY") : LNG.Get("SELL");
							int num2 = (int)(Items.GetValue(component4.m_type, component4.m_amountOrCond) * num + 0.5f);
							string text2 = (!Items.HasCondition(component4.m_type)) ? ("x " + component4.m_amountOrCond) : (component4.m_amountOrCond.ToString() + "%");
							ItemDef itemDef = Items.GetItemDef(component4.m_type);
							if (flag5 || this.m_client.GetGoldCount() >= num2)
							{
								string a_caption = string.Concat(new object[]
								{
									text,
									"\n",
									LNG.Get(itemDef.ident),
									"\n",
									text2,
									"\nfor ",
									num2,
									" ",
									LNG.Get("CURRENCY"),
									"?"
								});
								this.m_buySellPopupSessionId = this.m_popupGui.ShowGui(true, a_caption);
							}
							else
							{
								string a_caption2 = string.Concat(new object[]
								{
									LNG.Get("ITEMSHOP_TOO_LESS_GOLD"),
									"\n",
									num2,
									" ",
									LNG.Get("CURRENCY")
								});
								this.m_popupGui.ShowGui(true, a_caption2);
							}
						}
					}
					else if (Time.time < this.m_doubleClickTime)
					{
						this.m_doubleClickTime = 0f;
						if (null != raycastHit2.transform)
						{
							Vector3 localPosition = raycastHit2.transform.localPosition;
							Vector3 zero = Vector3.zero;
							if (this.m_inventory.DragDrop(ref localPosition, ref zero))
							{
								this.m_sendDragPos = localPosition;
								this.m_sendDropPos = Vector3.zero;
							}
						}
					}
					else
					{
						this.m_dragItem = raycastHit2.transform;
						this.m_startDragPos = this.m_dragItem.localPosition;
					}
					break;
				}
			}
			if (flag4)
			{
				this.CalculateRotTowardsMouse(mousePosition);
				this.m_stopAttackingTime = Time.time + 0.3f;
			}
		}
		else if (Input.GetMouseButtonUp(0) && null != this.m_dragItem)
		{
			Vector3 startDragPos = this.m_startDragPos;
			Vector3 localPosition2 = this.m_dragItem.localPosition;
			if (this.m_inventory.DragDrop(ref startDragPos, ref localPosition2))
			{
				if (startDragPos != localPosition2)
				{
					this.m_sendDragPos = startDragPos;
					this.m_sendDropPos = localPosition2;
				}
				else
				{
					this.m_dragItem.localPosition = this.m_startDragPos;
					this.m_doubleClickTime = Time.time + 0.5f;
				}
			}
			this.m_dragItem = null;
		}
		if (null != this.m_dragItem)
		{
			this.m_dragItem.position = ray.GetPoint(5f);
			Vector3 localPosition3 = this.m_dragItem.localPosition;
			localPosition3.z = 0f;
			this.m_dragItem.localPosition = localPosition3;
		}
		else if (!flag2)
		{
			if (this.m_mouseOverDur != -1f)
			{
				this.m_mouseOverDur += a_deltaTime;
				if (this.m_mouseOverDur > 0.1f)
				{
					this.m_buildingHealthIndicator.position = Vector3.up * 1000f;
					this.m_tooltip.position = Vector3.up * 1000f;
					this.m_tooltipHudR.position = Vector3.up * 1000f;
					this.m_tooltipHudR.parent = null;
					this.m_tooltipHudL.position = Vector3.up * 1000f;
					this.m_tooltipHudL.parent = null;
					if (this.m_mouseOverRenderers != null && this.m_mouseOverRenderers.Length != 0)
					{
						foreach (Renderer renderer in this.m_mouseOverRenderers)
						{
							if (null != renderer)
							{
								renderer.gameObject.layer = this.m_mouseOverLayer;
							}
						}
						this.m_mouseOverRenderers = null;
					}
					if (null != this.m_mouseOverTransform)
					{
						this.m_mouseOverTransform.localScale = this.m_initialMouseOverScale;
						this.m_mouseOverTransform = null;
					}
					RaycastHit raycastHit3;
					if (Physics.Raycast(ray, out raycastHit3, 100f, 7007776))
					{
						if (raycastHit3.transform.gameObject.layer == 5)
						{
							string[] array = raycastHit3.transform.gameObject.name.Split(new char[]
							{
								'-'
							});
							if (array != null && 1 < array.Length)
							{
								if ("tooltip" == array[0])
								{
									this.m_tooltipHudRText.text = LNG.Get(array[1]);
									this.m_tooltipHudR.position = raycastHit3.transform.position - raycastHit3.transform.right * 0.3f;
									this.m_tooltipHudR.rotation = raycastHit3.transform.rotation;
									this.m_tooltipHudR.parent = raycastHit3.transform;
								}
								else if ("mission" == array[0])
								{
									int a_index = 0;
									try
									{
										a_index = int.Parse(array[1]);
									}
									catch (Exception ex)
									{
										Debug.LogWarning("ClientInput.cs: " + ex.ToString());
									}
									Mission mission = this.m_client.GetMission(a_index);
									if (mission != null)
									{
										this.m_tooltipHudLText.text = string.Concat(new object[]
										{
											this.GetMissionText(mission),
											LNG.Get("TIME_LEFT"),
											": ",
											(int)(mission.m_dieTime / 60f),
											" min"
										});
										this.m_tooltipHudL.position = raycastHit3.transform.position + raycastHit3.transform.right * 0.3f;
										this.m_tooltipHudL.rotation = raycastHit3.transform.rotation;
										this.m_tooltipHudL.parent = raycastHit3.transform;
									}
								}
							}
						}
						else
						{
							this.m_mouseOverTransform = raycastHit3.transform;
							this.m_initialMouseOverScale = this.m_mouseOverTransform.localScale;
							if (raycastHit3.transform.gameObject.layer == 17)
							{
								if (this.m_inventory.IsVisible())
								{
									this.m_inventory.ShowInfo(raycastHit3.transform.position);
								}
							}
							else if (raycastHit3.transform.gameObject.layer == 10)
							{
								this.m_mouseOverTransform.localScale *= 1.33f;
							}
							else if (raycastHit3.transform.gameObject.layer == 19)
							{
								RemoteBuilding component5 = raycastHit3.transform.parent.GetComponent<RemoteBuilding>();
								if (null != this.m_buildingHealthIndicator && null != component5)
								{
									Vector3 b = Vector3.up * 4f;
									float x = 0.25f * (float)(3 - (int)(component5.m_health * 0.033f));
									this.m_buildingHealthIndicator.renderer.material.mainTextureOffset = new Vector2(x, 0f);
									this.m_buildingHealthIndicator.position = raycastHit3.transform.position + b;
								}
							}
							else if (raycastHit3.transform.gameObject.layer == 15)
							{
								bool flag6 = "building_10" == raycastHit3.transform.gameObject.name || "building_11" == raycastHit3.transform.gameObject.name;
								Vector3 b2 = Vector3.up * ((!flag6) ? 3f : 6.5f);
								this.m_tooltipText.text = LNG.Get("TOOLTIP_RESOURCE");
								this.m_tooltip.position = raycastHit3.transform.position + b2;
							}
							else if (raycastHit3.transform.gameObject.layer == 21)
							{
								Vector3 b3 = Vector3.up * 2f;
								this.m_tooltipText.text = LNG.Get("TOOLTIP_ITEMSTORAGE");
								this.m_tooltip.position = raycastHit3.transform.position + b3;
							}
							else if (raycastHit3.transform.gameObject.layer == 9)
							{
								RemoteCharacter component6 = raycastHit3.transform.GetComponent<RemoteCharacter>();
								if (null == component6)
								{
									Vector3 b4 = Vector3.up * 3f;
									this.m_tooltipText.text = LNG.Get("TOOLTIP_INTERACT");
									this.m_tooltip.position = raycastHit3.transform.position + b4;
								}
							}
							else if (raycastHit3.transform.gameObject.layer == 22)
							{
								MissionObjective component7 = raycastHit3.transform.GetComponent<MissionObjective>();
								if (null != component7)
								{
									Vector3 b5 = Vector3.up * 3f;
									this.m_tooltipText.text = LNG.Get("MISSION_TYPE_" + component7.m_type.ToString("d"));
									this.m_tooltip.position = raycastHit3.transform.position + b5;
								}
							}
							this.m_mouseOverRenderers = raycastHit3.transform.GetComponentsInChildren<Renderer>();
							if (this.m_mouseOverRenderers.Length == 0 && null != raycastHit3.transform.parent)
							{
								this.m_mouseOverRenderers = raycastHit3.transform.parent.GetComponentsInChildren<Renderer>();
							}
							if (this.m_mouseOverRenderers.Length != 0)
							{
								this.m_mouseOverLayer = this.m_mouseOverRenderers[0].gameObject.layer;
								foreach (Renderer renderer2 in this.m_mouseOverRenderers)
								{
									renderer2.gameObject.layer = 20;
								}
							}
						}
					}
					this.m_mouseOverDur = -1f;
				}
			}
		}
		else
		{
			this.m_mouseOverDur = 0f;
		}
	}

	private bool CalculatePath(Vector3 a_target, bool a_interact)
	{
		if (null != this.m_client && !this.m_client.IsInVehicle())
		{
			a_target.y = 0f;
			if (NavMesh.CalculatePath(this.m_client.GetPos(), a_target, -1, this.m_path))
			{
				if (this.m_path.corners.Length > 0 && (this.m_path.corners[this.m_path.corners.Length - 1] - a_target).sqrMagnitude > 1f)
				{
					this.m_pathCorners = new Vector3[2];
					this.m_pathCorners[0] = this.m_client.GetPos();
					this.m_pathCorners[1] = a_target;
				}
				else
				{
					this.m_pathCorners = this.m_path.corners;
				}
				this.m_nextPathPoint = 1;
				this.m_interactionAtPathEnd = a_interact;
				return true;
			}
		}
		return false;
	}

	private void CalculateRotTowardsMouse(Vector3 a_mousePos)
	{
		this.m_rotTowardsMousePos = -1f;
		if (!this.m_inventory.IsVisible() && !this.m_communicator.IsActive(true) && null != this.m_client && null != Camera.main)
		{
			Vector3 b = Camera.main.WorldToScreenPoint(this.m_client.GetPos() + Vector3.up);
			this.m_rotTowardsMousePos = Vector3.Angle((a_mousePos - b).normalized, Vector3.up);
			if (a_mousePos.x < b.x)
			{
				this.m_rotTowardsMousePos = 360f - this.m_rotTowardsMousePos;
			}
		}
	}

	private void ResetTarget()
	{
		this.m_rotTowardsMousePos = -1f;
		this.m_currentTarget = null;
		if (null != this.m_bullsEye)
		{
			this.m_bullsEye.position = Vector3.one * 1000f;
		}
	}

	private const int c_uiLayer = 5;

	private const int c_blockInputLayer = 14;

	private const int c_groundLayer = 12;

	private const int c_playerLayer = 13;

	private const int c_npcLayer = 9;

	private const int c_itemLayer = 10;

	private const int c_containerItemLayer = 17;

	private const int c_buildingLayer = 19;

	private const int c_resourceLayer = 15;

	private const int c_mouseOverLayer = 20;

	private const int c_itemStorageLayer = 21;

	private const int c_missionLayer = 22;

	private const int c_vehicleLayer = 11;

	private const int c_moSpecialLayers = 7007776;

	private const int c_lmbSpecialLayers = 6995488;

	private const int c_rmbSpecialLayers = 665600;

	public float m_minSendIntervall = 0.1f;

	public Transform m_bullsEye;

	public Transform m_buildingHealthIndicator;

	public Transform m_walkIndicator;

	public Transform m_tooltip;

	private TextMesh m_tooltipText;

	public Transform m_tooltipHudR;

	private TextMesh m_tooltipHudRText;

	public Transform m_tooltipHudL;

	private TextMesh m_tooltipHudLText;

	public AudioClip m_soundBeverage;

	public AudioClip m_soundFood;

	public AudioClip m_soundBurp;

	private float m_hideCursorTime = 3f;

	private float m_doubleClickTime;

	private bool m_burpFlag;

	private int m_input;

	private LidClient m_client;

	private InventoryGUI m_inventory;

	private QmunicatorGUI m_communicator;

	private ItemGUI m_itemGui;

	private float m_nextInputTime;

	private PopupGUI m_popupGui;

	private int m_buySellPopupSessionId = -1;

	private int m_repairPopupSessionId = -1;

	private int m_missionPopupSessionId = -1;

	private float m_stopAttackingTime;

	private float m_rotTowardsMousePos;

	private float m_attackNoticeTime;

	private RemoteCharacter m_currentTarget;

	private Transform m_dragItem;

	private Vector3 m_startDragPos = Vector3.zero;

	private Vector3 m_lastMousePos = Vector3.zero;

	private float m_mouseOverDur;

	private Vector3 m_sendDragPos = Vector3.zero;

	private Vector3 m_sendDropPos = Vector3.zero;

	private Vector3 m_buySellPos = Vector3.zero;

	private Vector3 m_invalidPos = Vector3.one * -1f;

	private Transform m_mouseOverTransform;

	private Vector3 m_initialMouseOverScale = Vector3.one;

	private Renderer[] m_mouseOverRenderers;

	private int m_mouseOverLayer;

	private RepairingNpc[] m_repairNpcs;

	private float m_buildRot;

	private NavMeshPath m_path;

	private Vector3[] m_pathCorners;

	private int m_nextPathPoint;

	private float m_vertAxis;

	private float m_horiAxis;

	private bool m_isMoving;

	private bool m_interactionAtPathEnd;

	private float m_forceInteractionTime;
}

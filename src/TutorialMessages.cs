using System;
using UnityEngine;

public class TutorialMessages : MonoBehaviour
{
	public TutorialMessages()
	{
	}

	private void Start()
	{
		this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
		this.m_msgBar = base.GetComponent<MessageBarGUI>();
		this.m_inventoryGui = (InventoryGUI)UnityEngine.Object.FindObjectOfType(typeof(InventoryGUI));
	}

	private void Update()
	{
		if (null != this.m_client)
		{
			if (string.Empty != this.m_client.m_notificationMsg)
			{
				this.m_msgBar.DisplayMessage(this.m_client.m_notificationMsg, 1000);
				this.m_client.m_notificationMsg = string.Empty;
			}
			if (null != this.m_client && PlayerPrefs.GetInt("prefHints", 1) == 1 && 0f < this.m_client.GetHealth() && Time.timeSinceLevelLoad > 20f)
			{
				for (int i = 0; i < 11; i++)
				{
					if (Time.time > this.m_nextMsgShowTime[i] && this.DisplayMessage((TutorialMessages.eMsg)i))
					{
						this.m_nextMsgShowTime[i] = Time.time + this.m_displayIntervall;
						break;
					}
				}
			}
		}
	}

	private bool DisplayMessage(TutorialMessages.eMsg a_msg)
	{
		bool flag = false;
		int a_prio = 100;
		Vector3 pos = this.m_client.GetPos();
		switch (a_msg)
		{
		case TutorialMessages.eMsg.ePickupItem:
		{
			RemoteItem nearestItem = this.m_client.GetNearestItem(pos);
			flag = (null != nearestItem && 25f > (nearestItem.transform.position - pos).sqrMagnitude);
			break;
		}
		case TutorialMessages.eMsg.eEatFood:
			flag = (this.m_inventoryGui.IsVisible() && this.m_inventoryGui.HasFood());
			break;
		case TutorialMessages.eMsg.eDriveCar:
		{
			RemoteCharacter nearestCharacter = this.m_client.GetNearestCharacter(pos, true);
			flag = (null != nearestCharacter && 36f > (nearestCharacter.transform.position - pos).sqrMagnitude);
			break;
		}
		case TutorialMessages.eMsg.eGatherResource:
		{
			RemoteBuilding nearestResource = this.m_client.GetNearestResource(pos);
			flag = (null != nearestResource && 25f > (nearestResource.transform.position - pos).sqrMagnitude);
			break;
		}
		case TutorialMessages.eMsg.eAttackEnemy:
		{
			RemoteCharacter nearestNpc = this.m_client.GetNearestNpc(pos);
			flag = (null != nearestNpc && 36f > (nearestNpc.transform.position - pos).sqrMagnitude);
			break;
		}
		case TutorialMessages.eMsg.eStarving:
			flag = (0f == this.m_client.GetEnergy());
			a_prio = 110;
			break;
		case TutorialMessages.eMsg.eChat:
		case TutorialMessages.eMsg.ePlayerProfile:
		{
			RemoteCharacter nearestCharacter2 = this.m_client.GetNearestCharacter(pos, false);
			flag = (null != nearestCharacter2 && 49f > (nearestCharacter2.transform.position - pos).sqrMagnitude);
			break;
		}
		case TutorialMessages.eMsg.eBuildBuilding:
			flag = (this.m_inventoryGui.IsVisible() && this.m_inventoryGui.HasBuilding());
			break;
		case TutorialMessages.eMsg.eShovel:
			flag = (this.m_inventoryGui.IsVisible() && this.m_inventoryGui.HasItemType(109));
			break;
		case TutorialMessages.eMsg.eBuildBuilding2:
			flag = (!this.m_inventoryGui.IsVisible() && Items.GetItemDef(this.m_client.GetHandItem()).buildingIndex > 0);
			break;
		}
		if (flag)
		{
			this.m_msgBar.DisplayMessage(LNG.Get("TUTORIAL_MESSAGE_" + (int)a_msg), a_prio);
		}
		return flag;
	}

	public float m_displayIntervall = 120f;

	private float[] m_nextMsgShowTime = new float[11];

	private MessageBarGUI m_msgBar;

	private LidClient m_client;

	private InventoryGUI m_inventoryGui;

	public enum eMsg
	{
		ePickupItem,
		eEatFood,
		eDriveCar,
		eGatherResource,
		eAttackEnemy,
		eStarving,
		eChat,
		eBuildBuilding,
		ePlayerProfile,
		eShovel,
		eBuildBuilding2,
		eMsgCount
	}
}

using System;
using UnityEngine;

public class ItemGUI : MonoBehaviour
{
	public ItemGUI()
	{
	}

	public void Show(RemoteItem a_item, Vector3 a_pos)
	{
		this.m_item = a_item;
		base.transform.position = a_pos;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.z = 4.5f;
		base.transform.localPosition = localPosition;
		this.ShowGui(Items.IsEatable(this.m_item.m_type) || Items.IsMedicine(this.m_item.m_type), Items.IsStackable(this.m_item.m_type) && 1 < this.m_item.m_amountOrCond);
		this.m_lastShowTime = Time.time;
	}

	public void Hide()
	{
		this.ShowGui(false, false);
	}

	private void ShowGui(bool a_consumable, bool a_splitable)
	{
		this.m_guiBlock.SetActive(a_consumable || a_splitable);
		this.m_guiEatSplit.SetActive(a_consumable && a_splitable);
		this.m_guiEat.SetActive(a_consumable && !a_splitable);
		this.m_guiSplit.SetActive(!a_consumable && a_splitable);
	}

	private void Start()
	{
		this.m_guimaster = (GUI3dMaster)UnityEngine.Object.FindObjectOfType(typeof(GUI3dMaster));
		this.m_input = (ClientInput)UnityEngine.Object.FindObjectOfType(typeof(ClientInput));
	}

	private void LateUpdate()
	{
		if (null != this.m_guimaster)
		{
			string clickedButtonName = this.m_guimaster.GetClickedButtonName();
			if (string.Empty != clickedButtonName)
			{
				if (this.m_btnSplitName == clickedButtonName)
				{
					this.m_input.SplitItem(this.m_item);
					this.Hide();
				}
				else if (this.m_btnEatName == clickedButtonName)
				{
					this.m_input.ConsumeItem(this.m_item);
					this.Hide();
				}
			}
		}
		if (Input.anyKeyDown && Time.time > this.m_lastShowTime + 0.5f)
		{
			this.Hide();
		}
	}

	public string m_btnSplitName = "todo";

	public string m_btnEatName = "todo";

	public GameObject m_guiBlock;

	public GameObject m_guiEatSplit;

	public GameObject m_guiEat;

	public GameObject m_guiSplit;

	private GUI3dMaster m_guimaster;

	private RemoteItem m_item;

	private ClientInput m_input;

	private float m_lastShowTime;
}

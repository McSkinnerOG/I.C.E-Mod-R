using System;
using UnityEngine;

public class RemoteItem : MonoBehaviour
{
	public RemoteItem()
	{
	}

	public void Refresh()
	{
		if (!this.m_visible && !this.m_isInventoryOrContainerItem)
		{
			this.SwitchVisibility();
		}
		this.m_lastUpdate = Time.time;
	}

	public void Init(Vector3 a_pos, int a_type, int a_amount, bool a_isContainerItem)
	{
		this.m_type = a_type;
		bool flag = Items.IsStackable(this.m_type);
		a_pos.y = 0.1f;
		base.transform.position = a_pos;
		this.m_amountOrCond = a_amount;
		this.m_isInventoryOrContainerItem = a_isContainerItem;
		this.m_isInventoryItem = (this.m_isInventoryOrContainerItem && base.transform.position.x < 5f);
		this.InstantiateItem(false);
		if (this.m_isInventoryOrContainerItem)
		{
			base.transform.localScale = Vector3.one * this.m_inventoryScale;
			if (Items.HasAmountOrCondition(this.m_type))
			{
				this.CreateLabel(this.m_labelAmountPrefab, this.m_amountLabelOffset, a_amount.ToString() + ((!flag) ? "%" : string.Empty));
			}
		}
		else
		{
			base.transform.localScale = Vector3.one * this.m_worldScale;
		}
		if (null != base.audio && Time.timeSinceLevelLoad > 5f)
		{
			base.audio.Play();
		}
		this.m_lastUpdate = Time.time;
	}

	public void CreateLabel(GameObject a_go, Vector3 a_offset, string a_caption)
	{
		if (null != a_go)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(a_go);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = a_offset;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localRotation = Quaternion.identity;
			EasyFontTextMesh component = gameObject.GetComponent<EasyFontTextMesh>();
			if (null != component)
			{
				component.Text = a_caption;
			}
		}
	}

	private void Update()
	{
		if (!this.m_isInventoryOrContainerItem)
		{
			if (this.m_visible)
			{
				if (this.m_lastUpdate + this.m_disappearTime < Time.time)
				{
					this.SwitchVisibility();
				}
			}
			else if (this.m_lastUpdate + this.m_dieTime < Time.time)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		if (null != this.m_riseItemEffect)
		{
			float num = 20f * Time.deltaTime;
			this.m_riseItemEffect.transform.position += new Vector3(0f, num, num * -0.3f);
			if (this.m_riseItemEffect.transform.position.y > 6f)
			{
				UnityEngine.Object.Destroy(this.m_riseItemEffect);
			}
		}
	}

	private GameObject InstantiateItem(bool a_justForEffect = false)
	{
		GameObject gameObject = (GameObject)Resources.Load("items/item_" + this.m_type);
		GameObject gameObject2;
		if (null != gameObject)
		{
			float x = 90f;
			Quaternion rotation = (!this.m_isInventoryOrContainerItem) ? Quaternion.Euler(x, UnityEngine.Random.Range(0f, 360f), 0f) : Quaternion.identity;
			gameObject2 = (GameObject)UnityEngine.Object.Instantiate(gameObject, base.transform.position, rotation);
			gameObject2.transform.parent = base.transform;
			Util.SetLayerRecursively(base.transform, (!this.m_isInventoryOrContainerItem) ? 10 : 17);
			if (!a_justForEffect)
			{
				this.m_renderers = gameObject2.GetComponentsInChildren<Renderer>();
			}
			Transform transform = gameObject2.transform.FindChild("Particles");
			if (null != transform)
			{
				transform.gameObject.SetActive(false);
			}
			Transform transform2 = gameObject2.transform.FindChild("Point light");
			if (null != transform2)
			{
				transform2.gameObject.SetActive(false);
			}
		}
		else
		{
			gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
			gameObject2.transform.position = base.transform.position;
			gameObject2.transform.localScale = Vector3.one * 0.5f;
			gameObject2.transform.parent = base.transform;
		}
		return gameObject2;
	}

	public bool IsVisible()
	{
		return this.m_visible;
	}

	public void SwitchVisibility()
	{
		this.m_visible = !this.m_visible;
		if (this.m_renderers != null)
		{
			foreach (Renderer renderer in this.m_renderers)
			{
				if (null != renderer)
				{
					renderer.enabled = this.m_visible;
				}
			}
		}
		if (!this.m_visible && !this.m_isInventoryOrContainerItem)
		{
			this.m_riseItemEffect = this.InstantiateItem(true);
		}
	}

	public GameObject m_labelAmountPrefab;

	public GameObject m_labelPricePrefab;

	public Vector3 m_amountLabelOffset = new Vector3(0.45f, -0.45f, 0.2f);

	public Vector3 m_priceLabelOffset = new Vector3(0.45f, 0.45f, 0.2f);

	public float m_inventoryScale = 0.26f;

	public float m_worldScale = 1.3f;

	[HideInInspector]
	public int m_type;

	[HideInInspector]
	public int m_amountOrCond;

	[HideInInspector]
	public bool m_isInventoryItem;

	[HideInInspector]
	public bool m_isInventoryOrContainerItem;

	private Renderer[] m_renderers;

	private GameObject m_riseItemEffect;

	private bool m_visible;

	private float m_lastUpdate;

	private float m_disappearTime = 0.5f;

	private float m_dieTime = 10f;
}

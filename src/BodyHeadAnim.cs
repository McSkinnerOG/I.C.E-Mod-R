using System;
using UnityEngine;

public class BodyHeadAnim : MonoBehaviour
{
	public BodyHeadAnim()
	{
	}

	private void Awake()
	{
		if (Global.isServer)
		{
			UnityEngine.Object.DestroyImmediate(this);
			return;
		}
		this.m_isMale = (!(null != this.m_bodyPrefab) || !this.m_bodyPrefab.name.Contains("female"));
		GameObject gameObject = this.SetupPart(this.m_bodyPrefab, base.transform, false);
		if (null != gameObject)
		{
			this.m_bodyRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
			if (null != this.m_bodyTexture)
			{
				this.m_bodyRenderer.material.mainTexture = this.m_bodyTexture;
			}
			else
			{
				this.m_bodyTexture = this.m_bodyRenderer.material.mainTexture;
			}
			this.m_boneHand.bone = gameObject.transform.Find(this.m_boneHand.name);
			this.m_boneArm.bone = gameObject.transform.Find(this.m_boneArm.name);
			this.m_boneBody.bone = gameObject.transform.Find(this.m_boneBody.name);
			this.m_boneHead.bone = gameObject.transform.Find(this.m_boneHead.name);
			if (null != this.m_boneBody.bone && null != this.m_rucksackPrefab)
			{
				GameObject gameObject2 = this.SetupPart(this.m_rucksackPrefab, this.m_boneBody.bone, false);
				gameObject2.transform.localPosition = new Vector3(0.11f, -0.14f, 0f);
				gameObject2.transform.localRotation = Quaternion.Euler(270f, 90f, 0f);
			}
			if (null != this.m_boneHead.bone)
			{
				this.m_boneHead.lookPart = this.SetupPart(this.m_headPrefab, this.m_boneHead.bone, false);
				this.m_boneHead.lookPart.transform.localPosition = new Vector3(-0.04f, 0.02f, 0f);
				this.m_boneHead.lookPart.transform.localRotation = Quaternion.Euler(0f, 270f, 180f);
				this.m_headRenderer = this.m_boneHead.lookPart.GetComponentInChildren<Renderer>();
				this.m_headTexture = this.m_headRenderer.material.mainTexture;
			}
			this.m_animator = gameObject.GetComponentInChildren<Animator>();
			this.m_animator.runtimeAnimatorController = this.m_animController;
			this.m_animator.Rebind();
		}
		this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
		this.m_lastPos = base.transform.position;
		this.m_sound = base.GetComponent<CharSounds>();
		this.ChangeHandItem(this.m_defaultHandItemType);
		this.ChangeHeadItem(this.m_defaultHeadItemType);
		this.ChangeBodyItem(this.m_defaultBodyItemType);
		this.ChangeSkin(this.m_defaultSkinItemType);
	}

	public void Init(bool a_isOwnPlayer)
	{
		if (a_isOwnPlayer)
		{
			this.m_input = (ClientInput)UnityEngine.Object.FindObjectOfType(typeof(ClientInput));
		}
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		Vector3 vector = base.transform.position - this.m_lastPos;
		this.m_lastPos = base.transform.position;
		float num = vector.magnitude / deltaTime;
		float to = (num <= 1f) ? 0f : Mathf.Clamp01(num / this.m_maxSpeed);
		this.m_animator.SetFloat("speed", Mathf.Lerp(this.m_animator.GetFloat("speed"), to, deltaTime * 6f));
	}

	private void LateUpdate()
	{
		if (this.m_isTakingAction && (null == this.m_input || this.m_input.IsAttacking()))
		{
			this.AnimateAttacking();
			if ((this.m_handItemDef.ammoItemType > 0 || this.m_handItemDef.buildingIndex > 0) && null != this.m_boneArm.bone)
			{
				this.m_boneArm.bone.localRotation = Quaternion.Euler(17f, 288f, 336f);
			}
		}
		this.m_animator.SetBool("attack", this.m_playAttackAnimTime > Time.time);
		this.m_animator.SetBool("sit", this.m_isSitting);
		if (null != this.m_animator)
		{
			this.m_animator.transform.localPosition = Vector3.zero;
		}
		this.HandleHologram();
	}

	private GameObject SetupPart(GameObject a_part, Transform a_parent, bool a_optional)
	{
		GameObject gameObject = null;
		if (null != a_part && null != a_parent)
		{
			gameObject = (GameObject)UnityEngine.Object.Instantiate(a_part);
			Collider[] componentsInChildren = gameObject.GetComponentsInChildren<Collider>();
			NavMeshObstacle[] componentsInChildren2 = gameObject.GetComponentsInChildren<NavMeshObstacle>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren[i]);
			}
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				UnityEngine.Object.Destroy(componentsInChildren2[j]);
			}
			if (a_optional)
			{
				gameObject.transform.parent = a_parent;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				Transform transform = gameObject.transform.FindChild("Handle");
				if (null != transform)
				{
					gameObject.transform.localPosition = transform.localPosition * -0.61f;
				}
				Transform transform2 = gameObject.transform.FindChild("Particles");
				if (null != transform2)
				{
					transform2.gameObject.SetActive(true);
				}
			}
			else
			{
				gameObject.transform.position = base.transform.position;
				gameObject.transform.rotation = base.transform.rotation;
				gameObject.transform.parent = a_parent;
			}
		}
		return gameObject;
	}

	private void AnimateAttacking()
	{
		ItemDef a_weapon = this.m_handItemDef;
		if (a_weapon.damage < 1f)
		{
			a_weapon = Items.GetItemDef(0);
		}
		if (a_weapon.buildingIndex == 0 && Time.time > this.m_nextAttackTime)
		{
			if (a_weapon.ammoItemType > 0)
			{
				Vector3 position;
				if (null != this.m_handItemExit)
				{
					position = this.m_handItemExit.position;
					Quaternion rotation = Quaternion.LookRotation(-this.m_handItemExit.forward);
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_shootEffectPrefab, position, rotation);
					gameObject.transform.parent = this.m_handItemExit;
				}
				Vector3 vector = (!(null != this.m_input) || !(null != this.m_input.GetTarget())) ? base.transform.forward : (this.m_input.GetTarget().transform.position - base.transform.position).normalized;
				position = base.transform.position + Quaternion.LookRotation(vector) * new Vector3(0.5f, 1.2f, 1.5f);
				GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(this.m_trailEffectPrefab, position, base.transform.rotation);
				RiseAndDie component = gameObject2.GetComponent<RiseAndDie>();
				component.m_riseVector = vector * a_weapon.range * 0.9f;
				component.SetEndByCollision(base.transform.position + Vector3.up * 1.5f);
			}
			else
			{
				this.m_playAttackAnimTime = Time.time + 0.6f;
			}
			this.HandleSpecialWeaponAttack();
			this.m_sound.Attack(a_weapon);
			this.DoRaycast();
			this.m_nextAttackTime = Time.time + a_weapon.attackdur;
		}
	}

	private void HandleSpecialWeaponAttack()
	{
		if (this.m_handItemType == 109)
		{
			UnityEngine.Object.Instantiate(this.m_digEffectPrefab, base.transform.position + base.transform.forward * 0.3f, Quaternion.identity);
		}
		else if (this.m_handItemType == 110)
		{
			Vector3 vector = base.transform.position + base.transform.forward * 3.5f;
			if (0.8f > Util.GetTerrainHeight(vector))
			{
				UnityEngine.Object.Instantiate(this.m_waterEffectPrefab, vector, Quaternion.identity);
			}
			else
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_textEffectPrefab, base.transform.position + Vector3.up * 3f, Quaternion.identity);
				TextLNG component = gameObject.GetComponent<TextLNG>();
				component.m_lngKey = "EVENT_NOWATER";
			}
		}
	}

	private void DoRaycast()
	{
		Transform transform = null;
		Raycaster.Attack(base.transform, this.m_handItemDef, base.transform.position + base.transform.forward * 1.2f, ref transform);
	}

	private void HandleHologram()
	{
		if (this.m_hologramRenderers != null && null != this.m_client && null != this.m_holoBuilding)
		{
			if (this.m_isSitting)
			{
				this.m_holoBuilding.SetActive(false);
			}
			else
			{
				if (!this.m_holoBuilding.activeSelf)
				{
					this.m_holoBuilding.SetActive(true);
				}
				foreach (Renderer renderer in this.m_hologramRenderers)
				{
					if (null == renderer)
					{
						this.m_hologramRenderers = null;
						break;
					}
					bool flag = this.m_client.IsValidBuildPos(this.m_holoBuilding.transform.position, this.m_handItemDef.buildingIndex);
					renderer.sharedMaterial = ((!flag) ? this.m_hologramInvalidMat : this.m_hologramValidMat);
				}
				this.m_holoBuilding.transform.rotation = Quaternion.Euler(0f, this.m_input.GetBuildRot(), 0f);
			}
		}
	}

	private Renderer[] GetRenderersAndDisableShadows(GameObject a_go)
	{
		Renderer[] componentsInChildren = a_go.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			if (null != renderer)
			{
				renderer.castShadows = false;
				renderer.receiveShadows = false;
			}
		}
		return componentsInChildren;
	}

	private void ChangePart(ref Bone a_bone, bool a_optional, int a_newPartType)
	{
		GameObject gameObject = (!a_optional) ? a_bone.lookPart : a_bone.addPart;
		if (null != gameObject)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
		if (a_bone == this.m_boneHand)
		{
			this.m_handItemExit = null;
			this.m_handItemType = 0;
			this.m_handItemDef = Items.GetItemDef(this.m_handItemType);
			if (null != this.m_holoBuilding)
			{
				UnityEngine.Object.Destroy(this.m_holoBuilding);
			}
			this.m_holoBuilding = null;
			this.m_hologramRenderers = null;
		}
		string path = string.Empty;
		if (a_bone == this.m_boneHead)
		{
			path = "inventory_steam/go_" + (a_newPartType - 1 + 10000);
		}
		else
		{
			path = "items/item_" + a_newPartType;
		}
		GameObject gameObject2 = (GameObject)Resources.Load(path);
		if (null == gameObject2 && !a_optional)
		{
			gameObject2 = ((a_bone != this.m_boneBody) ? this.m_headPrefab : this.m_rucksackPrefab);
		}
		else if (a_optional && a_bone == this.m_boneBody && !Items.IsBody(a_newPartType))
		{
			gameObject2 = null;
		}
		if (null != gameObject2)
		{
			if (a_optional)
			{
				a_bone.addPart = this.SetupPart(gameObject2, a_bone.bone, a_optional);
				if (null != a_bone.addPart)
				{
					this.GetRenderersAndDisableShadows(a_bone.addPart);
					if (a_bone == this.m_boneHand)
					{
						this.m_handItemType = a_newPartType;
						this.m_handItemDef = Items.GetItemDef(this.m_handItemType);
						if (this.m_handItemDef.ammoItemType > 0)
						{
							this.m_handItemExit = a_bone.addPart.transform.FindChild("Exit");
						}
						else if (this.m_handItemDef.buildingIndex > 0 && null != this.m_input)
						{
							GameObject gameObject3 = (GameObject)Resources.Load("buildings/building_" + this.m_handItemDef.buildingIndex);
							if (null != gameObject3)
							{
								this.m_holoBuilding = this.SetupPart(gameObject3, base.transform, false);
								if (null != this.m_holoBuilding)
								{
									ServerBuilding component = this.m_holoBuilding.GetComponent<ServerBuilding>();
									if (null != component)
									{
										UnityEngine.Object.Destroy(component);
									}
									this.m_holoBuilding.transform.localPosition = new Vector3(0f, 0f, 2f);
									this.m_holoBuilding.transform.localRotation = Quaternion.Euler(0f, this.m_input.GetBuildRot(), 0f);
									this.m_hologramRenderers = this.GetRenderersAndDisableShadows(this.m_holoBuilding);
								}
							}
						}
					}
					else if (a_bone == this.m_boneHead)
					{
						a_bone.addPart.transform.localPosition = new Vector3(-0.15f, 0.01f, 0f);
						a_bone.addPart.transform.localRotation = Quaternion.Euler(90f, 270f, 0f);
					}
					else if (a_bone == this.m_boneBody)
					{
						a_bone.addPart.transform.localPosition = new Vector3(0.104f, 0.018f, 0f);
						a_bone.addPart.transform.localRotation = Quaternion.Euler(90f, 270f, 0f);
					}
				}
			}
			else
			{
				a_bone.lookPart = this.SetupPart(gameObject2, a_bone.bone, a_optional);
			}
		}
	}

	public void ResetAnim()
	{
		this.ChangeHandItem(-1);
		this.ChangeBodyItem(-1);
		this.m_isTakingAction = false;
		this.m_isSitting = false;
	}

	public void ChangeSkin(int a_skinIndex)
	{
		if (1 > a_skinIndex)
		{
			this.m_bodyRenderer.material.mainTexture = this.m_bodyTexture;
			this.m_headRenderer.material.mainTexture = this.m_headTexture;
		}
		else
		{
			a_skinIndex = a_skinIndex - 1 + 20000;
			Texture texture = (Texture)Resources.Load("skins/skin_" + ((!this.m_isMale) ? "0_" : "1_") + a_skinIndex);
			if (null != texture)
			{
				this.m_bodyRenderer.material.mainTexture = texture;
				this.m_headRenderer.material.mainTexture = texture;
			}
		}
	}

	public void ChangeHeadItem(int a_itemIndex)
	{
		this.ChangePart(ref this.m_boneHead, true, a_itemIndex);
	}

	public void ChangeHandItem(int a_itemIndex)
	{
		this.ChangePart(ref this.m_boneHand, true, a_itemIndex);
	}

	public void ChangeBodyItem(int a_itemIndex)
	{
		this.ChangePart(ref this.m_boneBody, true, a_itemIndex);
	}

	public float m_maxSpeed = 6f;

	public GameObject m_bodyPrefab;

	public Texture m_bodyTexture;

	public GameObject m_headPrefab;

	public GameObject m_rucksackPrefab;

	public GameObject m_shootEffectPrefab;

	public GameObject m_trailEffectPrefab;

	public GameObject m_digEffectPrefab;

	public GameObject m_textEffectPrefab;

	public GameObject m_waterEffectPrefab;

	public RuntimeAnimatorController m_animController;

	public Material m_hologramValidMat;

	public Material m_hologramInvalidMat;

	public Bone m_boneHand;

	public Bone m_boneArm;

	public Bone m_boneBody;

	public Bone m_boneHead;

	public int m_defaultHandItemType = -1;

	public int m_defaultHeadItemType = -1;

	public int m_defaultBodyItemType = -1;

	public int m_defaultSkinItemType = -1;

	private Animator m_animator;

	private Vector3 m_lastPos;

	private ItemDef m_handItemDef = Items.GetItemDef(0);

	private int m_handItemType;

	private Transform m_handItemExit;

	private float m_nextAttackTime;

	private float m_playAttackAnimTime;

	private Renderer[] m_hologramRenderers;

	private GameObject m_holoBuilding;

	private SkinnedMeshRenderer m_bodyRenderer;

	private Renderer m_headRenderer;

	private Texture m_headTexture;

	private bool m_isMale;

	private ClientInput m_input;

	private LidClient m_client;

	private CharSounds m_sound;

	public bool m_isSitting;

	[HideInInspector]
	public bool m_isTakingAction;
}

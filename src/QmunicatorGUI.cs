using System;
using UnityEngine;
using UnityEngine.UI;

public class QmunicatorGUI : MonoBehaviour
{
	public QmunicatorGUI()
	{
	}

	private void Start()
	{
		this.m_guimaster = (GUI3dMaster)UnityEngine.Object.FindObjectOfType(typeof(GUI3dMaster));
		this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
		this.m_dayNightCycle = (DayNightCycle)UnityEngine.Object.FindObjectOfType(typeof(DayNightCycle));
		this.m_zOffset = this.m_guiRoot.localPosition.z;
		if (null != this.m_volumeSlider)
		{
			this.m_volumeSlider.value = PlayerPrefs.GetFloat("prefVolume", 1f);
		}
		if (null != this.m_hintsToggle)
		{
			this.m_hintsToggle.isOn = (PlayerPrefs.GetInt("prefHints", 1) == 1);
		}
		this.ActivateGui(this.m_activeApp);
	}

	private void SetVisible(bool a_visible)
	{
		if ((a_visible ^ this.IsActive(true)) && this.m_curSin == -1f)
		{
			this.m_sinOffset = ((!a_visible) ? 0.5f : 1.5f);
			this.m_curSin = this.m_sinOffset;
		}
	}

	private void ActivateGui(eActiveApp a_app)
	{
		this.m_activeApp = a_app;
		int num = a_app - eActiveApp.home;
		for (int i = 0; i < this.m_guis.Length; i++)
		{
			this.m_guis[i].SetActive(num == i);
			if (i < this.m_btnActiveRenderer.Length && null != this.m_btnActiveRenderer[i])
			{
				this.m_btnActiveRenderer[i].enabled = (num == i);
			}
		}
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.m_curSin > -1f)
		{
			Vector3 b = Vector3.forward * this.m_zOffset;
			this.m_guiRoot.localPosition = Vector3.up * ((FastSin.Get(this.m_curSin * 3.1415927f) - 1f) * 0.5f) + b;
			this.m_curSin += deltaTime * this.m_animSpeed;
			if (this.m_curSin > this.m_sinOffset + 1f)
			{
				this.m_curSin = -1f;
				this.m_guiRoot.localPosition = Vector3.up * ((this.m_guiRoot.localPosition.y >= -0.5f) ? 0f : -1f) + b;
			}
		}
		eActiveApp eActiveApp = eActiveApp.none;
		if (Input.GetButtonDown("Communicator"))
		{
			this.SetVisible(false == this.IsActive(true));
		}
		else if (Input.GetButtonDown("Inventory") || Input.GetButtonDown("Exit"))
		{
			this.SetVisible(false);
		}
		else if (Input.GetButtonDown("Help"))
		{
			eActiveApp = eActiveApp.help;
		}
		else if (Input.GetButtonDown("Crafting"))
		{
			eActiveApp = eActiveApp.crafting;
		}
		else if (Input.GetButtonDown("Global Chat"))
		{
			eActiveApp = eActiveApp.chat;
		}
		else if (Input.GetButtonDown("Map"))
		{
			eActiveApp = eActiveApp.maps;
		}
		if (eActiveApp != eActiveApp.none)
		{
			if (this.m_activeApp == eActiveApp || !this.IsActive(true))
			{
				this.SetVisible(false == this.IsActive(true));
			}
			this.ActivateGui(eActiveApp);
		}
		if (this.IsActive(true))
		{
			eActiveApp activeApp = this.m_activeApp;
			if (activeApp == eActiveApp.home)
			{
				this.UpdateHomeApp();
			}
			this.m_txtClock.text = this.m_dayNightCycle.GetTime();
			if (null != this.m_client)
			{
				this.m_txtPlayerCount.text = LNG.Get("PLAYERCOUNT") + ": " + this.m_client.GetPlayerCount();
			}
		}
	}

	private void UpdateHomeApp()
	{
		if (null != this.m_client)
		{
			this.m_txtHealth.text = ((int)this.m_client.GetHealth()).ToString();
			this.m_barHealth.localScale = new Vector3(this.m_client.GetHealth() * 0.01f, 1f, 1f);
			this.m_txtEnergy.text = ((int)this.m_client.GetEnergy()).ToString();
			this.m_barEnergy.localScale = new Vector3(this.m_client.GetEnergy() * 0.01f, 1f, 1f);
			this.m_txtRank.text = ((int)(this.m_client.GetRankProgress() * 100f)).ToString();
			this.m_barRank.localScale = new Vector3(this.m_client.GetRankProgress(), 1f, 1f);
			this.m_txtKarma.text = ((int)(this.m_client.GetKarma() * 0.50001f)).ToString();
			this.m_barKarma.localScale = new Vector3(this.m_client.GetKarma() / 200f, 1f, 1f);
		}
	}

	private void LateUpdate()
	{
		if (Time.timeSinceLevelLoad < 1f)
		{
			return;
		}
		if (null != this.m_guimaster)
		{
			string clickedButtonName = this.m_guimaster.GetClickedButtonName();
			if (string.Empty != clickedButtonName)
			{
				if (this.IsActive(true))
				{
					if (clickedButtonName.Length == 1)
					{
						try
						{
							this.ActivateGui((eActiveApp)int.Parse(clickedButtonName));
						}
						catch (Exception message)
						{
							Debug.Log(message);
						}
					}
					else if (clickedButtonName.StartsWith("HELP_"))
					{
						this.m_helpText.text = LNG.Get(clickedButtonName + "_TEXT");
					}
					else if (null != this.m_guiCloseBtn && this.m_guiCloseBtn.name == clickedButtonName)
					{
						this.SetVisible(false);
					}
					else if (null != this.m_guiQuitBtn && this.m_guiQuitBtn.name == clickedButtonName)
					{
						QuitGameGUI quitGameGUI = (QuitGameGUI)UnityEngine.Object.FindObjectOfType(typeof(QuitGameGUI));
						if (null != quitGameGUI)
						{
							quitGameGUI.ShowGui(true);
						}
					}
				}
				else if (null != this.m_guiComBtn && this.m_guiComBtn.name == clickedButtonName)
				{
					this.SetVisible(true);
				}
			}
		}
	}

	public bool IsActive(bool a_ignoreAnimation = true)
	{
		return (a_ignoreAnimation && this.m_guiRoot.localPosition.y != -1f) || (!a_ignoreAnimation && 0f == this.m_guiRoot.localPosition.y);
	}

	public void OpenCrafting()
	{
		this.SetVisible(true);
		this.ActivateGui(eActiveApp.crafting);
	}

	public void ToggleHints()
	{
		PlayerPrefs.SetInt("prefHints", (!this.m_hintsToggle.isOn) ? 0 : 1);
	}

	public void SetVolume()
	{
		AudioListener.volume = this.m_volumeSlider.value;
		PlayerPrefs.SetFloat("prefVolume", this.m_volumeSlider.value);
	}

	public void SetAppearance(int a_id)
	{
		PlayerPrefs.SetInt("prefAppearance", a_id);
		this.m_client.SendChatMsg("/char " + a_id, true);
	}

	public Transform m_guiRoot;

	public GameObject m_guiComBtn;

	public GameObject m_guiCloseBtn;

	public GameObject m_guiQuitBtn;

	public GameObject[] m_guis;

	public Renderer[] m_btnActiveRenderer;

	public TextMesh m_txtClock;

	public TextMesh m_txtPlayerCount;

	public TextMesh m_txtHealth;

	public TextMesh m_txtEnergy;

	public TextMesh m_txtRank;

	public TextMesh m_txtKarma;

	public Transform m_barHealth;

	public Transform m_barEnergy;

	public Transform m_barRank;

	public Transform m_barKarma;

	public TextMesh m_helpText;

	public Toggle m_hintsToggle;

	public Slider m_volumeSlider;

	public float m_animSpeed = 2f;

	private eActiveApp m_activeApp = eActiveApp.home;

	private GUI3dMaster m_guimaster;

	private LidClient m_client;

	private DayNightCycle m_dayNightCycle;

	private float m_sinOffset;

	private float m_curSin = -1f;

	private float m_zOffset;
}

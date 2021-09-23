using System;
using UnityEngine;

public class ServerVehicle : MonoBehaviour
{
	public ServerVehicle()
	{
	}

	private void Awake()
	{
		this.m_data.passengerIds = new int[4];
		this.m_vehicle = base.GetComponent<SimpleVehicle>();
		this.m_server = UnityEngine.Object.FindObjectOfType<LidServer>();
		this.m_spawnPoint = (this.m_lastPos = base.transform.position);
		this.KillAndResetPassengers();
	}

	private void Update()
	{
		if (this.m_respawnTime > 0f && Time.time > this.m_respawnTime)
		{
			base.transform.position = this.m_spawnPoint;
			this.ChangeHealthBy(10000f);
			base.collider.enabled = true;
			this.m_respawnTime = -1f;
		}
	}

	private void FixedUpdate()
	{
		float sqrMagnitude = (this.m_lastPos - base.transform.position).sqrMagnitude;
		bool flag = sqrMagnitude > 0f;
		this.m_lastPos = base.transform.position;
		if (flag && 0.8f > Util.GetTerrainHeight(base.transform.position + base.transform.forward))
		{
			base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
			if (Time.time > this.m_nextWaterDamageTime)
			{
				this.ChangeHealthBy(-50f);
				this.m_nextWaterDamageTime = Time.time + 1f;
			}
		}
		if (this.m_health < 20f)
		{
			this.ChangeHealthBy(-Time.fixedDeltaTime);
		}
	}

	private void LateUpdate()
	{
		Vector3 position = base.transform.position;
		position.y = 0f;
		base.transform.position = position;
	}

	private void KillAndResetPassengers()
	{
		for (int i = 0; i < 4; i++)
		{
			if (this.m_data.passengerIds[i] != -1)
			{
				if (null != this.m_server)
				{
					ServerPlayer playerByOnlineid = this.m_server.GetPlayerByOnlineid(this.m_data.passengerIds[i]);
					if (playerByOnlineid != null)
					{
						playerByOnlineid.ChangeHealthBy(-10000f);
					}
				}
				if (i == 0)
				{
					this.m_vehicle.AssignInput(0f, 0f, false);
				}
				this.m_data.passengerIds[i] = -1;
			}
		}
	}

	private bool ChangePassenger(int a_getId, int a_setId)
	{
		if (a_getId == -1 && a_setId == -1)
		{
			Debug.Log("ServerVehicle.cs: Error: ChangePassenger( -1, -1 ) is not valid");
			return false;
		}
		for (int i = 0; i < 4; i++)
		{
			if (a_getId == this.m_data.passengerIds[i])
			{
				if (i == 0 && a_setId == -1)
				{
					this.m_vehicle.AssignInput(0f, 0f, false);
				}
				this.m_data.passengerIds[i] = a_setId;
				return true;
			}
		}
		return false;
	}

	private void OnCollisionEnter(Collision a_col)
	{
		if (Time.time > this.m_nextCollisionTime)
		{
			float num = 0.5f * Mathf.Clamp(a_col.relativeVelocity.sqrMagnitude - 10f, 0f, 100000f);
			if (num > 1f)
			{
				this.ChangeHealthBy(-num);
			}
			this.m_nextCollisionTime = Time.time + 0.3f;
		}
	}

	private void CausedDamage(float a_dmg)
	{
		if (null != this.m_server && this.m_data.passengerIds[0] != -1)
		{
			ServerPlayer playerByOnlineid = this.m_server.GetPlayerByOnlineid(this.m_data.passengerIds[0]);
			if (playerByOnlineid != null)
			{
				playerByOnlineid.ChangeKarmaBy(Mathf.Clamp(a_dmg, 0f, 100f) * -0.5f);
			}
		}
	}

	public float ChangeHealthBy(float a_delta)
	{
		if (a_delta < 0f)
		{
			a_delta *= 1f - this.m_armorPercent;
		}
		this.m_health = Mathf.Clamp(this.m_health + a_delta, 0f, 100f);
		if (this.IsDead() && this.m_respawnTime < 0f)
		{
			this.KillAndResetPassengers();
			this.m_vehicle.AssignInput(0f, 0f, false);
			base.collider.enabled = false;
			if (null != this.m_server)
			{
				this.m_server.CreateFreeWorldItem(131, 5, base.transform.position);
			}
			this.m_respawnTime = Time.time + 180f;
		}
		return this.m_health;
	}

	public void DestroyCarAndForceExitPassengers()
	{
		for (int i = 0; i < 4; i++)
		{
			if (this.m_data.passengerIds[i] != -1)
			{
				if (null != this.m_server)
				{
					ServerPlayer playerByOnlineid = this.m_server.GetPlayerByOnlineid(this.m_data.passengerIds[i]);
					if (playerByOnlineid != null)
					{
						playerByOnlineid.ExitVehicle(true);
					}
				}
				this.m_data.passengerIds[i] = -1;
			}
		}
		this.ChangeHealthBy(-100000f);
	}

	public int GetPassengerCount()
	{
		return this.m_passengerCount;
	}

	public bool IsNpcControlled()
	{
		return false;
	}

	public float GetHealth()
	{
		return this.m_health;
	}

	public bool IsDead()
	{
		return this.m_health < 1f;
	}

	public void AssignInput(float a_v, float a_h, bool a_space)
	{
		if (null != this.m_vehicle)
		{
			this.m_vehicle.AssignInput(a_v, a_h, a_space);
		}
	}

	public bool AddPassenger(int a_id)
	{
		bool flag = false;
		if (!this.IsDead())
		{
			flag = this.ChangePassenger(-1, a_id);
			if (flag)
			{
				this.m_passengerCount++;
			}
		}
		return flag;
	}

	public bool RemovePassenger(int a_id)
	{
		bool flag = this.ChangePassenger(a_id, -1);
		if (flag)
		{
			this.m_passengerCount--;
		}
		return flag;
	}

	public Vector3 GetPassengerExitPos(int a_id)
	{
		Vector3 result = Vector3.zero;
		Vector3 position = this.m_vehicle.transform.position;
		position.y = 0f;
		Vector3 vector = new Vector3(this.m_vehicle.transform.right.x, 0f, this.m_vehicle.transform.right.z);
		Vector3 normalized = vector.normalized;
		Vector3 vector2 = new Vector3(this.m_vehicle.transform.forward.x, 0f, this.m_vehicle.transform.forward.z);
		Vector3 normalized2 = vector2.normalized;
		for (int i = 0; i < 4; i++)
		{
			if (a_id == this.m_data.passengerIds[i])
			{
				float num = (i % 2 != 0) ? 2.1f : -2.1f;
				float d = -num;
				float d2 = ((2 <= i) ? -1.5f : 0f) + UnityEngine.Random.Range(-0.5f, 0.5f);
				float num2 = 0.6f;
				Vector3 vector3 = position + normalized * num + normalized2 * d2;
				Vector3 vector4 = position + normalized * d + normalized2 * d2;
				vector3.y = (vector4.y = num2 + 0.2f);
				if (!Raycaster.CheckSphere(vector3, num2) && 0.8f < Util.GetTerrainHeight(vector3))
				{
					result = vector3;
				}
				else if (!Raycaster.CheckSphere(vector4, num2) && 0.8f < Util.GetTerrainHeight(vector4))
				{
					result = vector4;
				}
				break;
			}
		}
		return result;
	}

	private const int c_allLayersExceptVehicle = 268433407;

	private const int c_allLayers = 268435455;

	public int m_id = -1;

	public CarData m_data;

	public float m_crashDamageMultip = 3f;

	public float m_armorPercent = 0.8f;

	private int m_passengerCount;

	private float m_health = 100f;

	private SimpleVehicle m_vehicle;

	private float m_nextWaterDamageTime;

	private float m_nextCollisionTime;

	private float m_respawnTime = -1f;

	private Vector3 m_spawnPoint;

	private Vector3 m_lastPos;

	private float m_lastSpeed;

	private LidServer m_server;
}

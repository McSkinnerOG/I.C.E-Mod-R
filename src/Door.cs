using System;
using UnityEngine;

public class Door : ServerBuilding
{
	public Door()
	{
	}

	public override bool Use(ServerPlayer a_player)
	{
		bool result = false;
		base.Use(a_player);
		if (a_player.m_pid == this.m_ownerPid || this.m_server.PartyContainsPid(a_player.m_partyId, this.m_ownerPid))
		{
			this.SwitchDoorState(a_player.GetPosition());
			result = true;
		}
		return result;
	}

	protected override void Awake()
	{
		this.m_startRot = base.transform.rotation;
		base.Awake();
	}

	private void SwitchDoorState(Vector3 a_pos)
	{
		float y = (Vector3.Dot((a_pos - base.transform.position).normalized, base.transform.forward) >= 0f) ? 90f : -90f;
		this.m_doorOpen = !this.m_doorOpen;
		if (this.m_doorOpen)
		{
			base.transform.rotation = this.m_startRot * Quaternion.Euler(0f, y, 0f);
		}
		else
		{
			base.transform.rotation = this.m_startRot;
		}
	}

	private bool m_doorOpen;

	private Quaternion m_startRot = Quaternion.identity;
}

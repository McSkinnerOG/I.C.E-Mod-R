using System;
using Lidgren.Network;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
	public PlayerAnimator()
	{
	}

	private void Start()
	{
		this.target.wrapMode = WrapMode.Loop;
		this.target["Jump"].wrapMode = WrapMode.Once;
		this.target["Jump"].layer = 1;
		this.target["Land"].wrapMode = WrapMode.Once;
		this.target["Land"].layer = 1;
		this.target["Run"].speed = 1.75f;
		this.target["Walk"].speed = -1.25f;
		this.target.Play("Idle");
		this.lgo = base.GetComponent<LidgrenGameObject>();
	}

	private void Update()
	{
		if (this.lgo.IsMine)
		{
			switch (this.state)
			{
			case 1:
				this.rotate(0f);
				break;
			case 2:
				this.rotate(0f);
				break;
			case 7:
				this.rotate(-90f);
				break;
			case 8:
				this.rotate(90f);
				break;
			case 9:
				this.rotate(-45f);
				break;
			case 10:
				this.rotate(45f);
				break;
			case 11:
				this.rotate(45f);
				break;
			case 12:
				this.rotate(-45f);
				break;
			}
		}
		switch (this.state)
		{
		case 1:
		case 7:
		case 8:
		case 9:
		case 10:
			this.target.CrossFade("Run");
			break;
		case 2:
		case 11:
		case 12:
			this.target.CrossFade("Walk");
			break;
		case 4:
			this.target.CrossFade("Fall");
			break;
		case 6:
			this.target.CrossFade("Idle");
			break;
		}
		if (this.lgo.IsMine)
		{
			if (this.state == 6)
			{
				if (Input.GetMouseButtonDown(1))
				{
					this.target.transform.rotation = Quaternion.LookRotation(base.transform.forward);
				}
			}
			else
			{
				this.target.transform.rotation = Quaternion.Slerp(this.target.transform.rotation, this.rotation, Time.deltaTime * 10f);
			}
		}
	}

	private void rotate(float yaw)
	{
		this.rotation = Quaternion.LookRotation(Quaternion.Euler(0f, yaw, 0f) * base.transform.forward, Vector3.up);
	}

	public void OnPlayerMovement(byte newState)
	{
		if (this.lgo.IsMine)
		{
			NetOutgoingMessage netOutgoingMessage = this.lgo.Connection.Peer.CreateMessage();
			netOutgoingMessage.Write(LidgrenMessageHeaders.Movement);
			netOutgoingMessage.Write(this.lgo.Id);
			netOutgoingMessage.Write(newState);
			this.lgo.Connection.SendMessage(netOutgoingMessage, NetDeliveryMethod.ReliableOrdered, 1);
		}
		this.state = newState;
		switch (this.state)
		{
		case 3:
			this.target.CrossFade("Jump");
			break;
		case 5:
			this.target.CrossFade("Land");
			break;
		}
	}

	[SerializeField]
	private Animation target;

	private byte state;

	private Quaternion rotation = Quaternion.identity;

	private LidgrenGameObject lgo;
}

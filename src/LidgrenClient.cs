using System;
using System.Collections.Generic;
using Lidgren.Network;
using UnityEngine;

public class LidgrenClient : LidgrenPeer
{
	public LidgrenClient()
	{
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		this.client = new NetClient(new NetPeerConfiguration("LidgrenDemo"));
		this.client.Start();
		this.client.Connect(this.host, this.port);
		base.SetPeer(this.client);
		base.Connected += this.onConnected;
		base.Disconnected += this.onDisconnected;
		base.RegisterMessageHandler(LidgrenMessageHeaders.Hello, new Action<NetIncomingMessage>(this.onHello));
		base.RegisterMessageHandler(LidgrenMessageHeaders.Spawn, new Action<NetIncomingMessage>(this.onSpawn));
		base.RegisterMessageHandler(LidgrenMessageHeaders.Despawn, new Action<NetIncomingMessage>(this.onDespawn));
		base.RegisterMessageHandler(LidgrenMessageHeaders.Movement, new Action<NetIncomingMessage>(this.onMovement));
		base.RegisterMessageHandler(LidgrenMessageHeaders.Position, new Action<NetIncomingMessage>(this.onPosition));
	}

	private void onConnected(NetIncomingMessage msg)
	{
		Debug.Log("Connected to server");
	}

	private void onDisconnected(NetIncomingMessage msg)
	{
		Debug.Log("Disconnected from server");
	}

	private void onMovement(NetIncomingMessage msg)
	{
		Debug.Log("onMovement");
		int key = msg.ReadInt32();
		LidgrenGameObject lidgrenGameObject = null;
		if (this.lgos.TryGetValue(key, out lidgrenGameObject))
		{
			lidgrenGameObject.GetComponent<PlayerAnimator>().OnPlayerMovement(msg.ReadByte());
		}
	}

	private void onPosition(NetIncomingMessage msg)
	{
		Debug.Log("onPosition");
		int key = msg.ReadInt32();
		LidgrenGameObject lidgrenGameObject = null;
		if (this.lgos.TryGetValue(key, out lidgrenGameObject))
		{
			Vector3 position = new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
			lidgrenGameObject.transform.position = position;
			Quaternion rotation = new Quaternion(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
			lidgrenGameObject.transform.GetChild(0).rotation = rotation;
		}
	}

	private void onSpawn(NetIncomingMessage msg)
	{
		int num = msg.ReadInt32();
		this.lgos.Add(num, LidgrenGameObject.Spawn(this.clientId, num, msg.SenderConnection));
	}

	private void onDespawn(NetIncomingMessage msg)
	{
		try
		{
			int key = msg.ReadInt32();
			UnityEngine.Object.Destroy(this.lgos[key]);
			this.lgos.Remove(key);
		}
		catch
		{
		}
	}

	private void onHello(NetIncomingMessage msg)
	{
		this.clientId = msg.ReadInt32();
		NetOutgoingMessage netOutgoingMessage = msg.SenderConnection.Peer.CreateMessage();
		netOutgoingMessage.Write(LidgrenMessageHeaders.RequestSpawn);
		msg.SenderConnection.SendMessage(netOutgoingMessage, NetDeliveryMethod.ReliableOrdered, 1);
	}

	private int clientId;

	private NetClient client;

	private Dictionary<int, LidgrenGameObject> lgos = new Dictionary<int, LidgrenGameObject>();

	[SerializeField]
	private int port = 10000;

	[SerializeField]
	private string host = "127.0.0.1";
}

using System;
using System.Linq;
using Lidgren.Network;
using UnityEngine;

public class LidgrenServer : LidgrenPeer
{
	public LidgrenServer()
	{
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		this.server = new NetServer(new NetPeerConfiguration("LidgrenDemo")
		{
			Port = this.port
		});
		this.server.Start();
		base.SetPeer(this.server);
		base.Connected += this.onConnected;
		base.Disconnected += this.onDisconnected;
		base.RegisterMessageHandler(LidgrenMessageHeaders.RequestSpawn, new Action<NetIncomingMessage>(this.onRequestSpawn));
		base.RegisterMessageHandler(LidgrenMessageHeaders.Movement, new Action<NetIncomingMessage>(this.onMovement));
		base.RegisterMessageHandler(LidgrenMessageHeaders.Position, new Action<NetIncomingMessage>(this.onPosition));
	}

	private void spawnOn(LidgrenGameObject go, NetConnection c)
	{
		NetOutgoingMessage netOutgoingMessage = c.Peer.CreateMessage();
		netOutgoingMessage.Write(LidgrenMessageHeaders.Spawn);
		netOutgoingMessage.Write(go.Id);
		c.SendMessage(netOutgoingMessage, NetDeliveryMethod.ReliableOrdered, 1);
	}

	private void onMovement(NetIncomingMessage msg)
	{
		LidgrenPlayer lidgrenPlayer = (LidgrenPlayer)msg.SenderConnection.Tag;
		NetOutgoingMessage netOutgoingMessage = msg.SenderConnection.Peer.CreateMessage();
		netOutgoingMessage.Write(msg);
		this.server.SendToAll(netOutgoingMessage, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, 1);
		msg.ReadInt32();
		lidgrenPlayer.GameObject.GetComponent<PlayerAnimator>().OnPlayerMovement(msg.ReadByte());
	}

	private void onPosition(NetIncomingMessage msg)
	{
		LidgrenPlayer lidgrenPlayer = (LidgrenPlayer)msg.SenderConnection.Tag;
		NetOutgoingMessage netOutgoingMessage = msg.SenderConnection.Peer.CreateMessage();
		netOutgoingMessage.Write(msg);
		this.server.SendToAll(netOutgoingMessage, msg.SenderConnection, NetDeliveryMethod.Unreliable, 0);
		msg.ReadInt32();
		Vector3 position = new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
		lidgrenPlayer.GameObject.transform.position = position;
		Quaternion rotation = new Quaternion(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
		lidgrenPlayer.GameObject.transform.GetChild(0).rotation = rotation;
	}

	private void onRequestSpawn(NetIncomingMessage msg)
	{
		LidgrenPlayer lidgrenPlayer = (LidgrenPlayer)msg.SenderConnection.Tag;
		if (lidgrenPlayer.GameObject == null)
		{
			lidgrenPlayer.GameObject = LidgrenGameObject.Spawn(-1, lidgrenPlayer.Id, msg.SenderConnection);
			foreach (NetConnection c in this.server.Connections)
			{
				this.spawnOn(lidgrenPlayer.GameObject, c);
			}
		}
	}

	private void onConnected(NetIncomingMessage a_msg)
	{
		NetOutgoingMessage netOutgoingMessage = a_msg.SenderConnection.Peer.CreateMessage();
		netOutgoingMessage.Write(LidgrenMessageHeaders.Hello);
		netOutgoingMessage.Write(++this.clientCounter);
		a_msg.SenderConnection.Tag = new LidgrenPlayer(this.clientCounter);
		a_msg.SenderConnection.SendMessage(netOutgoingMessage, NetDeliveryMethod.ReliableOrdered, 1);
		foreach (LidgrenGameObject go in UnityEngine.Object.FindObjectsOfType(typeof(LidgrenGameObject)).Cast<LidgrenGameObject>())
		{
			this.spawnOn(go, a_msg.SenderConnection);
		}
		Debug.Log("Client connected");
	}

	private void onDisconnected(NetIncomingMessage a_msg)
	{
		LidgrenPlayer lidgrenPlayer = (LidgrenPlayer)a_msg.SenderConnection.Tag;
		NetOutgoingMessage netOutgoingMessage = this.server.CreateMessage();
		netOutgoingMessage.Write(LidgrenMessageHeaders.Despawn);
		netOutgoingMessage.Write(lidgrenPlayer.Id);
		this.server.SendToAll(netOutgoingMessage, NetDeliveryMethod.ReliableOrdered);
		if (lidgrenPlayer.GameObject != null)
		{
			UnityEngine.Object.Destroy(lidgrenPlayer.GameObject);
		}
		Debug.Log("Client disconnected");
	}

	private int clientCounter;

	private NetServer server;

	[SerializeField]
	private int port = 10000;
}

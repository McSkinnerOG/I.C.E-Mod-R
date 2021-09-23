using Lidgren.Network;
using UnityEngine;

public class LidgrenGameObject : MonoBehaviour
{
	public int Id;

	public bool IsMine;

	public NetConnection Connection { get; set; }

	private void FixedUpdate()
	{
		if (IsMine)
		{
			NetOutgoingMessage netOutgoingMessage = Connection.Peer.CreateMessage();
			netOutgoingMessage.Write(LidgrenMessageHeaders.Position);
			netOutgoingMessage.Write(Id);
			netOutgoingMessage.Write(base.transform.position.x);
			netOutgoingMessage.Write(base.transform.position.y);
			netOutgoingMessage.Write(base.transform.position.z);
			Transform child = base.transform.GetChild(0);
			netOutgoingMessage.Write(child.rotation.x);
			netOutgoingMessage.Write(child.rotation.y);
			netOutgoingMessage.Write(child.rotation.z);
			netOutgoingMessage.Write(child.rotation.w);
			Connection.SendMessage(netOutgoingMessage, NetDeliveryMethod.Unreliable, 0);
		}
	}

	public static LidgrenGameObject Spawn(int myId, int id, NetConnection con)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Player"), new Vector3(45f, 10f, 45f), Quaternion.identity);
		LidgrenGameObject component = gameObject.GetComponent<LidgrenGameObject>();
		component.Id = id;
		component.IsMine = myId == id;
		component.Connection = con;
		return component;
	}
}

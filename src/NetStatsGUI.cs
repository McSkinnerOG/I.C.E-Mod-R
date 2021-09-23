using System;
using UnityEngine;

public class NetStatsGUI : MonoBehaviour
{
	public NetStatsGUI()
	{
	}

	private void Start()
	{
		this.m_client = (LidClient)UnityEngine.Object.FindObjectOfType(typeof(LidClient));
	}

	private void Update()
	{
		if (!Global.isServer && null != this.m_text && null != this.m_client && this.m_client.enabled && this.m_client.GetStats() != null)
		{
			this.m_text.text = string.Concat(new object[]
			{
				"ReceivedBytes: ",
				this.m_client.GetStats().ReceivedBytes,
				" - kbs: ",
				(float)(this.m_client.GetStats().ReceivedBytes / 1024) / Time.timeSinceLevelLoad,
				"\nReceivedPackets: ",
				this.m_client.GetStats().ReceivedPackets,
				"\nSentBytes: ",
				this.m_client.GetStats().SentBytes,
				" - kbs: ",
				(float)(this.m_client.GetStats().SentBytes / 1024) / Time.timeSinceLevelLoad,
				"\nResentMessages: ",
				this.m_client.GetStats().ResentMessages
			});
		}
	}

	public GUIText m_text;

	private LidClient m_client;
}

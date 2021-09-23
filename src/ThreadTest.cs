using System;
using System.Threading;
using UnityEngine;

public class ThreadTest : MonoBehaviour
{
	public ThreadTest()
	{
	}

	private void Start()
	{
		this.m_thread = new Thread(new ThreadStart(this.ThreadFunc));
		this.m_thread.IsBackground = true;
		this.m_thread.Start();
	}

	private void ThreadFunc()
	{
		for (;;)
		{
			Thread.Sleep(1000);
			float num = 0f;
			object obj = this.oThreadLock;
			lock (obj)
			{
				num = this.m_threadTime;
			}
			Debug.Log(num);
		}
	}

	private void OnApplicationQuit()
	{
		this.m_thread.Abort();
		Debug.Log("OnApplicationQuit()");
	}

	private void Update()
	{
		object obj = this.oThreadLock;
		lock (obj)
		{
			this.m_threadTime += Time.deltaTime;
		}
	}

	private object oThreadLock = new object();

	private float m_threadTime;

	private Thread m_thread;
}

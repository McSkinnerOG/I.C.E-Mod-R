using System;
using UnityEngine;

public abstract class JobBase : MonoBehaviour
{
	protected JobBase()
	{
	}

	protected void Init()
	{
		this.m_brain = base.GetComponent<BrainBase>();
		this.m_body = base.GetComponent<BodyBase>();
	}

	public abstract void Execute(float deltaTime);

	protected BrainBase m_brain;

	protected BodyBase m_body;
}

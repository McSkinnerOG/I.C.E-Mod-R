using System;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class GUI3dText : GUI3dButton
{
	public GUI3dText()
	{
	}

	protected override void Start()
	{
		base.Start();
		this.m_textMesh = base.GetComponent<TextMesh>();
		this.m_textMesh.renderer.material.color = this.m_color;
		if (this.m_LNGkey.Length > 0)
		{
			this.m_textMesh.text = LNG.Get(this.m_LNGkey);
		}
		if (this.m_dropShadow)
		{
			this.CreateShadow();
		}
	}

	protected override void Update()
	{
		base.Update();
		if (this.m_dropShadow)
		{
			this.m_shadowText.text = this.m_textMesh.text;
		}
	}

	private void CreateShadow()
	{
		Transform transform = base.transform.Find("DropShadow");
		if (null != transform)
		{
			UnityEngine.Object.DestroyImmediate(transform);
		}
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(base.gameObject);
		UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<GUI3dText>());
		gameObject.layer = 2;
		gameObject.renderer.material.color = this.m_shadowColor;
		gameObject.transform.parent = base.transform;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localPosition = this.m_shadowOffset;
		this.m_shadowText = gameObject.GetComponent<TextMesh>();
	}

	public Color m_color = Color.white;

	public bool m_dropShadow;

	public Color m_shadowColor = Color.black;

	public Vector3 m_shadowOffset = new Vector3(-1f, -2f, 0f);

	public string m_LNGkey = string.Empty;

	private TextMesh m_shadowText;

	private TextMesh m_textMesh;
}

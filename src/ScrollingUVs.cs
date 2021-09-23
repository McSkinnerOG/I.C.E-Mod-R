using System;
using UnityEngine;

public class ScrollingUVs : MonoBehaviour
{
	public ScrollingUVs()
	{
	}

	private void LateUpdate()
	{
		this.uvOffset += this.uvAnimationRate * Time.deltaTime;
		if (base.renderer.enabled)
		{
			base.renderer.materials[this.materialIndex].SetTextureOffset(this.textureName, this.uvOffset);
			if (this.ScrollBump)
			{
				base.renderer.materials[this.materialIndex].SetTextureOffset(this.bumpName, this.uvOffset);
			}
		}
	}

	public int materialIndex;

	public Vector2 uvAnimationRate = new Vector2(1f, 0f);

	public string textureName = "_MainTex";

	public bool ScrollBump = true;

	public string bumpName = "_BumpMap";

	private Vector2 uvOffset = Vector2.zero;
}

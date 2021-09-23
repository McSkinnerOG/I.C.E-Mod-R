using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class EasyFontTextMesh : MonoBehaviour
{
	public EasyFontTextMesh()
	{
	}

	public string Text
	{
		get
		{
			return this._privateProperties.text;
		}
		set
		{
			this._privateProperties.text = value;
			this.isDirty = true;
		}
	}

	public Font FontType
	{
		get
		{
			return this._privateProperties.font;
		}
		set
		{
			this._privateProperties.font = value;
			this.ChangeFont();
		}
	}

	public Material CustomFillMaterial
	{
		get
		{
			return this._privateProperties.customFillMaterial;
		}
		set
		{
			this._privateProperties.customFillMaterial = value;
			this.isDirty = true;
		}
	}

	public int FontSize
	{
		get
		{
			return this._privateProperties.fontSize;
		}
		set
		{
			this._privateProperties.fontSize = value;
			this.isDirty = true;
		}
	}

	public float Size
	{
		get
		{
			return this._privateProperties.size;
		}
		set
		{
			this._privateProperties.size = value;
			this.isDirty = true;
		}
	}

	public EasyFontTextMesh.TEXT_ANCHOR Textanchor
	{
		get
		{
			return this._privateProperties.textAnchor;
		}
		set
		{
			this._privateProperties.textAnchor = value;
			this.isDirty = true;
		}
	}

	public EasyFontTextMesh.TEXT_ALIGNMENT Textalignment
	{
		get
		{
			return this._privateProperties.textAlignment;
		}
		set
		{
			this._privateProperties.textAlignment = value;
			this.isDirty = true;
		}
	}

	public float LineSpacing
	{
		get
		{
			return this._privateProperties.lineSpacing;
		}
		set
		{
			this._privateProperties.lineSpacing = value;
			this.isDirty = true;
		}
	}

	public Color FontColorTop
	{
		get
		{
			return this._privateProperties.fontColorTop;
		}
		set
		{
			this._privateProperties.fontColorTop = value;
			this.SetColor(this._privateProperties.fontColorTop, this._privateProperties.fontColorBottom);
		}
	}

	public Color FontColorBottom
	{
		get
		{
			return this._privateProperties.fontColorBottom;
		}
		set
		{
			this._privateProperties.fontColorBottom = value;
			this.SetColor(this._privateProperties.fontColorTop, this._privateProperties.fontColorBottom);
		}
	}

	public bool EnableShadow
	{
		get
		{
			return this._privateProperties.enableShadow;
		}
		set
		{
			this._privateProperties.enableShadow = value;
			this.isDirty = true;
		}
	}

	public Color ShadowColor
	{
		get
		{
			return this._privateProperties.shadowColor;
		}
		set
		{
			this._privateProperties.shadowColor = value;
			this.SetShadowColor(this._privateProperties.shadowColor);
		}
	}

	public Vector3 ShadowDistance
	{
		get
		{
			return this._privateProperties.shadowDistance;
		}
		set
		{
			this._privateProperties.shadowDistance = value;
			this.isDirty = true;
		}
	}

	public bool EnableOutline
	{
		get
		{
			return this._privateProperties.enableOutline;
		}
		set
		{
			this._privateProperties.enableOutline = value;
			this.isDirty = true;
		}
	}

	public Color OutlineColor
	{
		get
		{
			return this._privateProperties.outlineColor;
		}
		set
		{
			this._privateProperties.outlineColor = value;
			this.SetOutlineColor(this._privateProperties.outlineColor);
		}
	}

	public float OutLineWidth
	{
		get
		{
			return this._privateProperties.outLineWidth;
		}
		set
		{
			this._privateProperties.outLineWidth = value;
			this.isDirty = true;
		}
	}

	private void OnEnable()
	{
		Font font = this._privateProperties.font;
		font.textureRebuildCallback = (Font.FontTextureRebuildCallback)Delegate.Combine(font.textureRebuildCallback, new Font.FontTextureRebuildCallback(this.FontTexureRebuild));
	}

	private void Start()
	{
		this.CacheTextVars();
		this.RefreshMesh(true);
	}

	public void CacheTextVars()
	{
		this.textMeshFilter = base.GetComponent<MeshFilter>();
		if (this.textMeshFilter == null)
		{
			this.textMeshFilter = base.gameObject.AddComponent<MeshFilter>();
		}
		this.textMesh = this.textMeshFilter.sharedMesh;
		if (this.textMesh == null)
		{
			this.textMesh = new Mesh();
			this.textMesh.name = base.gameObject.name + base.GetInstanceID().ToString();
			this.textMeshFilter.sharedMesh = this.textMesh;
		}
		this.textRenderer = base.renderer;
		if (this.textRenderer == null)
		{
			this.textRenderer = base.gameObject.AddComponent<MeshRenderer>();
		}
		if (!this.dontOverrideMaterials)
		{
			if (this._privateProperties.customFillMaterial != null)
			{
				if (this._privateProperties.enableShadow || this._privateProperties.enableOutline)
				{
					if (this.textRenderer.sharedMaterials.Length < 2)
					{
						this.textRenderer.sharedMaterials = new Material[]
						{
							this._privateProperties.font.material,
							this._privateProperties.customFillMaterial
						};
					}
					this._privateProperties.customFillMaterial.mainTexture = this._privateProperties.font.material.mainTexture;
					this.textRenderer.sharedMaterial = this._privateProperties.font.material;
				}
				else
				{
					this._privateProperties.customFillMaterial.mainTexture = this._privateProperties.font.material.mainTexture;
					this.textRenderer.sharedMaterial = this._privateProperties.customFillMaterial;
				}
			}
			else if (this.textRenderer.sharedMaterials == null)
			{
				this.textRenderer.sharedMaterials = new Material[]
				{
					this._privateProperties.font.material
				};
			}
			else
			{
				this.textRenderer.sharedMaterials = new Material[]
				{
					this.textRenderer.sharedMaterial
				};
			}
		}
	}

	private void RefreshMesh(bool _updateTexureInfo)
	{
		if (_updateTexureInfo)
		{
			this._privateProperties.font.RequestCharactersInTexture(this._privateProperties.text, this._privateProperties.fontSize);
		}
		this.textChars = null;
		this.textChars = this._privateProperties.text.ToCharArray();
		this.AnalizeText();
		int num = 1;
		if (this._privateProperties.enableShadow && this._privateProperties.enableOutline)
		{
			num = 6;
		}
		else if (this._privateProperties.enableOutline)
		{
			num = 5;
		}
		else if (this._privateProperties.enableShadow)
		{
			num = 2;
		}
		this.vertices = new Vector3[this.textChars.Length * 4 * num];
		this.triangles = new int[this.textChars.Length * 6 * num];
		this.uv = new Vector2[this.textChars.Length * 4 * num];
		this.uv2 = new Vector2[this.textChars.Length * 4 * num];
		this.colors = new Color[this.textChars.Length * 4 * num];
		int num2 = 0;
		int num3 = 0;
		if (this._privateProperties.enableShadow)
		{
			this.ResetHelperVariables();
			foreach (char character in this.textChars)
			{
				this.CreateCharacter(character, num2, this._privateProperties.shadowDistance, this._privateProperties.shadowColor, this._privateProperties.shadowColor);
				num2++;
			}
			this.SetAlignment(num3++);
		}
		if (this._privateProperties.enableOutline)
		{
			this.ResetHelperVariables();
			foreach (char character2 in this.textChars)
			{
				this.CreateCharacter(character2, num2, Vector3.right * this._privateProperties.outLineWidth, this._privateProperties.outlineColor, this._privateProperties.outlineColor);
				num2++;
			}
			this.SetAlignment(num3++);
			this.ResetHelperVariables();
			foreach (char character3 in this.textChars)
			{
				this.CreateCharacter(character3, num2, Vector3.left * this._privateProperties.outLineWidth, this._privateProperties.outlineColor, this._privateProperties.outlineColor);
				num2++;
			}
			this.SetAlignment(num3++);
			this.ResetHelperVariables();
			foreach (char character4 in this.textChars)
			{
				this.CreateCharacter(character4, num2, Vector3.up * this._privateProperties.outLineWidth, this._privateProperties.outlineColor, this._privateProperties.outlineColor);
				num2++;
			}
			this.SetAlignment(num3++);
			this.ResetHelperVariables();
			foreach (char character5 in this.textChars)
			{
				this.CreateCharacter(character5, num2, Vector3.down * this._privateProperties.outLineWidth, this._privateProperties.outlineColor, this._privateProperties.outlineColor);
				num2++;
			}
			this.SetAlignment(num3++);
		}
		this.ResetHelperVariables();
		foreach (char character6 in this.textChars)
		{
			this.CreateCharacter(character6, num2, Vector3.zero, this._privateProperties.fontColorTop, this._privateProperties.fontColorBottom);
			num2++;
		}
		this.SetAlignment(num3++);
		if (this.textMesh != null)
		{
			this.textMesh.Clear(true);
			this.SetAnchor();
			this.textMesh.vertices = this.vertices;
			this.textMesh.uv = this.uv;
			this.textMesh.uv2 = this.uv2;
			if (this._privateProperties.customFillMaterial != null && (this._privateProperties.enableShadow || this._privateProperties.enableOutline))
			{
				this.SetTrianglesForMultimesh();
			}
			else
			{
				this.textMesh.triangles = this.triangles;
			}
			this.textMesh.colors = this.colors;
		}
	}

	private void ResetHelperVariables()
	{
		this.lineBreakAccumulatedDistance.Clear();
		this.lineBreakCharCounter.Clear();
		this.currentLineBreak = 0;
		this.heightSum = 0f;
	}

	private void AnalizeText()
	{
		bool flag = true;
		while (flag)
		{
			flag = false;
			for (int i = 0; i < this.textChars.Length; i++)
			{
				if (this.textChars[i] == '\\' && i + 1 < this.textChars.Length && this.textChars[i + 1] == 'n')
				{
					char[] array = new char[this.textChars.Length - 1];
					int num = 0;
					for (int j = 0; j < this.textChars.Length; j++)
					{
						if (j == i)
						{
							array[num] = this.LINE_BREAK;
							num++;
						}
						else
						{
							if (j == i + 1)
							{
								j++;
								if (j >= this.textChars.Length)
								{
									goto IL_A8;
								}
							}
							array[num] = this.textChars[j];
							num++;
						}
						IL_A8:;
					}
					this.textChars = array;
					flag = true;
					break;
				}
			}
		}
	}

	private void CreateCharacter(char _character, int _arrayPosition, Vector3 _offset, Color _colorTop, Color _colorBottom)
	{
		if (this.lineBreakAccumulatedDistance.Count == 0)
		{
			this.lineBreakAccumulatedDistance.Add(0f);
		}
		if (this.lineBreakCharCounter.Count == 0)
		{
			this.lineBreakCharCounter.Add(0);
		}
		CharacterInfo characterInfo = default(CharacterInfo);
		if (!this._privateProperties.font.GetCharacterInfo(_character, out characterInfo, this._privateProperties.fontSize))
		{
			this.lineBreakCharCounter.Add(this.lineBreakCharCounter[this.currentLineBreak]);
			this.lineBreakAccumulatedDistance.Add(0f);
			this.currentLineBreak++;
			return;
		}
		List<int> list2;
		List<int> list = list2 = this.lineBreakCharCounter;
		int num;
		int index = num = this.currentLineBreak;
		num = list2[num];
		list[index] = num + 1;
		float num2 = this._privateProperties.size / (float)this._privateProperties.fontSize;
		_offset *= this._privateProperties.size * 0.1f;
		float num3 = characterInfo.vert.width * num2;
		float num4 = characterInfo.vert.height * num2;
		Vector2 vector = new Vector2(characterInfo.vert.x, characterInfo.vert.y) * num2;
		if (_character != ' ')
		{
			this.heightSum += (characterInfo.vert.y + characterInfo.vert.height * 0.5f) * num2;
		}
		Vector3 b = new Vector3(this.lineBreakAccumulatedDistance[this.currentLineBreak] * num2, -this._privateProperties.size * (float)this.currentLineBreak * this._privateProperties.lineSpacing, 0f);
		if (characterInfo.flipped)
		{
			this.vertices[4 * _arrayPosition] = new Vector3(vector.x + num3, num4 + vector.y, 0f) + _offset + b;
			this.vertices[4 * _arrayPosition + 1] = new Vector3(vector.x, num4 + vector.y, 0f) + _offset + b;
			this.vertices[4 * _arrayPosition + 2] = new Vector3(vector.x, vector.y, 0f) + _offset + b;
			this.vertices[4 * _arrayPosition + 3] = new Vector3(vector.x + num3, vector.y, 0f) + _offset + b;
		}
		else
		{
			this.vertices[4 * _arrayPosition] = new Vector3(vector.x + num3, num4 + vector.y, 0f) + _offset + b;
			this.vertices[4 * _arrayPosition + 1] = new Vector3(vector.x, num4 + vector.y, 0f) + _offset + b;
			this.vertices[4 * _arrayPosition + 2] = new Vector3(vector.x, vector.y, 0f) + _offset + b;
			this.vertices[4 * _arrayPosition + 3] = new Vector3(vector.x + num3, vector.y, 0f) + _offset + b;
		}
		List<float> list4;
		List<float> list3 = list4 = this.lineBreakAccumulatedDistance;
		int index2 = num = this.currentLineBreak;
		float num5 = list4[num];
		list3[index2] = num5 + characterInfo.width;
		this.triangles[6 * _arrayPosition] = _arrayPosition * 4;
		this.triangles[6 * _arrayPosition + 1] = _arrayPosition * 4 + 1;
		this.triangles[6 * _arrayPosition + 2] = _arrayPosition * 4 + 2;
		this.triangles[6 * _arrayPosition + 3] = _arrayPosition * 4;
		this.triangles[6 * _arrayPosition + 4] = _arrayPosition * 4 + 2;
		this.triangles[6 * _arrayPosition + 5] = _arrayPosition * 4 + 3;
		if (characterInfo.flipped)
		{
			this.uv[4 * _arrayPosition] = new Vector2(characterInfo.uv.x, characterInfo.uv.y + characterInfo.uv.height);
			this.uv[4 * _arrayPosition + 1] = new Vector2(characterInfo.uv.x, characterInfo.uv.y);
			this.uv[4 * _arrayPosition + 2] = new Vector2(characterInfo.uv.x + characterInfo.uv.width, characterInfo.uv.y);
			this.uv[4 * _arrayPosition + 3] = new Vector2(characterInfo.uv.x + characterInfo.uv.width, characterInfo.uv.y + characterInfo.uv.height);
		}
		else
		{
			this.uv[4 * _arrayPosition] = new Vector2(characterInfo.uv.x + characterInfo.uv.width, characterInfo.uv.y);
			this.uv[4 * _arrayPosition + 1] = new Vector2(characterInfo.uv.x, characterInfo.uv.y);
			this.uv[4 * _arrayPosition + 2] = new Vector2(characterInfo.uv.x, characterInfo.uv.y + characterInfo.uv.height);
			this.uv[4 * _arrayPosition + 3] = new Vector2(characterInfo.uv.x + characterInfo.uv.width, characterInfo.uv.y + characterInfo.uv.height);
		}
		if (this._privateProperties.customFillMaterial != null)
		{
			Vector2 b2 = new Vector2(_offset.x, _offset.y);
			Vector2 b3 = new Vector2(b.x, b.y);
			this.uv2[4 * _arrayPosition] = new Vector2(vector.x + num3, num4 + vector.y) + b2 + b3;
			this.uv2[4 * _arrayPosition + 1] = new Vector2(vector.x, num4 + vector.y) + b2 + b3;
			this.uv2[4 * _arrayPosition + 2] = new Vector2(vector.x, vector.y) + b2 + b3;
			this.uv2[4 * _arrayPosition + 3] = new Vector2(vector.x + num3, vector.y) + b2 + b3;
		}
		this.colors[4 * _arrayPosition] = _colorBottom;
		this.colors[4 * _arrayPosition + 1] = _colorBottom;
		this.colors[4 * _arrayPosition + 2] = _colorTop;
		this.colors[4 * _arrayPosition + 3] = _colorTop;
	}

	private void SetAnchor()
	{
		Vector2 zero = Vector2.zero;
		float num = 0f;
		for (int i = 0; i < this.lineBreakAccumulatedDistance.Count; i++)
		{
			if (this.lineBreakAccumulatedDistance[i] > num)
			{
				num = this.lineBreakAccumulatedDistance[i];
			}
		}
		switch (this._privateProperties.textAnchor)
		{
		case EasyFontTextMesh.TEXT_ANCHOR.UpperLeft:
		case EasyFontTextMesh.TEXT_ANCHOR.MiddleLeft:
		case EasyFontTextMesh.TEXT_ANCHOR.LowerLeft:
			switch (this._privateProperties.textAlignment)
			{
			case EasyFontTextMesh.TEXT_ALIGNMENT.left:
				zero.x = 0f;
				break;
			case EasyFontTextMesh.TEXT_ALIGNMENT.right:
				zero.x = num * this._privateProperties.size / (float)this._privateProperties.fontSize;
				break;
			case EasyFontTextMesh.TEXT_ALIGNMENT.center:
				zero.x += num * 0.5f * this._privateProperties.size / (float)this._privateProperties.fontSize;
				break;
			}
			break;
		case EasyFontTextMesh.TEXT_ANCHOR.UpperRight:
		case EasyFontTextMesh.TEXT_ANCHOR.MiddleRight:
		case EasyFontTextMesh.TEXT_ANCHOR.LowerRight:
			switch (this._privateProperties.textAlignment)
			{
			case EasyFontTextMesh.TEXT_ALIGNMENT.left:
				zero.x -= num * this._privateProperties.size / (float)this._privateProperties.fontSize;
				break;
			case EasyFontTextMesh.TEXT_ALIGNMENT.right:
				zero.x = 0f;
				break;
			case EasyFontTextMesh.TEXT_ALIGNMENT.center:
				zero.x -= num * 0.5f * this._privateProperties.size / (float)this._privateProperties.fontSize;
				break;
			}
			break;
		case EasyFontTextMesh.TEXT_ANCHOR.UpperCenter:
		case EasyFontTextMesh.TEXT_ANCHOR.MiddleCenter:
		case EasyFontTextMesh.TEXT_ANCHOR.LowerCenter:
			switch (this._privateProperties.textAlignment)
			{
			case EasyFontTextMesh.TEXT_ALIGNMENT.left:
				zero.x -= num * this._privateProperties.size * 0.5f / (float)this._privateProperties.fontSize;
				break;
			case EasyFontTextMesh.TEXT_ALIGNMENT.right:
				zero.x = num * 0.5f * this._privateProperties.size / (float)this._privateProperties.fontSize;
				break;
			case EasyFontTextMesh.TEXT_ALIGNMENT.center:
				zero.x = 0f;
				break;
			}
			break;
		}
		if (this._privateProperties.textAnchor == EasyFontTextMesh.TEXT_ANCHOR.UpperLeft || this._privateProperties.textAnchor == EasyFontTextMesh.TEXT_ANCHOR.UpperRight || this._privateProperties.textAnchor == EasyFontTextMesh.TEXT_ANCHOR.UpperCenter)
		{
			zero.y = -this.heightSum / (float)this.textChars.Length;
		}
		else if (this._privateProperties.textAnchor == EasyFontTextMesh.TEXT_ANCHOR.MiddleCenter || this._privateProperties.textAnchor == EasyFontTextMesh.TEXT_ANCHOR.MiddleLeft || this._privateProperties.textAnchor == EasyFontTextMesh.TEXT_ANCHOR.MiddleRight)
		{
			zero.y = -(this.heightSum / (float)this.textChars.Length) + this._privateProperties.size * (float)this.currentLineBreak * this._privateProperties.lineSpacing * 0.5f;
		}
		else if (this._privateProperties.textAnchor == EasyFontTextMesh.TEXT_ANCHOR.LowerLeft || this._privateProperties.textAnchor == EasyFontTextMesh.TEXT_ANCHOR.LowerRight || this._privateProperties.textAnchor == EasyFontTextMesh.TEXT_ANCHOR.LowerCenter)
		{
			zero.y = -this.heightSum / (float)this.textChars.Length + this._privateProperties.size * (float)this.currentLineBreak * this._privateProperties.lineSpacing;
		}
		for (int j = 0; j < this.vertices.Length; j++)
		{
			Vector3[] array = this.vertices;
			int num2 = j;
			array[num2].x = array[num2].x + zero.x;
			Vector3[] array2 = this.vertices;
			int num3 = j;
			array2[num3].y = array2[num3].y + zero.y;
		}
	}

	private void SetAlignment(int _pass)
	{
		int num = _pass * this.textChars.Length * 4;
		float num2 = 0f;
		for (int i = 0; i < this.lineBreakCharCounter.Count; i++)
		{
			switch (this._privateProperties.textAlignment)
			{
			case EasyFontTextMesh.TEXT_ALIGNMENT.right:
				num2 = -this.lineBreakAccumulatedDistance[i] * this._privateProperties.size / (float)this._privateProperties.fontSize;
				break;
			case EasyFontTextMesh.TEXT_ALIGNMENT.center:
				num2 = -this.lineBreakAccumulatedDistance[i] * 0.5f * this._privateProperties.size / (float)this._privateProperties.fontSize;
				break;
			}
			int num3;
			if (i == 0)
			{
				num3 = 0;
			}
			else
			{
				num3 = this.lineBreakCharCounter[i - 1] * 4;
			}
			int num4 = this.lineBreakCharCounter[i] * 4 - 1;
			for (int j = num3 + i * 4 + num; j <= num4 + i * 4 + num; j++)
			{
				Vector3[] array = this.vertices;
				int num5 = j;
				array[num5].x = array[num5].x + num2;
			}
		}
	}

	private void SetTrianglesForMultimesh()
	{
		int num = 0;
		if (this._privateProperties.enableOutline && this._privateProperties.enableShadow)
		{
			num = 5;
		}
		else if (this._privateProperties.enableOutline)
		{
			num = 4;
		}
		else if (this._privateProperties.enableShadow)
		{
			num = 1;
		}
		int num2 = num * 6 * this.textChars.Length;
		int[] array = new int[this.textChars.Length * 6];
		int num3 = 0;
		for (int i = num2; i < this.triangles.Length; i++)
		{
			array[num3] = this.triangles[i];
			num3++;
		}
		num3 = 0;
		int num4 = this.textChars.Length * num * 6;
		int[] array2 = new int[num4];
		for (int j = 0; j < num4; j++)
		{
			array2[num3] = this.triangles[j];
			num3++;
		}
		this.textMeshFilter.sharedMesh.subMeshCount = 2;
		this.textMeshFilter.sharedMesh.SetTriangles(array, 1);
		this.textMeshFilter.sharedMesh.SetTriangles(array2, 0);
	}

	private void FontTexureRebuild()
	{
		this.RefreshMesh(true);
	}

	private void OnDisable()
	{
		Font font = this._privateProperties.font;
		font.textureRebuildCallback = (Font.FontTextureRebuildCallback)Delegate.Remove(font.textureRebuildCallback, new Font.FontTextureRebuildCallback(this.FontTexureRebuild));
	}

	public void RefreshMeshEditor()
	{
		this.CacheTextVars();
		UnityEngine.Object.DestroyImmediate(this.textMesh);
		this.textMesh = new Mesh();
		this.textMesh.name = base.GetInstanceID().ToString();
		MeshFilter component = base.GetComponent<MeshFilter>();
		if (component != null)
		{
			component.sharedMesh = this.textMesh;
			if (base.renderer.sharedMaterial == null)
			{
				base.renderer.sharedMaterial = this._privateProperties.font.material;
			}
			this.RefreshMesh(true);
		}
	}

	public int GetVertexCount()
	{
		if (this.vertices != null)
		{
			return this.vertices.Length;
		}
		return 0;
	}

	private void LateUpdate()
	{
		if (this.isDirty)
		{
			this.isDirty = false;
			this.RefreshMesh(true);
		}
	}

	private void SetColor(Color _topColor, Color _bottomColor)
	{
		if (this.colors == null || this.textMesh == null)
		{
			return;
		}
		int initialVertexToColorize = this.GetInitialVertexToColorize(EasyFontTextMesh.TEXT_COMPONENT.Main);
		int num = 0;
		for (int i = initialVertexToColorize; i < this.GetFinalVertexToColorize(EasyFontTextMesh.TEXT_COMPONENT.Main); i++)
		{
			if (num == 0 || num == 1)
			{
				this.colors[i] = _bottomColor;
			}
			else
			{
				this.colors[i] = _topColor;
			}
			num++;
			if (num > 3)
			{
				num = 0;
			}
		}
		this.textMesh.colors = this.colors;
	}

	public void SetColor(Color _color)
	{
		if (this.colors == null || this.textMesh == null)
		{
			return;
		}
		int initialVertexToColorize = this.GetInitialVertexToColorize(EasyFontTextMesh.TEXT_COMPONENT.Main);
		for (int i = initialVertexToColorize; i < this.GetFinalVertexToColorize(EasyFontTextMesh.TEXT_COMPONENT.Main); i++)
		{
			this.colors[i] = _color;
		}
		this.textMesh.colors = this.colors;
	}

	private void SetShadowColor(Color _color)
	{
		if (this.colors == null || this.textMesh == null)
		{
			return;
		}
		int initialVertexToColorize = this.GetInitialVertexToColorize(EasyFontTextMesh.TEXT_COMPONENT.Shadow);
		for (int i = initialVertexToColorize; i < this.GetFinalVertexToColorize(EasyFontTextMesh.TEXT_COMPONENT.Shadow); i++)
		{
			this.colors[i] = _color;
		}
		this.textMesh.colors = this.colors;
	}

	private void SetOutlineColor(Color _color)
	{
		if (this.colors == null || this.textMesh == null)
		{
			return;
		}
		int initialVertexToColorize = this.GetInitialVertexToColorize(EasyFontTextMesh.TEXT_COMPONENT.Outline);
		for (int i = initialVertexToColorize; i < this.GetFinalVertexToColorize(EasyFontTextMesh.TEXT_COMPONENT.Shadow); i++)
		{
			this.colors[i] = _color;
		}
		this.textMesh.colors = this.colors;
	}

	private int GetInitialVertexToColorize(EasyFontTextMesh.TEXT_COMPONENT _textComponent)
	{
		if (this.textChars == null)
		{
			this.textChars = this._privateProperties.text.ToCharArray();
		}
		int num = 0;
		switch (_textComponent)
		{
		case EasyFontTextMesh.TEXT_COMPONENT.Main:
			if (this._privateProperties.enableShadow && this._privateProperties.enableOutline)
			{
				num = 5;
			}
			else if (this._privateProperties.enableOutline)
			{
				num = 4;
			}
			else if (this._privateProperties.enableShadow)
			{
				num = 1;
			}
			break;
		case EasyFontTextMesh.TEXT_COMPONENT.Shadow:
			num = 0;
			break;
		case EasyFontTextMesh.TEXT_COMPONENT.Outline:
			num = 1;
			break;
		}
		return this.textChars.Length * 4 * num;
	}

	private int GetFinalVertexToColorize(EasyFontTextMesh.TEXT_COMPONENT _textComponent)
	{
		if (this.textChars == null)
		{
			this.textChars = this._privateProperties.text.ToCharArray();
		}
		int result = 0;
		int num = 0;
		switch (_textComponent)
		{
		case EasyFontTextMesh.TEXT_COMPONENT.Main:
			if (this._privateProperties.enableShadow && this._privateProperties.enableOutline)
			{
				num = 6;
			}
			else if (this._privateProperties.enableOutline)
			{
				num = 5;
			}
			else if (this._privateProperties.enableShadow)
			{
				num = 2;
			}
			result = this.textChars.Length * 4 * num;
			break;
		case EasyFontTextMesh.TEXT_COMPONENT.Shadow:
			result = this.textChars.Length * 4;
			break;
		case EasyFontTextMesh.TEXT_COMPONENT.Outline:
			if (this._privateProperties.enableShadow)
			{
				num = 2;
			}
			else
			{
				num = 1;
			}
			result = this.textChars.Length * 4 * (num + 4);
			break;
		}
		return result;
	}

	private void ChangeFont()
	{
		if (!this.dontOverrideMaterials && this._privateProperties.customFillMaterial == null)
		{
			this.textRenderer.sharedMaterial = this._privateProperties.font.material;
		}
		this.isDirty = true;
	}

	[HideInInspector]
	public EasyFontTextMesh.TextProperties _privateProperties;

	public bool dontOverrideMaterials;

	private Mesh textMesh;

	private MeshFilter textMeshFilter;

	private Material fontMaterial;

	private Renderer textRenderer;

	private char[] textChars;

	private bool isDirty;

	private int currentLineBreak;

	private float heightSum;

	private List<int> lineBreakCharCounter = new List<int>();

	private List<float> lineBreakAccumulatedDistance = new List<float>();

	private Vector3[] vertices;

	private int[] triangles;

	private Vector2[] uv;

	private Vector2[] uv2;

	private Color[] colors;

	[HideInInspector]
	public bool GUIChanged;

	private char LINE_BREAK = Convert.ToChar(10);

	public enum TEXT_ANCHOR
	{
		UpperLeft,
		UpperRight,
		UpperCenter,
		MiddleLeft,
		MiddleRight,
		MiddleCenter,
		LowerLeft,
		LowerRight,
		LowerCenter
	}

	public enum TEXT_ALIGNMENT
	{
		left,
		right,
		center
	}

	private enum TEXT_COMPONENT
	{
		Main,
		Shadow,
		Outline
	}

	[Serializable]
	public class TextProperties
	{
		public TextProperties()
		{
		}

		public string text = "Hello World!";

		public Font font;

		public Material customFillMaterial;

		public int fontSize = 16;

		public float size = 16f;

		public EasyFontTextMesh.TEXT_ANCHOR textAnchor;

		public EasyFontTextMesh.TEXT_ALIGNMENT textAlignment;

		public float lineSpacing = 1f;

		public Color fontColorTop = new Color(1f, 1f, 1f, 1f);

		public Color fontColorBottom = new Color(1f, 1f, 1f, 1f);

		public bool enableShadow;

		public Color shadowColor = new Color(0f, 0f, 0f, 1f);

		public Vector3 shadowDistance = new Vector3(0f, -1f, 0f);

		public bool enableOutline;

		public Color outlineColor = new Color(0f, 0f, 0f, 1f);

		public float outLineWidth = 0.3f;
	}
}

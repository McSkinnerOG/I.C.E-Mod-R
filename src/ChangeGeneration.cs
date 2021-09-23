using System;
using UnityEngine;

public class ChangeGeneration : MonoBehaviour
{
	public ChangeGeneration()
	{
	}

	private void Start()
	{
		this.m_ants = UnityEngine.Object.FindObjectsOfType<NeuralAnt>();
	}

	private void Update()
	{
		if (this.m_ants != null)
		{
			this.m_generationDuration += Time.deltaTime;
			string text = string.Empty;
			int num = -1;
			for (int i = 0; i < this.m_ants.Length; i++)
			{
				if (9 < this.m_ants[i].m_score)
				{
					num = i;
				}
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					i.ToString(),
					": ",
					this.m_ants[i].m_score,
					"\n"
				});
			}
			this.m_debugTxt.text = text;
			if (num != -1)
			{
				this.m_generationCounter++;
				int num2 = -1;
				int num3 = -1;
				for (int j = 0; j < this.m_ants.Length; j++)
				{
					if (num3 < this.m_ants[j].m_score && j != num)
					{
						num2 = j;
						num3 = this.m_ants[j].m_score;
					}
				}
				for (int k = 0; k < this.m_ants.Length; k++)
				{
					if (k != num && k != num2)
					{
						this.m_ants[k].BecomeChild(this.m_ants[num], this.m_ants[num2]);
					}
					this.m_ants[k].m_score = 0;
				}
				Debug.Log(string.Concat(new object[]
				{
					"Generation ",
					this.m_generationCounter,
					" over! It took: ",
					this.m_generationDuration,
					" and index ",
					num,
					" won. Second: ",
					num2,
					" Avg Gen Dur: ",
					Time.time / (float)this.m_generationCounter
				}));
				this.m_generationDuration = 0f;
			}
		}
	}

	public GUIText m_debugTxt;

	private NeuralAnt[] m_ants;

	private float m_generationDuration;

	private int m_generationCounter;
}

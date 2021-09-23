using System;
using UnityEngine;

public class SoccerManager : MonoBehaviour
{
	public SoccerManager()
	{
	}

	private void Start()
	{
		this.m_ballStartPos = this.m_ball.position;
		this.m_bots = UnityEngine.Object.FindObjectsOfType<SoccerBot>();
		this.m_botStartPos = new Vector3[this.m_bots.Length];
		for (int i = 0; i < this.m_bots.Length; i++)
		{
			this.m_botStartPos[i] = this.m_bots[i].transform.position;
		}
	}

	private void Update()
	{
		Time.timeScale = this.m_timeScale;
		if (this.m_bots != null)
		{
			this.m_generationDuration += Time.deltaTime;
			this.m_debugTxt.text = this.m_goalsA + " : " + this.m_goalsB;
			bool flag = 0 < this.m_goalsA;
			bool flag2 = 0 < this.m_goalsB;
			if (flag || flag2)
			{
				this.m_generationCounter++;
				int num = -1;
				int num2 = -1;
				int num3 = -1;
				int num4 = -1;
				for (int i = 0; i < this.m_bots.Length; i++)
				{
					if (flag)
					{
						if (this.m_bots[i].m_inTeamA)
						{
							if (num == -1)
							{
								num = i;
							}
							else
							{
								num2 = i;
							}
						}
						else if (num3 == -1)
						{
							num3 = i;
						}
						else
						{
							num4 = i;
						}
					}
					else if (this.m_bots[i].m_inTeamA)
					{
						if (num3 == -1)
						{
							num3 = i;
						}
						else
						{
							num4 = i;
						}
					}
					else if (num == -1)
					{
						num = i;
					}
					else
					{
						num2 = i;
					}
				}
				this.m_bots[num3].m_neuralNet.BecomeChild(this.m_bots[num].m_neuralNet, this.m_bots[num2].m_neuralNet);
				this.m_bots[num4].m_neuralNet.BecomeChild(this.m_bots[num].m_neuralNet, this.m_bots[num2].m_neuralNet);
				Debug.Log(string.Concat(new object[]
				{
					"Generation ",
					this.m_generationCounter,
					" over! Team A Won: ",
					flag,
					" It took: ",
					this.m_generationDuration,
					" Avg Gen Dur: ",
					Time.time / (float)this.m_generationCounter
				}));
				this.ResetRound();
				this.m_generationDuration = 0f;
			}
		}
	}

	private void ResetRound()
	{
		this.m_ball.transform.position = this.m_ballStartPos;
		for (int i = 0; i < this.m_bots.Length; i++)
		{
			this.m_bots[i].transform.position = this.m_botStartPos[i];
		}
		this.m_goalsA = (this.m_goalsB = 0);
	}

	public Transform m_ball;

	public GUIText m_debugTxt;

	public float m_timeScale = 1f;

	[HideInInspector]
	public int m_goalsA;

	[HideInInspector]
	public int m_goalsB;

	private SoccerBot[] m_bots;

	private float m_generationDuration;

	private int m_generationCounter;

	private Vector3 m_ballStartPos = Vector3.zero;

	private Vector3[] m_botStartPos;
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class EsportManTest : MonoBehaviour
{
	public EsportManTest()
	{
	}

	private void FixedUpdate()
	{
		this.m_startSkill += UnityEngine.Random.Range(-1f, 1f);
		this.m_gamesPlayed++;
		this.m_strDevTxt.text = string.Concat(new object[]
		{
			"Games played: ",
			this.m_gamesPlayed,
			"\nSkill: ",
			this.m_startSkill
		});
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			this.m_teamA.Clear();
			this.m_teamB.Clear();
			this.m_teamAtxt.text = string.Empty;
			this.m_teamBtxt.text = string.Empty;
			this.m_resultsTxt.text = string.Empty;
			for (int i = 0; i < 5; i++)
			{
				this.m_teamA.Add(new EsportPlayer(UnityEngine.Random.Range(0f, 99f), UnityEngine.Random.Range(0f, 99f), UnityEngine.Random.Range(0f, 99f)));
				this.m_teamB.Add(new EsportPlayer(UnityEngine.Random.Range(0f, 99f), UnityEngine.Random.Range(0f, 99f), UnityEngine.Random.Range(0f, 99f)));
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			string text;
			for (int j = 0; j < this.m_matchCount; j++)
			{
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				while (num6 < 30 && num4 < 16 && num5 < 16)
				{
					while (0 < this.GetAlivePlayerCount(this.m_teamA) && 0 < this.GetAlivePlayerCount(this.m_teamB))
					{
						int randomAlivePlayerIndex = this.GetRandomAlivePlayerIndex(this.m_teamA);
						int randomAlivePlayerIndex2 = this.GetRandomAlivePlayerIndex(this.m_teamB);
						float strength = this.m_teamA[randomAlivePlayerIndex].strength;
						float strength2 = this.m_teamB[randomAlivePlayerIndex2].strength;
						float num7 = strength / (strength + strength2);
						if (UnityEngine.Random.Range(0f, 1f) < num7)
						{
							this.m_teamA[randomAlivePlayerIndex].kills++;
							this.m_teamB[randomAlivePlayerIndex2].deaths++;
							this.m_teamB[randomAlivePlayerIndex2].alive = false;
						}
						else
						{
							this.m_teamA[randomAlivePlayerIndex].deaths++;
							this.m_teamB[randomAlivePlayerIndex2].kills++;
							this.m_teamA[randomAlivePlayerIndex].alive = false;
						}
					}
					if (this.GetAlivePlayerCount(this.m_teamA) == 0)
					{
						num5++;
					}
					else
					{
						num4++;
					}
					for (int k = 0; k < 5; k++)
					{
						this.m_teamA[k].alive = true;
						this.m_teamB[k].alive = true;
					}
					num6++;
				}
				if (num4 > num5)
				{
					num++;
				}
				else if (num4 == num5)
				{
					num3++;
				}
				else
				{
					num2++;
				}
				TextMesh resultsTxt = this.m_resultsTxt;
				text = resultsTxt.text;
				resultsTxt.text = string.Concat(new object[]
				{
					text,
					num4.ToString(),
					":",
					num5,
					", "
				});
			}
			TextMesh resultsTxt2 = this.m_resultsTxt;
			text = resultsTxt2.text;
			resultsTxt2.text = string.Concat(new object[]
			{
				text,
				"\n",
				num,
				" : ",
				num3,
				" : ",
				num2
			});
			float num8 = 0f;
			float num9 = 0f;
			float num10 = 0f;
			float num11 = 0f;
			for (int l = 0; l < 5; l++)
			{
				num8 += this.m_teamA[l].strength;
				num9 += this.m_teamB[l].strength;
				num10 += this.m_teamA[l].skill;
				num11 += this.m_teamB[l].skill;
			}
			num8 /= 5f;
			num9 /= 5f;
			num10 /= 5f;
			num11 /= 5f;
			for (int m = 0; m < 5; m++)
			{
				TextMesh teamAtxt = this.m_teamAtxt;
				text = teamAtxt.text;
				teamAtxt.text = string.Concat(new object[]
				{
					text,
					m.ToString(),
					" S ",
					(int)this.m_teamA[m].skill,
					" E ",
					(int)this.m_teamA[m].experience,
					" M ",
					(int)this.m_teamA[m].motivation,
					" Str ",
					(int)this.m_teamA[m].strength,
					" KD ",
					this.m_teamA[m].kills / this.m_matchCount,
					":",
					this.m_teamA[m].deaths / this.m_matchCount,
					"\n"
				});
				TextMesh teamBtxt = this.m_teamBtxt;
				text = teamBtxt.text;
				teamBtxt.text = string.Concat(new object[]
				{
					text,
					m.ToString(),
					" S ",
					(int)this.m_teamB[m].skill,
					" E ",
					(int)this.m_teamB[m].experience,
					" M ",
					(int)this.m_teamB[m].motivation,
					" Str ",
					(int)this.m_teamB[m].strength,
					" KD ",
					this.m_teamB[m].kills / this.m_matchCount,
					":",
					this.m_teamB[m].deaths / this.m_matchCount,
					"\n"
				});
			}
			TextMesh teamAtxt2 = this.m_teamAtxt;
			text = teamAtxt2.text;
			teamAtxt2.text = string.Concat(new object[]
			{
				text,
				" Str: ",
				num8,
				" T: ",
				num10
			});
			TextMesh teamBtxt2 = this.m_teamBtxt;
			text = teamBtxt2.text;
			teamBtxt2.text = string.Concat(new object[]
			{
				text,
				" Str: ",
				num9,
				" T: ",
				num11
			});
		}
	}

	private int GetAlivePlayerCount(List<EsportPlayer> a_players)
	{
		int num = 0;
		if (a_players != null)
		{
			for (int i = 0; i < a_players.Count; i++)
			{
				if (a_players[i].alive)
				{
					num++;
				}
			}
		}
		return num;
	}

	private int GetRandomAlivePlayerIndex(List<EsportPlayer> a_players)
	{
		if (a_players != null)
		{
			int num = UnityEngine.Random.Range(0, a_players.Count);
			for (int i = 0; i < a_players.Count; i++)
			{
				int num2 = (i + num) % a_players.Count;
				if (a_players[num2].alive)
				{
					return num2;
				}
			}
		}
		return -1;
	}

	public TextMesh m_teamAtxt;

	public TextMesh m_teamBtxt;

	public TextMesh m_resultsTxt;

	public TextMesh m_strDevTxt;

	public int m_matchCount = 1;

	private List<EsportPlayer> m_teamA = new List<EsportPlayer>();

	private List<EsportPlayer> m_teamB = new List<EsportPlayer>();

	private float m_startSkill = 50f;

	private int m_gamesPlayed;
}

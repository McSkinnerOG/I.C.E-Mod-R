using System;

public class EsportPlayer
{
	public EsportPlayer(float a_skill, float a_experience, float a_motivation)
	{
		this.skill = a_skill;
		this.experience = a_experience;
		this.motivation = a_motivation;
		this.alive = true;
		this.kills = 0;
		this.deaths = 0;
		this.strength = (this.skill * 3f + this.experience * 2f + this.motivation * 1f) / 6f;
	}

	public float skill;

	public float experience;

	public float motivation;

	public float strength;

	public bool alive;

	public int kills;

	public int deaths;
}

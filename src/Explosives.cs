using System;

public class Explosives : ServerBuilding
{
	public Explosives()
	{
	}

	public override bool Use(ServerPlayer a_player)
	{
		return false;
	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Update()
	{
		if (0.1f > this.GetState() && !this.m_exploded && null != this.m_server)
		{
			this.m_server.DealExplosionDamage(base.transform.position, this.m_maxDamage, this.m_radius);
			this.m_exploded = true;
		}
		base.Update();
	}

	public float m_maxDamage = 10000f;

	public float m_radius = 5f;

	private bool m_exploded;
}

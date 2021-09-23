using System;

public struct BuildingDef
{
	public BuildingDef(string a_ident, bool a_persistent = false, double a_decayTime = 0.0)
	{
		this.ident = a_ident;
		this.persistent = a_persistent;
		this.decayTime = a_decayTime;
	}

	public string ident;

	public bool persistent;

	public double decayTime;
}

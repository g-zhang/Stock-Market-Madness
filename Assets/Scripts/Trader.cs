using System.Collections.Generic;

public class Trader
{
	#region Fields
	public readonly string name;
	public float money;
	public Dictionary<Stock, int> shares;
	#endregion

	#region Constructor
	public Trader(string inName, float startMoney)
	{
		name = inName;
		money = startMoney;

		shares = new Dictionary<Stock, int>();

		return;
	}
	#endregion
}
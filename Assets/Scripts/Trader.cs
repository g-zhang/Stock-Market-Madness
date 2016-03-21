using System.Collections.Generic;

public class Trader {

	public string name;
	public float money;
	public Dictionary<Stock, int> shares;

	public Trader(string n, float m) {
		shares = new Dictionary<Stock, int>();
		name = n; money = m;
	}
	public Trader() : this("", 0f) { }
	public Trader(float m) : this("", m) { }
	public Trader(string n) : this(n, 0f) { }
}
using System.Collections.Generic;

public class Stock {

	public string name;
	public int numAvailable;
	public List<List<float>> priceHistoryByPeriod;

	public float Price {
		get {
			return (ThisPeriodData.Count == 0) ? float.MaxValue : ThisPeriodData[ThisPeriodData.Count - 1];
		}
		set { ThisPeriodData.Add(value); }
	}

	public Dictionary<Trader, int> owners {
		get {
			Dictionary<Trader, int> view = new Dictionary<Trader, int>();
			foreach (Trader t in Model.S.traders) view.Add(t, t.shares[this]);
			return view;
		}
	}

	public void AdvancePeriod() {
		priceHistoryByPeriod.Add(new List<float>());
	}

	public float PeriodDelta {
		get { return Price - ThisPeriodData[0]; }
	}

	public List<float> ThisPeriodData {
		get { return PeriodsAgoData(0); }
	}

	public List<float> LastNPeriods(int n) {
		List<float> l = new List<float>();
		for (int i = 0; i < n; i++) l.AddRange(PeriodsAgoData(0));
		return l;
	}

	public List<float> PeriodsAgoData(int periodsAgo) {
		int periodIdx = priceHistoryByPeriod.Count - periodsAgo - 1;
		return (periodIdx < 0) ? new List<float>() : priceHistoryByPeriod[periodIdx];
	}
}

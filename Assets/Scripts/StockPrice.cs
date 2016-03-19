using UnityEngine;
using System.Collections.Generic;

public class StockPrice {
	
	public List<List<float>> priceHistoryByPeriod;
	
	public void AdvancePeriod() {
		priceHistoryByPeriod.Add(new List<float>());
	}

	public float PeriodDelta {
		get {
			return Price - ThisPeriodData[0];
		}
	}

	public List<float> ThisPeriodData {
		get {
			return PeriodsAgoData(0);
		}
	}

	public List<float> PeriodsAgoData(int periodsAgo) {
		int periodIdx = priceHistoryByPeriod.Count - periodsAgo - 1;
		return (periodIdx < 0) ? new List<float>() : priceHistoryByPeriod[periodIdx];
	}

	public float Price {
		get {
			return (ThisPeriodData.Count == 0) ? Mathf.Infinity : ThisPeriodData[ThisPeriodData.Count - 1];
		}
		set {
			ThisPeriodData.Add(value);
		}
	}
}

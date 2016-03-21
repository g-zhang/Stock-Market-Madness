using UnityEngine;
using System.Collections.Generic;

public class Stock
{
	#region Tuning Fields
	public string name;
	public int numAvailable;

	// TODO: Wrap this into a "generator" class that can be plugged in.
	// I want this more flexible than the straight linear interpolation here.
	public float negMinDiffWeight;
	public float negMaxDiffWeight;

	public float posMinDiffWeight;
	public float posMaxDiffWeight;

	public float buyValLerpWeight;
	public float sellValLerpWeight;

	public float randStartMinValue;
	public float randStartMaxValue;
	#endregion

	#region Dynamic Fields
	public int currNumSold;
	public int currNumBought;

	public List<List<float>> priceHistoryByPeriod;
	#endregion

	public Stock()
	{
		currNumSold = 0;
		currNumBought = 0;

		return;
	}

	public float Price
	{
		get
		{
			return (ThisPeriodData.Count == 0) ?
				float.MaxValue : ThisPeriodData[ThisPeriodData.Count - 1];
		}
		set
		{
			ThisPeriodData.Add(value);
			return;
		}
	}

	public void Tick()
	{
		float minVal = randStartMinValue;
		float maxVal = randStartMaxValue;

		int difference = currNumBought - currNumSold;
		if (difference < 0)
		{
			minVal += negMinDiffWeight * difference;
			maxVal += negMaxDiffWeight * difference;
		}
		else if (difference > 0)
		{
			minVal += posMinDiffWeight * difference;
			maxVal += posMaxDiffWeight * difference;
		}
		minVal = Mathf.Max(minVal, 1 - Price);

		currNumSold =
			Mathf.FloorToInt(Mathf.Lerp(0f, currNumSold, sellValLerpWeight));
		currNumBought =
			Mathf.FloorToInt(Mathf.Lerp(0f, currNumBought, buyValLerpWeight));

		Price += Random.Range(minVal, maxVal);
		return;
	}

	public Dictionary<Trader, int> owners
	{
		get
		{
			Dictionary<Trader, int> view = new Dictionary<Trader, int>();
			foreach (Trader t in Model.Instance.traders) view.Add(t, t.shares[this]);
			return view;
		}
	}

	public void AdvancePeriod()
	{
		priceHistoryByPeriod.Add(new List<float>());
	}

	public float PeriodDelta
	{
		get { return Price - ThisPeriodData[0]; }
	}

	public List<float> ThisPeriodData
	{
		get { return PeriodsAgoData(0); }
	}

	public List<float> LastNPeriods(int n)
	{
		List<float> l = new List<float>();
		for (int i = 0; i < n; i++)
		{
			l.AddRange(PeriodsAgoData(0));
		}
		return l;
	}

	public List<float> PeriodsAgoData(int periodsAgo)
	{
		int periodIdx = priceHistoryByPeriod.Count - periodsAgo - 1;
		return (periodIdx < 0) ? new List<float>() : priceHistoryByPeriod[periodIdx];
	}
}
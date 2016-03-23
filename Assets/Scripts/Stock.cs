using UnityEngine;
using System.Collections.Generic;

public class Stock
{
	#region Tuning Fields
	public readonly string name;
	private int numAvailable;

	// TODO: Wrap this into a "generator" class that can be plugged in.
	// I want this more flexible than the straight linear interpolation here.
	private float diffWeights;
	private float lerpWeights;
	private float randStartAbsVal;
	private float minStockPrice = 1f;
	#endregion

	#region Dynamic Fields
	private Dictionary<Trader, int> tradersCurrNumSold;
	private int generalCurrNumSold;
	private int companyCurrNumSold;

	private Dictionary<Trader, int> tradersCurrNumBought;
	private int generalCurrNumBought;
	private int companyCurrNumBought;
	
	public List<float> priceHistory;
	#endregion

	#region Constructor
	public Stock(string inName, int inNumStocks, float startPriceBase,
		float inDiffWeights, float inLerpWeights, float inRandStartAbsVal)
	{
		name = inName;
		numAvailable = inNumStocks;

		tradersCurrNumSold = new Dictionary<Trader, int>();
		generalCurrNumSold = 0;
		companyCurrNumSold = 0;

		tradersCurrNumBought = new Dictionary<Trader, int>();
		generalCurrNumBought = 0;
		companyCurrNumBought = 0;

		diffWeights = inDiffWeights;
		lerpWeights = inLerpWeights;
		randStartAbsVal = inRandStartAbsVal;

		priceHistory = new List<float>();

		Price = startPriceBase + Random.Range(-randStartAbsVal, randStartAbsVal);

		return;
	}
	#endregion

	#region Properties
	public int NumStocksAvailable
	{
		get
		{
			return numAvailable;
		}
	}

	public float Price
	{
		get
		{
			return (priceHistory.Count == 0) ?
				float.MaxValue :
				priceHistory[priceHistory.Count - 1];
		}
		private set
		{
			priceHistory.Add(value);
			return;
		}
	}
	#endregion

	#region Data Progression Methods
	public void Tick()
	{
		int currNumSold = generalCurrNumSold + companyCurrNumSold;
		foreach (int numSold in tradersCurrNumSold.Values)
		{
			currNumSold += numSold;
		}

		int currNumBought = generalCurrNumBought + companyCurrNumBought;
		foreach (int numBought in tradersCurrNumBought.Values)
		{
			currNumBought += numBought;
		}

		float minVal = -randStartAbsVal;
		float maxVal = randStartAbsVal;

		int difference = currNumBought - currNumSold;
		if (difference < 0)
		{
			minVal += diffWeights * difference;
			maxVal += diffWeights * difference;
		}
		else if (difference > 0)
		{
			minVal += diffWeights * difference;
			maxVal += diffWeights * difference;
		}

		minVal = Mathf.Max(minVal, 1 - Price);
		maxVal = Mathf.Max(minVal, maxVal);

		generalCurrNumSold =
			Mathf.FloorToInt(Mathf.Lerp(0f, generalCurrNumSold, lerpWeights));
		companyCurrNumSold =
			Mathf.FloorToInt(Mathf.Lerp(0f, companyCurrNumSold, lerpWeights));

		generalCurrNumBought =
			Mathf.FloorToInt(Mathf.Lerp(0f, generalCurrNumBought, lerpWeights));
		companyCurrNumBought =
			Mathf.FloorToInt(Mathf.Lerp(0f, companyCurrNumBought, lerpWeights));

		foreach (Trader trader in Model.Instance.traders)
		{
			tradersCurrNumSold[trader] =
				Mathf.FloorToInt(Mathf.Lerp(0f, tradersCurrNumSold[trader], lerpWeights));
			tradersCurrNumBought[trader] =
				Mathf.FloorToInt(Mathf.Lerp(0f, tradersCurrNumBought[trader], lerpWeights));
		}

		currNumSold =
			Mathf.FloorToInt(Mathf.Lerp(0f, currNumSold, lerpWeights));
		currNumBought =
			Mathf.FloorToInt(Mathf.Lerp(0f, currNumBought, lerpWeights));

		Price += Random.Range(minVal, maxVal);
		return;
	}
	#endregion

	#region General Interface Methods
	public bool Buy(Trader trader, int numStocks)
	{
		if (numStocks > numAvailable)
		{
			return false;
		}
		numAvailable -= numStocks;

		if (tradersCurrNumBought.ContainsKey(trader))
		{
			tradersCurrNumBought[trader] += numStocks;
		}

		return true;
	}

	public void Sell(Trader trader, int numStocks)
	{
		numAvailable += numStocks;

		if (tradersCurrNumSold.ContainsKey(trader))
		{
			tradersCurrNumSold[trader] += numStocks;
		}

		return;
	}

	public void AddTrader(Trader trader)
	{
		tradersCurrNumSold.Add(trader, 0);
		tradersCurrNumBought.Add(trader, 0);

		return;
	}
	#endregion

	/*
	I'm not sure what the purpose of this is, so I'm commenting it out for now.

	David - the purpose is that you can weight players' decisions' importances based on who owns how much of the company.

	public Dictionary<Trader, int> owners
	{
		get
		{
			Dictionary<Trader, int> view = new Dictionary<Trader, int>();
			foreach (Trader t in Model.Instance.traders) view.Add(t, t.shares[this]);
			return view;
		}
	}
	*/
}
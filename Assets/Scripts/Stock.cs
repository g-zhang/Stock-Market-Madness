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
	#endregion

	#region Dynamic Fields
	private int currNumSold;
	private int currNumBought;

	public List<float> priceHistory;
	#endregion

	#region Constructor
	public Stock(string inName, int inNumStocks, float startPriceBase,
		float inDiffWeights, float inLerpWeights, float inRandStartAbsVal)
	{
		currNumSold = 0;
		currNumBought = 0;

		name = inName;
		numAvailable = inNumStocks;

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

		currNumSold =
			Mathf.FloorToInt(Mathf.Lerp(0f, currNumSold, lerpWeights));
		currNumBought =
			Mathf.FloorToInt(Mathf.Lerp(0f, currNumBought, lerpWeights));

		Price += Random.Range(minVal, maxVal);
		return;
	}
	#endregion

	public bool Buy(int numStocks)
	{
		if (numStocks > numAvailable)
		{
			return false;
		}

		currNumBought += numStocks;
		numAvailable -= numStocks;

		return true;
	}

	public void Sell(int numStocks)
	{
		currNumSold += numStocks;
		numAvailable -= numStocks;

		return;
	}
	

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
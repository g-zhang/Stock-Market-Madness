using UnityEngine;
using System.Collections.Generic;

public struct BuySellData
{
	public Dictionary<Trader, int> tradersCurrNumSold;
	public int generalCurrNumSold;
	public int companyCurrNumSold;

	public Dictionary<Trader, int> tradersCurrNumBought;
	public int generalCurrNumBought;
	public int companyCurrNumBought;
}

public class Stock
{
	#region Tuning Fields
	public readonly string name;
	private int numAvailable;

	public StockGenerator generator;
	private float minStockPrice = 1f;
	#endregion

	#region Dynamic Fields
	private BuySellData buySellData;
	
	public List<float> priceHistory;
	#endregion

	#region Constructor
	public Stock(string inName, int inNumStocks, float startPriceBase,
		float startRandAbsVal, StockGenerator inGenerator)
	{
		name = inName;
		numAvailable = inNumStocks;

		buySellData.tradersCurrNumSold = new Dictionary<Trader, int>();
		buySellData.generalCurrNumSold = 0;
		buySellData.companyCurrNumSold = 0;

		buySellData.tradersCurrNumBought = new Dictionary<Trader, int>();
		buySellData.generalCurrNumBought = 0;
		buySellData.companyCurrNumBought = 0;

		generator = inGenerator;
		priceHistory = new List<float>();

		Price = startPriceBase + Random.Range(-startRandAbsVal, startRandAbsVal);

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
		float minVal;
		float maxVal;
		generator.getNextStockValue(buySellData, out minVal, out maxVal);

		minVal = Mathf.Max(minVal, minStockPrice - Price);
		maxVal = Mathf.Max(minVal, maxVal);

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

		if (buySellData.tradersCurrNumBought.ContainsKey(trader))
		{
			buySellData.tradersCurrNumBought[trader] += numStocks;
		}

		return true;
	}

	public void Sell(Trader trader, int numStocks)
	{
		numAvailable += numStocks;

		if (buySellData.tradersCurrNumSold.ContainsKey(trader))
		{
			buySellData.tradersCurrNumSold[trader] += numStocks;
		}

		return;
	}

	public void AddTrader(Trader trader)
	{
		buySellData.tradersCurrNumSold.Add(trader, 0);
		buySellData.tradersCurrNumBought.Add(trader, 0);

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
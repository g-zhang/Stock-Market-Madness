using UnityEngine;
using System.Collections.Generic;

public class Model
{
	#region Tuning Fields
	public readonly List<Stock> stocks = new List<Stock>
	{
		// I want to change this due to encapsulation issues.
		// I'll probably use a constructor, but I'm not sure yet.
		new Stock
		{
			name = "Company A",
			numAvailable = 1000000,

			negMinDiffWeight = 0.0001f,
			negMaxDiffWeight = 0.0001f,

			posMinDiffWeight = 0.0001f,
			posMaxDiffWeight = 0.0001f,

			buyValLerpWeight = 0.9f,
			sellValLerpWeight = 0.9f,

			randStartMinValue = -1f,
			randStartMaxValue = 1f,

			Price = Random.Range(-5f, 5f) + 25f
		},
		new Stock
		{
			name = "Company B",
			numAvailable = 1000000,

			negMinDiffWeight = 0.0001f,
			negMaxDiffWeight = 0.0001f,

			posMinDiffWeight = 0.0001f,
			posMaxDiffWeight = 0.0001f,

			buyValLerpWeight = 0.9f,
			sellValLerpWeight = 0.9f,

			randStartMinValue = -1f,
			randStartMaxValue = 1f,

			Price = Random.Range(-5f, 5f) + 25f
		},
		new Stock
		{
			name = "Company C",
			numAvailable = 1000000,

			negMinDiffWeight = 0.0001f,
			negMaxDiffWeight = 0.0001f,

			posMinDiffWeight = 0.0001f,
			posMaxDiffWeight = 0.0001f,

			buyValLerpWeight = 0.9f,
			sellValLerpWeight = 0.9f,

			randStartMinValue = -1f,
			randStartMaxValue = 1f,

			Price = Random.Range(-5f, 5f) + 25f
		}
	};


	public List<MarketForce> marketForces = new List<MarketForce>
	{
		// TODO
	};
	#endregion

	#region Dynamic Fields
	public Queue<StockEvent> eventQueue;
	public List<Trader> traders;
	#endregion

	#region Singleton Implementation
	private static Model S;
	public static Model Instance
	{
		get
		{
			if (S == null)
			{
				S = new Model();
			}

			return S;
		}
	}
	private Model() { }
	#endregion

	#region Functions
	public void Tick()
	{
		foreach (MarketForce mf in marketForces)
		{
			mf.Tick();
		}

		foreach (Stock s in stocks)
		{
			s.Tick();
		}

		return;
	}

	public void BeginNewPeriod()
	{
		foreach (Stock s in stocks)
		{
			s.AdvancePeriod();
		}

		return;
	}

	public bool Buy(Trader trader, Stock stock, int number)
	{
		if (trader.money < stock.Price * number || stock.numAvailable < number)
		{
			return false;
		}

		trader.money -= stock.Price * number;
		stock.numAvailable -= number;
		if (trader.shares.ContainsKey(stock)) trader.shares[stock] += number;
		else trader.shares.Add(stock, number);
		return true;
	}

	public bool Sell(Trader trader, Stock stock, int number)
	{
		if (!trader.shares.ContainsKey(stock) || trader.shares[stock] < number)
		{
			return false;
		}

		trader.money += stock.Price * number;
		trader.shares[stock] -= number;
		stock.numAvailable += number;
		return true;
	}

	public void EnqueueEvent(StockEvent stockEvent)
	{
		eventQueue.Enqueue(stockEvent);
	}

	public void DequeueEvent()
	{
		// Take the next event to do, and do it.
		StockEvent nextEvent = eventQueue.Dequeue();
	}
	#endregion
}
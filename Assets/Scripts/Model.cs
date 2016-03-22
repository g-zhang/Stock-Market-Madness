using System.Collections.Generic;
using System.Collections.ObjectModel;

public class Model
{
	#region Tuning Fields
	private const int defaultStartingMoney = 50000;

	public readonly ReadOnlyCollection<Stock> stocks =
		new ReadOnlyCollection<Stock>(new List<Stock>
		{
			new Stock("Company A", 1000000, 25f, 0.0001f, 0.9f, 1f),
			new Stock("Company B", 1000000, 25f, 0.0001f, 0.9f, 1f),
			new Stock("Company C", 1000000, 25f, 0.0001f, 0.9f, 1f)
		});

	public readonly ReadOnlyCollection<MarketForce> marketForces =
		new ReadOnlyCollection<MarketForce>(new List<MarketForce>
		{
			// TODO
		});
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

	#region Methods
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

	public void AddTrader(string inName)
	{
		AddTrader(defaultStartingMoney, inName);
		return;
	}

	public void AddTrader(int startMoney = defaultStartingMoney, string inName = null)
	{
		if (inName == null)
		{
			inName = string.Format("Player {0}", traders.Count + 1);
		}

		traders.Add(new Trader(inName, startMoney));
		return;
	}

	public bool Buy(Trader trader, Stock stock, int number)
	{
		if ((trader.money < stock.Price * number) || !stock.Buy(number))
		{
			return false;
		}

		trader.money -= stock.Price * number;
		if (trader.shares.ContainsKey(stock))
		{
			trader.shares[stock] += number;
		}
		else
		{
			trader.shares.Add(stock, number);
		}

		return true;
	}

	public bool Sell(Trader trader, Stock stock, int number)
	{
		if (!trader.shares.ContainsKey(stock) || trader.shares[stock] < number)
		{
			return false;
		}

		stock.Sell(number);

		trader.money += stock.Price * number;
		trader.shares[stock] -= number;

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
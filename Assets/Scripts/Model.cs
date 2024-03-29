﻿using UnityEngine;

using System.Collections.Generic;
using System.Collections.ObjectModel;

public enum GamePhases
{
	PreStart,
	Market,
	Business
}

public class Model
{
	#region Tuning Fields
	private const int startingMoney = 50000;
	private const int startingSharesPerCompany = 100000;

	private const int sharesPerEventWeight = 10000;

	public const float preStartRoundTimeSeconds = 5f;
	public const int numPreStartRounds = 2;
	
	public const float roundTimeSeconds = 60f;
	public const int roundDataPoints = 120;

	public readonly ReadOnlyCollection<Stock> stocks =
		new ReadOnlyCollection<Stock>(new List<Stock>
		{
			new Stock("Company A", 1000000, 25f, 5f,
				new TestStockGenerator(0.00005f, 0.9f, 1f)),
			new Stock("Company B", 1000000, 25f, 5f,
				new TestStockGenerator(0.00005f, 0.9f, 1f)),
			new Stock("Company C", 1000000, 25f, 5f,
				new TestStockGenerator(0.00005f, 0.9f, 1f))
		});

	public readonly ReadOnlyCollection<MarketForce> marketForces =
		new ReadOnlyCollection<MarketForce>(new List<MarketForce>
		{
			new TestMarketForce(5000, 0, 25),
			new TestMarketForce(10000, 15, 50),
			new TestMarketForce(20000, 55, 100)
		});

	private Dictionary<Stock, List<KeyValuePair<int, float>>> forcedDeltas =
		new Dictionary<Stock, List<KeyValuePair<int, float>>>();
	#endregion

	#region Dynamic Fields
	public GamePhases gamePhase = GamePhases.PreStart;

	private float roundElapsedTime = 0f;
	public int roundDataPointsAdded = 0;
	public int preStartRoundsPassed = 0;

	private float timeBetweenDataPoints =
		roundTimeSeconds / roundDataPoints;
	private float preStartTimeBetweenDataPoints =
		preStartRoundTimeSeconds / roundDataPoints;
	
	public List<Trader> traders = new List<Trader>();
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

	private Model()
	{
		foreach (Stock s in stocks)
		{
			forcedDeltas.Add(s, new List<KeyValuePair<int, float>>());
		}

		return;
	}
	#endregion

	#region Methods
	public void Tick()
	{
		switch (gamePhase)
		{
		case GamePhases.PreStart:
		{
			PreStartMarketTick();
			break;
		}

		case GamePhases.Market:
		{
			MarketTick();
			break;
		}

		case GamePhases.Business:
		{
			break;
		}

		default:
		{
			break;
		}
		}

			return;
		}

	private void MarketTick()
	{
		roundElapsedTime += Time.deltaTime;
		int neededDataPoints =
			Mathf.FloorToInt(roundElapsedTime / timeBetweenDataPoints);

		while (neededDataPoints > roundDataPointsAdded)
		{
			foreach (MarketForce mf in marketForces)
			{
				mf.Tick();
			}

			foreach (Stock s in stocks)
			{
				foreach (KeyValuePair<int, float> pair in forcedDeltas[s])
				{
					if (pair.Key != roundDataPointsAdded)
					{
						continue;
					}

					s.AddForcedDelta(pair.Value);
				}
				forcedDeltas[s].RemoveAll(p => p.Key == roundDataPointsAdded);

				s.Tick();
			}

			++roundDataPointsAdded;
		}

		if (roundElapsedTime >= roundTimeSeconds)
		{
			roundElapsedTime -= roundTimeSeconds;
			roundDataPointsAdded = 0;
			gamePhase = GamePhases.Business;
		}

		return;
	}

	private void PreStartMarketTick()
	{
		roundElapsedTime += Time.deltaTime;
		int neededDataPoints =
			Mathf.FloorToInt(roundElapsedTime / preStartTimeBetweenDataPoints);

		while (neededDataPoints > roundDataPointsAdded)
		{
			foreach (MarketForce mf in marketForces)
			{
				mf.Tick();
			}

			foreach (Stock s in stocks)
			{
				s.Tick();
			}

			++roundDataPointsAdded;
		}

		if (roundElapsedTime >= preStartRoundTimeSeconds)
		{
			roundElapsedTime -= preStartRoundTimeSeconds;
			roundDataPointsAdded = 0;

			++preStartRoundsPassed;
		}

		if (preStartRoundsPassed >= numPreStartRounds)
		{
			gamePhase = GamePhases.Market;
		}

		return;
	}

	public void AddTrader(string inName = null)
	{
		if (inName == null)
		{
			inName = string.Format("Player {0}", traders.Count + 1);
		}

		Trader newTrader = new Trader(inName, startingMoney);
		foreach (Stock s in stocks)
		{
			newTrader.shares.Add(s, startingSharesPerCompany);
			s.Buy(newTrader, startingSharesPerCompany);

			s.AddTrader(newTrader);
		}
		traders.Add(newTrader);

		return;
	}

	public bool Buy(Trader trader, Stock stock, int number)
	{
		if ((trader.money < stock.Price * number) || !stock.Buy(trader, number))
		{
			return false;
		}

		trader.money -= stock.Price * number;
			trader.shares[stock] += number;

		return true;
	}

	public bool Sell(Trader trader, Stock stock, int number)
	{
		if (!trader.shares.ContainsKey(stock) || trader.shares[stock] < number)
		{
			return false;
		}

		stock.Sell(trader, number);

		trader.money += stock.Price * number;
		trader.shares[stock] -= number;

		return true;
	}


	public void MarketEvent(StockEvent stockEvent, Stock stock, int shares)
	{
		for (int i = 0; i < (shares / sharesPerEventWeight); ++i)
		{
			forcedDeltas[stock].Add(new KeyValuePair<int, float>(
				Random.Range(0, roundDataPoints),
				stockEvent.priceChangeDistribution.sample));
		}

		return;
	}
	
	#endregion
}
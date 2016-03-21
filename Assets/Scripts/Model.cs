using System.Collections.Generic;

public class Model {
	
	public Queue<StockEvent> eventQueue;
	public List<Trader> traders;
	public List<Stock> stocks;
	public List<MarketForce> marketForces;

	public static Model S;
	public Model() { S = this; }

	public void Tick() {
		foreach (MarketForce mf in marketForces) mf.Tick();
		foreach (Stock s in stocks) s.Tick();
	}

	public void BeginNewPeriod() {
		foreach (Stock s in stocks) s.AdvancePeriod();
	}

	public bool Buy(Trader trader, Stock stock, int number) {
		if (trader.money < stock.Price * number ||
			stock.numAvailable < number) return false;
		trader.money -= stock.Price * number;
		stock.numAvailable -= number;
		if (trader.shares.ContainsKey(stock)) trader.shares[stock] += number;
		else trader.shares.Add(stock, number);
		return true;
	}

	public bool Sell(Trader trader, Stock stock, int number) {
		if (!trader.shares.ContainsKey(stock) ||
			trader.shares[stock] < number) return false;
		trader.money += stock.Price * number;
		trader.shares[stock] -= number;
		stock.numAvailable += number;
		return true;
	}

	public void EnqueueEvent(StockEvent stockEvent) {
		eventQueue.Enqueue(stockEvent);
	}

	public void DequeueEvent() {
		// Take the next event to do, and do it.
		StockEvent nextEvent = eventQueue.Dequeue();
	}
}

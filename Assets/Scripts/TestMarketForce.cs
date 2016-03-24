using System;

public class TestMarketForce : MarketForce
{
	private int maxStockTransaction;
	private int startWaitTime;
	private int generalDelayTime;

	private int roundsPassed = 0;

	public TestMarketForce(int inMaxStockTransaction,
		int inStartWaitTime, int inGeneralDelayTime)
	{
		maxStockTransaction = inMaxStockTransaction;
		startWaitTime = inStartWaitTime;
		generalDelayTime = inGeneralDelayTime;

		return;
	}

	public void Tick()
	{
		++roundsPassed;
		if (startWaitTime > 0)
		{
			if (roundsPassed < startWaitTime)
			{
				return;
			}
			roundsPassed = 0;
			return;
		}

		if (roundsPassed < generalDelayTime)
		{
			return;
		}
		roundsPassed = 0;

		float totalPrice = 0f;
		foreach (Stock s in Model.Instance.stocks)
		{
			totalPrice += s.Price;
		}
		float averagePrice = totalPrice / Model.Instance.stocks.Count;

		float greatestDifference = 0f;
		foreach (Stock s in Model.Instance.stocks)
		{
			greatestDifference = Math.Max(
				Math.Abs(s.Price - averagePrice), greatestDifference);
		}

		foreach (Stock s in Model.Instance.stocks)
		{
			float difference = Math.Abs(s.Price - averagePrice);
			int numStocks = (int)(maxStockTransaction *
				(difference / greatestDifference));

			if (s.Price <= averagePrice)
			{
				s.AIBuy(numStocks);
			}
			else
			{
				s.AISell(numStocks);
			}
		}

		return;
	}
}
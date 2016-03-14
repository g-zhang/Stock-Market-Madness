using UnityEngine;

public class TestStockGenerator : StockGenerator
{
	public float prevValue = 50f;
	public int numStocksAvailable = 1000000;

	public override float getNextStockValue()
	{
		float minVal = Mathf.Max(-10, 1 - prevValue);

		prevValue = prevValue + Random.Range(minVal, 10f);
		return prevValue;
	}

	public override bool RecordBuy(int numStocks)
	{
		if (numStocks > numStocksAvailable)
		{
			return false;
		}

		numStocksAvailable -= numStocks;
		return true;
	}

	public override void RecordSell(int numStocks)
	{
		return;
	}
}
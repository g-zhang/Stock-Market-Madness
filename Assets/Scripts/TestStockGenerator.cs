using UnityEngine;

public class TestStockGenerator : StockGenerator
{
	[Header("TestStockGenerator: Inspector Set Fields")]
	public float prevValue = 50f;

	public float negDiffWeight = 0.01f;
	public float posDiffWeight = 0.01f;

	[Header("TestStockGenerator: Dynamically Set Fields")]
	public int currNumSold;
	public int currNumBought;

	void Awake()
	{
		currNumSold = 0;
		currNumBought = 0;
	}

	public override float getNextStockValue()
	{
		float minVal = -10f;
		float maxVal = 10f;

		int difference = currNumBought - currNumSold;
		if (difference < 0)
		{
			minVal -= negDiffWeight * difference;
		}
		else if (difference > 0)
		{
			maxVal += posDiffWeight * difference;
		}
		currNumBought = 0;
		currNumSold = 0;

		minVal = Mathf.Max(minVal, 1 - prevValue);

		prevValue = prevValue + Random.Range(minVal, maxVal);
		return prevValue;
	}

	public override bool RecordBuy(int numStocks)
	{
		if (numStocks > numStocksAvailable)
		{
			return false;
		}

		currNumBought += numStocks;
		numStocksAvailable -= numStocks;

		return true;
	}

	public override void RecordSell(int numStocks)
	{
		currNumSold += numStocks;
		numStocksAvailable += numStocks;

		return;
	}
}
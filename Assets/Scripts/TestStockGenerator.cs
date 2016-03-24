using UnityEngine;

public class TestStockGenerator : StockGenerator
{
	private float diffWeights;
	private float lerpWeights;
	private float randStartAbsVal;

	public TestStockGenerator(float inDiffWeights,
		float inLerpWeights, float inRandStartAbsVal)
	{
		diffWeights = inDiffWeights;
		lerpWeights = inLerpWeights;
		randStartAbsVal = inRandStartAbsVal;

		return;
	}

	public void getNextStockValue(BuySellData data,
		out float minRandVal, out float maxRandVal)
	{
		int currNumSold = data.generalCurrNumSold + data.companyCurrNumSold;
		foreach (int numSold in data.tradersCurrNumSold.Values)
		{
			currNumSold += numSold;
		}

		int currNumBought = data.generalCurrNumBought + data.companyCurrNumBought;
		foreach (int numBought in data.tradersCurrNumBought.Values)
		{
			currNumBought += numBought;
		}

		minRandVal = -randStartAbsVal;
		maxRandVal = randStartAbsVal;

		int difference = currNumBought - currNumSold;
		if (difference < 0)
		{
			minRandVal += diffWeights * difference;
			maxRandVal += diffWeights * difference;
		}
		else if (difference > 0)
		{
			minRandVal += diffWeights * difference;
			maxRandVal += diffWeights * difference;
		}

		data.generalCurrNumSold =
			Mathf.FloorToInt(Mathf.Lerp(0f, data.generalCurrNumSold, lerpWeights));
		data.companyCurrNumSold =
			Mathf.FloorToInt(Mathf.Lerp(0f, data.companyCurrNumSold, lerpWeights));

		data.generalCurrNumBought =
			Mathf.FloorToInt(Mathf.Lerp(0f, data.generalCurrNumBought, lerpWeights));
		data.companyCurrNumBought =
			Mathf.FloorToInt(Mathf.Lerp(0f, data.companyCurrNumBought, lerpWeights));

		foreach (Trader trader in Model.Instance.traders)
		{
			data.tradersCurrNumSold[trader] =
				Mathf.FloorToInt(Mathf.Lerp(0f, data.tradersCurrNumSold[trader], lerpWeights));
			data.tradersCurrNumBought[trader] =
				Mathf.FloorToInt(Mathf.Lerp(0f, data.tradersCurrNumBought[trader], lerpWeights));
		}

		return;
	}
}
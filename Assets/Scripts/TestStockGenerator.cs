using UnityEngine;

public class TestStockGenerator : StockGenerator
{
	private float prevValue = 50f;

	public float getNextStockValue()
	{
		float minVal = Mathf.Max(-10, 1 - prevValue);

		prevValue = prevValue + Random.Range(minVal, 10f);
		return prevValue;
	}
}
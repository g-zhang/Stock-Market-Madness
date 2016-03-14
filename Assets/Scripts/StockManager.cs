using UnityEngine;

public class StockManager : MonoBehaviour
{
	[Header("StockManager: Inspector Set Fields")]
	public StockGraph graph;

	public float roundTimeSeconds = 60f;
	public int roundDataPoints = 120;

	public StockGenerator stockGenerator = new TestStockGenerator();

	[Header("StockManager: Dynamically Set Fields")]
	public float roundElapsedTime;

	public float timeBetweenDataPoints;
	public int roundDataPointsAdded;

	void Awake()
	{
		roundElapsedTime = 0f;

		timeBetweenDataPoints = roundTimeSeconds / roundDataPoints;
		roundDataPointsAdded = 0;

		return;
	}

	void Update()
	{
		roundElapsedTime += Time.deltaTime;
		float neededDataPoints =
			Mathf.Floor(roundElapsedTime / timeBetweenDataPoints);

		while (neededDataPoints > roundDataPointsAdded)
		{
			float nextStockPrice = stockGenerator.getNextStockValue();
			graph.Price = nextStockPrice;

			++roundDataPointsAdded;
		}

		// This is just a placeholder for now to keep the values moving.
		if (roundElapsedTime >= roundTimeSeconds)
		{
			roundElapsedTime -= roundTimeSeconds;
			roundDataPointsAdded = 0;

			// graph.AdvancePeriod();
		}

		return;
	}

	public void RecordBuy(int numStocks)
	{
		stockGenerator.RecordBuy(numStocks);
		return;
	}

	public void RecordSell(int numStocks)
	{
		stockGenerator.RecordSell(numStocks);
		return;
	}
}
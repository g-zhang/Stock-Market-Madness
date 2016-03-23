using UnityEngine;

public class StockManager : MonoBehaviour
{
	[Header("StockManager: Inspector Set Fields")]
	public StockGraph graph;

	public float roundTimeSeconds = 60f;
	public int roundDataPoints = 120;

	[Header("StockManager: Dynamically Set Fields")]
	public float roundElapsedTime;

	public float timeBetweenDataPoints;
	public int roundDataPointsAdded;

	public StockGenerator stockGenerator;

	public int StocksAvailable
	{
		//get { return stockGenerator.numStocksAvailable; }
		get { return 1; }
	}

	public float Price
	{
		get { return graph.Price; }
	}

	void Awake()
	{
		roundElapsedTime = 0f;

		timeBetweenDataPoints = roundTimeSeconds / roundDataPoints;
		roundDataPointsAdded = 0;

		stockGenerator = GetComponent<StockGenerator>();

		return;
	}

	void Start()
	{
		//graph.Price = stockGenerator.getNextStockValue();
		return;
	}

	void Update()
	{
		roundElapsedTime += Time.deltaTime;
		float neededDataPoints =
			Mathf.Floor(roundElapsedTime / timeBetweenDataPoints);

		while (neededDataPoints > roundDataPointsAdded)
		{
			//float nextStockPrice = stockGenerator.getNextStockValue();
			//graph.Price = nextStockPrice;

			++roundDataPointsAdded;
		}

		// This is just a placeholder for now to keep the values moving.
		if (roundElapsedTime >= roundTimeSeconds)
		{
			roundElapsedTime -= roundTimeSeconds;
			roundDataPointsAdded = 0;

			graph.AdvancePeriod();
		}

		return;
	}

	public bool RecordBuy(int numStocks)
	{
		return true;
		//return stockGenerator.RecordBuy(numStocks);
	}

	public void RecordSell(int numStocks)
	{
		//stockGenerator.RecordSell(numStocks);
		return;
	}
}
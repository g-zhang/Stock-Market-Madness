using UnityEngine;

public class StockManager : MonoBehaviour
{
	[Header("StockManager: Inspector Set Fields")]
	public float roundTimeSeconds = 60f;
	public int roundDataPoints = 120;

	public StockGenerator stockGenerator;

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
			print(nextStockPrice);
		}

		return;
	}
}
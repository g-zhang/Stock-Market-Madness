using UnityEngine;
using System.Collections.Generic;

public class Controller : MonoBehaviour
{
	[Header("Controller: Inspector Set Variables")]
	public float roundTimeSeconds = 60.0f;
	public int roundDataPoints = 120;

	[Header("Controller: Dynamically Set Variables")]
	public float roundElapsedTime;
	public float timeBetweenDataPoints;

	public int roundDataPointsAdded;

	public List<View> views;

	void Awake()
	{
		roundElapsedTime = 0;
		timeBetweenDataPoints = roundDataPoints / roundTimeSeconds;

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
			// Advance Model.
			++roundDataPointsAdded;
		}

		// This is just a placeholder for now to keep the values moving.
		if (roundElapsedTime >= roundTimeSeconds)
		{
			roundElapsedTime -= roundTimeSeconds;
			roundDataPointsAdded = 0;
			
			// End Model round.
		}

		return;
	}
}
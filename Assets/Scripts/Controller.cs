using UnityEngine;
using System.Collections.Generic;

public class Controller : MonoBehaviour
{
	#region Fields
	[Header("Controller: Inspector Set Variables")]
	public float roundTimeSeconds = 60.0f;
	public int roundDataPoints = 120;

	[Header("Controller: Dynamically Set Variables")]
	public float roundElapsedTime;
	public float timeBetweenDataPoints;

	public int roundDataPointsAdded;

	public List<View> views;
	#endregion

	#region Unity Methods
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
			Model.Instance.Tick();
			++roundDataPointsAdded;
		}

		// This is just a placeholder for now to keep the values moving.
		if (roundElapsedTime >= roundTimeSeconds)
		{
			roundElapsedTime -= roundTimeSeconds;
			roundDataPointsAdded = 0;

			Model.Instance.BeginNewPeriod();
		}

		return;
	}
	#endregion
}
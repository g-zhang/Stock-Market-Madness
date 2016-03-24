using UnityEngine;
using System.Collections;

public class ClockController : MonoBehaviour {

	public GameObject minHand;

	float roundTimeSeconds;
	int roundDataPoints;

	int currRound;

	// I need two because I cant think of a clever way to force it to turn the right way
	Vector3 UpFlip1, UpFlip2, DownFlip1, DownFlip2;

	// Hand distance from center
	float radius;

	// Use this for initialization
	void Start () {

		roundTimeSeconds = Model.roundTimeSeconds;
		roundDataPoints = Model.roundDataPoints;
		currRound = Model.Instance.roundDataPointsAdded;

		radius = minHand.transform.localPosition.y;
	}
	
	// Update is called once per frame
	void Update () {
		currRound = Model.Instance.roundDataPointsAdded + 1;

		float u = 2f * (float)currRound / (float)roundDataPoints;

		Vector3 UpFlip1 = new Vector3( minHand.transform.localRotation.eulerAngles.x, minHand.transform.localRotation.eulerAngles.y, 359.001f);
		Vector3 DownFlip1 = new Vector3( minHand.transform.localRotation.eulerAngles.x, minHand.transform.localRotation.eulerAngles.y, 180f);
		Vector3 UpFlip2 = new Vector3( minHand.transform.localRotation.eulerAngles.x, minHand.transform.localRotation.eulerAngles.y, 0f);
		Vector3 DownFlip2 = new Vector3( minHand.transform.localRotation.eulerAngles.x, minHand.transform.localRotation.eulerAngles.y, 179.001f);

		if (u <= 1) {
			minHand.transform.localRotation = Quaternion.Euler (Vector3.Slerp (UpFlip1, DownFlip1, u)); 
		} else {
			minHand.transform.localRotation = Quaternion.Euler (Vector3.Slerp (DownFlip2, UpFlip2, u - 1)); 
		}
	}
}

using UnityEngine;
using System.Collections;

public class RulerController : MonoBehaviour {

	public GameObject[] pointers = new GameObject[(int)CompanyName.size];
	public GameObject[] rulers = new GameObject[(int)CompanyName.size];

	float maxPrice;
	public float averagePrice;

	public float pointerSpeed;
	float timeOfLastMaxPrice;

	// The min and max Y position for the pointer
	float minY, maxY;

	// Use this for initialization
	void Start () {
		// Assuming all rulers are the same height, set minY maxY
		maxY = rulers[0].transform.localPosition.y + rulers[0].transform.localScale.y/2;
		minY = rulers[0].transform.localPosition.y - rulers[0].transform.localScale.y/2;

		timeOfLastMaxPrice = Time.timeSinceLevelLoad;
		maxPrice = 0;
	}
	
	// Update is called once per frame
	void Update () {

		for (CompanyName i = 0; i != CompanyName.size; i++) {
			float price = Model.Instance.stocks [(int)i].Price;

			// Update maxPrice
			if (price > maxPrice) {
				maxPrice = price;
				timeOfLastMaxPrice = Time.timeSinceLevelLoad;
			}

			updateStockPrice (i, price);
		}
	}

	void updateStockPrice(CompanyName whichCom, float newPrice) {
		float newPointYPos = (maxY - minY) * (newPrice / maxPrice) + minY;
		Vector3 newPos = pointers [(int)whichCom].transform.localPosition;
		newPos = new Vector3 (newPos.x, newPointYPos, newPos.z);

		float u = pointers [(int)whichCom].transform.localPosition.y / newPos.y;

		// lerp = p0(1-u) + p1(u)

		pointers [(int)whichCom].transform.localPosition = Vector3.Lerp(pointers [(int)whichCom].transform.localPosition, newPos, u + pointerSpeed);
	}
}

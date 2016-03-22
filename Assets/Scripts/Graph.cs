using UnityEngine;
using System.Collections;

public class Graph : MonoBehaviour {

	public GameObject linePrefab;
	public GameObject dateLinePrefab;
	public int periodsToShow;
	public Vector2 graphDimensions;
	public Color[] lineColors;

	void Awake () {

		if (linePrefab.GetComponent<LineRenderer>() == null ||
			linePrefab.GetComponent<StockLine>() == null) {
			Debug.LogError("The stock line prefab is not set up correctly. It needs a line renderer and the StockLine script.");
			Destroy(this);
		}

		if (dateLinePrefab.GetComponent<LineRenderer>() == null ||
			dateLinePrefab.GetComponent<DateLine>() == null) {
			Debug.LogError("The date line prefab is not set up correctly. It needs a line renderer and the DateLine script.");
			Destroy(this);
		}

		int colorIdx = 0;

		foreach (Stock stock in Model.Instance.stocks) {
			GameObject lineObj = Instantiate(linePrefab) as GameObject;
			lineObj.transform.parent = transform;
			StockLine lineScr = lineObj.GetComponent<StockLine>();
			if (lineScr == null) return;
			lineScr.stock = stock;
			lineScr.maxDataPointsOnGraph = periodsToShow * Model.roundDataPoints;
			lineScr.maxPriceOnGraph = 70f;
			lineScr.graphDimensions = graphDimensions;
			lineScr.color = lineColors[colorIdx++];
		}

		for (int i = 0; i < periodsToShow; i++) {
			GameObject dateLine = Instantiate(dateLinePrefab) as GameObject;
			dateLine.transform.parent = transform;
			DateLine dateLineScr = dateLine.GetComponent<DateLine>();
			dateLineScr.maxDataPointsOnGraph = periodsToShow* Model.roundDataPoints;
			dateLineScr.graphDimensions = graphDimensions;
			dateLineScr.periodsAgo = i;
		}
	}
}

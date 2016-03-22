using UnityEngine;
using System.Collections;

public class Graph : MonoBehaviour {
	public GameObject linePrefab;
	public int periodsToShow;
	void Start () {
		if (linePrefab.GetComponent<LineRenderer>() == null ||
			linePrefab.GetComponent<StockLine>() == null) {
			Debug.LogError("The stock line prefab is not set up correctly. It needs a line renderer and the StockLine script.");
			Destroy(this);
		}
		foreach (Stock stock in Model.Instance.stocks) {
			GameObject lineObj = Instantiate(linePrefab) as GameObject;
			lineObj.transform.parent = transform;
			StockLine lineScr = lineObj.GetComponent<StockLine>();
			if (lineScr == null) return;
			lineScr.stock = stock;
			lineScr.maxDataPointsOnGraph = periodsToShow * Model.Instance.roundDataPoints;
		}
	}
}

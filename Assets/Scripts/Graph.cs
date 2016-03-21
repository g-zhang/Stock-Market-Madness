using UnityEngine;
using System.Collections.Generic;

public class StockLine : MonoBehaviour {

	public Stock stock;
	private LineRenderer lineRenderer;

	public List<float> visiblePriceHistory;
	public float lineHeight;
	public float maxPriceOnGraph;
	public int maxDataPointsOnGraph;

	void Awake() {
		lineRenderer = GetComponent<LineRenderer>();
		nullCheck(lineRenderer, "LineRenderer component not found on this object. Exiting.");
	}

	void nullCheck(Object obj, string msg) {
		if (obj == null) {
			Debug.LogError(msg);
			Destroy(this);
		}
	}

	void Update() {
		DrawGraph();
	}
	
	void DrawGraph() {

		visiblePriceHistory = stock.LastNPeriods(3);

		Vector3[] newLinePoints = new Vector3[visiblePriceHistory.Count];
		for (int idx = 0; idx < visiblePriceHistory.Count; idx++)
			newLinePoints[idx] = PriceDataToLocalPoint(idx + maxDataPointsOnGraph - visiblePriceHistory.Count, visiblePriceHistory[idx]);
		lineRenderer.SetVertexCount(newLinePoints.Length);
		lineRenderer.SetPositions(newLinePoints);
	}

	public Vector3 PriceDataToLocalPoint(int idx, float price) {
		float maxX = transform.localScale.x * 2;
		float maxY = transform.localScale.y * 2;
		float x = idx * maxX / maxDataPointsOnGraph;
		float y = price * maxY / maxPriceOnGraph;
		return new Vector3(x, y, 0) - transform.localScale;
	}

}

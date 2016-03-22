using UnityEngine;
using System.Collections.Generic;

public class StockLine : MonoBehaviour {

	public Stock stock;
	private LineRenderer lineRenderer;
	public float maxPriceOnGraph;
	public int maxDataPointsOnGraph;

	void Awake() {
		lineRenderer = GetComponent<LineRenderer>();
		if (lineRenderer == null) {
			Debug.LogError("LineRenderer component not found on this object. Exiting.");
			Destroy(this);
		}
	}

	void Update() {
		List<float> prices = stock.LastNPeriods(3);
		Vector3[] newLinePoints = new Vector3[prices.Count];
		for (int idx = 0; idx < prices.Count; idx++)
			newLinePoints[idx] = PriceDataToLocalPoint(idx + maxDataPointsOnGraph - prices.Count, prices[idx]);
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

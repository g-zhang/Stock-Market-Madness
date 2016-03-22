using UnityEngine;
using System.Collections.Generic;

public class StockLine : MonoBehaviour {

	public Stock stock;
	private LineRenderer lineRenderer;
	public float maxPriceOnGraph;
	public int maxDataPointsOnGraph;
	public Vector2 graphDimensions;

	void Awake() {
		lineRenderer = GetComponent<LineRenderer>();
		if (lineRenderer == null) {
			Debug.LogError("LineRenderer component not found on this object. Exiting.");
			Destroy(this);
		}
	}

	void Update() {
		List<float> prices = stock.priceHistory;
		while (prices.Count > maxDataPointsOnGraph) prices.RemoveAt(0);
		Vector3[] newLinePoints = new Vector3[prices.Count];
		for (int idx = 0; idx < prices.Count; idx++)
			newLinePoints[idx] = PriceDataToLocalPoint(idx + maxDataPointsOnGraph - prices.Count, prices[idx]);
		lineRenderer.SetVertexCount(newLinePoints.Length);
		lineRenderer.SetPositions(newLinePoints);
	}

	public Vector3 PriceDataToLocalPoint(int idx, float price) {
		float x = idx * graphDimensions.x / maxDataPointsOnGraph - graphDimensions.x / 2f;
		float y = price * graphDimensions.y / maxPriceOnGraph - graphDimensions.y / 2f;
		//print("price:"+price+" idx:"+idx+" x:"+x+" y:"+y);
		return new Vector3(x, y, -0.1f);
	}
}

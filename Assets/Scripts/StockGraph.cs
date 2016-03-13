using UnityEngine;
using System.Collections.Generic;

public class StockGraph : MonoBehaviour {

	public List<float> visiblePriceHistory;
	public List<List<float>> priceHistoryByPeriod;
	public Vector2 margins;
	public float lineHeight;
	public float maxPriceOnGraph;
	public int maxDataPointsOnGraph;

	private LineRenderer lineRenderer;
	public GameObject trendObject;
	private TextMesh trendText;
	public Vector3 graphOrigin;

	void Awake() {
		lineRenderer = GetComponent<LineRenderer>();
		if (lineRenderer == null) {
			Debug.LogError("LineRenderer component not found on this object. Exiting.");
			Destroy(this); // stop running the script, keep the gameObject.
		}
		lineRenderer.useWorldSpace = true;
		if (trendObject == null) {
			Debug.LogError("trendObject must be set in inspector. Exiting.");
			Destroy(this); // stop running the script, keep the gameObject.
		}
		trendText = trendObject.GetComponent<TextMesh>();
		if (trendText == null) {
			Debug.LogError("trendObject must have a TextMesh. Exiting.");
			Destroy(this); // stop running the script, keep the gameObject.
		}

		priceHistoryByPeriod = new List<List<float>>();
	}

	// Use this for initialization
	void Start () {
		graphOrigin.z = -lineHeight;
		graphOrigin.x = -(transform.localScale.x / 2 - margins.x);
		graphOrigin.y = -(transform.localScale.y / 2 - margins.y);

		margins = new Vector2(0, 0);
		lineHeight = .1f;
		maxPriceOnGraph = 100f;
		maxDataPointsOnGraph = 50;
		InvokeRepeating("AddWalkData", 0.0f, 0.1f);

		AdvancePeriod();
	}
	
	public void AddPrice(float price) {
		int lastPeriodIdx = 0;
		if (priceHistoryByPeriod.Count > 0)
			lastPeriodIdx = priceHistoryByPeriod.Count - 1;
		priceHistoryByPeriod[lastPeriodIdx].Add(price);
	}

	public void AdvancePeriod() {
		priceHistoryByPeriod.Add(new List<float>());
	}

	// Update is called once per frame
	void Update () {
		DrawGraph();
	}

	void GetVisibleData() {
		visiblePriceHistory = priceHistoryByPeriod[0];
		while (visiblePriceHistory.Count > maxDataPointsOnGraph)
			visiblePriceHistory.RemoveAt(0);
	}

	void DrawGraph() {
		GetVisibleData();
		if (visiblePriceHistory.Count < 2) return;
		Vector3[] newLinePoints = new Vector3[visiblePriceHistory.Count];
		maxPriceOnGraph = 0;
		for (int idx = 0; idx < visiblePriceHistory.Count; idx++)
			if (maxPriceOnGraph < visiblePriceHistory[idx])
				maxPriceOnGraph = visiblePriceHistory[idx];
		if (maxPriceOnGraph <= 0) return;
		maxPriceOnGraph += 10;
		for (int idx = 0; idx < visiblePriceHistory.Count; idx++) {
			newLinePoints[idx] = linePoint(idx);
		}
		lineRenderer.SetVertexCount(newLinePoints.Length);
		lineRenderer.SetPositions(newLinePoints);
		if (newLinePoints.Length != 0)
			trendObject.transform.position = Vector3.Lerp(trendObject.transform.position, newLinePoints[newLinePoints.Length-1], Time.deltaTime*4f);
	}

	public Vector3 linePoint(int day) {
		float price = visiblePriceHistory[day];
		Vector3 point = new Vector3();
		point.z = -lineHeight;
		point.x = day * (transform.localScale.x - 2 * margins.x) / (visiblePriceHistory.Count - 1);
		point.y = price * (transform.localScale.y - 2 * margins.y) / maxPriceOnGraph;
		point += graphOrigin + transform.position;
		return point;
	}
	
	void AddWalkData() {
		float lastPrice = 50f;
		if (visiblePriceHistory.Count != 0)
			lastPrice = visiblePriceHistory[visiblePriceHistory.Count - 1];
		float bottom = Mathf.Max(lastPrice - 10f, 1f);
		float top = lastPrice + 10f;
		if (Input.GetKey(KeyCode.D)) top = 0;
		AddPrice(Random.Range(bottom, top));
	}
}

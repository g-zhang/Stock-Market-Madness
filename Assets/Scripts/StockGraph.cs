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
	public GameObject StockManagerObject;
	private StockManager stockManager;
	private Vector3 trendTextOffset;

	private LineRenderer deadLineOne, deadLineTwo;

	void Awake() {

		lineRenderer = GetComponent<LineRenderer>();
		nullCheck(lineRenderer, "LineRenderer component not found on this object. Exiting.");
		lineRenderer.useWorldSpace = true;

		nullCheck(trendObject, "trendObject must be set in inspector. Exiting.");
		trendText = trendObject.GetComponent<TextMesh>();
		nullCheck(trendText, "trendObject must have a TextMesh. Exiting.");

		nullCheck(StockManagerObject,"StockManagerObject must be set in inspector. Exiting.");
		stockManager = StockManagerObject.GetComponent<StockManager>();
		nullCheck(stockManager, "stockManagerObject must have a StockManager. Exiting.");

		Transform DL1trans = transform.Find("DeadLine1");
		nullCheck(DL1trans, "Deadline1 not found as a child of StockGraph. Exiting.");
		GameObject DL1obj = DL1trans.gameObject;
		deadLineOne = DL1obj.GetComponent<LineRenderer>();
		nullCheck(deadLineOne, "Deadline1 object must have a LineRenderer component. Exiting.");

		Transform DL2trans = transform.Find("DeadLine2");
		nullCheck(DL2trans, "Deadline2 not found as a child of StockGraph. Exiting.");
		GameObject DL2obj = DL2trans.gameObject;
		deadLineTwo = DL2obj.GetComponent<LineRenderer>();
		nullCheck(deadLineTwo, "Deadline2 object must have a LineRenderer component. Exiting.");

		priceHistoryByPeriod = new List<List<float>>();
	}

	void nullCheck(Object obj, string msg) {
		if (obj == null) {
			Debug.LogError(msg);
			Destroy(this);
		}
	}
	
	void Start () {
		margins = new Vector2(0.5f, 0.5f);
		lineHeight = .1f;

		graphOrigin.z = -lineHeight;
		graphOrigin.x = -(transform.localScale.x / 2 - margins.x);
		graphOrigin.y = -(transform.localScale.y / 2 - margins.y);

		maxPriceOnGraph = 100f;
		maxDataPointsOnGraph = stockManager.roundDataPoints * 2;

		trendTextOffset = new Vector3(0.7f, 0, 0);

		AdvancePeriod();
	}

	public void AdvancePeriod() {
		priceHistoryByPeriod.Add(new List<float>());
	}
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.A)) AdvancePeriod();
		DrawGraph();
	}

	void UpdateVisibleData() {

		maxDataPointsOnGraph = stockManager.roundDataPoints * 2;

		visiblePriceHistory = new List<float>();
		visiblePriceHistory.AddRange(PeriodsAgoData(2));
		visiblePriceHistory.AddRange(PeriodsAgoData(1));
		visiblePriceHistory.AddRange(ThisPeriodData);

		while (visiblePriceHistory.Count > maxDataPointsOnGraph)
			visiblePriceHistory.RemoveAt(0);

		maxPriceOnGraph = 0;
		foreach (float price in visiblePriceHistory)
			maxPriceOnGraph = Mathf.Max(maxPriceOnGraph, price);

		if (visiblePriceHistory.Count <= 0) return;
		DrawDeadlines(maxDataPointsOnGraph - ThisPeriodData.Count, maxDataPointsOnGraph - ThisPeriodData.Count - stockManager.roundDataPoints);

	}

	public void DrawDeadlines(int idx1, int idx2) {
		//print(idx1); print(idx2);
		deadLineOne.SetPositions(
			new Vector3[2] {
				PriceDataToWorldPoint(idx1, 0),
				PriceDataToWorldPoint(idx1, maxPriceOnGraph)
			}
		);
		deadLineTwo.SetPositions(
			new Vector3[2] {
				PriceDataToWorldPoint(idx2, 0),
				PriceDataToWorldPoint(idx2, maxPriceOnGraph)
			}
		);
	}

	public float PeriodDelta {
		get { return Price - ThisPeriodData[0]; }
	}

	public List<float> ThisPeriodData {
		get { return PeriodsAgoData(0); }
	}

	public List<float> PeriodsAgoData(int periodsAgo) {
		int periodIdx = priceHistoryByPeriod.Count - periodsAgo - 1;
		return (periodIdx < 0) ? new List<float>() : priceHistoryByPeriod[periodIdx];
	}

	public float Price {
		get {
			if (ThisPeriodData.Count == 0) return Mathf.Infinity;
			return ThisPeriodData[ThisPeriodData.Count - 1];
		}
		set { ThisPeriodData.Add(value); }
	}

	void DrawGraph() {

		UpdateVisibleData();
		//if (visiblePriceHistory.Count < 2) return;

		Vector3[] newLinePoints = new Vector3[visiblePriceHistory.Count];
		for (int idx = 0; idx < visiblePriceHistory.Count; idx++)
			newLinePoints[idx] = PriceDataToWorldPoint(idx + maxDataPointsOnGraph - visiblePriceHistory.Count, visiblePriceHistory[idx]);
		lineRenderer.SetVertexCount(newLinePoints.Length);
		lineRenderer.SetPositions(newLinePoints);

		if (newLinePoints.Length != 0)
			trendObject.transform.position = Vector3.Lerp(trendObject.transform.position, newLinePoints[newLinePoints.Length-1] + trendTextOffset, Time.deltaTime*4f);
		trendText.text = ((PeriodDelta >= 0) ? "+" : "") + PeriodDelta.ToString("0.00");
		trendText.color = (PeriodDelta >= 0) ? Color.green : Color.red;

	}
	
	public Vector3 PriceDataToWorldPoint(int idx, float price) {
		float maxX = transform.localScale.x - 2 * margins.x;
		float maxY = transform.localScale.y - 2 * margins.y;
		float x = idx * maxX / maxDataPointsOnGraph;
		float y = price * maxY / maxPriceOnGraph;
		return new Vector3(x,y,0) + graphOrigin + transform.position;
	}

}

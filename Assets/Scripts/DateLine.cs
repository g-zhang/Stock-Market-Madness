using UnityEngine;
using System.Collections;

public class DateLine : MonoBehaviour {

	public int periodsAgo;
	private LineRenderer lineRenderer;
	public Vector2 graphDimensions;
	public int maxDataPointsOnGraph;

	void Awake() {
		lineRenderer = GetComponent<LineRenderer>();
		if (lineRenderer == null) {
			Debug.LogError("LineRenderer component not found on this object. Exiting.");
			Destroy(this);
		}
		lineRenderer.SetVertexCount(2);
	}

	void Update() {
		Vector3[] pts = new Vector3[2] {
			new Vector3(xCoord, -graphDimensions.y / 2, -0.1f),
			new Vector3(xCoord, graphDimensions.y / 2, -0.1f)
		};
		lineRenderer.SetPositions(pts);
	}

	private float xCoord {
		get {
			int offsetFromRight = Model.Instance.roundDataPointsAdded + periodsAgo * Model.roundDataPoints;
			return graphDimensions.x / 2 - offsetFromRight * graphDimensions.x / maxDataPointsOnGraph;
		}
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TickerTextController : MonoBehaviour {

	public float scrollSpeed;
	public bool isTemplate = true;
	
	// Update is called once per frame
	void Update () {
		// If object is offscreen, destroy itself
		if (transform.localPosition.x <= -1f * GetComponent<RectTransform> ().sizeDelta.x) {
			TickerController.S.destroyOldNews ();
		}
		if (!isTemplate) {
			transform.localPosition += new Vector3 (-scrollSpeed, 0, 0);
		}
	}

	// returns true if all of the text is shown
	public bool hasFullyShown() {
		if (transform.localPosition.x <= (800f - GetComponent<RectTransform> ().sizeDelta.x)) {
			return true;
		}
		return false;
	}
}

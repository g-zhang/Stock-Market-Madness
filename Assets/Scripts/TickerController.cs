using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TickerController : MonoBehaviour {

	public static TickerController S;

	public List<string> tickerQueue;
	public List<Text> currentTexts;

	public Text textTemplate;

	void Awake() {
		S = this;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Q)) {
			displayNextNews ();
			print (currentTexts.Count);
		}
			
		if (Input.GetKeyDown (KeyCode.W)) {
			destroyOldNews ();
		}
			
		// If there is strings left in tickerQueue,
			// then show the next text
		if (tickerQueue.Count > 0) {
			// if there is nothing currently on the ticker
			if (currentTexts.Count == 0) {
				displayNextNews ();
			}
			// or if the most recent text in currentTexts is fully shown
			else if (currentTexts[currentTexts.Count-1].GetComponent<TickerTextController>().hasFullyShown()) {
				displayNextNews ();
			}
		}
	}

	// Adds to the back of the queue
	public void addNews(string message) {
		tickerQueue.Add (message);
	}

	// Adds to the front of the queue
	public void addBreakingNews (string message) {
		tickerQueue.Insert (0, message);
	}

	void displayNextNews () {
		// Create text object
		Text tempText = Instantiate (textTemplate, textTemplate.transform.position, textTemplate.transform.rotation) as Text;
		tempText.transform.SetParent (this.transform);
		tempText.transform.localScale = new Vector3 (1f, 1f, 1f);
		tempText.GetComponent<TickerTextController> ().isTemplate = false;

		// Set text value to first in queue
		tempText.text = tickerQueue [0];
		tickerQueue.RemoveAt (0);
		if (tickerQueue.Count > 0) {
			tempText.text += "   ||  ";
		}
		tempText.GetComponent<RectTransform> ().sizeDelta = new Vector2 (tempText.preferredWidth, 20f);

		currentTexts.Add (tempText);
	}

	public void destroyOldNews () {
		Destroy (currentTexts [0].gameObject);
		currentTexts.RemoveAt (0);
	}
		
}

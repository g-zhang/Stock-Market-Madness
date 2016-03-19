using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TickerController : MonoBehaviour {

	public List<string> tickerQueue;
	public List<Text> currentTexts;

	public Text textTemplate;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.A)) {
			displayNextNews ();
		}
			
		if (Input.GetKeyDown (KeyCode.S)) {
			destroyOldNews ();
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
		currentTexts.Add (tempText);

		// Set text value to first in queue
		tempText.text = tickerQueue [0];
		tickerQueue.RemoveAt (0);
	}

	void destroyOldNews () {
		Destroy (currentTexts [0].gameObject);
		currentTexts.RemoveAt (0);
	}
		
}

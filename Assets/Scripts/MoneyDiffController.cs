using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoneyDiffController : MonoBehaviour {
	
	public float scrollSpeed;
	public float sineAmplitude;

	public float timeTilDestroy;
	public bool isTemplate = true;

	float creationTime;

	void Start () {
		creationTime = Time.timeSinceLevelLoad;
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Q)) {
			displayValue (100);
		}


		if (!isTemplate) {
			// How long it's been alive
			float currLifetime = (Time.timeSinceLevelLoad - creationTime);
			//print (currLifetime);

			// Make it float away and also wiggle lol
			float xPos = Mathf.Sin(Time.timeSinceLevelLoad * sineAmplitude);
			transform.position += new Vector3 (xPos, scrollSpeed, 0);

			// Once it's lived half it's life, start fading out
			if (currLifetime >= timeTilDestroy / 2) {
				// cur = p0(1-u) + p1(u)
				GetComponent<Text>().CrossFadeAlpha(0f, timeTilDestroy/2, true);
			}

			// If it's lived too long, destroy it
			if (currLifetime >= timeTilDestroy) {
				Destroy (this.gameObject);
			}
		}
	}

	// Requires changeValue to not equal 0
	public void displayValue(float changeValue) {
		isTemplate = false;

		creationTime = Time.timeSinceLevelLoad;

		// if changeValue is a positive number
		if (changeValue > 0) {
			GetComponent<Text>().text = string.Format ("{0:C}", changeValue).Insert (1, " ").Insert (0, "+");
			GetComponent<Text>().color = Color.green;
		}
		// if negative...
		else if (changeValue < 0) {
			changeValue *= -1;
			
			GetComponent<Text>().text = string.Format ("{0:C}", changeValue).Insert (1, " ").Insert (0, "-");
			GetComponent<Text>().color = Color.red;
		}

		GetComponent<Text> ().enabled = true;
	}
}	

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdatePlayerPanel : MonoBehaviour {

	public Image highlight;

	[Header("Text Objects")]
	public Text CurrentMoney;
	public Text MoneyDiff;
	public Text[] CompanyShares;

	public float timeTilDiffText;
	public float diffTextDisplayTime;
	float timeSinceMoneyDiff;
	float timeSinceDisplay;
	float lastMoney;
	float MoneyDiffValue;

	void Start() {
		timeSinceMoneyDiff = Time.timeSinceLevelLoad;
		MoneyDiffValue = 0;
		lastMoney = GetComponent<Player> ().currentMoney;
	}

	void Update () {
		updateSelectedCom (GetComponent<Player> ().selectedCompany);

		for (CompanyName i = 0; i != CompanyName.size; i++) {
			updateShare (i, GetComponent<Player> ().getPlayerStockData(i));
		}
		updateMoney(GetComponent<Player>().currentMoney);
	}

	void updateMoney(float newValue) {
		// If money is changing since last time
		if (lastMoney != newValue) {
			// start timer
			if ((Time.timeSinceLevelLoad - timeSinceMoneyDiff) >= timeTilDiffText) {
				MoneyDiffValue = 0;
			}

			timeSinceMoneyDiff = Time.timeSinceLevelLoad;
			MoneyDiffValue += newValue - lastMoney;
			lastMoney = newValue;
		}

		displayChange (MoneyDiffValue);

		string finalValue = string.Format ("{0:C}", newValue);
		finalValue = finalValue.Insert (1, " ");
		CurrentMoney.text = finalValue;
	}

	void updateShare(CompanyName whichCom, int newValue) {
		string finalValue = (newValue/1000).ToString () + "k";
		CompanyShares [(int)whichCom].text = finalValue;
	}

	void updateSelectedCom(CompanyName whichCom) {
		if (whichCom == CompanyName.none) {
			highlight.enabled = false;
		} 
		else {
			highlight.enabled = true;
		}

		if (whichCom == CompanyName.A) {
			highlight.transform.localPosition = new Vector3 (7f, -30f, 0);
		} 
		else if (whichCom == CompanyName.B) {
			highlight.transform.localPosition = new Vector3 (67f, -30f, 0);
		} 
		else if (whichCom == CompanyName.C) {
			highlight.transform.localPosition = new Vector3 (128f, -30f, 0);
		} 
	}

	void displayChange(float changeValue) {
		if ((Time.timeSinceLevelLoad - timeSinceDisplay) >= diffTextDisplayTime) {
			MoneyDiff.enabled = false;
		}

		// If the amount didn't change, or there was a change made recently
		if (changeValue == 0 || (Time.timeSinceLevelLoad - timeSinceMoneyDiff) < timeTilDiffText) {
			MoneyDiff.enabled = false;
			return;
		}

		if (MoneyDiff.enabled == false) {
			MoneyDiff.enabled = true;
			timeSinceDisplay = Time.timeSinceLevelLoad;
		}
		// if changeValue is a positive number
		if (changeValue > 0) {
			MoneyDiff.text = string.Format ("{0:C}", changeValue).Insert (1, " ").Insert (0, "+");
			MoneyDiff.color = Color.green;
		}
		// if negative...
		else {
			changeValue *= -1;
			MoneyDiff.text = string.Format ("{0:C}", changeValue).Insert(1, " ").Insert(0, "-");
			MoneyDiff.color = Color.red;
		}
	}
}

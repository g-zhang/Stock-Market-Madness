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
	float timeSinceMoneyDiff;
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
		// start timer and reset MoneyDiffValue if its been a while since last change
		if ((Time.timeSinceLevelLoad - timeSinceMoneyDiff) >= timeTilDiffText) {
			if (MoneyDiffValue != 0) {
				timeSinceMoneyDiff = Time.timeSinceLevelLoad;
				displayChange (MoneyDiffValue);
			}
			MoneyDiffValue = 0;
		}

		// If money is changing since last time,
		// update MoneyDiffValue
		if (lastMoney != newValue) {
			timeSinceMoneyDiff = Time.timeSinceLevelLoad;
			MoneyDiffValue += newValue - lastMoney;
			lastMoney = newValue;
		}

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
		Text tempText = Instantiate (MoneyDiff, MoneyDiff.transform.position, MoneyDiff.transform.rotation) as Text;
		tempText.transform.SetParent (this.transform);
		tempText.transform.localScale = new Vector3 (1f, 1f, 1f);

		tempText.GetComponent<MoneyDiffController> ().displayValue (changeValue);

		if (changeValue > 0) {
			string message = "Player " + (GetComponent<Player> ().playerNum + 1).ToString() + " has earned " + string.Format ("{0:C}", changeValue);
			TickerController.S.addBreakingNews (message);
		}
		if (changeValue < 0) {
			changeValue *= -1;

			string message = "Player " + (GetComponent<Player> ().playerNum + 1).ToString() + " has lost " + string.Format ("{0:C}", changeValue);
			TickerController.S.addBreakingNews (message);
		}
	}
}

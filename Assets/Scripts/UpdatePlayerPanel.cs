using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdatePlayerPanel : MonoBehaviour {

	public Image highlight;

	[Header("Text Objects")]
	public Text CurrentMoney;
	public Text MoneyDiff;
	public Text[] CompanyShares;

	float lastMoney;
	float MoneyDiffValue;

	void Start() {
		lastMoney = GetComponent<Player> ().currentMoney;
		MoneyDiffValue = 0;
	}

	void Update () {
		updateSelectedCom (GetComponent<Player> ().selectedCompany);

		for (CompanyName i = 0; i != CompanyName.size; i++) {
			updateShare (i, GetComponent<Player> ().CompanyShares [(int)i]);
		}
		updateMoney(GetComponent<Player>().currentMoney);
	}

	void updateMoney(float newValue) {
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
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdatePlayerPanel : MonoBehaviour {
	public enum companies {ComA = 0, ComB, ComC, size};

	[Header("Text Objects")]
	public Text CurrentMoney;
	public Text[] CompanyShares;

	void Update () {
		for (companies i = 0; i != companies.size; i++) {
			updateShare (i, GetComponent<Player> ().CompanyShares [(int)i]);
		}
		updateMoney(GetComponent<Player>().currentMoney);
	}

	void updateMoney(float newValue) {
		string finalValue = "$ ";
		int dollarValue = Mathf.FloorToInt (newValue);
		int centValue = Mathf.FloorToInt ((newValue - dollarValue) * 100);
		string dollar = dollarValue.ToString ();

		for (int i = dollar.Length - 3; i > 0; i -= 3) {
			dollar = dollar.Insert (i, ",");
		}

		string cent = "0";
		if (centValue < 10) {
			cent += centValue.ToString ();
		} else {
			cent = centValue.ToString ();
		}

		finalValue += dollar + "." + cent;

		CurrentMoney.text = finalValue;
	}

	void updateShare(companies whichCom, int newValue) {
		string finalValue = newValue.ToString () + "k";
		CompanyShares [(int)whichCom].text = finalValue;
	}
}

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
		string finalValue = string.Format ("{0:C}", newValue);
		finalValue = finalValue.Insert (1, " ");
		CurrentMoney.text = finalValue;
	}

	void updateShare(companies whichCom, int newValue) {
		string finalValue = newValue.ToString () + "k";
		CompanyShares [(int)whichCom].text = finalValue;
	}
}

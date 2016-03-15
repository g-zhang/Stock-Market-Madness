using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateComPanel : MonoBehaviour {

	public Text[] stockPricesText = new Text[(int)CompanyName.size];
	public Text[] stockAvailText = new Text[(int)CompanyName.size];
	
	// Update is called once per frame
	void Update () {
		for (CompanyName i = 0; i != CompanyName.size; i++) {
			updateStockPrice (i, CompanyManager.S.GetStocks (CompanyName.A).Price);
			updateStockAvail (i, CompanyManager.S.GetStocks (CompanyName.A).StocksAvailable);
		}
	}

	void updateStockPrice(CompanyName whichCom, float newPrice) {
		string finalValue = string.Format ("{0:C}", newPrice);
		finalValue = finalValue.Insert (1, " ");

		stockPricesText [(int)whichCom].text = finalValue;
	}

	void updateStockAvail(CompanyName whichCom, int newValue) {
		string numStock = newValue.ToString ();
		for (int i = (numStock.Length - 3); i > 0; i -= 3) {
			numStock = numStock.Insert (i, ",");
		}

		stockAvailText [(int)whichCom].text = numStock;
	}
}

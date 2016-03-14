using UnityEngine;

abstract public class StockGenerator : MonoBehaviour
{
	public int numStocksAvailable = 1000000;

	abstract public float getNextStockValue();
	abstract public bool RecordBuy(int numStocks);
	abstract public void RecordSell(int numStocks);
}
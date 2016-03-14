using UnityEngine;

abstract public class StockGenerator : MonoBehaviour
{
	abstract public float getNextStockValue();
	abstract public bool RecordBuy(int numStocks);
	abstract public void RecordSell(int numStocks);
}
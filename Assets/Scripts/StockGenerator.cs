abstract public class StockGenerator
{
	abstract public void getNextStockValue(BuySellData data,
		out float minRandVal, out float maxRandVal);
}
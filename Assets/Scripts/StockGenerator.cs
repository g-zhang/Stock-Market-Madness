public interface StockGenerator
{
	void getNextStockValue(ref BuySellData data,
		out float minRandVal, out float maxRandVal);
}
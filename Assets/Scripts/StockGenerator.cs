public interface StockGenerator
{
	void getNextStockValue(BuySellData data,
		out float minRandVal, out float maxRandVal);
}
public interface StockGenerator
{
	float getNextStockValue();
	void RecordBuy(int numStocks);
	void RecordSell(int numStocks);
}
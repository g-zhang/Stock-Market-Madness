using System.Collections.Generic;

public class Model {

	public StockEvent[] availableEvents;
	public Queue<StockEvent> eventQueue;

	public void Tick() {

	}

	public bool Buy(string player, string stock, int number) {
		return true;
	}

	public bool Sell(string player, string stock, int number) {
		return true;
	}

	public void ActivateEvent(StockEvent stockEvent) {
		return;
	}

}

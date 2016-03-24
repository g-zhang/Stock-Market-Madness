using System.Collections.Generic;
using System;
class EventLibrary {

	// http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp
	private static Random rng = new Random();
	public static void Shuffle<T>(List<T> list) {
		int n = list.Count;
		while (n > 1) {
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	static public List<StockEvent> getEventsShuffled(int n) {
		List<StockEvent> result = new List<StockEvent>();
		Shuffle(result);
		return result;
	}

	// CREATE AN EVENT:
	// 1. Make a static private StockEvent, with string name, then alternating
	//    min/max values for all ranges in the distribution for that event.
	// 2. Add that StockEvent to the stockEvents list

	static private List<StockEvent> stockEvents = new List<StockEvent> {
		RISKY_CEO,
		STEADY_GAINS,
		RISKY_PROJECT,
		SLOW_DEATH
	};

	static private StockEvent RISKY_CEO =
		new StockEvent("Elect Cocaine Charlie CEO", 5f, 6f, -5f, -6f);
	static private StockEvent STEADY_GAINS =
		new StockEvent("Take the beaten path. Do what we know.", -.5f, 3f);
	static private StockEvent RISKY_PROJECT =
		new StockEvent("Take on a risky new project.", -4f, -.1f, 5.5f, 6f);
	static private StockEvent SLOW_DEATH =
		new StockEvent("Waste money on company parties.", -4f, -2f);
}

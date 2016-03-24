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

	static public List<StockEvent> getEventsShuffled() {
		Shuffle(stockEvents);
		return stockEvents;
	}

    static public List<StockEvent> getEventsCurrent()
    {
        return stockEvents;
    }

    // CREATE AN EVENT:
    // 1. Make a static private StockEvent, with string name, then alternating
    //    min/max values for all ranges in the distribution for that event.
    // 2. Add that StockEvent to the stockEvents list
    static private List<StockEvent> stockEvents;
    static EventLibrary()
    {
        stockEvents = new List<StockEvent> {
            WILDCARD_CEO,
            UNRELIABLY_DOWN_CEO,
            UNRELIABLY_UP_CEO,
            DOWNER_CEO,
            UPPER_CEO,
            STEADY_GAINS_PROJECT,
            RISKY_PROJECT,
            WASTE_MONEY
        };
    }

    static private StockEvent WILDCARD_CEO =
		new StockEvent("Elect Wild-Card Charlie CEO", .5f, .6f, -.5f, -.6f);
	static private StockEvent UNRELIABLY_DOWN_CEO =
		new StockEvent("Elect Duncy Dee CEO", -.1f, -.6f, .4f, .5f);
	static private StockEvent UNRELIABLY_UP_CEO =
		new StockEvent("Elect Try-Hard Dennis CEO", .1f, .6f, -.4f, -.5f);
	static private StockEvent DOWNER_CEO =
		new StockEvent("Elect Frank the Trash-man CEO", -.1f, -.3f);
	static private StockEvent UPPER_CEO =
		new StockEvent("Elect Earnest Mac CEO", .1f, .3f);
	static private StockEvent STEADY_GAINS_PROJECT =
		new StockEvent("Take the tried and true path.", -.05f, .3f);
	static private StockEvent RISKY_PROJECT =
		new StockEvent("Take on a risky new project.", -.4f, -.01f, .55f, .6f);
	static private StockEvent WASTE_MONEY =
		new StockEvent("Waste money on company parties.", -.4f, -.2f);
}

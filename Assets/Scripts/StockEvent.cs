using System;
using System.Collections.Generic;

public class FloatRange {
	
	public float min, max;
	Random r;

	public FloatRange(float inMin, float inMax) {
		min = inMin;
		max = inMax;
		orient();
		r = new Random();
	}

	public FloatRange() : this(0f, 1f) { }

	public float random { get { return range * (float) r.NextDouble() - min; } }
	public float range { get { return max - min; } }

	private void orient() {
		if (min < max) return;
		float t = max;
		max = min;
		min = t;
	}
}

public class Distribution {

	private List<FloatRange> ranges = new List<FloatRange>();
	private Random r = new Random();
	
	public float sample {
		get {
			if (ranges.Count == 0) return 0.0f;
			List<float> dist = new List<float>();
			dist.Add(0f);
			foreach (FloatRange range in ranges) dist.Add(dist[dist.Count - 1] + range.range);
			double rand = r.NextDouble() * dist[dist.Count - 1];
			int i = 1; while (rand > dist[i++]);
			return ranges[i].random;
		}
	}

	public void AddRange(float min, float max) {
		ranges.Add(new FloatRange(min, max));
	}
}

public class StockEvent {

	public string eventName;
	public Distribution priceChangeDistribution = new Distribution();

	public StockEvent(string name, params float[] bounds) {
		List<float> boundList = new List<float>(bounds);
		float lastMin = 0;
		for (int i=0; i < boundList.Count; i++)
			if (i % 2 == 0) lastMin = boundList[i];
			else priceChangeDistribution.AddRange(lastMin, boundList[i]);
	}
}

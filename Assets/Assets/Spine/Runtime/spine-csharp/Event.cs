
using System;

namespace Spine {
	/// <summary>Stores the current pose values for an Event.</summary>
	public class Event {
		internal readonly EventData data;
		internal readonly float time;
		internal int intValue;
		internal float floatValue;
		internal string stringValue;
		internal float volume;
		internal float balance;

		public EventData Data { get { return data; } }
		/// <summary>The animation time this event was keyed.</summary>
		public float Time { get { return time; } }

		public int Int { get { return intValue; } set { intValue = value; } }
		public float Float { get { return floatValue; } set { floatValue = value; } }
		public string String { get { return stringValue; } set { stringValue = value; } }

		public float Volume { get { return volume; } set { volume = value; } }
		public float Balance { get { return balance; } set { balance = value; } }

		public Event (float time, EventData data) {
			if (data == null) throw new ArgumentNullException("data", "data cannot be null.");
			this.time = time;
			this.data = data;
		}

		override public string ToString () {
			return this.data.Name;
		}
	}
}

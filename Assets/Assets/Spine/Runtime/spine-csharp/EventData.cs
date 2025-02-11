
using System;

namespace Spine {
	/// <summary>Stores the setup pose values for an Event.</summary>
	public class EventData {
		internal string name;

		/// <summary>The name of the event, which is unique across all events in the skeleton.</summary>
		public string Name { get { return name; } }
		public int Int { get; set; }
		public float Float { get; set; }
		public string @String { get; set; }

		public string AudioPath { get; set; }
		public float Volume { get; set; }
		public float Balance { get; set; }

		public EventData (string name) {
			if (name == null) throw new ArgumentNullException("name", "name cannot be null.");
			this.name = name;
		}

		override public string ToString () {
			return Name;
		}
	}
}

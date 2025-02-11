
using System;

namespace Spine {
	public class ClippingAttachment : VertexAttachment {
		internal SlotData endSlot;

		public SlotData EndSlot { get { return endSlot; } set { endSlot = value; } }

		public ClippingAttachment(string name) : base(name) {
		}

		public override Attachment Copy () {
			ClippingAttachment copy = new ClippingAttachment(this.Name);
			CopyTo(copy);
			copy.endSlot = endSlot;
			return copy;
		}
	}
}

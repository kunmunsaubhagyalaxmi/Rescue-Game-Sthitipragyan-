
using System;
using System.Collections.Generic;

namespace Spine {
	public class PathAttachment : VertexAttachment {
		internal float[] lengths;
		internal bool closed, constantSpeed;

		/// <summary>The length in the setup pose from the start of the path to the end of each curve.</summary>
		public float[] Lengths { get { return lengths; } set { lengths = value; } }
		public bool Closed { get { return closed; } set { closed = value; } }
		public bool ConstantSpeed { get { return constantSpeed; } set { constantSpeed = value; } }

		public PathAttachment (String name)
			: base(name) {
		}

		public override Attachment Copy () {
			PathAttachment copy = new PathAttachment(this.Name);
			CopyTo(copy);
			copy.lengths = new float[lengths.Length];
			Array.Copy(lengths, 0, copy.lengths, 0, lengths.Length);
			copy.closed = closed;
			copy.constantSpeed = constantSpeed;
			return copy;
		}
	}
}

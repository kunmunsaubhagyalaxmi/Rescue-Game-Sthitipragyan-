
using System;
using System.Collections.Generic;

namespace Spine {
	/// <summary>Stores the setup pose for an IkConstraint.</summary>
	public class IkConstraintData : ConstraintData {
		internal ExposedList<BoneData> bones = new ExposedList<BoneData>();
		internal BoneData target;
		internal int bendDirection = 1;
		internal bool compress, stretch, uniform;
		internal float mix = 1, softness;

		public IkConstraintData (string name) : base(name) {
		}

		/// <summary>The bones that are constrained by this IK Constraint.</summary>
		public ExposedList<BoneData> Bones {
			get { return bones; }
		}

		/// <summary>The bone that is the IK target.</summary>
		public BoneData Target {
			get { return target; }
			set { target = value; }
		}

		/// <summary>
		/// A percentage (0-1) that controls the mix between the constraint and unconstrained rotations.</summary>
		public float Mix {
			get { return mix; }
			set { mix = value; }
		}

		///<summary>For two bone IK, the distance from the maximum reach of the bones that rotation will slow.</summary>
		public float Softness {
			get { return softness; }
			set { softness = value; }
		}

		/// <summary>Controls the bend direction of the IK bones, either 1 or -1.</summary>
		public int BendDirection {
			get { return bendDirection; }
			set { bendDirection = value; }
		}

		/// <summary>
		/// When true, and only a single bone is being constrained,
		/// if the target is too close, the bone is scaled to reach it. </summary>
		public bool Compress {
			get { return compress; }
			set { compress = value; }
		}

		/// <summary>
		/// When true, if the target is out of range, the parent bone is scaled on the X axis to reach it.
		/// If the bone has local nonuniform scale, stretching is not applied.</summary>
		public bool Stretch {
			get { return stretch; }
			set { stretch = value; }
		}

		/// <summary>
		/// When true, only a single bone is being constrained and Compress or Stretch is used,
		/// the bone is scaled both on the X and Y axes.</summary>
		public bool Uniform {
			get { return uniform; }
			set { uniform = value; }
		}
	}
}


using System;
using System.Collections.Generic;

namespace Spine
{
	/// <summary>The base class for all constraint datas.</summary>
	public abstract class ConstraintData {
		internal readonly string name;
		internal int order;
		internal bool skinRequired;

		public ConstraintData (string name) {
			if (name == null) throw new ArgumentNullException("name", "name cannot be null.");
			this.name = name;
		}

		/// <summary> The constraint's name, which is unique across all constraints in the skeleton of the same type.</summary>
		public string Name { get { return name; } }

		///<summary>The ordinal of this constraint for the order a skeleton's constraints will be applied by
		/// <see cref="Skeleton.UpdateWorldTransform()"/>.</summary>
		public int Order { get { return order; } set { order = value; } }

		///<summary>When true, <see cref="Skeleton.UpdateWorldTransform()"/> only updates this constraint if the <see cref="Skeleton.Skin"/> contains
		/// this constraint.</summary>
		///<seealso cref="Skin.Constraints"/>
		public bool SkinRequired { get { return skinRequired; } set { skinRequired = value; } }

		override public string ToString () {
			return name;
		}
	}
}

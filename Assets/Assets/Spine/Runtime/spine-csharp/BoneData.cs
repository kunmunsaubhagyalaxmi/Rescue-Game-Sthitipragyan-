
using System;

namespace Spine {
	public class BoneData {
		internal int index;
		internal string name;
		internal BoneData parent;
		internal float length;
		internal float x, y, rotation, scaleX = 1, scaleY = 1, shearX, shearY;
		internal TransformMode transformMode = TransformMode.Normal;
		internal bool skinRequired;

		/// <summary>The index of the bone in Skeleton.Bones</summary>
		public int Index { get { return index; } }

		/// <summary>The name of the bone, which is unique across all bones in the skeleton.</summary>
		public string Name { get { return name; } }

		/// <summary>May be null.</summary>
		public BoneData Parent { get { return parent; } }

		public float Length { get { return length; } set { length = value; } }

		/// <summary>Local X translation.</summary>
		public float X { get { return x; } set { x = value; } }

		/// <summary>Local Y translation.</summary>
		public float Y { get { return y; } set { y = value; } }

		/// <summary>Local rotation.</summary>
		public float Rotation { get { return rotation; } set { rotation = value; } }

		/// <summary>Local scaleX.</summary>
		public float ScaleX { get { return scaleX; } set { scaleX = value; } }

		/// <summary>Local scaleY.</summary>
		public float ScaleY { get { return scaleY; } set { scaleY = value; } }

		/// <summary>Local shearX.</summary>
		public float ShearX { get { return shearX; } set { shearX = value; } }

		/// <summary>Local shearY.</summary>
		public float ShearY { get { return shearY; } set { shearY = value; } }

		/// <summary>The transform mode for how parent world transforms affect this bone.</summary>
		public TransformMode TransformMode { get { return transformMode; } set { transformMode = value; } }

		///<summary>When true, <see cref="Skeleton.UpdateWorldTransform()"/> only updates this bone if the <see cref="Skeleton.Skin"/> contains this
		/// bone.</summary>
		/// <seealso cref="Skin.Bones"/>
		public bool SkinRequired { get { return skinRequired; } set { skinRequired = value; } }

		/// <param name="parent">May be null.</param>
		public BoneData (int index, string name, BoneData parent) {
			if (index < 0) throw new ArgumentException("index must be >= 0", "index");
			if (name == null) throw new ArgumentNullException("name", "name cannot be null.");
			this.index = index;
			this.name = name;
			this.parent = parent;
		}

		override public string ToString () {
			return name;
		}
	}

	[Flags]
	public enum TransformMode {
		//0000 0 Flip Scale Rotation
		Normal = 0, // 0000
		OnlyTranslation = 7, // 0111
		NoRotationOrReflection = 1, // 0001
		NoScale = 2, // 0010
		NoScaleOrReflection = 6, // 0110
	}
}

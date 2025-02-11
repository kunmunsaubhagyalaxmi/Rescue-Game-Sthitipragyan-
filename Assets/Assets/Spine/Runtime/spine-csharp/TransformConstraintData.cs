
using System;

namespace Spine {
	public class TransformConstraintData : ConstraintData {
		internal ExposedList<BoneData> bones = new ExposedList<BoneData>();
		internal BoneData target;
		internal float rotateMix, translateMix, scaleMix, shearMix;
		internal float offsetRotation, offsetX, offsetY, offsetScaleX, offsetScaleY, offsetShearY;
		internal bool relative, local;

		public ExposedList<BoneData> Bones { get { return bones; } }
		public BoneData Target { get { return target; } set { target = value; } }
		public float RotateMix { get { return rotateMix; } set { rotateMix = value; } }
		public float TranslateMix { get { return translateMix; } set { translateMix = value; } }
		public float ScaleMix { get { return scaleMix; } set { scaleMix = value; } }
		public float ShearMix { get { return shearMix; } set { shearMix = value; } }

		public float OffsetRotation { get { return offsetRotation; } set { offsetRotation = value; } }
		public float OffsetX { get { return offsetX; } set { offsetX = value; } }
		public float OffsetY { get { return offsetY; } set { offsetY = value; } }
		public float OffsetScaleX { get { return offsetScaleX; } set { offsetScaleX = value; } }
		public float OffsetScaleY { get { return offsetScaleY; } set { offsetScaleY = value; } }
		public float OffsetShearY { get { return offsetShearY; } set { offsetShearY = value; } }

		public bool Relative { get { return relative; } set { relative = value; } }
		public bool Local { get { return local; } set { local = value; } }

		public TransformConstraintData (string name) : base(name) {
		}
	}
}

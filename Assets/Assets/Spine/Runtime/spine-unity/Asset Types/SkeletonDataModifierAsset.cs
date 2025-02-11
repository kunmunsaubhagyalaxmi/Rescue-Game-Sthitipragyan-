
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity {
	/// <summary>Can be stored by SkeletonDataAsset to automatically apply modifications to loaded SkeletonData.</summary>
	public abstract class SkeletonDataModifierAsset : ScriptableObject {
		public abstract void Apply (SkeletonData skeletonData);
	}
}

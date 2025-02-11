
#define AUTOINIT_SPINEREFERENCE

using UnityEngine;

namespace Spine.Unity {
	[CreateAssetMenu(menuName = "Spine/Animation Reference Asset", order = 100)]
	public class AnimationReferenceAsset : ScriptableObject, IHasSkeletonDataAsset {
		const bool QuietSkeletonData = true;

		[SerializeField] protected SkeletonDataAsset skeletonDataAsset;
		[SerializeField, SpineAnimation] protected string animationName;
		private Animation animation;

		public SkeletonDataAsset SkeletonDataAsset { get { return skeletonDataAsset; } }

		public Animation Animation {
			get {
				#if AUTOINIT_SPINEREFERENCE
				if (animation == null)
					Initialize();
				#endif

				return animation;
			}
		}

		public void Initialize () {
			if (skeletonDataAsset == null) return;
			this.animation = skeletonDataAsset.GetSkeletonData(AnimationReferenceAsset.QuietSkeletonData).FindAnimation(animationName);
			if (this.animation == null) Debug.LogWarningFormat("Animation '{0}' not found in SkeletonData : {1}.", animationName, skeletonDataAsset.name);
		}

		public static implicit operator Animation (AnimationReferenceAsset asset) {
			return asset.Animation;
		}
	}
}

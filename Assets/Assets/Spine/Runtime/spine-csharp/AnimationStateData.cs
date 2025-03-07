
using System;
using System.Collections.Generic;

namespace Spine {

	/// <summary>Stores mix (crossfade) durations to be applied when AnimationState animations are changed.</summary>
	public class AnimationStateData {
		internal SkeletonData skeletonData;
		readonly Dictionary<AnimationPair, float> animationToMixTime = new Dictionary<AnimationPair, float>(AnimationPairComparer.Instance);
		internal float defaultMix;

		/// <summary>The SkeletonData to look up animations when they are specified by name.</summary>
		public SkeletonData SkeletonData { get { return skeletonData; } }

		/// <summary>
		/// The mix duration to use when no mix duration has been specifically defined between two animations.</summary>
		public float DefaultMix { get { return defaultMix; } set { defaultMix = value; } }

		public AnimationStateData (SkeletonData skeletonData) {
			if (skeletonData == null) throw new ArgumentException("skeletonData cannot be null.", "skeletonData");
			this.skeletonData = skeletonData;
		}

		/// <summary>Sets a mix duration by animation names.</summary>
		public void SetMix (string fromName, string toName, float duration) {
			Animation from = skeletonData.FindAnimation(fromName);
			if (from == null) throw new ArgumentException("Animation not found: " + fromName, "fromName");
			Animation to = skeletonData.FindAnimation(toName);
			if (to == null) throw new ArgumentException("Animation not found: " + toName, "toName");
			SetMix(from, to, duration);
		}

		/// <summary>Sets a mix duration when changing from the specified animation to the other.
		/// See TrackEntry.MixDuration.</summary>
		public void SetMix (Animation from, Animation to, float duration) {
			if (from == null) throw new ArgumentNullException("from", "from cannot be null.");
			if (to == null) throw new ArgumentNullException("to", "to cannot be null.");
			AnimationPair key = new AnimationPair(from, to);
			animationToMixTime.Remove(key);
			animationToMixTime.Add(key, duration);
		}

		/// <summary>
		/// The mix duration to use when changing from the specified animation to the other,
		/// or the DefaultMix if no mix duration has been set.
		/// </summary>
		public float GetMix (Animation from, Animation to) {
			if (from == null) throw new ArgumentNullException("from", "from cannot be null.");
			if (to == null) throw new ArgumentNullException("to", "to cannot be null.");
			AnimationPair key = new AnimationPair(from, to);
			float duration;
			if (animationToMixTime.TryGetValue(key, out duration)) return duration;
			return defaultMix;
		}

		public struct AnimationPair {
			public readonly Animation a1;
			public readonly Animation a2;

			public AnimationPair (Animation a1, Animation a2) {
				this.a1 = a1;
				this.a2 = a2;
			}

			public override string ToString () {
				return a1.name + "->" + a2.name;
			}
		}

		// Avoids boxing in the dictionary.
		public class AnimationPairComparer : IEqualityComparer<AnimationPair> {
			public static readonly AnimationPairComparer Instance = new AnimationPairComparer();

			bool IEqualityComparer<AnimationPair>.Equals (AnimationPair x, AnimationPair y) {
				return ReferenceEquals(x.a1, y.a1) && ReferenceEquals(x.a2, y.a2);
			}

			int IEqualityComparer<AnimationPair>.GetHashCode (AnimationPair obj) {
				// from Tuple.CombineHashCodes // return (((h1 << 5) + h1) ^ h2);
				int h1 = obj.a1.GetHashCode();
				return (((h1 << 5) + h1) ^ obj.a2.GetHashCode());
			}
		}
	}
}

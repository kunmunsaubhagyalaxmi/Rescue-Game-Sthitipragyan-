
namespace Spine.Unity {
	public delegate void UpdateBonesDelegate (ISkeletonAnimation animated);

	/// <summary>A Spine-Unity Component that animates a Skeleton but not necessarily with a Spine.AnimationState.</summary>
	public interface ISkeletonAnimation {
		event UpdateBonesDelegate UpdateLocal;
		event UpdateBonesDelegate UpdateWorld;
		event UpdateBonesDelegate UpdateComplete;
		Skeleton Skeleton { get; }
	}

	/// <summary>Holds a reference to a SkeletonDataAsset.</summary>
	public interface IHasSkeletonDataAsset {
		/// <summary>Gets the SkeletonDataAsset of the Spine Component.</summary>
		SkeletonDataAsset SkeletonDataAsset { get; }
	}

	/// <summary>A Spine-Unity Component that manages a Spine.Skeleton instance, instantiated from a SkeletonDataAsset.</summary>
	public interface ISkeletonComponent {
		/// <summary>Gets the SkeletonDataAsset of the Spine Component.</summary>
		//[System.Obsolete]
		SkeletonDataAsset SkeletonDataAsset { get; }

		/// <summary>Gets the Spine.Skeleton instance of the Spine Component. This is equivalent to SkeletonRenderer's .skeleton.</summary>
		Skeleton Skeleton { get; }
	}

	/// <summary>A Spine-Unity Component that uses a Spine.AnimationState to animate its skeleton.</summary>
	public interface IAnimationStateComponent {
		/// <summary>Gets the Spine.AnimationState of the animated Spine Component. This is equivalent to SkeletonAnimation.state.</summary>
		AnimationState AnimationState { get; }
	}

	/// <summary>A Spine-Unity Component that holds a reference to a SkeletonRenderer.</summary>
	public interface IHasSkeletonRenderer {
		SkeletonRenderer SkeletonRenderer { get; }
	}

	/// <summary>A Spine-Unity Component that holds a reference to an ISkeletonComponent.</summary>
	public interface IHasSkeletonComponent {
		ISkeletonComponent SkeletonComponent { get; }
	}
}

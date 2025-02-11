
#if UNITY_2018_3 || UNITY_2019 || UNITY_2018_3_OR_NEWER
#define NEW_PREFAB_SYSTEM
#endif

using UnityEngine;

namespace Spine.Unity {

	#if NEW_PREFAB_SYSTEM
	[ExecuteAlways]
	#else
	[ExecuteInEditMode]
	#endif
	[RequireComponent(typeof(SkeletonUtilityBone))]
	public abstract class SkeletonUtilityConstraint : MonoBehaviour {

		protected SkeletonUtilityBone bone;
		protected SkeletonUtility hierarchy;

		protected virtual void OnEnable () {
			bone = GetComponent<SkeletonUtilityBone>();
			hierarchy = transform.GetComponentInParent<SkeletonUtility>();
			hierarchy.RegisterConstraint(this);
		}

		protected virtual void OnDisable () {
			hierarchy.UnregisterConstraint(this);
		}

		public abstract void DoUpdate ();
	}
}


using UnityEngine;

namespace Spine.Unity {

	/// <summary>
	/// Utility component to support flipping of hinge chains (chains of HingeJoint objects) along with the parent skeleton.
	/// Note that flipping needs to be performed by 180 degree rotation at <see cref="SkeletonUtility"/>,
	/// by setting <see cref="SkeletonUtility.flipBy180DegreeRotation"/> to true, not via negative scale.
	///
	/// Note: This component is automatically attached when calling "Create Hinge Chain" at <see cref="SkeletonUtilityBone"/>,
	/// do not attempt to use this component for other purposes.
	/// </summary>
	public class FollowSkeletonUtilityRootRotation : MonoBehaviour {
	
		const float FLIP_ANGLE_THRESHOLD = 100.0f;

		public Transform reference;
		Vector3 prevLocalEulerAngles;

		private void Start () {
			prevLocalEulerAngles = this.transform.localEulerAngles;
		}

		void FixedUpdate () {
			this.transform.rotation = reference.rotation;

			bool wasFlippedAroundY = Mathf.Abs(this.transform.localEulerAngles.y - prevLocalEulerAngles.y) > FLIP_ANGLE_THRESHOLD;
			bool wasFlippedAroundX = Mathf.Abs(this.transform.localEulerAngles.x - prevLocalEulerAngles.x) > FLIP_ANGLE_THRESHOLD;
			if (wasFlippedAroundY)
				CompensatePositionToYRotation();
			if (wasFlippedAroundX)
				CompensatePositionToXRotation();

			prevLocalEulerAngles = this.transform.localEulerAngles;
		}

		/// <summary>
		/// Compensates the position so that a child at the reference position remains in the same place,
		/// to counter any movement that occurred by rotation.
		/// </summary>
		void CompensatePositionToYRotation () {
			Vector3 newPosition = reference.position + (reference.position - this.transform.position);
			newPosition.y = this.transform.position.y;
			this.transform.position = newPosition;
		}

		/// <summary>
		/// Compensates the position so that a child at the reference position remains in the same place,
		/// to counter any movement that occurred by rotation.
		/// </summary>
		void CompensatePositionToXRotation () {
			Vector3 newPosition = reference.position + (reference.position - this.transform.position);
			newPosition.x = this.transform.position.x;
			this.transform.position = newPosition;
		}
	}
}

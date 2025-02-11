
using UnityEngine;

namespace Spine.Unity {

	/// <summary>
	/// Utility component to support flipping of hinge chains (chains of HingeJoint objects) along with the parent skeleton.
	/// 
	/// Note: This component is automatically attached when calling "Create Hinge Chain" at <see cref="SkeletonUtilityBone"/>.
	/// </summary>
	[RequireComponent(typeof(Rigidbody2D))]
	public class FollowLocationRigidbody2D : MonoBehaviour {
	
		public Transform reference;
		public bool followFlippedX;
		Rigidbody2D ownRigidbody;

		private void Awake () {
			ownRigidbody = this.GetComponent<Rigidbody2D>();
		}

		void FixedUpdate () {
			if (followFlippedX) {
				ownRigidbody.rotation = ((-reference.rotation.eulerAngles.z + 270f) % 360f) - 90f;
			}
			else
				ownRigidbody.rotation = reference.rotation.eulerAngles.z;
			ownRigidbody.position = reference.position;
		}
	}
}

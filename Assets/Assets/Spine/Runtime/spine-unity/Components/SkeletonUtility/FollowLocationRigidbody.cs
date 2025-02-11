
using UnityEngine;

namespace Spine.Unity {

	/// <summary>
	/// Utility component to support flipping of hinge chains (chains of HingeJoint objects) along with the parent skeleton.
	/// 
	/// Note: This component is automatically attached when calling "Create Hinge Chain" at <see cref="SkeletonUtilityBone"/>.
	/// </summary>
	[RequireComponent(typeof(Rigidbody))]
	public class FollowLocationRigidbody : MonoBehaviour {
	
		public Transform reference;
		Rigidbody ownRigidbody;

		private void Awake () {
			ownRigidbody = this.GetComponent<Rigidbody>();
		}

		void FixedUpdate () {
			ownRigidbody.rotation = reference.rotation;
			ownRigidbody.position = reference.position;
		}
	}
}

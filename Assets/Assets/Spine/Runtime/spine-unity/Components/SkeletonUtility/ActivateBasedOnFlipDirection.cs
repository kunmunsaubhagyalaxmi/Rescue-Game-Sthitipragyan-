
using UnityEngine;

namespace Spine.Unity {

	/// <summary>
	/// Utility component to support flipping of 2D hinge chains (chains of HingeJoint2D objects) along
	/// with the parent skeleton by activating the respective mirrored versions of the hinge chain.
	/// Note: This component is automatically attached when calling "Create Hinge Chain 2D" at <see cref="SkeletonUtilityBone"/>,
	/// do not attempt to use this component for other purposes.
	/// </summary>
	public class ActivateBasedOnFlipDirection : MonoBehaviour {
	
		public SkeletonRenderer skeletonRenderer;
		public GameObject activeOnNormalX;
		public GameObject activeOnFlippedX;
		HingeJoint2D[] jointsNormalX;
		HingeJoint2D[] jointsFlippedX;

		bool wasFlippedXBefore = false;

		private void Start () {
			jointsNormalX = activeOnNormalX.GetComponentsInChildren<HingeJoint2D>();
			jointsFlippedX = activeOnFlippedX.GetComponentsInChildren<HingeJoint2D>();
		}

		private void FixedUpdate () {
			bool isFlippedX = (skeletonRenderer.Skeleton.ScaleX < 0);
			if (isFlippedX != wasFlippedXBefore) {
				HandleFlip(isFlippedX);
			}
			wasFlippedXBefore = isFlippedX;
		}

		void HandleFlip (bool isFlippedX) {
			GameObject gameObjectToActivate = isFlippedX ? activeOnFlippedX : activeOnNormalX;
			GameObject gameObjectToDeactivate = isFlippedX ? activeOnNormalX : activeOnFlippedX;

			gameObjectToActivate.SetActive(true);
			gameObjectToDeactivate.SetActive(false);

			ResetJointPositions(isFlippedX ? jointsFlippedX : jointsNormalX);
			ResetJointPositions(isFlippedX ? jointsNormalX : jointsFlippedX);
			CompensateMovementAfterFlipX(gameObjectToActivate.transform, gameObjectToDeactivate.transform);
		}

		void ResetJointPositions (HingeJoint2D[] joints) {
			for (int i = 0; i < joints.Length; ++i) {
				var joint = joints[i];
				var parent = joint.connectedBody.transform;
				joint.transform.position = parent.TransformPoint(joint.connectedAnchor);
			}
		}

		void CompensateMovementAfterFlipX (Transform toActivate, Transform toDeactivate) {
			Transform targetLocation = toDeactivate.GetChild(0);
			Transform currentLocation = toActivate.GetChild(0);
			toActivate.position += targetLocation.position - currentLocation.position;
		}
	}
}

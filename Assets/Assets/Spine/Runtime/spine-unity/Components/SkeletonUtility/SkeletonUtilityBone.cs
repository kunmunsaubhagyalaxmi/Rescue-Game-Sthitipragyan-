
#if UNITY_2018_3 || UNITY_2019 || UNITY_2018_3_OR_NEWER
#define NEW_PREFAB_SYSTEM
#endif

using UnityEngine;

namespace Spine.Unity {
	/// <summary>Sets a GameObject's transform to match a bone on a Spine skeleton.</summary>
	#if NEW_PREFAB_SYSTEM
	[ExecuteAlways]
	#else
	[ExecuteInEditMode]
	#endif
	[AddComponentMenu("Spine/SkeletonGameObjectsBone")]
	public class SkeletonUtilityBone : MonoBehaviour {
		public enum Mode {
			Follow,
			Override
		}

		public enum UpdatePhase {
			Local,
			World,
			Complete
		}

		#region Inspector
		/// <summary>If a bone isn't set, boneName is used to find the bone.</summary>
		public string boneName;
		public Transform parentReference;
		public Mode mode;
		public bool position, rotation, scale, zPosition = true;
		[Range(0f, 1f)]
		public float overrideAlpha = 1;
		#endregion

		public SkeletonUtility hierarchy;
		[System.NonSerialized] public Bone bone;
		[System.NonSerialized] public bool transformLerpComplete;
		[System.NonSerialized] public bool valid;
		Transform cachedTransform;
		Transform skeletonTransform;
		bool incompatibleTransformMode;
		public bool IncompatibleTransformMode { get { return incompatibleTransformMode; } }

		public void Reset () {
			bone = null;
			cachedTransform = transform;
			valid = hierarchy != null && hierarchy.skeletonRenderer != null && hierarchy.skeletonRenderer.valid;
			if (!valid)
				return;
			skeletonTransform = hierarchy.transform;
			hierarchy.OnReset -= HandleOnReset;
			hierarchy.OnReset += HandleOnReset;
			DoUpdate(UpdatePhase.Local);
		}

		void OnEnable () {
			if (hierarchy == null) hierarchy = transform.GetComponentInParent<SkeletonUtility>();
			if (hierarchy == null) return;

			hierarchy.RegisterBone(this);
			hierarchy.OnReset += HandleOnReset;
		}

		void HandleOnReset () {
			Reset();
		}

		void OnDisable () {
			if (hierarchy != null) {
				hierarchy.OnReset -= HandleOnReset;
				hierarchy.UnregisterBone(this);
			}
		}

		public void DoUpdate (UpdatePhase phase) {
			if (!valid) {
				Reset();
				return;
			}

			var skeleton = hierarchy.skeletonRenderer.skeleton;

			if (bone == null) {
				if (string.IsNullOrEmpty(boneName)) return;
				bone = skeleton.FindBone(boneName);
				if (bone == null) {
					Debug.LogError("Bone not found: " + boneName, this);
					return;
				}
			}
			if (!bone.Active) return;

			var thisTransform = cachedTransform;
			float skeletonFlipRotation = Mathf.Sign(skeleton.ScaleX * skeleton.ScaleY);
			if (mode == Mode.Follow) {
				switch (phase) {
					case UpdatePhase.Local:
						if (position)
							thisTransform.localPosition = new Vector3(bone.x, bone.y, 0);

						if (rotation) {
							if (bone.data.transformMode.InheritsRotation()) {
								thisTransform.localRotation = Quaternion.Euler(0, 0, bone.rotation);
							} else {
								Vector3 euler = skeletonTransform.rotation.eulerAngles;
								thisTransform.rotation = Quaternion.Euler(euler.x, euler.y, euler.z + (bone.WorldRotationX * skeletonFlipRotation));
							}
						}

						if (scale) {
							thisTransform.localScale = new Vector3(bone.scaleX, bone.scaleY, 1f);
							incompatibleTransformMode = BoneTransformModeIncompatible(bone);
						}
						break;
					case UpdatePhase.World:
					case UpdatePhase.Complete:
						// Use Applied transform values (ax, ay, AppliedRotation, ascale) if world values were modified by constraints.
						if (!bone.appliedValid) {
							bone.UpdateAppliedTransform();
						}

						if (position)
							thisTransform.localPosition = new Vector3(bone.ax, bone.ay, 0);

						if (rotation) {
							if (bone.data.transformMode.InheritsRotation()) {
								thisTransform.localRotation = Quaternion.Euler(0, 0, bone.AppliedRotation);
							} else {
								Vector3 euler = skeletonTransform.rotation.eulerAngles;
								thisTransform.rotation = Quaternion.Euler(euler.x, euler.y, euler.z + (bone.WorldRotationX * skeletonFlipRotation));
							}
						}

						if (scale) {
							thisTransform.localScale = new Vector3(bone.ascaleX, bone.ascaleY, 1f);
							incompatibleTransformMode = BoneTransformModeIncompatible(bone);
						}
						break;
				}

			} else if (mode == Mode.Override) {
				if (transformLerpComplete)
					return;

				if (parentReference == null) {
					if (position) {
						Vector3 clp = thisTransform.localPosition;
						bone.x = Mathf.Lerp(bone.x, clp.x, overrideAlpha);
						bone.y = Mathf.Lerp(bone.y, clp.y, overrideAlpha);
					}

					if (rotation) {
						float angle = Mathf.LerpAngle(bone.Rotation, thisTransform.localRotation.eulerAngles.z, overrideAlpha);
						bone.Rotation = angle;
						bone.AppliedRotation = angle;
					}

					if (scale) {
						Vector3 cls = thisTransform.localScale;
						bone.scaleX = Mathf.Lerp(bone.scaleX, cls.x, overrideAlpha);
						bone.scaleY = Mathf.Lerp(bone.scaleY, cls.y, overrideAlpha);
					}

				} else {
					if (transformLerpComplete)
						return;

					if (position) {
						Vector3 pos = parentReference.InverseTransformPoint(thisTransform.position);
						bone.x = Mathf.Lerp(bone.x, pos.x, overrideAlpha);
						bone.y = Mathf.Lerp(bone.y, pos.y, overrideAlpha);
					}

					if (rotation) {
						float angle = Mathf.LerpAngle(bone.Rotation, Quaternion.LookRotation(Vector3.forward, parentReference.InverseTransformDirection(thisTransform.up)).eulerAngles.z, overrideAlpha);
						bone.Rotation = angle;
						bone.AppliedRotation = angle;
					}

					if (scale) {
						Vector3 cls = thisTransform.localScale;
						bone.scaleX = Mathf.Lerp(bone.scaleX, cls.x, overrideAlpha);
						bone.scaleY = Mathf.Lerp(bone.scaleY, cls.y, overrideAlpha);
					}

					incompatibleTransformMode = BoneTransformModeIncompatible(bone);
				}

				transformLerpComplete = true;
			}
		}

		public static bool BoneTransformModeIncompatible (Bone bone) {
			return !bone.data.transformMode.InheritsScale();
		}

		public void AddBoundingBox (string skinName, string slotName, string attachmentName) {
			SkeletonUtility.AddBoneRigidbody2D(transform.gameObject);
			SkeletonUtility.AddBoundingBoxGameObject(bone.skeleton, skinName, slotName, attachmentName, transform);
		}

#if UNITY_EDITOR
		void OnDrawGizmos () {
			if (IncompatibleTransformMode)
				Gizmos.DrawIcon(transform.position + new Vector3(0, 0.128f, 0), "icon-warning");
		}
		#endif
	}
}

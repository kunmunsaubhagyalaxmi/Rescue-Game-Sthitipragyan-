
using UnityEditor;
using UnityEngine;

namespace Spine.Unity.Editor {

	using Editor = UnityEditor.Editor;
	using Event = UnityEngine.Event;

	[CustomEditor(typeof(BoneFollower)), CanEditMultipleObjects]
	public class BoneFollowerInspector : Editor {
		SerializedProperty boneName, skeletonRenderer, followXYPosition, followZPosition, followBoneRotation, followLocalScale, followSkeletonFlip;
		BoneFollower targetBoneFollower;
		bool needsReset;

		#region Context Menu Item
		[MenuItem ("CONTEXT/SkeletonRenderer/Add BoneFollower GameObject")]
		static void AddBoneFollowerGameObject (MenuCommand cmd) {
			var skeletonRenderer = cmd.context as SkeletonRenderer;
			var go = EditorInstantiation.NewGameObject("New BoneFollower");
			var t = go.transform;
			t.SetParent(skeletonRenderer.transform);
			t.localPosition = Vector3.zero;

			var f = go.AddComponent<BoneFollower>();
			f.skeletonRenderer = skeletonRenderer;

			EditorGUIUtility.PingObject(t);

			Undo.RegisterCreatedObjectUndo(go, "Add BoneFollower");
		}

		// Validate
		[MenuItem ("CONTEXT/SkeletonRenderer/Add BoneFollower GameObject", true)]
		static bool ValidateAddBoneFollowerGameObject (MenuCommand cmd) {
			var skeletonRenderer = cmd.context as SkeletonRenderer;
			return skeletonRenderer.valid;
		}

		[MenuItem("CONTEXT/BoneFollower/Rename BoneFollower GameObject")]
		static void RenameGameObject (MenuCommand cmd) {
			AutonameGameObject(cmd.context as BoneFollower);
		}
		#endregion

		static void AutonameGameObject (BoneFollower boneFollower) {
			if (boneFollower == null) return;

			string boneName = boneFollower.boneName;
			boneFollower.gameObject.name = string.IsNullOrEmpty(boneName) ? "BoneFollower" : string.Format("{0} (BoneFollower)", boneName);
		}

		void OnEnable () {
			skeletonRenderer = serializedObject.FindProperty("skeletonRenderer");
			boneName = serializedObject.FindProperty("boneName");
			followBoneRotation = serializedObject.FindProperty("followBoneRotation");
			followXYPosition = serializedObject.FindProperty("followXYPosition");
			followZPosition = serializedObject.FindProperty("followZPosition");
			followLocalScale = serializedObject.FindProperty("followLocalScale");
			followSkeletonFlip = serializedObject.FindProperty("followSkeletonFlip");

			targetBoneFollower = (BoneFollower)target;
			if (targetBoneFollower.SkeletonRenderer != null)
				targetBoneFollower.SkeletonRenderer.Initialize(false);

			if (!targetBoneFollower.valid || needsReset) {
				targetBoneFollower.Initialize();
				targetBoneFollower.LateUpdate();
				needsReset = false;
				SceneView.RepaintAll();
			}
		}

		public void OnSceneGUI () {
			var tbf = target as BoneFollower;
			var skeletonRendererComponent = tbf.skeletonRenderer;
			if (skeletonRendererComponent == null) return;

			var transform = skeletonRendererComponent.transform;
			var skeleton = skeletonRendererComponent.skeleton;

			if (string.IsNullOrEmpty(boneName.stringValue)) {
				SpineHandles.DrawBones(transform, skeleton);
				SpineHandles.DrawBoneNames(transform, skeleton);
				Handles.Label(tbf.transform.position, "No bone selected", EditorStyles.helpBox);
			} else {
				var targetBone = tbf.bone;
				if (targetBone == null) return;
				SpineHandles.DrawBoneWireframe(transform, targetBone, SpineHandles.TransformContraintColor);
				Handles.Label(targetBone.GetWorldPosition(transform), targetBone.Data.Name, SpineHandles.BoneNameStyle);
			}
		}

		override public void OnInspectorGUI () {
			if (serializedObject.isEditingMultipleObjects) {
				if (needsReset) {
					needsReset = false;
					foreach (var o in targets) {
						var bf = (BoneFollower)o;
						bf.Initialize();
						bf.LateUpdate();
					}
					SceneView.RepaintAll();
				}

				EditorGUI.BeginChangeCheck();
				DrawDefaultInspector();
				needsReset |= EditorGUI.EndChangeCheck();
				return;
			}

			if (needsReset && Event.current.type == EventType.Layout) {
				targetBoneFollower.Initialize();
				targetBoneFollower.LateUpdate();
				needsReset = false;
				SceneView.RepaintAll();
			}
			serializedObject.Update();

			// Find Renderer
			if (skeletonRenderer.objectReferenceValue == null) {
				SkeletonRenderer parentRenderer = targetBoneFollower.GetComponentInParent<SkeletonRenderer>();
				if (parentRenderer != null && parentRenderer.gameObject != targetBoneFollower.gameObject) {
					skeletonRenderer.objectReferenceValue = parentRenderer;
					Debug.Log("Inspector automatically assigned BoneFollower.SkeletonRenderer");
				}
			}

			EditorGUILayout.PropertyField(skeletonRenderer);
			var skeletonRendererReference = skeletonRenderer.objectReferenceValue as SkeletonRenderer;
			if (skeletonRendererReference != null) {
				if (skeletonRendererReference.gameObject == targetBoneFollower.gameObject) {
					skeletonRenderer.objectReferenceValue = null;
					EditorUtility.DisplayDialog("Invalid assignment.", "BoneFollower can only follow a skeleton on a separate GameObject.\n\nCreate a new GameObject for your BoneFollower, or choose a SkeletonRenderer from a different GameObject.", "Ok");
				}
			}

			if (!targetBoneFollower.valid) {
				needsReset = true;
			}

			if (targetBoneFollower.valid) {
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(boneName);
				needsReset |= EditorGUI.EndChangeCheck();

				EditorGUILayout.PropertyField(followBoneRotation);
				EditorGUILayout.PropertyField(followXYPosition);
				EditorGUILayout.PropertyField(followZPosition);
				EditorGUILayout.PropertyField(followLocalScale);
				EditorGUILayout.PropertyField(followSkeletonFlip);

				BoneFollowerInspector.RecommendRigidbodyButton(targetBoneFollower);
			} else {
				var boneFollowerSkeletonRenderer = targetBoneFollower.skeletonRenderer;
				if (boneFollowerSkeletonRenderer == null) {
					EditorGUILayout.HelpBox("SkeletonRenderer is unassigned. Please assign a SkeletonRenderer (SkeletonAnimation or SkeletonMecanim).", MessageType.Warning);
				} else {
					boneFollowerSkeletonRenderer.Initialize(false);

					if (boneFollowerSkeletonRenderer.skeletonDataAsset == null)
						EditorGUILayout.HelpBox("Assigned SkeletonRenderer does not have SkeletonData assigned to it.", MessageType.Warning);

					if (!boneFollowerSkeletonRenderer.valid)
						EditorGUILayout.HelpBox("Assigned SkeletonRenderer is invalid. Check target SkeletonRenderer, its SkeletonDataAsset or the console for other errors.", MessageType.Warning);
				}
			}

			var current = Event.current;
			bool wasUndo = (current.type == EventType.ValidateCommand && current.commandName == "UndoRedoPerformed");
			if (wasUndo)
				targetBoneFollower.Initialize();

			serializedObject.ApplyModifiedProperties();
		}

		internal static void RecommendRigidbodyButton (Component component) {
			bool hasCollider2D = component.GetComponent<Collider2D>() != null || component.GetComponent<BoundingBoxFollower>() != null;
			bool hasCollider3D = !hasCollider2D && component.GetComponent<Collider>();
			bool missingRigidBody = (hasCollider2D && component.GetComponent<Rigidbody2D>() == null) || (hasCollider3D && component.GetComponent<Rigidbody>() == null);
			if (missingRigidBody) {
				using (new SpineInspectorUtility.BoxScope()) {
					EditorGUILayout.HelpBox("Collider detected. Unity recommends adding a Rigidbody to the Transforms of any colliders that are intended to be dynamically repositioned and rotated.", MessageType.Warning);
					var rbType = hasCollider2D ? typeof(Rigidbody2D) : typeof(Rigidbody);
					string rbLabel = string.Format("Add {0}", rbType.Name);
					var rbContent = SpineInspectorUtility.TempContent(rbLabel, SpineInspectorUtility.UnityIcon(rbType), "Add a rigidbody to this GameObject to be the Physics body parent of the attached collider.");
					if (SpineInspectorUtility.CenteredButton(rbContent)) component.gameObject.AddComponent(rbType);
				}
			}
		}
	}

}

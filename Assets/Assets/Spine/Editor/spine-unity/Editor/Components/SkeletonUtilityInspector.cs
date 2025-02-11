
#if UNITY_2018_3 || UNITY_2019 || UNITY_2018_3_OR_NEWER
#define NEW_PREFAB_SYSTEM
#endif

using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;
using Spine;
using System.Reflection;

namespace Spine.Unity.Editor {
	using Icons = SpineEditorUtilities.Icons;

	[CustomEditor(typeof(SkeletonUtility))]
	public class SkeletonUtilityInspector : UnityEditor.Editor {

		SkeletonUtility skeletonUtility;
		Skeleton skeleton;
		SkeletonRenderer skeletonRenderer;

		#if !NEW_PREFAB_SYSTEM
		bool isPrefab;
		#endif

		readonly GUIContent SpawnHierarchyButtonLabel = new GUIContent("Spawn Hierarchy", Icons.skeleton);

		void OnEnable () {
			skeletonUtility = (SkeletonUtility)target;
			skeletonRenderer = skeletonUtility.GetComponent<SkeletonRenderer>();
			skeleton = skeletonRenderer.Skeleton;

			if (skeleton == null) {
				skeletonRenderer.Initialize(false);
				skeletonRenderer.LateUpdate();
				skeleton = skeletonRenderer.skeleton;
			}

			if (!skeletonRenderer.valid) return;

			#if !NEW_PREFAB_SYSTEM
			isPrefab |= PrefabUtility.GetPrefabType(this.target) == PrefabType.Prefab;
			#endif
		}

		public override void OnInspectorGUI () {

			#if !NEW_PREFAB_SYSTEM
			if (isPrefab) {
				GUILayout.Label(new GUIContent("Cannot edit Prefabs", Icons.warning));
				return;
			}
			#endif

			serializedObject.Update();

			if (!skeletonRenderer.valid) {
				GUILayout.Label(new GUIContent("Spine Component invalid. Check Skeleton Data Asset.", Icons.warning));
				return;
			}

			EditorGUILayout.PropertyField(serializedObject.FindProperty("boneRoot"), SpineInspectorUtility.TempContent("Skeleton Root"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("flipBy180DegreeRotation"), SpineInspectorUtility.TempContent("Flip by Rotation", null,
				"If true, Skeleton.ScaleX and Skeleton.ScaleY are followed " +
				"by 180 degree rotation. If false, negative Transform scale is used. " +
				"Note that using negative scale is consistent with previous behaviour (hence the default), " +
				"however causes serious problems with rigidbodies and physics. Therefore, it is recommended to " +
				"enable this parameter where possible. When creating hinge chains for a chain of skeleton bones " +
				"via SkeletonUtilityBone, it is mandatory to have this parameter enabled."));

			bool hasRootBone = skeletonUtility.boneRoot != null;

			if (!hasRootBone)
				EditorGUILayout.HelpBox("No hierarchy found. Use Spawn Hierarchy to generate GameObjects for bones.", MessageType.Info);

			using (new EditorGUI.DisabledGroupScope(hasRootBone)) {
				if (SpineInspectorUtility.LargeCenteredButton(SpawnHierarchyButtonLabel))
					SpawnHierarchyContextMenu();
			}

			if (hasRootBone) {
				if (SpineInspectorUtility.CenteredButton(new GUIContent("Remove Hierarchy"))) {
					Undo.RegisterCompleteObjectUndo(skeletonUtility, "Remove Hierarchy");
					Undo.DestroyObjectImmediate(skeletonUtility.boneRoot.gameObject);
					skeletonUtility.boneRoot = null;
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		void SpawnHierarchyContextMenu () {
			var menu = new GenericMenu();

			menu.AddItem(new GUIContent("Follow all bones"), false, SpawnFollowHierarchy);
			menu.AddItem(new GUIContent("Follow (Root Only)"), false, SpawnFollowHierarchyRootOnly);
			menu.AddSeparator("");
			menu.AddItem(new GUIContent("Override all bones"), false, SpawnOverrideHierarchy);
			menu.AddItem(new GUIContent("Override (Root Only)"), false, SpawnOverrideHierarchyRootOnly);

			menu.ShowAsContext();
		}

		public static void AttachIcon (SkeletonUtilityBone boneComponent) {
			Skeleton skeleton = boneComponent.hierarchy.skeletonRenderer.skeleton;
			Texture2D icon = boneComponent.bone.Data.Length == 0 ? Icons.nullBone : Icons.boneNib;

			foreach (IkConstraint c in skeleton.IkConstraints)
				if (c.Target == boneComponent.bone) {
					icon = Icons.constraintNib;
					break;
				}

			typeof(EditorGUIUtility).InvokeMember("SetIconForObject", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic, null, null, new object[2] {
				boneComponent.gameObject,
				icon
			});
		}

		static void AttachIconsToChildren (Transform root) {
			if (root != null) {
				var utilityBones = root.GetComponentsInChildren<SkeletonUtilityBone>();
				foreach (var utilBone in utilityBones)
					AttachIcon(utilBone);
			}
		}

		void SpawnFollowHierarchy () {
			Undo.RegisterCompleteObjectUndo(skeletonUtility, "Spawn Hierarchy");
			Selection.activeGameObject = skeletonUtility.SpawnHierarchy(SkeletonUtilityBone.Mode.Follow, true, true, true);
			AttachIconsToChildren(skeletonUtility.boneRoot);
		}

		void SpawnFollowHierarchyRootOnly () {
			Undo.RegisterCompleteObjectUndo(skeletonUtility, "Spawn Root");
			Selection.activeGameObject = skeletonUtility.SpawnRoot(SkeletonUtilityBone.Mode.Follow, true, true, true);
			AttachIconsToChildren(skeletonUtility.boneRoot);
		}

		void SpawnOverrideHierarchy () {
			Undo.RegisterCompleteObjectUndo(skeletonUtility, "Spawn Hierarchy");
			Selection.activeGameObject = skeletonUtility.SpawnHierarchy(SkeletonUtilityBone.Mode.Override, true, true, true);
			AttachIconsToChildren(skeletonUtility.boneRoot);
		}

		void SpawnOverrideHierarchyRootOnly () {
			Undo.RegisterCompleteObjectUndo(skeletonUtility, "Spawn Root");
			Selection.activeGameObject = skeletonUtility.SpawnRoot(SkeletonUtilityBone.Mode.Override, true, true, true);
			AttachIconsToChildren(skeletonUtility.boneRoot);
		}
	}

}

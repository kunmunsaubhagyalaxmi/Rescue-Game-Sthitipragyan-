
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.Reflection;

namespace Spine.Unity.Editor {
	using Editor = UnityEditor.Editor;

	[CustomEditor(typeof(AnimationReferenceAsset))]
	public class AnimationReferenceAssetEditor : Editor {

		const string InspectorHelpText = "This is a Spine-Unity Animation Reference Asset. It serializes a reference to a SkeletonDataAsset and an animationName. It does not contain actual animation data. At runtime, it stores a reference to a Spine.Animation.\n\n" +
				"You can use this in your AnimationState calls instead of a string animation name or a Spine.Animation reference. Use its implicit conversion into Spine.Animation or its .Animation property.\n\n" +
				"Use AnimationReferenceAssets as an alternative to storing strings or finding animations and caching per component. This only does the lookup by string once, and allows you to store and manage animations via asset references.";

		readonly SkeletonInspectorPreview preview = new SkeletonInspectorPreview();
		FieldInfo skeletonDataAssetField = typeof(AnimationReferenceAsset).GetField("skeletonDataAsset", BindingFlags.NonPublic | BindingFlags.Instance);
		FieldInfo nameField = typeof(AnimationReferenceAsset).GetField("animationName", BindingFlags.NonPublic | BindingFlags.Instance);

		AnimationReferenceAsset ThisAnimationReferenceAsset { get { return target as AnimationReferenceAsset; } }
		SkeletonDataAsset ThisSkeletonDataAsset { get { return skeletonDataAssetField.GetValue(ThisAnimationReferenceAsset) as SkeletonDataAsset; } }
		string ThisAnimationName { get { return nameField.GetValue(ThisAnimationReferenceAsset) as string; } }

		bool changeNextFrame = false;
		SerializedProperty animationNameProperty;
		SkeletonDataAsset lastSkeletonDataAsset;
		SkeletonData lastSkeletonData;

		void OnEnable () { HandleOnEnablePreview(); }
		void OnDestroy () { HandleOnDestroyPreview(); }

		public override void OnInspectorGUI () {
			animationNameProperty = animationNameProperty ?? serializedObject.FindProperty("animationName");
			string animationName = animationNameProperty.stringValue;

			Animation animation = null;
			if (ThisSkeletonDataAsset != null) {
				var skeletonData = ThisSkeletonDataAsset.GetSkeletonData(true);
				if (skeletonData != null) {
					animation = skeletonData.FindAnimation(animationName);
				}
			}
			bool animationNotFound = (animation == null);

			if (changeNextFrame) {
				changeNextFrame = false;

				if (ThisSkeletonDataAsset != lastSkeletonDataAsset || ThisSkeletonDataAsset.GetSkeletonData(true) != lastSkeletonData) {
					preview.Clear();
					preview.Initialize(Repaint, ThisSkeletonDataAsset, LastSkinName);

					if (animationNotFound) {
						animationNameProperty.stringValue = "";
						preview.ClearAnimationSetupPose();
					}
				}

				preview.ClearAnimationSetupPose();

				if (!string.IsNullOrEmpty(animationNameProperty.stringValue))
					preview.PlayPauseAnimation(animationNameProperty.stringValue, true);
			}

			lastSkeletonDataAsset = ThisSkeletonDataAsset;
			lastSkeletonData = ThisSkeletonDataAsset.GetSkeletonData(true);

			//EditorGUILayout.HelpBox(AnimationReferenceAssetEditor.InspectorHelpText, MessageType.Info, true);
			EditorGUILayout.Space();
			EditorGUI.BeginChangeCheck();
			DrawDefaultInspector();
			if (EditorGUI.EndChangeCheck()) {
				changeNextFrame = true;
			}

			// Draw extra info below default inspector.
			EditorGUILayout.Space();
			if (ThisSkeletonDataAsset == null) {
				EditorGUILayout.HelpBox("SkeletonDataAsset is missing.", MessageType.Error);
			} else if (string.IsNullOrEmpty(animationName)) {
				EditorGUILayout.HelpBox("No animation selected.", MessageType.Warning);
			} else if (animationNotFound) {
				EditorGUILayout.HelpBox(string.Format("Animation named {0} was not found for this Skeleton.", animationNameProperty.stringValue), MessageType.Warning);
			} else {
				using (new SpineInspectorUtility.BoxScope()) {
					if (!string.Equals(AssetUtility.GetPathSafeName(animationName), ThisAnimationReferenceAsset.name, System.StringComparison.OrdinalIgnoreCase))
						EditorGUILayout.HelpBox("Animation name value does not match this asset's name. Inspectors using this asset may be misleading.", MessageType.None);

					EditorGUILayout.LabelField(SpineInspectorUtility.TempContent(animationName, SpineEditorUtilities.Icons.animation));
					if (animation != null) {
						EditorGUILayout.LabelField(string.Format("Timelines: {0}", animation.Timelines.Count));
						EditorGUILayout.LabelField(string.Format("Duration: {0} sec", animation.Duration));
					}
				}
			}
		}

		#region Preview Handlers
		string TargetAssetGUID { get { return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(ThisSkeletonDataAsset)); } }
		string LastSkinKey { get { return TargetAssetGUID + "_lastSkin"; } }
		string LastSkinName { get { return EditorPrefs.GetString(LastSkinKey, ""); } }

		void HandleOnEnablePreview () {
			if (ThisSkeletonDataAsset != null && ThisSkeletonDataAsset.skeletonJSON == null)
				return;

			preview.Initialize(this.Repaint, ThisSkeletonDataAsset, LastSkinName);
			preview.PlayPauseAnimation(ThisAnimationName, true);
			preview.OnSkinChanged -= HandleOnSkinChanged;
			preview.OnSkinChanged += HandleOnSkinChanged;
			EditorApplication.update -= preview.HandleEditorUpdate;
			EditorApplication.update += preview.HandleEditorUpdate;
		}

		private void HandleOnSkinChanged (string skinName) {
			EditorPrefs.SetString(LastSkinKey, skinName);
			preview.PlayPauseAnimation(ThisAnimationName, true);
		}

		void HandleOnDestroyPreview () {
			EditorApplication.update -= preview.HandleEditorUpdate;
			preview.OnDestroy();
		}

		override public bool HasPreviewGUI () {
			if (serializedObject.isEditingMultipleObjects) return false;
			return ThisSkeletonDataAsset != null && ThisSkeletonDataAsset.GetSkeletonData(true) != null;
		}

		override public void OnInteractivePreviewGUI (Rect r, GUIStyle background) {
			preview.Initialize(this.Repaint, ThisSkeletonDataAsset);
			preview.HandleInteractivePreviewGUI(r, background);
		}

		public override GUIContent GetPreviewTitle () { return SpineInspectorUtility.TempContent("Preview"); }
		public override void OnPreviewSettings () { preview.HandleDrawSettings(); }
		public override Texture2D RenderStaticPreview (string assetPath, UnityEngine.Object[] subAssets, int width, int height) { return preview.GetStaticPreview(width, height); }
		#endregion
	}

}


using UnityEditor;
using UnityEngine;
using Spine;

namespace Spine.Unity.Editor {

	[CustomEditor(typeof(SkeletonAnimation))]
	[CanEditMultipleObjects]
	public class SkeletonAnimationInspector : SkeletonRendererInspector {
		protected SerializedProperty animationName, loop, timeScale, autoReset;
		protected bool wasAnimationNameChanged;
		protected bool requireRepaint;
		readonly GUIContent LoopLabel = new GUIContent("Loop", "Whether or not .AnimationName should loop. This only applies to the initial animation specified in the inspector, or any subsequent Animations played through .AnimationName. Animations set through state.SetAnimation are unaffected.");
		readonly GUIContent TimeScaleLabel = new GUIContent("Time Scale", "The rate at which animations progress over time. 1 means normal speed. 0.5 means 50% speed.");

		protected override void OnEnable () {
			base.OnEnable();
			animationName = serializedObject.FindProperty("_animationName");
			loop = serializedObject.FindProperty("loop");
			timeScale = serializedObject.FindProperty("timeScale");
		}

		protected override void DrawInspectorGUI (bool multi) {
			base.DrawInspectorGUI(multi);
			if (!TargetIsValid) return;
			bool sameData = SpineInspectorUtility.TargetsUseSameData(serializedObject);

			foreach (var o in targets)
				TrySetAnimation(o as SkeletonAnimation, multi);
			wasAnimationNameChanged = false;

			EditorGUILayout.Space();
			if (!sameData) {
				EditorGUILayout.DelayedTextField(animationName);
			} else {
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(animationName);
				wasAnimationNameChanged |= EditorGUI.EndChangeCheck(); // Value used in the next update.
			}
			EditorGUILayout.PropertyField(loop, LoopLabel);
			EditorGUILayout.PropertyField(timeScale, TimeScaleLabel);
			foreach (var o in targets) {
				var component = o as SkeletonAnimation;
				component.timeScale = Mathf.Max(component.timeScale, 0);
			}
			EditorGUILayout.Space();

			if (!isInspectingPrefab) {
				if (requireRepaint) {
					SceneView.RepaintAll();
					requireRepaint = false;
				}
			}
		}

		protected void TrySetAnimation (SkeletonAnimation skeletonAnimation, bool multi) {
			if (skeletonAnimation == null) return;
			if (!skeletonAnimation.valid)
				return;

			if (!isInspectingPrefab) {
				if (wasAnimationNameChanged) {
					var skeleton = skeletonAnimation.Skeleton;
					var state = skeletonAnimation.AnimationState;

					if (!Application.isPlaying) {
						if (state != null) state.ClearTrack(0);
						skeleton.SetToSetupPose();
					}

					Spine.Animation animationToUse = skeleton.Data.FindAnimation(animationName.stringValue);

					if (!Application.isPlaying) {
						if (animationToUse != null) {
							skeletonAnimation.AnimationState.SetAnimation(0, animationToUse, loop.boolValue);
						}
						skeleton.UpdateWorldTransform();
						skeletonAnimation.LateUpdate();
						requireRepaint = true;
					} else {
						if (animationToUse != null)
							state.SetAnimation(0, animationToUse, loop.boolValue);
						else
							state.ClearTrack(0);
					}
				}

				// Reflect animationName serialized property in the inspector even if SetAnimation API was used.
				if (Application.isPlaying) {
					TrackEntry current = skeletonAnimation.AnimationState.GetCurrent(0);
					if (current != null) {
						if (skeletonAnimation.AnimationName != animationName.stringValue)
							animationName.stringValue = current.Animation.Name;
					}
				}
			}
		}
	}
}

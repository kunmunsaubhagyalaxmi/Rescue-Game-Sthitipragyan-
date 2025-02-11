
using UnityEngine;
using UnityEditor;
using Spine.Unity.Editor;

namespace Spine.Unity.Examples {
	[CustomEditor(typeof(SkeletonPartsRenderer))]
	public class SkeletonRenderPartInspector : UnityEditor.Editor {
		SpineInspectorUtility.SerializedSortingProperties sortingProperties;

		void OnEnable () {
			sortingProperties = new SpineInspectorUtility.SerializedSortingProperties(SpineInspectorUtility.GetRenderersSerializedObject(serializedObject));
		}

		public override void OnInspectorGUI () {
			SpineInspectorUtility.SortingPropertyFields(sortingProperties, true);

			if (!serializedObject.isEditingMultipleObjects) {
				EditorGUILayout.Space();
				if (SpineInspectorUtility.LargeCenteredButton(new GUIContent("Select SkeletonRenderer", SpineEditorUtilities.Icons.spine))) {
					var thisSkeletonPartsRenderer = target as SkeletonPartsRenderer;
					var srs = thisSkeletonPartsRenderer.GetComponentInParent<SkeletonRenderSeparator>();
					if (srs != null && srs.partsRenderers.Contains(thisSkeletonPartsRenderer) && srs.SkeletonRenderer != null)
						Selection.activeGameObject = srs.SkeletonRenderer.gameObject;
				}
			}
		}
	}

}

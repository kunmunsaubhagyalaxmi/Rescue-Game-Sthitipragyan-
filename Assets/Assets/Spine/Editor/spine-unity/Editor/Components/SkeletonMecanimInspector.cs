
// Contributed by: Mitch Thompson

using UnityEditor;

namespace Spine.Unity.Editor {
	[CustomEditor(typeof(SkeletonMecanim))]
	[CanEditMultipleObjects]
	public class SkeletonMecanimInspector : SkeletonRendererInspector {
		protected SerializedProperty mecanimTranslator;

		protected override void OnEnable () {
			base.OnEnable();
			mecanimTranslator = serializedObject.FindProperty("translator");
		}

		protected override void DrawInspectorGUI (bool multi) {
			base.DrawInspectorGUI(multi);
			EditorGUILayout.PropertyField(mecanimTranslator, true);
		}
	}
}

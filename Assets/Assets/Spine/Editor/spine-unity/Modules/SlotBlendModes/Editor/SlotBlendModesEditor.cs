
using UnityEngine;
using UnityEditor;
using Spine.Unity.Deprecated;
using System;

namespace Spine.Unity.Editor {
	using Editor = UnityEditor.Editor;

	[Obsolete("The spine-unity 3.7 runtime introduced SkeletonDataModifierAssets BlendModeMaterials which replaced SlotBlendModes. Will be removed in spine-unity 3.9.", false)]
	public class SlotBlendModesEditor : Editor {

		[MenuItem("CONTEXT/SkeletonRenderer/Add Slot Blend Modes Component")]
		static void AddSlotBlendModesComponent (MenuCommand command) {
			var skeletonRenderer = (SkeletonRenderer)command.context;
			skeletonRenderer.gameObject.AddComponent<SlotBlendModes>();
		}
	}
}


using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Spine.Unity.Editor {
	public static class Menus {
		[MenuItem("GameObject/Spine/SkeletonRenderer", false, 10)]
		static public void CreateSkeletonRendererGameObject () {
			EditorInstantiation.InstantiateEmptySpineGameObject<SkeletonRenderer>("New SkeletonRenderer");
		}

		[MenuItem("GameObject/Spine/SkeletonAnimation", false, 10)]
		static public void CreateSkeletonAnimationGameObject () {
			EditorInstantiation.InstantiateEmptySpineGameObject<SkeletonAnimation>("New SkeletonAnimation");
		}
	}
}

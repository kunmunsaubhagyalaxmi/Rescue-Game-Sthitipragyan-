
#pragma warning disable 0219

#define SPINE_SKELETONMECANIM

#if UNITY_2017_2_OR_NEWER
#define NEWPLAYMODECALLBACKS
#endif

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Globalization;

namespace Spine.Unity.Editor {

	public partial class SpineEditorUtilities {
		public static class DataReloadHandler {

			internal static Dictionary<int, string> savedSkeletonDataAssetAtSKeletonGraphicID = new Dictionary<int, string>();

		#if NEWPLAYMODECALLBACKS
			internal static void OnPlaymodeStateChanged (PlayModeStateChange stateChange) {
		#else
			internal static void OnPlaymodeStateChanged () {
		#endif
				ReloadAllActiveSkeletonsEditMode();
			}

			public static void ReloadAllActiveSkeletonsEditMode () {

				if (EditorApplication.isPaused) return;
				if (EditorApplication.isPlaying) return;
				if (EditorApplication.isCompiling) return;
				if (EditorApplication.isPlayingOrWillChangePlaymode) return;

				var skeletonDataAssetsToReload = new HashSet<SkeletonDataAsset>();

				var activeSkeletonRenderers = GameObject.FindObjectsOfType<SkeletonRenderer>();
				foreach (var sr in activeSkeletonRenderers) {
					var skeletonDataAsset = sr.skeletonDataAsset;
					if (skeletonDataAsset != null) skeletonDataAssetsToReload.Add(skeletonDataAsset);
				}

				// Under some circumstances (e.g. on first import) SkeletonGraphic objects
				// have their skeletonGraphic.skeletonDataAsset reference corrupted
				// by the instance of the ScriptableObject being destroyed but still assigned.
				// Here we save the skeletonGraphic.skeletonDataAsset asset path in order
				// to restore it later.
				var activeSkeletonGraphics = GameObject.FindObjectsOfType<SkeletonGraphic>();
				foreach (var sg in activeSkeletonGraphics) {
					var skeletonDataAsset = sg.skeletonDataAsset;
					if (skeletonDataAsset != null) {
						var assetPath = AssetDatabase.GetAssetPath(skeletonDataAsset);
						var sgID = sg.GetInstanceID();
						savedSkeletonDataAssetAtSKeletonGraphicID[sgID] = assetPath;
						skeletonDataAssetsToReload.Add(skeletonDataAsset);
					}
				}

				foreach (var sda in skeletonDataAssetsToReload) {
					sda.Clear();
					sda.GetSkeletonData(true);
				}

				foreach (var sr in activeSkeletonRenderers) {
					var meshRenderer = sr.GetComponent<MeshRenderer>();
					var sharedMaterials = meshRenderer.sharedMaterials;
					foreach (var m in sharedMaterials) {
						if (m == null) {
							sr.Initialize(true);
							break;
						}
					}
				}

				foreach (var sg in activeSkeletonGraphics) {
					if (sg.mainTexture == null)
						sg.Initialize(true);
				}
			}

			public static void ReloadSceneSkeletonComponents (SkeletonDataAsset skeletonDataAsset) {
				if (EditorApplication.isPaused) return;
				if (EditorApplication.isPlaying) return;
				if (EditorApplication.isCompiling) return;
				if (EditorApplication.isPlayingOrWillChangePlaymode) return;

				var activeSkeletonRenderers = GameObject.FindObjectsOfType<SkeletonRenderer>();
				foreach (var sr in activeSkeletonRenderers) {
					if (sr.isActiveAndEnabled && sr.skeletonDataAsset == skeletonDataAsset) sr.Initialize(true);
				}

				var activeSkeletonGraphics = GameObject.FindObjectsOfType<SkeletonGraphic>();
				foreach (var sg in activeSkeletonGraphics) {
					if (sg.isActiveAndEnabled && sg.skeletonDataAsset == skeletonDataAsset) sg.Initialize(true);
				}
			}
		}
	}
}

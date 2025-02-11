
using UnityEngine;

namespace Spine.Unity.Editor {
	public static class AssetDatabaseAvailabilityDetector {
		const string MarkerResourceName = "SpineAssetDatabaseMarker";
		private static bool isMarkerLoaded;

		public static bool IsAssetDatabaseAvailable (bool forceCheck = false) {
			if (!forceCheck && isMarkerLoaded)
				return true;

			TextAsset markerTextAsset = Resources.Load<TextAsset>(AssetDatabaseAvailabilityDetector.MarkerResourceName);
			isMarkerLoaded = markerTextAsset != null;
			if (markerTextAsset != null) {
				Resources.UnloadAsset(markerTextAsset);
			}

			return isMarkerLoaded;
		}
	}
}

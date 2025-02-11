
#define AUTOINIT_SPINEREFERENCE

using UnityEngine;

namespace Spine.Unity {
	[CreateAssetMenu(menuName = "Spine/EventData Reference Asset", order = 100)]
	public class EventDataReferenceAsset : ScriptableObject {
		const bool QuietSkeletonData = true;

		[SerializeField] protected SkeletonDataAsset skeletonDataAsset;
		[SerializeField, SpineEvent(dataField: "skeletonDataAsset")] protected string eventName;

		EventData eventData;
		public EventData EventData {
			get {
				#if AUTOINIT_SPINEREFERENCE
				if (eventData == null)
					Initialize();
				#endif
				return eventData;
			}
		}

		public void Initialize () {
			if (skeletonDataAsset == null)
				return;
			this.eventData = skeletonDataAsset.GetSkeletonData(EventDataReferenceAsset.QuietSkeletonData).FindEvent(eventName);
			if (this.eventData == null)
				Debug.LogWarningFormat("Event Data '{0}' not found in SkeletonData : {1}.", eventName, skeletonDataAsset.name);
		}

		public static implicit operator EventData (EventDataReferenceAsset asset) {
			return asset.EventData;
		}
	}
}

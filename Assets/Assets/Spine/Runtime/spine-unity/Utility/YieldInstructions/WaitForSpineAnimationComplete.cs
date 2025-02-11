
using UnityEngine;
using System.Collections;
using Spine;

namespace Spine.Unity {
	/// <summary>
	/// Use this as a condition-blocking yield instruction for Unity Coroutines.
	/// The routine will pause until the AnimationState.TrackEntry fires its Complete event.
	/// It can be configured to trigger on the End event as well to cover interruption.
	/// <p/>
	/// See the <see cref="http://esotericsoftware.com/spine-unity-events">Spine Unity Events documentation page</see>
	/// and <see cref="http://esotericsoftware.com/spine-api-reference#AnimationStateListener"/>
	/// for more information on when track events will be triggered.</summary>
	public class WaitForSpineAnimationComplete : WaitForSpineAnimation, IEnumerator {

		public WaitForSpineAnimationComplete (Spine.TrackEntry trackEntry, bool includeEndEvent = false) :
			base(trackEntry,
				includeEndEvent ? (AnimationEventTypes.Complete | AnimationEventTypes.End) : AnimationEventTypes.Complete)
		{
		}

		#region Reuse
		/// <summary>
		/// One optimization high-frequency YieldInstruction returns is to cache instances to minimize GC pressure.
		/// Use NowWaitFor to reuse the same instance of WaitForSpineAnimationComplete.</summary>
		public WaitForSpineAnimationComplete NowWaitFor (Spine.TrackEntry trackEntry, bool includeEndEvent = false) {
			SafeSubscribe(trackEntry,
				includeEndEvent ? (AnimationEventTypes.Complete | AnimationEventTypes.End) : AnimationEventTypes.Complete);
			return this;
		}
		#endregion
	}
}

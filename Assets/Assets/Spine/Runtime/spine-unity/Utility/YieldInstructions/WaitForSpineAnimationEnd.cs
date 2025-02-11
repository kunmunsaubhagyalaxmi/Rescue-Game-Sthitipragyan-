
using UnityEngine;
using System.Collections;
using Spine;

namespace Spine.Unity {
	/// <summary>
	/// Use this as a condition-blocking yield instruction for Unity Coroutines.
	/// The routine will pause until the AnimationState.TrackEntry fires its End event.
	/// <p/>
	/// See the <see cref="http://esotericsoftware.com/spine-unity-events">Spine Unity Events documentation page</see>
	/// and <see cref="http://esotericsoftware.com/spine-api-reference#AnimationStateListener"/>
	/// for more information on when track events will be triggered.</summary>
	public class WaitForSpineAnimationEnd : WaitForSpineAnimation, IEnumerator {

		public WaitForSpineAnimationEnd (Spine.TrackEntry trackEntry) :
			base(trackEntry, AnimationEventTypes.End)
		{
		}

		#region Reuse
		/// <summary>
		/// One optimization high-frequency YieldInstruction returns is to cache instances to minimize GC pressure.
		/// Use NowWaitFor to reuse the same instance of WaitForSpineAnimationComplete.</summary>
		public WaitForSpineAnimationEnd NowWaitFor (Spine.TrackEntry trackEntry) {
			SafeSubscribe(trackEntry, AnimationEventTypes.End);
			return this;
		}
		#endregion
	}
}


using UnityEngine;
using System.Collections;
using System;

namespace Spine.Unity {
	/// <summary>
	/// Use this as a condition-blocking yield instruction for Unity Coroutines.
	/// The routine will pause until the AnimationState.TrackEntry fires any of the
	/// configured events.
	/// <p/>
	/// See the <see cref="http://esotericsoftware.com/spine-unity-events">Spine Unity Events documentation page</see>
	/// and <see cref="http://esotericsoftware.com/spine-api-reference#AnimationStateListener"/>
	/// for more information on when track events will be triggered.</summary>
	public class WaitForSpineAnimation : IEnumerator {

		[Flags]
		public enum AnimationEventTypes
		{
			Start = 1,
			Interrupt = 2,
			End = 4,
			Dispose = 8,
			Complete = 16
		}

		bool m_WasFired = false;

		public WaitForSpineAnimation (Spine.TrackEntry trackEntry, AnimationEventTypes eventsToWaitFor) {
			SafeSubscribe(trackEntry, eventsToWaitFor);
		}

		#region Reuse
		/// <summary>
		/// One optimization high-frequency YieldInstruction returns is to cache instances to minimize GC pressure.
		/// Use NowWaitFor to reuse the same instance of WaitForSpineAnimationComplete.</summary>
		public WaitForSpineAnimation NowWaitFor (Spine.TrackEntry trackEntry, AnimationEventTypes eventsToWaitFor) {
			SafeSubscribe(trackEntry, eventsToWaitFor);
			return this;
		}
		#endregion

		#region IEnumerator
		bool IEnumerator.MoveNext () {
			if (m_WasFired) {
				((IEnumerator)this).Reset();	// auto-reset for YieldInstruction reuse
				return false;
			}

			return true;
		}
		void IEnumerator.Reset () { m_WasFired = false; }
		object IEnumerator.Current { get { return null; } }
		#endregion

		protected void SafeSubscribe (Spine.TrackEntry trackEntry, AnimationEventTypes eventsToWaitFor) {
			if (trackEntry == null) {
				// Break immediately if trackEntry is null.
				Debug.LogWarning("TrackEntry was null. Coroutine will continue immediately.");
				m_WasFired = true;
			}
			else {
				if ((eventsToWaitFor & AnimationEventTypes.Start) != 0)
					trackEntry.Start += HandleComplete;
				if ((eventsToWaitFor & AnimationEventTypes.Interrupt) != 0)
					trackEntry.Interrupt += HandleComplete;
				if ((eventsToWaitFor & AnimationEventTypes.End) != 0)
					trackEntry.End += HandleComplete;
				if ((eventsToWaitFor & AnimationEventTypes.Dispose) != 0)
					trackEntry.Dispose += HandleComplete;
				if ((eventsToWaitFor & AnimationEventTypes.Complete) != 0)
					trackEntry.Complete += HandleComplete;
			}
		}

		void HandleComplete (TrackEntry trackEntry) {
			m_WasFired = true;
		}
	}
}

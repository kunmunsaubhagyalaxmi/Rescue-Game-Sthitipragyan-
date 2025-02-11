
using UnityEngine;
using System.Collections;
using Spine;

namespace Spine.Unity {
	/// <summary>
	/// Use this as a condition-blocking yield instruction for Unity Coroutines.
	/// The routine will pause until the AnimationState.TrackEntry fires its End event.</summary>
	public class WaitForSpineTrackEntryEnd : IEnumerator {

		bool m_WasFired = false;

		public WaitForSpineTrackEntryEnd (Spine.TrackEntry trackEntry) {
			SafeSubscribe(trackEntry);
		}

		void HandleEnd (TrackEntry trackEntry) {
			m_WasFired = true;
		}

		void SafeSubscribe (Spine.TrackEntry trackEntry) {
			if (trackEntry == null) {
				// Break immediately if trackEntry is null.
				Debug.LogWarning("TrackEntry was null. Coroutine will continue immediately.");
				m_WasFired = true;
			} else {
				trackEntry.End += HandleEnd;
			}
		}

		#region Reuse
		/// <summary>
		/// One optimization high-frequency YieldInstruction returns is to cache instances to minimize GC pressure.
		/// Use NowWaitFor to reuse the same instance of WaitForSpineAnimationEnd.</summary>
		public WaitForSpineTrackEntryEnd NowWaitFor (Spine.TrackEntry trackEntry) {
			SafeSubscribe(trackEntry);
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

	}

}

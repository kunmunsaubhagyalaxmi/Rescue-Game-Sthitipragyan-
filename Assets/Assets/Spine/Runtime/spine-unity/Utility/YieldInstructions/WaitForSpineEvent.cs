
using UnityEngine;
using System.Collections;
using Spine;

namespace Spine.Unity {
	/// <summary>
	/// Use this as a condition-blocking yield instruction for Unity Coroutines.
	/// The routine will pause until the AnimationState fires an event matching the given event name or EventData reference.</summary>
	public class WaitForSpineEvent : IEnumerator {

		Spine.EventData m_TargetEvent;
		string m_EventName;
		Spine.AnimationState m_AnimationState;

		bool m_WasFired = false;
		bool m_unsubscribeAfterFiring = false;

		#region Constructors
		void Subscribe (Spine.AnimationState state, Spine.EventData eventDataReference, bool unsubscribe) {
			if (state == null) {
				Debug.LogWarning("AnimationState argument was null. Coroutine will continue immediately.");
				m_WasFired = true;
				return;
			} else if (eventDataReference == null) {
				Debug.LogWarning("eventDataReference argument was null. Coroutine will continue immediately.");
				m_WasFired = true;
				return;
			}

			m_AnimationState = state;
			m_TargetEvent = eventDataReference;
			state.Event += HandleAnimationStateEvent;

			m_unsubscribeAfterFiring = unsubscribe;

		}

		void SubscribeByName (Spine.AnimationState state, string eventName, bool unsubscribe) {
			if (state == null) {
				Debug.LogWarning("AnimationState argument was null. Coroutine will continue immediately.");
				m_WasFired = true;
				return;
			} else if (string.IsNullOrEmpty(eventName)) {
				Debug.LogWarning("eventName argument was null. Coroutine will continue immediately.");
				m_WasFired = true;
				return;
			}

			m_AnimationState = state;
			m_EventName = eventName;
			state.Event += HandleAnimationStateEventByName;

			m_unsubscribeAfterFiring = unsubscribe;
		}

		public WaitForSpineEvent (Spine.AnimationState state, Spine.EventData eventDataReference, bool unsubscribeAfterFiring = true) {
			Subscribe(state, eventDataReference, unsubscribeAfterFiring);
		}

		public WaitForSpineEvent (SkeletonAnimation skeletonAnimation, Spine.EventData eventDataReference, bool unsubscribeAfterFiring = true) {
			// If skeletonAnimation is invalid, its state will be null. Subscribe handles null states just fine.
			Subscribe(skeletonAnimation.state, eventDataReference, unsubscribeAfterFiring);
		}

		public WaitForSpineEvent (Spine.AnimationState state, string eventName, bool unsubscribeAfterFiring = true) {
			SubscribeByName(state, eventName, unsubscribeAfterFiring);
		}

		public WaitForSpineEvent (SkeletonAnimation skeletonAnimation, string eventName, bool unsubscribeAfterFiring = true) {
			// If skeletonAnimation is invalid, its state will be null. Subscribe handles null states just fine.
			SubscribeByName(skeletonAnimation.state, eventName, unsubscribeAfterFiring);
		}
		#endregion

		#region Event Handlers
		void HandleAnimationStateEventByName (Spine.TrackEntry trackEntry, Spine.Event e) {
			m_WasFired |= (e.Data.Name == m_EventName);			// Check event name string match.
			if (m_WasFired && m_unsubscribeAfterFiring)
				m_AnimationState.Event -= HandleAnimationStateEventByName;	// Unsubscribe after correct event fires.
		}

		void HandleAnimationStateEvent (Spine.TrackEntry trackEntry, Spine.Event e) {
			m_WasFired |= (e.Data == m_TargetEvent);			// Check event data reference match.
			if (m_WasFired && m_unsubscribeAfterFiring)
				m_AnimationState.Event -= HandleAnimationStateEvent; 		// Usubscribe after correct event fires.
		}
		#endregion

		#region Reuse
		/// <summary>
		/// By default, WaitForSpineEvent will unsubscribe from the event immediately after it fires a correct matching event.
		/// If you want to reuse this WaitForSpineEvent instance on the same event, you can set this to false.</summary>
		public bool WillUnsubscribeAfterFiring { get { return m_unsubscribeAfterFiring; } set { m_unsubscribeAfterFiring = value; } }

		public WaitForSpineEvent NowWaitFor (Spine.AnimationState state, Spine.EventData eventDataReference, bool unsubscribeAfterFiring = true) {
			((IEnumerator)this).Reset();
			Clear(state);
			Subscribe(state, eventDataReference, unsubscribeAfterFiring);

			return this;
		}

		public WaitForSpineEvent NowWaitFor (Spine.AnimationState state, string eventName, bool unsubscribeAfterFiring = true) {
			((IEnumerator)this).Reset();
			Clear(state);
			SubscribeByName(state, eventName, unsubscribeAfterFiring);

			return this;
		}

		void Clear (Spine.AnimationState state) {
			state.Event -= HandleAnimationStateEvent;
			state.Event -= HandleAnimationStateEventByName;
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

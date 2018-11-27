using System;
using BeatThat.Notifications;
using UnityEngine;


namespace BeatThat.StateStores
{

    public static class ResolveAdviceHelper
	{
		public const float DEFAULT_LOAD_TIMEOUT_SECS = 5f;
		public const float DEFAULT_RETRY_MIN_INTERVAL_SECS = 2f;
		public const float DEFAULT_TTL_SECS = -1f;

        public static ResolveAdvice AdviseOnAndSendErrorIfCoolingDown(
            HasStateResolveStatus hasLoadStatus,
            string errorNotification,
            float loadTimeoutSecs = DEFAULT_LOAD_TIMEOUT_SECS,
            float retryMinIntervalSecs = DEFAULT_RETRY_MIN_INTERVAL_SECS,
            float ttlSecs = DEFAULT_TTL_SECS,
            bool debug = false)
        {
            var advice = AdviseOn(hasLoadStatus, loadTimeoutSecs, retryMinIntervalSecs, ttlSecs, debug);
            if(advice == ResolveAdvice.CANCEL_ERROR_COOL_DOWN) {
                NotificationBus.Send(errorNotification, new ResolveFailedDTO
                {
                    error = "load has failed for id and is in cooldown period"
                });
            }
            return advice;
        }

		public static ResolveAdvice AdviseOn(
			HasStateResolveStatus hasLoadStatus, 
			float loadTimeoutSecs = DEFAULT_LOAD_TIMEOUT_SECS,
			float retryMinIntervalSecs = DEFAULT_RETRY_MIN_INTERVAL_SECS, 
			float ttlSecs = DEFAULT_TTL_SECS, 
			bool debug = false)
		{
            ResolveStatus loadStatus = hasLoadStatus.resolveStatus;

			if(loadStatus.hasResolved && (ttlSecs < 0f || loadStatus.updatedAt.AddSeconds(ttlSecs) > DateTimeOffset.Now)) {
				#if UNITY_EDITOR || DEBUG_UNSTRIP
				if(debug) {
					Debug.Log("[" + Time.frameCount + "] skipping load attempt (already loaded and not expired)");
				}
				#endif
				return ResolveAdvice.CANCEL_LOADED_AND_UNEXPIRED;
			}

			if (loadStatus.isResolveInProgress && loadStatus.resolveStartedAt.AddSeconds(loadTimeoutSecs) > DateTimeOffset.Now) {
				#if UNITY_EDITOR || DEBUG_UNSTRIP
				if(debug) {
					Debug.Log("[" + Time.frameCount + "] skipping load attempt (load in progress started at " + loadStatus.resolveStartedAt + ")");
				}
				#endif
				return ResolveAdvice.CANCEL_IN_PROGRESS;
			}

			if (!string.IsNullOrEmpty(loadStatus.loadError) && loadStatus.updatedAt.AddSeconds(retryMinIntervalSecs) > DateTimeOffset.Now) {
				#if UNITY_EDITOR || DEBUG_UNSTRIP
				if(debug) {
					Debug.Log("[" + Time.frameCount + "] skipping load attempt (load in progress started at " + loadStatus.resolveStartedAt + ")");
				}
				#endif
				return ResolveAdvice.CANCEL_ERROR_COOL_DOWN;
			}

			return ResolveAdvice.PROCEED;
		}
	}
}


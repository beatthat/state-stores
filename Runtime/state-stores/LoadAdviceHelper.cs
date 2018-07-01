using System;
using BeatThat.Notifications;
using UnityEngine;


namespace BeatThat.StateStores
{

    public static class LoadAdviceHelper
	{
		public const float DEFAULT_LOAD_TIMEOUT_SECS = 5f;
		public const float DEFAULT_RETRY_MIN_INTERVAL_SECS = 2f;
		public const float DEFAULT_TTL_SECS = -1f;

        public static LoadAdvice AdviseOnAndSendErrorIfCoolingDown(
            HasStateLoadStatus hasLoadStatus,
            string errorNotification,
            float loadTimeoutSecs = DEFAULT_LOAD_TIMEOUT_SECS,
            float retryMinIntervalSecs = DEFAULT_RETRY_MIN_INTERVAL_SECS,
            float ttlSecs = DEFAULT_TTL_SECS,
            bool debug = false)
        {
            var advice = AdviseOn(hasLoadStatus, loadTimeoutSecs, retryMinIntervalSecs, ttlSecs, debug);
            if(advice == LoadAdvice.CANCEL_ERROR_COOL_DOWN) {
                NotificationBus.Send(errorNotification, new LoadFailedDTO
                {
                    error = "load has failed for id and is in cooldown period"
                });
            }
            return advice;
        }

		public static LoadAdvice AdviseOn(
			HasStateLoadStatus hasLoadStatus, 
			float loadTimeoutSecs = DEFAULT_LOAD_TIMEOUT_SECS,
			float retryMinIntervalSecs = DEFAULT_RETRY_MIN_INTERVAL_SECS, 
			float ttlSecs = DEFAULT_TTL_SECS, 
			bool debug = false)
		{
            LoadStatus loadStatus = hasLoadStatus.loadStatus;

			if(loadStatus.hasLoaded && (ttlSecs < 0f || loadStatus.updatedAt.AddSeconds(ttlSecs) > DateTime.Now)) {
				#if UNITY_EDITOR || DEBUG_UNSTRIP
				if(debug) {
					Debug.Log("[" + Time.frameCount + "] skipping load attempt (already loaded and not expired)");
				}
				#endif
				return LoadAdvice.CANCEL_LOADED_AND_UNEXPIRED;
			}

			if (loadStatus.isLoadInProgress && loadStatus.loadStartedAt.AddSeconds(loadTimeoutSecs) > DateTime.Now) {
				#if UNITY_EDITOR || DEBUG_UNSTRIP
				if(debug) {
					Debug.Log("[" + Time.frameCount + "] skipping load attempt (load in progress started at " + loadStatus.loadStartedAt + ")");
				}
				#endif
				return LoadAdvice.CANCEL_IN_PROGRESS;
			}

			if (!string.IsNullOrEmpty(loadStatus.loadError) && loadStatus.updatedAt.AddSeconds(retryMinIntervalSecs) > DateTime.Now) {
				#if UNITY_EDITOR || DEBUG_UNSTRIP
				if(debug) {
					Debug.Log("[" + Time.frameCount + "] skipping load attempt (load in progress started at " + loadStatus.loadStartedAt + ")");
				}
				#endif
				return LoadAdvice.CANCEL_ERROR_COOL_DOWN;
			}

			return LoadAdvice.PROCEED;
		}
	}
}


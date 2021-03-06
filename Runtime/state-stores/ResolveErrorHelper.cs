using BeatThat.Notifications;
using BeatThat.Requests;
using UnityEngine;

namespace BeatThat.StateStores
{

    public static class ResolveErrorHelper
	{
		public static bool HandledError(HasError r, string errorNotification, bool debug = false)
		{
			if (string.IsNullOrEmpty (r.error)) {
				return false;
			}


			#if UNITY_EDITOR || DEBUG_UNSTRIP
			Debug.LogWarning("[" + Time.frameCount + "] error loading : " + r.error);
			#endif

			NotificationBus.Send(errorNotification, new ResolveFailedDTO {
				error = r
			});
			return true;
		}
	}
}


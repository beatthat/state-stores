using System;
using BeatThat.Requests;

namespace BeatThat.StateStores
{
    [Serializable]
	public struct ResolveFailedDTO
	{
		public object error;

        public string errorMessage {
            get {
                if(this.error == null) {
                    return null;
                }

                var hasErr = this.error as HasError;
                if(hasErr != null) {
                    return hasErr.error;
                }

                return this.error.ToString();
            }
        }
	}
}


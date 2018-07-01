using System;

namespace BeatThat.StateStores
{
    [Serializable]
	public struct LoadStatus
	{
		public bool hasLoaded;
		public bool isLoadInProgress;
		public DateTime loadStartedAt;
		public DateTime updatedAt;
		public string loadError;

		public LoadStatus LoadFailed(LoadFailedDTO dto, DateTime updateTime)
		{
			return new LoadStatus {
				hasLoaded = this.hasLoaded,
				isLoadInProgress = false,
				loadStartedAt = this.loadStartedAt,
				updatedAt = updateTime,
                loadError = dto.errorMessage
			};
		}

		public LoadStatus LoadStarted(DateTime updateTime)
		{
			return new LoadStatus {
				hasLoaded = this.hasLoaded,
				isLoadInProgress = true,
				loadStartedAt = updateTime,
				updatedAt = this.updatedAt,
				loadError = this.loadError
			};
		}

		public LoadStatus LoadSucceeded(DateTime updateTime)
		{
			return new LoadStatus {
				hasLoaded = true,
				isLoadInProgress = false,
				loadStartedAt = updateTime,
				updatedAt = updateTime,
				loadError = this.loadError
			};
		}
	}
}



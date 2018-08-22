using System;

namespace BeatThat.StateStores
{
    [Serializable]
	public struct ResolveStatus
	{
		public bool hasLoaded;
		public bool isLoadInProgress;
		public DateTimeOffset loadStartedAt;
		public DateTimeOffset updatedAt;
		public string loadError;

		public ResolveStatus LoadFailed(ResolveFailedDTO dto, DateTimeOffset updateTime)
		{
			return new ResolveStatus {
				hasLoaded = this.hasLoaded,
				isLoadInProgress = false,
				loadStartedAt = this.loadStartedAt,
				updatedAt = updateTime,
                loadError = dto.errorMessage
			};
		}

		public ResolveStatus LoadStarted(DateTimeOffset updateTime)
		{
			return new ResolveStatus {
				hasLoaded = this.hasLoaded,
				isLoadInProgress = true,
				loadStartedAt = updateTime,
				updatedAt = this.updatedAt,
				loadError = this.loadError
			};
		}

		public ResolveStatus LoadSucceeded(DateTimeOffset updateTime)
		{
			return new ResolveStatus {
				hasLoaded = true,
				isLoadInProgress = false,
				loadStartedAt = updateTime,
				updatedAt = updateTime,
				loadError = this.loadError
			};
		}
	}
}



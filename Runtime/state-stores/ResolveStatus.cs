using System;

namespace BeatThat.StateStores
{
    [Serializable]
	public struct ResolveStatus
	{
		public bool hasLoaded;
		public bool isLoadInProgress;
		public DateTime loadStartedAt;
		public DateTime updatedAt;
		public string loadError;

		public ResolveStatus LoadFailed(ResolveFailedDTO dto, DateTime updateTime)
		{
			return new ResolveStatus {
				hasLoaded = this.hasLoaded,
				isLoadInProgress = false,
				loadStartedAt = this.loadStartedAt,
				updatedAt = updateTime,
                loadError = dto.errorMessage
			};
		}

		public ResolveStatus LoadStarted(DateTime updateTime)
		{
			return new ResolveStatus {
				hasLoaded = this.hasLoaded,
				isLoadInProgress = true,
				loadStartedAt = updateTime,
				updatedAt = this.updatedAt,
				loadError = this.loadError
			};
		}

		public ResolveStatus LoadSucceeded(DateTime updateTime)
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



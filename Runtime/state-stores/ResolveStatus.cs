using System;

namespace BeatThat.StateStores
{
    [Serializable]
	public struct ResolveStatus
	{
		public bool hasResolved;
		public bool isResolveInProgress;
		public DateTimeOffset resolveStartedAt;
		public DateTimeOffset updatedAt;
		public string loadError;

		public ResolveStatus LoadFailed(ResolveFailedDTO dto, DateTimeOffset updateTime)
		{
			return new ResolveStatus {
				hasResolved = this.hasResolved,
				isResolveInProgress = false,
				resolveStartedAt = this.resolveStartedAt,
				updatedAt = updateTime,
                loadError = dto.errorMessage
			};
		}

		public ResolveStatus LoadStarted(DateTimeOffset updateTime)
		{
			return new ResolveStatus {
				hasResolved = this.hasResolved,
				isResolveInProgress = true,
				resolveStartedAt = updateTime,
				updatedAt = this.updatedAt,
				loadError = this.loadError
			};
		}

		public ResolveStatus LoadSucceeded(DateTimeOffset updateTime)
		{
			return new ResolveStatus {
				hasResolved = true,
				isResolveInProgress = false,
				resolveStartedAt = updateTime,
				updatedAt = updateTime,
				loadError = this.loadError
			};
		}
	}
}



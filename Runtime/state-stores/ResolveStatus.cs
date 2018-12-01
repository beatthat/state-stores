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

        public ResolveStatus Failed(ResolveFailedDTO dto, DateTimeOffset updateTime)
		{
			return new ResolveStatus {
				hasResolved = this.hasResolved,
				isResolveInProgress = false,
				resolveStartedAt = this.resolveStartedAt,
				updatedAt = updateTime,
                loadError = dto.errorMessage
			};
		}

        public ResolveStatus Started(DateTimeOffset updateTime)
		{
			return new ResolveStatus {
				hasResolved = this.hasResolved,
				isResolveInProgress = true,
				resolveStartedAt = updateTime,
				updatedAt = this.updatedAt,
				loadError = this.loadError
			};
		}

        public ResolveStatus Succeeded(DateTimeOffset updateTime)
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



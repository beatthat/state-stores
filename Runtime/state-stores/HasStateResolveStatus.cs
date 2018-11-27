namespace BeatThat.StateStores
{
    public interface HasStateResolveStatus
	{
        ResolveStatus resolveStatus { get; }
	}

    public static class HasStateResolveStatusExt
    {
        /// <summary>
        /// Convenience to test whether state has not resolved or a resolve is in progress.
        /// </summary>
        public static bool HasNotResolvedOrInProgress(this HasStateResolveStatus has)
        {
            var status = has.resolveStatus;
            return status.hasResolved == false || status.isResolveInProgress;
        }

        public static bool HasResolved(this HasStateResolveStatus has)
        {
            return has.resolveStatus.hasResolved;
        }

        public static bool HasNotResolved(this HasStateResolveStatus has)
        {
            return has.resolveStatus.hasResolved == false;
        }
    }
}



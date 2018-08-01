namespace BeatThat.StateStores
{
    public interface HasStateLoadStatus
	{
        bool isLoaded { get; }

        ResolveStatus loadStatus { get; }
	}
}



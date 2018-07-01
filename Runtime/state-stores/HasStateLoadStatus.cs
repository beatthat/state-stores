namespace BeatThat.StateStores
{
    public interface HasStateLoadStatus
	{
        bool isLoaded { get; }

        LoadStatus loadStatus { get; }
	}
}



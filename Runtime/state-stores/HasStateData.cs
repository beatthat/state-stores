namespace BeatThat.StateStores
{
    public interface HasStateData<DataType>
	{
        DataType stateData { get; }
	}
}



namespace BeatThat.StateStores
{
    public interface HasState<DataType> : HasStateData<DataType>, HasStateLoadStatus
	{
        State<DataType> state { get; }
	}
}



namespace BeatThat.StateStores
{
    public interface HasState<DataType> : HasStateData<DataType>, HasStateResolveStatus
	{
        State<DataType> state { get; }
	}


}



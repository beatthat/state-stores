using System;
using BeatThat.Requests;

namespace BeatThat.StateStores
{
    /// <summary>
    /// an API capable of loading data a state item that needs no parameters
    /// </summary>
    public interface StateAPI<DataType>
	{
        Request<LoadResponseDTO<DataType>> Get(Action<Request<LoadResponseDTO<DataType>>> callback);
	}
}



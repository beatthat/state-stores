using System;
using BeatThat.Requests;

namespace BeatThat.StateStores
{
    /// <summary>
    /// an API capable of loading data a state item that needs no parameters
    /// </summary>
    public interface StateResolver<DataType>
	{
        Request<ResolveResponseDTO<DataType>> Resolve(Action<Request<ResolveResponseDTO<DataType>>> callback);
	}
}



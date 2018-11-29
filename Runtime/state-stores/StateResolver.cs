using System;
using BeatThat.Requests;

namespace BeatThat.StateStores
{
    /// <summary>
    /// an API capable of loading data a state item that needs no parameters
    /// </summary>
    public interface StateResolver<DataType>
	{
        Request<ResolveResultDTO<DataType>> Resolve(Action<Request<ResolveResultDTO<DataType>>> callback = null);

#if NET_4_6
        System.Threading.Tasks.Task<ResolveResultDTO<DataType>> ResolveAsync();
#endif
	}
}



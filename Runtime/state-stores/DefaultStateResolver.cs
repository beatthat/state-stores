using BeatThat.Bindings;
using BeatThat.Requests;
using BeatThat.Service;
using System;

#if NET_4_6
using System.Threading.Tasks;
#endif

namespace BeatThat.StateStores
{
    /// <summary>
    /// The default state resolver just checks the StateStore
    /// and returns whatever is already there.
    ///
    /// The more common case is that entites are resolved by, say,
    /// making a REST request.
    ///
    /// Even if you're overriding the resolve behaviour,
    /// you may still want to use this base class
    /// and override ResolveAsync though,
    /// to save you from having to write the boilerplate call that
    /// maps pre NET4.6 resolves to the async version of the function.
    /// </summary>
    public class DefaultStateResolver<DataType> : BindingService, StateResolver<DataType>
    {
        virtual protected ResolveResultDTO<DataType> GetStoredStateAsResolveResult()
        {
            var store = Services.Require<HasState<DataType>>();
            State<DataType> s = store.state;

            return new ResolveResultDTO<DataType>
            {
                status = s.resolveStatus.hasResolved ?
                          ResolveStatusCode.OK : "not found",
                message = s.resolveStatus.hasResolved ? "ok" : "not found",
                data = s.data
            };
        }

#if NET_4_6
        /// <summary>
        /// Wraps call to ResolveAsync in a request
        /// </summary>
        virtual public Request<ResolveResultDTO<DataType>> Resolve(
            Action<Request<ResolveResultDTO<DataType>>> callback = null)
        {
            var r = new TaskRequest<ResolveResultDTO<DataType>>(ResolveAsync());
            r.Execute(callback);
            return r;
        }

#pragma warning disable 1998
        virtual public async Task<ResolveResultDTO<DataType>> ResolveAsync()
#pragma warning restore 1998
        {
            return GetStoredStateAsResolveResult();
        }

#else
        virtual public Request<ResolveResultDTO<DataType>> Resolve(
            Action<Request<ResolveResultDTO<DataType>>> callback = null)
        {
            var result = GetStoredStateAsResolveResult();
            var request = new LocalRequest<ResolveResultDTO<DataType>>(result);
            request.Execute(callback);
            return request;
        }
#endif

    }
}



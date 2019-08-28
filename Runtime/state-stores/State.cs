using System;
using BeatThat.Bindings;
using BeatThat.Notifications;
using BeatThat.Requests;
using UnityEngine;

namespace BeatThat.StateStores
{
    using N = NotificationBus;
    using Opts = NotificationReceiverOptions;
    [Serializable]
    public struct State<DataType> 
    {
        
        public DataType data;
        public ResolveStatus resolveStatus;

        public static readonly string RESOLVE_REQUESTED = typeof(DataType).FullName + "_RESOLVE_REQUESTED";
        public static void ResolveRequested(Opts opts = Opts.RequireReceiver)
        {
            ResolveRequested(default(ResolveRequestDTO), opts);
        }

        public static void ResolveRequested(ResolveRequestDTO dto, Opts opts = Opts.RequireReceiver)
        {
            N.Send(RESOLVE_REQUESTED, dto, opts);
        }

        public static readonly string RESOLVE_STARTED = typeof(DataType).FullName + "_RESOLVE_STARTED";
        public static void ResolveStarted(Opts opts = Opts.RequireReceiver)
        {
            N.Send(RESOLVE_STARTED, opts);
        }

        public static readonly string RESOLVE_SUCCEEDED = typeof(DataType).FullName + "_RESOLVE_SUCCEEDED";
        public static void ResolveSucceeded(ResolveSucceededDTO<DataType> dto, Opts opts = Opts.RequireReceiver)
        {
            N.Send(RESOLVE_SUCCEEDED, dto, opts);
        }

        public static readonly string RESOLVE_FAILED = typeof(DataType).FullName + "_RESOLVE_FAILED";
        public static void ResolveFailed(ResolveFailedDTO dto, Opts opts = Opts.RequireReceiver)
        {
            N.Send(RESOLVE_FAILED, dto, opts);
        }

        public static readonly string STORE = typeof(DataType).FullName + "_STORED";
        public static void Store(StoreStateDTO<DataType> dto, Opts opts = Opts.RequireReceiver)
        {
            N.Send(STORE, dto, opts);
        }


        public static readonly string UPDATED = typeof(DataType).FullName + "_UPDATED";
        public static void Updated(Opts opts = Opts.DontRequireReceiver)
        {
            N.Send(UPDATED, opts);
        }

        /// <summary>
        /// Allows you to request an entity (from the store) and get a callback when load succeeds or fails.
        /// If the entity is not initially loaded, sends the usual notifications and then listens for updates
        /// </summary>
        public static Request<DataType> Get(
            HasState<DataType> store, 
            Action<Request<DataType>> callback = null)
        {
            var r = new StateRequest(store);
            r.Execute(callback);
            return r;
        }


#if NET_4_6
        /// <summary>
        /// Resolve the data and metadata for StateStore using async/await.
        /// NOTE: this will throw an exception if the item cannot be found.
        /// If you want to handle 'not found' or other errors without exceptions,
        /// use @see ResolveAsync instead; it returns a ResolveResult<DataType> 
        /// that would include details on failed resolves.
        /// </summary>
        public static async System.Threading.Tasks.Task<DataType> ResolveOrThrowAsync(
            HasState<DataType> store)
        {
            return await Get(store);
        }

        /// <summary>
        /// Resolve the data and metadata for StateStore using async/await.
        /// NOTE: this method will return a ResolveResult<DataType> even if the item
        /// fails to load, e.g. either not found or resolve error.
        /// Use this version if you want to handle failed responses without try/catch.
        /// If you'd rather an exception be thrown any time the data isn't available,
        /// use @see ResolveOrThrowAsync.
        /// </summary>
        /// <returns>The async.</returns>
        public static async System.Threading.Tasks.Task<ResolveResultDTO<DataType>> ResolveAsync(
            HasState<DataType> store)
        {
            var r = new StateRequest(store);
            try
            {
                var data = await r.ExecuteAsync();
                return ResolveResultDTO<DataType>.ResolveSucceeded(data);
            }
            catch (Exception e)
            {

#if UNITY_EDITOR || DEBUG_UNSTRIP
                var ae = e as AggregateException;
                if (ae != null)
                {
                    foreach (var ie in ae.InnerExceptions)
                    {
                        Debug.LogError("error on execute async for type "
                                       + typeof(DataType).Name
                                       + ": " + ie.Message
                                       + "\n" + ie.StackTrace
                                      );
                    }
                }
                else
                {
                    Debug.LogError("error on execute async for type "
                                   + typeof(DataType).Name
                                   + ": " + e.Message
                                  );
                }
#endif
                return ResolveResultDTO<DataType>.ResolveError(e.Message);
            }
        }
#endif

        class StateRequest : RequestBase, Request<DataType>
        {
            public StateRequest(HasState<DataType> store)
            {
                this.store = store;
            }

            public DataType item { get; private set; }

            public object GetItem()
            {
                return this.item;
            }

            protected override void ExecuteRequest()
            {
                if(TryComplete()) {
                    return;
                }

                CleanupBinding();
                this.storeBinding = N.Add(UPDATED, this.OnStoreUpdate);
                State<DataType>.ResolveRequested();
            }

            private bool TryComplete()
            {
                ResolveStatus loadStatus = store.resolveStatus;
                if (loadStatus.hasResolved)
                {
                    this.item = store.stateData;
                    CompleteRequest();
                    return true;
                }

                if (!string.IsNullOrEmpty(loadStatus.loadError))
                {
                    CompleteWithError(loadStatus.loadError);
                    return true;
                }

                return false;
            }

            private void OnStoreUpdate()
            {
                TryComplete();
            }

            protected override void DisposeRequest()
            {
                CleanupBinding();
                base.DisposeRequest();
            }

            protected override void AfterCompletionCallback()
            {
                CleanupBinding();
                base.AfterCompletionCallback();
            }

            private void CleanupBinding()
            {
                if (this.storeBinding != null)
                {
                    this.storeBinding.Unbind();
                    this.storeBinding = null;
                }
            }

            private Binding storeBinding { get; set; }
            private HasState<DataType> store { get; set; }

        }
    }
}

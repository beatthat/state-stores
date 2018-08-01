using System;
using BeatThat.Bindings;
using BeatThat.Notifications;
using BeatThat.Requests;

namespace BeatThat.StateStores
{
    using N = NotificationBus;
    using Opts = NotificationReceiverOptions;
    public struct State<DataType> 
    {
        
        public DataType data;
        public ResolveStatus loadStatus;

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
            Action<Request<DataType>> callback)
        {
            var r = new StateRequest(store);
            r.Execute(callback);
            return r;
        }

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
                ResolveStatus loadStatus = store.loadStatus;
                if (loadStatus.hasLoaded)
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

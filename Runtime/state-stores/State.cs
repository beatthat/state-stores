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
        public LoadStatus loadStatus;

        public static readonly string LOAD_REQUESTED = typeof(DataType).FullName + "_LOAD_REQUESTED";
        public static void LoadRequested(Opts opts = Opts.RequireReceiver)
        {
            N.Send(LOAD_REQUESTED, opts);
        }

        public static readonly string LOAD_STARTED = typeof(DataType).FullName + "_LOAD_STARTED";
        public static void LoadStarted(Opts opts = Opts.RequireReceiver)
        {
            N.Send(LOAD_STARTED, opts);
        }

        public static readonly string LOAD_SUCCEEDED = typeof(DataType).FullName + "_LOAD_SUCCEEDED";
        public static void LoadSucceeded(LoadSucceededDTO<DataType> dto, Opts opts = Opts.RequireReceiver)
        {
            N.Send(LOAD_SUCCEEDED, dto, opts);
        }

        public static readonly string LOAD_FAILED = typeof(DataType).FullName + "_LOAD_FAILED";
        public static void LoadFailed(LoadFailedDTO dto, Opts opts = Opts.RequireReceiver)
        {
            N.Send(LOAD_FAILED, dto, opts);
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
                State<DataType>.LoadRequested();
            }

            private bool TryComplete()
            {
                LoadStatus loadStatus = store.loadStatus;
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

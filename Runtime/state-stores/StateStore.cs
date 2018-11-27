using System;
using BeatThat.Bindings;

namespace BeatThat.StateStores
{
    /// <summary>
    /// Non generic base class mainly to enable things like a default Unity editor
    /// </summary>
    public abstract class StateStore : BindingService, HasStateResolveStatus
    {
        public abstract bool isLoaded { get; }
        public abstract ResolveStatus resolveStatus { get; }
    }

    public class StateStore<DataType> : StateStore, HasState<DataType>
	{
        public bool m_debug;

		override protected void BindAll()
		{
            Bind <ResolveSucceededDTO<DataType>>(State<DataType>.RESOLVE_SUCCEEDED, this.OnLoadSucceeded);
            Bind(State<DataType>.RESOLVE_STARTED, this.OnLoadStarted);
            Bind <ResolveFailedDTO>(State<DataType>.RESOLVE_FAILED, this.OnLoadFailed);
            Bind<StoreStateDTO<DataType>>(State<DataType>.STORE, this.OnStore);
            BindStore();
		}

        /// <summary>
        /// Override to add additional bindings
        /// </summary>
        virtual protected void BindStore() {}

        override public bool isLoaded { get { return m_state.loadStatus.hasResolved; } }


        override public ResolveStatus resolveStatus  { get { return m_state.loadStatus; } }
			
        public State<DataType> state { get { return m_state; } }

        public DataType stateData { get { return m_state.data; } }

		virtual protected void OnLoadFailed(ResolveFailedDTO err)
		{
            UpdateLoadStatus(this.resolveStatus.LoadFailed(err, DateTimeOffset.Now));
		}

        virtual protected void OnLoadStarted()
		{
            UpdateLoadStatus(this.resolveStatus.LoadStarted(DateTimeOffset.Now));
		}

        virtual protected void OnLoadSucceeded(ResolveSucceededDTO<DataType> dto)
		{
            State<DataType> s;
            GetState(out s);
            s.loadStatus = state.loadStatus.LoadSucceeded(DateTimeOffset.Now);
            s.data = dto.data;
            UpdateState(ref s);
		}

        virtual protected void OnStore(StoreStateDTO<DataType> dto)
        {
            State<DataType> s;
            GetState(out s);
            s.loadStatus = state.loadStatus.LoadSucceeded(DateTimeOffset.Now);
            s.data = dto.data;
            UpdateState(ref s);
        }

        virtual protected void UpdateState(ref State<DataType> state, bool sendUpdated = true)
        {
            m_state = state;
            if (sendUpdated)
            {
                State<DataType>.Updated();
            }
        }

        virtual protected void UpdateData(ref DataType data, bool sendUpdated = true)
        {
            m_state.data = data;
            if (sendUpdated)
            {
                State<DataType>.Updated();
            }
        }

        virtual protected void UpdateLoadStatus(ResolveStatus loadStatus, bool sendUpdated = true)
        {
            m_state.loadStatus = loadStatus;
            if (sendUpdated)
            {
                State<DataType>.Updated();
            }
        }

        protected void ResetState()
        {
            State<DataType> s = default(State<DataType>);
            UpdateState(ref s);
        }

        protected void GetState(out State<DataType> state)
        {
            state = m_state;
        }

        protected void GetData(out DataType data)
        {
            data = m_state.data;
        }

        private State<DataType> m_state;
	}

}



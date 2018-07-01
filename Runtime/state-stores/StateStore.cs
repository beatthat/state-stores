using System;
using BeatThat.Bindings;

namespace BeatThat.StateStores
{
    /// <summary>
    /// Non generic base class mainly to enable things like a default Unity editor
    /// </summary>
    public abstract class StateStore : BindingService, HasStateLoadStatus
    {
        public abstract bool isLoaded { get; }
        public abstract LoadStatus loadStatus { get; }
    }

    public class StateStore<DataType> : StateStore, HasState<DataType>
	{
        public bool m_debug;

		override protected void BindAll()
		{
            Bind <LoadSucceededDTO<DataType>>(State<DataType>.LOAD_SUCCEEDED, this.OnLoadSucceeded);
            Bind(State<DataType>.LOAD_STARTED, this.OnLoadStarted);
            Bind <LoadFailedDTO>(State<DataType>.LOAD_FAILED, this.OnLoadFailed);
            BindStore();
		}

        /// <summary>
        /// Override to add additional bindings
        /// </summary>
        virtual protected void BindStore() {}

        override public bool isLoaded { get { return m_state.loadStatus.hasLoaded; } }


        override public LoadStatus loadStatus  { get { return m_state.loadStatus; } }
			
        public State<DataType> state { get { return m_state; } }

        public DataType stateData { get { return m_state.data; } }

		private void OnLoadFailed(LoadFailedDTO err)
		{
            UpdateLoadStatus(this.loadStatus.LoadFailed(err, DateTime.Now));
		}

        private void OnLoadStarted()
		{
            UpdateLoadStatus(this.loadStatus.LoadStarted(DateTime.Now));
		}

        private void OnLoadSucceeded(LoadSucceededDTO<DataType> dto)
		{
            State<DataType> s;
            GetState(out s);
            s.loadStatus = state.loadStatus.LoadSucceeded(DateTime.Now);
            s.data = dto.data;
            UpdateState(ref s);
		}

        protected void UpdateState(ref State<DataType> state)
        {
            m_state = state;
            State<DataType>.Updated();
        }

        protected void UpdateData(ref DataType data)
        {
            m_state.data = data;
            State<DataType>.Updated();
        }

        protected void UpdateLoadStatus(LoadStatus loadStatus)
        {
            m_state.loadStatus = loadStatus;
            State<DataType>.Updated();
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



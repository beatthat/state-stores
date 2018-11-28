using System;
using BeatThat.Bindings;
using UnityEngine;

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

        override public bool isLoaded { get { return m_state.resolveStatus.hasResolved; } }


        override public ResolveStatus resolveStatus  { get { return m_state.resolveStatus; } }
			
        public State<DataType> state { get { return m_state; } }

        public DataType stateData { get { return m_state.data; } }

		virtual protected void OnLoadFailed(ResolveFailedDTO err)
		{
            UpdateResolveStatus(this.resolveStatus.LoadFailed(err, DateTimeOffset.Now));
		}

        virtual protected void OnLoadStarted()
		{
            UpdateResolveStatus(this.resolveStatus.LoadStarted(DateTimeOffset.Now));
		}

        virtual protected void OnLoadSucceeded(ResolveSucceededDTO<DataType> dto)
		{
            State<DataType> s;
            GetState(out s);
            s.resolveStatus = state.resolveStatus.LoadSucceeded(DateTimeOffset.Now);
            s.data = dto.data;
            UpdateState(ref s);
		}

        virtual protected void OnStore(StoreStateDTO<DataType> dto)
        {
            State<DataType> s;
            GetState(out s);
            s.resolveStatus = state.resolveStatus.LoadSucceeded(DateTimeOffset.Now);
            s.data = dto.data;
            UpdateState(ref s);
        }

        virtual protected void UpdateState(
            ref State<DataType> state, bool sendUpdated = true
        )
        {
#if UNITY_EDITOR || DEBUG_UNSTRIP
            if(m_debug) {
                Debug.Log("[" + GetType() + "] UpdateState from " 
                          + JsonUtility.ToJson(m_state)
                          + " to " + JsonUtility.ToJson(state));
            }
#endif

            m_state = state;
            if (sendUpdated)
            {
                State<DataType>.Updated();
            }
        }

        virtual protected void UpdateData(
            ref DataType data, bool sendUpdated = true
        )
        {
#if UNITY_EDITOR || DEBUG_UNSTRIP
            if (m_debug)
            {
                Debug.Log("[" + GetType() + "] UpdateData from " 
                          + JsonUtility.ToJson(m_state.data)
                          + " to " + JsonUtility.ToJson(data));
            }
#endif

            m_state.data = data;
            if (sendUpdated)
            {
                State<DataType>.Updated();
            }
        }

        virtual protected void UpdateResolveStatus(
            ResolveStatus resolveStatus, bool sendUpdated = true
        )
        {

#if UNITY_EDITOR || DEBUG_UNSTRIP
            if (m_debug)
            {
                Debug.Log("[" + GetType() + "] UpdateResolveStatus from " 
                          + JsonUtility.ToJson(m_state.resolveStatus)
                          + " to " + JsonUtility.ToJson(resolveStatus));
            }
#endif

            m_state.resolveStatus = resolveStatus;
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



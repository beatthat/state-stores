using BeatThat.Commands;
using BeatThat.Notifications;
using BeatThat.DependencyInjection;

namespace BeatThat.StateStores
{
    /// <summary>
    /// Convenience implementation requires just the DataType as generic parameter.
    ///
    /// Assumes that the services for HasEntities&lt;YourDataType&gt; and StateAPI&lt;YourDataType&gt;
    /// can be located/injected using just those interfaces. 
    /// 
    /// This means you must register those services with the interfaces specified, e.g.
    /// 
    /// <code>
    /// [RegisterService(proxyInterfaces: new Type[] { typeof(HasState&lt;YourDataType&gt;) } ]
    /// public class YourStoreClass : StateStore&lt;YourDataType&gt; { }
    ///
    /// </code>
    /// </summary>
    public class LoadStateCmd<DataType> : LoadStateCmd<DataType, HasState<DataType>, StateResolver<DataType>> {}

    /// <summary>
    /// Generic command to load a single entity by a load key (id or alias)
    /// </summary>
    public class LoadStateCmd<DataType, StoreType, APIType> : NotificationCommand
        where StoreType : HasState<DataType>
        where APIType : StateResolver<DataType>
	{
        public bool m_debug;

        [Inject] private StoreType hasData { get; set; }
        [Inject] private APIType api { get; set; }

        public override string notificationType { get { return State<DataType>.LOAD_REQUESTED; } }

        public override void Execute ()
		{
            switch(LoadAdviceHelper.AdviseOnAndSendErrorIfCoolingDown (hasData, State<DataType>.LOAD_FAILED, debug: m_debug)) {
                case LoadAdvice.PROCEED:
                    break;
                default:
                    return;
			}

            State<DataType>.LoadStarted ();

            this.api.Resolve((r =>
            {
                if (LoadErrorHelper.HandledError(r, State<DataType>.LOAD_FAILED, debug: m_debug))
                {
                    return;
                }

                var resultItem = r.item;

                if(!IsOk(resultItem.status)) {
                    NotificationBus.Send(State<DataType>.LOAD_FAILED, new LoadFailedDTO
                    {
                        error = resultItem.status
                    });
                    return;
                }

                State<DataType>.LoadSucceeded(
                    new LoadSucceededDTO<DataType> {
                    data = resultItem.data
                });
			}));

		}

        virtual protected bool IsOk(string status) 
        {
            return status == Constants.STATUS_OK;
        }
	}
}



using BeatThat.Commands;
using BeatThat.Notifications;
using BeatThat.DependencyInjection;
using System;

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
    public class ResolveStateCmd<DataType> : ResolveStateCmd<DataType, HasState<DataType>, StateResolver<DataType>> {}

    /// <summary>
    /// Generic command to load a single entity by a load key (id or alias)
    /// </summary>
    public class ResolveStateCmd<DataType, StoreType, ResolverType> : NotificationCommandBase<ResolveRequestDTO>
        where StoreType : HasState<DataType>
        where ResolverType : StateResolver<DataType>
    {
        public bool m_debug;

        [Inject] private StoreType hasData { get; set; }
        [Inject] private ResolverType resolver { get; set; }

        public override string notificationType { get { return State<DataType>.RESOLVE_REQUESTED; } }

        public override void Execute(ResolveRequestDTO dto)
        {
            if (!dto.forceUpdate)
            {
                switch (ResolveAdviceHelper.AdviseOnAndSendErrorIfCoolingDown(hasData, State<DataType>.RESOLVE_FAILED, debug: m_debug))
                {
                    case ResolveAdvice.PROCEED:
                        break;
                    default:
                        return;
                }
            }

            State<DataType>.ResolveStarted();

            Resolve();
        }

        virtual protected bool IsOk(string status)
        {
            return status == ResolveStatusCode.OK;
        }

#if NET_4_6
        private async void Resolve()
        {
            try {
                var data = await this.resolver.ResolveAsync();
                HandleResult(ref data);
            }
            catch(Exception e) {
                State<DataType>.ResolveFailed(new ResolveFailedDTO
                {
                    error = e
                });
            }
        }
#else 
        private void Resolve()
        {
            this.resolver.Resolve((r =>
            {
                if (ResolveErrorHelper.HandledError(r, State<DataType>.RESOLVE_FAILED, debug: m_debug))
                {
                    return;
                }

                var resultItem = r.item;
                HandleResult(ref resultItem);
            }));
        }
#endif

        private void HandleResult(ref ResolveResultDTO<DataType> resultItem)
        {
            if (!IsOk(resultItem.status))
            {
                NotificationBus.Send(State<DataType>.RESOLVE_FAILED, new ResolveFailedDTO
                {
                    error = resultItem.status
                });
                return;
            }

            State<DataType>.ResolveSucceeded(
                new ResolveSucceededDTO<DataType>
                {
                    data = resultItem.data
                });
            
        }

    }
}



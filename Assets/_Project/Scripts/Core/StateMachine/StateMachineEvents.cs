using System;

namespace TWR.Core
{
    public struct ChangeStateEvent : IEvent
    {
        public Type stateType;
    }
}

using System;
using System.Collections.Generic;

namespace TWR.Core
{
    public class StateMachine
    {
        IState _current;
        readonly Dictionary<Type, IState> _states = new();

        public IState Current => _current;

        public StateMachine()
        {
            EventBus<ChangeStateEvent>.Subscribe(OnChangeStateRequested);
        }

        public void AddState(IState state)
        {
            _states[state.GetType()] = state;
        }

        public void ChangeState<T>() where T : IState
        {
            ChangeState(typeof(T));
        }

        public void Update()
        {
            _current?.OnUpdate();
        }

        void OnChangeStateRequested(ChangeStateEvent evt)
        {
            ChangeState(evt.stateType);
        }

        void ChangeState(Type stateType)
        {
            if (_states.TryGetValue(stateType, out var next))
                Transition(next);
        }

        void Transition(IState next)
        {
            _current?.OnExit();
            _current = next;
            _current?.OnEnter();
        }
    }
}
using System;
using System.Collections.Generic;

namespace TWR.Core
{
    public interface IEvent { }

    public static class EventBus<T> where T : IEvent
    {
        static readonly Dictionary<Type, List<Delegate>> _handlers = new();

        public static void Subscribe(Action<T> handler)
        {
            var type = typeof(T);
            if (!_handlers.ContainsKey(type))
                _handlers[type] = new List<Delegate>();
            _handlers[type].Add(handler);
        }

        public static void Unsubscribe(Action<T> handler)
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var list))
                list.Remove(handler);
        }

        public static void Publish(T evt)
        {
            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var list)) return;
            for (int i = list.Count - 1; i >= 0; i--)
                ((Action<T>)list[i])(evt);
        }

        public static void Clear()
        {
            _handlers.Clear();
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace sb.eventbus
{
    public static class EventBus<T> where T : IEvent
    {
        private static readonly List<EventListener<T>> listeners = new List<EventListener<T>>();

        public static void AddListener(EventListener<T> listener)
        {
            listeners.Add(listener);
        }

        public static void RemoveListener(EventListener<T> listener)
        {
            listeners.Remove(listener);
        }

        public static void Emit(T @event)
        {
            foreach (var listener in listeners)
            {
                listener.OnEvent.Invoke(@event);
            }
        }

        private static void ClearListeners()
        {
            listeners.Clear();
            Debug.Log("Cleared Listeners");
        }
    }
}
using System;

namespace sb.eventbus
{
    public class EventListener<T> where T : IEvent
    {
        private Action<T> onEvent { get; set; }

        public Action<T> OnEvent
        {
            get => onEvent;
            set => onEvent = value;
        }

        public EventListener(Action<T> onEvent)
        {
            this.OnEvent = onEvent;
        }

        public void AddListener(Action<T> onEvent)
        {
            this.OnEvent += onEvent;
        }

        public void RemoveListener(Action<T> onEvent)
        {
            this.OnEvent -= onEvent;
        }
    }
}